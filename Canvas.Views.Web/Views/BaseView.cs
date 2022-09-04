using Canvas.Core;
using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.EnumSpace;
using Canvas.Core.MessageSpace;
using Canvas.Core.ModelSpace;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ScriptContainer;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Canvas.Views.Web.Views
{
  public class BaseView : ComponentBase, IMessenger
  {
    [Parameter] public PositionEnum View { get; set; }

    [Inject] protected virtual IJSRuntime RuntimeService { get; set; }

    public virtual Task Updater { get; set; }
    public virtual IEngine Engine { get; set; }
    public virtual IComposer Composer { get; set; }
    public virtual ViewMessage Move { get; set; }
    public virtual ViewMessage Cursor { get; set; }
    public virtual ScriptService Service { get; set; }
    public virtual IDictionary<string, IList<double>> Series { get; set; }

    protected virtual string Route { get; set; }
    protected virtual ElementReference CanvasContainer { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public virtual string Name => $"{ GetHashCode() }";

    /// <summary>
    /// Create to override
    /// </summary>
    public virtual void CreateView() { }

    /// <summary>
    /// Update to override
    /// </summary>
    public virtual void UpdateView() { }

    /// <summary>
    /// Render
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public virtual Task Update(DomainMessage message = null)
    {
      if (message is null && Updater?.IsCompleted is false)
      {
        return Updater;
      }

      return Updater = InvokeAsync(() =>
      {
        if (Engine?.GetInstance() is null)
        {
          return;
        }

        Engine.Clear();

        if (message is not null)
        {
          Composer.Update(message);
        }

        UpdateView();

        Route = "data:image/webp;base64," + Convert.ToBase64String(Engine.Encode(SKEncodedImageFormat.Webp, 100));

        StateHasChanged();
      });
    }

    /// <summary>
    /// Create canvas
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual async Task Create<T>(Func<IEngine, IComposer> action) where T : IEngine, new()
    {
      async Task setup()
      {
        Dispose();

        var engine = new T();
        var dimensions = await CreateViewMessage();

        Engine = engine.Create(dimensions.X, dimensions.Y);
        Composer = action(Engine);
        Composer.Views[Name] = this;

        CreateView();
      }

      Service = await (new ScriptService(RuntimeService)).CreateModule();
      Service.OnSize = async o => await setup();

      await setup();
    }

    /// <summary>
    /// Get information about event
    /// </summary>
    /// <returns></returns>
    public virtual async Task<ViewMessage> CreateViewMessage()
    {
      var bounds = await Service.GetElementBounds(CanvasContainer);

      return new ViewMessage
      {
        X = bounds.X,
        Y = bounds.Y
      };
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
      Engine?.Dispose();

      if (Composer is not null && Composer.Views.ContainsKey(Name))
      {
        Composer.Views.Remove(Name);
      }
    }

    /// <summary>
    /// Get cursor position
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public virtual ViewMessage GetDelta(MouseEventArgs e)
    {
      if (Engine?.GetInstance() is null)
      {
        return null;
      }

      var values = Composer.GetValues(Engine, new ItemModel
      {
        X = e.OffsetX,
        Y = e.OffsetY
      });

      var item = Composer.Items.ElementAtOrDefault((int)values.X);

      if (item is not null)
      {
        var coordinates = new ItemModel
        {
          X = e.OffsetX,
          Y = e.OffsetY
        };

        item.Composer = Composer;
        Series = item.GetSeries(coordinates, values);
      }

      return new ViewMessage
      {
        X = e.OffsetX,
        Y = e.OffsetY,
        ValueX = Composer.ShowIndex(values.X.Value),
        ValueY = Composer.ShowValue(values.Y.Value)
      };
    }

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnWheel(WheelEventArgs e)
    {
      if (Engine?.GetInstance() is null)
      {
        return;
      }

      var isZoom = e.ShiftKey;
      var message = new DomainMessage
      {
        Name = Name
      };

      switch (true)
      {
        case true when e.DeltaY > 0: message.IndexDomain = isZoom ? Composer.ZoomIndexScale(Engine, -1) : Composer.PanIndexScale(Engine, 1); break;
        case true when e.DeltaY < 0: message.IndexDomain = isZoom ? Composer.ZoomIndexScale(Engine, 1) : Composer.PanIndexScale(Engine, -1); break;
      }

      Update(message);
    }

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnMouseMove(MouseEventArgs e)
    {
      if (Engine?.GetInstance() is null)
      {
        return;
      }

      var position = GetDelta(e);

      Cursor ??= position;

      if (e.Buttons == 1)
      {
        var isZoom = e.ShiftKey;
        var deltaX = Cursor.X - position.X;
        var deltaY = Cursor.Y - position.Y;
        var message = new DomainMessage
        {
          Name = Name
        };

        switch (true)
        {
          case true when deltaX > 0: message.IndexDomain = isZoom ? Composer.ZoomIndexScale(Engine, -1) : Composer.PanIndexScale(Engine, 1); break;
          case true when deltaX < 0: message.IndexDomain = isZoom ? Composer.ZoomIndexScale(Engine, 1) : Composer.PanIndexScale(Engine, -1); break;
        }

        Update(message);
      }

      Cursor = position;
    }

    /// <summary>
    /// Resize event
    /// </summary>
    /// <param name="e"></param>
    /// <param name="direction"></param>
    public virtual void OnScaleMove(MouseEventArgs e, int direction = 0)
    {
      if (Engine?.GetInstance() is null)
      {
        return;
      }

      var position = GetDelta(e);

      Move ??= position;

      if (e.Buttons == 1)
      {
        var isZoom = e.ShiftKey;
        var deltaX = Move.X - position.X;
        var deltaY = Move.Y - position.Y;
        var message = new DomainMessage
        {
          Name = Name
        };

        switch (direction > 0)
        {
          case true when deltaX > 0: message.IndexDomain = Composer.ZoomIndexScale(Engine, -1); break;
          case true when deltaX < 0: message.IndexDomain = Composer.ZoomIndexScale(Engine, 1); break;
        }

        switch (direction < 0)
        {
          case true when deltaY > 0: message.ValueDomain = Composer.ZoomValueScale(Engine, -1); break;
          case true when deltaY < 0: message.ValueDomain = Composer.ZoomValueScale(Engine, 1); break;
        }

        Update(message);
      }

      Move = position;
    }

    /// <summary>
    /// Double clck event in the view area
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnMouseDown(MouseEventArgs e)
    {
      if (Engine?.GetInstance() is null)
      {
        return;
      }

      if (e.CtrlKey)
      {
        var message = new DomainMessage
        {
          Name = Name,
          ValueUpdate = true
        };

        Update(message);
      }
    }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnMouseLeave(MouseEventArgs e)
    {
      if (Engine?.GetInstance() is null)
      {
        return;
      }

      Cursor = null;
    }
  }
}
