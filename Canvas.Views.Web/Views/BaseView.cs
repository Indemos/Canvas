using Canvas.Core;
using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.EnumSpace;
using Canvas.Core.MessageSpace;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ScriptContainer;
using SkiaSharp;
using System;
using System.Threading.Tasks;

namespace Canvas.Views.Web.Views
{
  public class BaseView : ComponentBase, IView
  {
    [Parameter] public virtual PositionEnum Position { get; set; }

    [Inject] protected virtual IJSRuntime RuntimeService { get; set; }

    protected virtual int? Code { get; set; }
    protected virtual string Route { get; set; }
    protected virtual Task Updater { get; set; }
    protected virtual IComposer Composer { get; set; }
    protected virtual Reactor Reactor { get; set; }
    protected virtual ScriptService Service { get; set; }
    protected virtual ElementReference CanvasContainer { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    public virtual int Id => Code ??= GetHashCode();

    /// <summary>
    /// Engine
    /// </summary>
    public virtual IEngine Engine { get; protected set; }

    /// <summary>
    /// Events
    /// </summary>
    public virtual Action<ViewMessage> OnLoad { get; set; } = e => { };
    public virtual Action<ViewMessage> OnWheel { get; set; } = e => { };
    public virtual Action<ViewMessage> OnMouseMove { get; set; } = e => { };
    public virtual Action<ViewMessage> OnMouseDown { get; set; } = e => { };
    public virtual Action<ViewMessage> OnMouseLeave { get; set; } = e => { };

    /// <summary>
    /// Render
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public virtual Task Update(DomainMessage message = null)
    {
      if (Updater?.IsCompleted is false)
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
    public virtual async Task<IView> Create<T>(Func<IEngine, IComposer> action) where T : IEngine, new()
    {
      async Task setup()
      {
        Dispose();

        var engine = new T();
        var message = await CreateViewMessage();

        Engine = engine.Create(message.X, message.Y);
        Composer = action(Engine);

        await Update();

        Reactor = new Reactor
        {
          Engine = Engine,
          Composer = Composer,
          View = this
        };

        OnLoad(message);
      }

      Service = await (new ScriptService(RuntimeService)).CreateModule();
      Service.OnSize = async o => await setup();

      await setup();

      return this;
    }

    /// <summary>
    /// Update to override
    /// </summary>
    protected virtual void UpdateView() { }

    /// <summary>
    /// Get information about event
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<ViewMessage> CreateViewMessage()
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
    protected virtual void Dispose()
    {
      Engine?.Dispose();
    }

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnWheelAction(WheelEventArgs e)
    {
      var message = new ViewMessage
      {
        X = e.DeltaX,
        Y = e.DeltaY,
        IsShape = e.ShiftKey
      };

      Reactor?.OnWheel(message);
      OnWheel(message);
    }

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnMouseMoveAction(MouseEventArgs e)
    {
      var message = new ViewMessage
      {
        X = e.OffsetX,
        Y = e.OffsetY,
        IsSnap = e.Buttons == 1
      };

      Reactor?.OnMouseMove(message);
      OnMouseMove(message);
    }

    /// <summary>
    /// Resize event
    /// </summary>
    /// <param name="e"></param>
    /// <param name="direction"></param>
    protected virtual void OnScaleMoveAction(MouseEventArgs e, int direction = 0)
    {
      var message = new ViewMessage
      {
        X = e.OffsetX,
        Y = e.OffsetY,
        IsSnap = e.Buttons == 1
      };

      Reactor?.OnScaleMove(message, direction);
    }

    /// <summary>
    /// Double clck event in the view area
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnMouseDownAction(MouseEventArgs e)
    {
      var message = new ViewMessage
      {
        IsControl = e.CtrlKey
      };

      Reactor?.OnMouseDown(message);
      OnMouseDown(message);
    }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnMouseLeaveAction(MouseEventArgs e)
    {
      Reactor?.OnMouseLeave(null);
      OnMouseLeave(null);
    }
  }
}
