using Canvas.Core;
using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ScriptContainer;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Canvas.Views.Web
{
  public partial class CanvasWebView : IDisposable
  {
    [Inject] protected virtual IJSRuntime RuntimeService { get; set; }

    /// <summary>
    /// Parameters
    /// </summary>
    public virtual Composer Composer { get; set; }
    public virtual Action<ViewMessage> OnUpdate { get; set; } = o => { };

    /// <summary>
    /// Accessors
    /// </summary>
    protected virtual Task Updater { get; set; }
    protected virtual ViewMessage Cursor { get; set; }
    protected virtual StreamServer Server { get; set; }
    protected virtual ScriptMessage Bounds { get; set; }
    protected virtual ScriptService Service { get; set; }
    protected virtual ElementReference ScaleContainer { get; set; }
    protected virtual ElementReference CanvasContainer { get; set; }

    /// <summary>
    /// Enumerate indices
    /// </summary>
    public virtual IEnumerable<IPointModel> GetIndexEnumerator()
    {
      if (Composer?.Engine is not null)
      {
        var cnt = Composer.IndexLabelCount + 1;
        var step = Composer.Engine.IndexSize / cnt;
        var stepValue = (Composer.MaxIndex - Composer.MinIndex) / cnt;

        for (var i = 1; i < cnt; i++)
        {
          yield return new PointModel
          {
            Index = step * i,
            Value = Composer.ShowIndex(Composer.MinIndex + i * stepValue)
          };
        }
      }
    }

    /// <summary>
    /// Enumerate values
    /// </summary>
    public virtual IEnumerable<IPointModel> GetValueEnumerator()
    {
      if (Composer?.Engine is not null)
      {
        var cnt = Composer.ValueLabelCount + 1;
        var step = Composer.Engine.ValueSize / cnt;
        var stepValue = (Composer.MaxValue - Composer.MinValue) / cnt;

        for (var i = 1; i < cnt; i++)
        {
          var Index = step * i;
          var Value = Composer.ShowValue(Composer.MinValue + (cnt - i) * stepValue);

          yield return new PointModel
          {
            Index = step * i,
            Value = Composer.ShowValue(Composer.MinValue + (cnt - i) * stepValue)
          };
        }
      }
    }

    /// <summary>
    /// Render
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public virtual Task Update(Composer message = null)
    {
      if (message is null && Updater?.IsCompleted is false)
      {
        return Updater;
      }

      return Updater = InvokeAsync(async () =>
      {
        var engine = Composer?.Engine as CanvasEngine;

        if (engine?.Map is null)
        {
          return;
        }

        Composer.Engine.Clear();
        Composer.Update();

        using (var image = engine.Map.Encode(SKEncodedImageFormat.Webp, 100))
        {
          var data = image.ToArray();

          await Server.Stream.Writer.WriteAsync(data);
          await Server.Stream.Writer.WriteAsync(data);
        }

        if (message is not null)
        {
          OnUpdate(await CreateMessage());
        }

        StateHasChanged();
      });
    }

    /// <summary>
    /// Create canvas
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual async Task Create(Action<ViewMessage> action)
    {
      Dispose(0);

      Server = await (new StreamServer()).Create();
      Service = await (new ScriptService(RuntimeService)).CreateModule();
      Service.OnSize = async o =>
      {
        action(await CreateMessage());
        await Update();
      };

      action(await CreateMessage());
      await Update();
    }

    /// <summary>
    /// Get information about event
    /// </summary>
    /// <returns></returns>
    protected async Task<ViewMessage> CreateMessage()
    {
      Bounds = await Service.GetElementBounds(CanvasContainer);

      return new ViewMessage
      {
        View = this,
        X = Bounds.X,
        Y = Bounds.Y
      };
    }

    /// <summary>
    /// Dispose
    /// </summary>
    protected void Dispose(int o)
    {
      Composer?.Dispose();
      Server?.Dispose();
    }

    /// <summary>
    /// Dispose
    /// </summary>
    void IDisposable.Dispose()
    {
      Dispose(0);
    }

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="e"></param>
    protected void OnWheel(WheelEventArgs e)
    {
      if (Composer?.Engine is null)
      {
        return;
      }

      var isZoom = e.ShiftKey;

      switch (true)
      {
        case true when e.DeltaY > 0: _ = isZoom ? Composer.ZoomIndexScale(-1) : Composer.PanIndexScale(1); break;
        case true when e.DeltaY < 0: _ = isZoom ? Composer.ZoomIndexScale(1) : Composer.PanIndexScale(-1); break;
      }

      Update(Composer);
    }

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="e"></param>
    protected void OnMouseMove(MouseEventArgs e)
    {
      if (Composer?.Engine is null)
      {
        return;
      }

      var values = Composer.GetValues(Composer.Engine, new PointModel
      {
        Index = e.OffsetX,
        Value = e.OffsetY
      });

      var position = new ViewMessage
      {
        X = e.OffsetX,
        Y = e.OffsetY,
        ValueX = Composer.ShowIndex(values.Index.Value),
        ValueY = Composer.ShowValue(values.Value)
      };

      Cursor ??= position;

      if (e.Buttons == 1)
      {
        var deltaX = Cursor.X - position.X;
        var deltaY = Cursor.Y - position.Y;
        var isZoom = e.ShiftKey;

        switch (true)
        {
          case true when deltaX > 0: _ = isZoom ? Composer.ZoomIndexScale(-1) : Composer.PanIndexScale(1); break;
          case true when deltaX < 0: _ = isZoom ? Composer.ZoomIndexScale(1) : Composer.PanIndexScale(-1); break;
        }

        switch (true)
        {
          case true when deltaY > 0: Composer.ZoomValueScale(-1); break;
          case true when deltaY < 0: Composer.ZoomValueScale(1); break;
        }

        Update(Composer);
      }

      Cursor = position;
    }

    /// <summary>
    /// Double clck event in the view area
    /// </summary>
    /// <param name="e"></param>
    protected void OnMouseDown(MouseEventArgs e)
    {
      if (Composer?.Engine is null)
      {
        return;
      }

      if (e.CtrlKey)
      {
        Composer.ValueDomain = null;
        Update(Composer);
      }
    }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="e"></param>
    protected void OnMouseLeave(MouseEventArgs e)
    {
      if (Composer?.Engine is null)
      {
        return;
      }

      Cursor = null;
    }
  }
}
