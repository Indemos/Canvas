using Canvas.Core;
using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.EnumSpace;
using Canvas.Core.ModelSpace;
using Canvas.Core.ServiceSpace;
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

    protected virtual string Route { get; set; }
    protected virtual ViewService ViewService { get; set; }
    protected virtual ScriptService ScriptService { get; set; }
    protected virtual SchedulerService ScheduleService { get; set; }
    protected virtual ElementReference Container { get; set; }

    /// <summary>
    /// Engine
    /// </summary>
    public virtual IEngine Engine { get; protected set; }

    /// <summary>
    /// Composer
    /// </summary>
    public virtual IComposer Composer { get; protected set; }

    /// <summary>
    /// Events
    /// </summary>
    public virtual Action<ViewModel> OnMouseMove { get; set; } = o => { };
    public virtual Action<ViewModel> OnMouseLeave { get; set; } = o => { };

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual void Update(DomainModel message, string source = null)
    {
      ScheduleService.Send(() =>
      {
        if (Engine?.GetInstance() is not null)
        {
          Engine.Clear();

          Render();

          Route = "data:image/webp;base64," + Convert.ToBase64String(Engine.Encode(SKEncodedImageFormat.Webp, 100));

          InvokeAsync(StateHasChanged);
        }

        return 0;
      });
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual async Task<IView> Create<T>(Func<IEngine, IComposer> action) where T : IEngine, new()
    {
      async Task setup()
      {
        Dispose();

        ScheduleService = new SchedulerService();
        ViewService = new ViewService { View = this };
        ScriptService = await (new ScriptService(RuntimeService)).CreateModule();
        ScriptService.OnSize = async o => await setup();

        var engine = new T();
        var message = await CreateViewMessage();

        Engine = await ScheduleService.Send(() => engine.Create(message.Data?.X ?? 1, message.Data?.Y ?? 1)).Task;
        Composer = action(Engine);
      }

      await setup();

      return this;
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
      Engine?.Dispose();
      ViewService?.Dispose();
      ScriptService?.Dispose();
      ScheduleService?.Dispose();
    }

    /// <summary>
    /// Get information about event
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<ViewModel> CreateViewMessage()
    {
      var bounds = await ScriptService.GetElementBounds(Container);

      return new ViewModel
      {
        Data = new DataModel
        {
          X = bounds.X,
          Y = bounds.Y
        }
      };
    }

    /// <summary>
    /// Update to override
    /// </summary>
    protected virtual void Render() { }

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnWheelAction(WheelEventArgs e) => ViewService?.OnWheel(new ViewModel
    {
      IsShape = e.ShiftKey,
      Data = new DataModel
      {
        X = e.DeltaX,
        Y = e.DeltaY
      }
    });

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnMouseMoveAction(MouseEventArgs e)
    {
      var message = new ViewModel
      {
        IsMove = e.Buttons == 1,
        Data = new DataModel
        {
          X = e.OffsetX,
          Y = e.OffsetY
        }
      };

      ViewService?.OnMouseMove(message);
      OnMouseMove(message);
    }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnMouseLeaveAction(MouseEventArgs e)
    {
      ViewService?.OnMouseLeave(default);
      OnMouseLeave(default);
    }

    /// <summary>
    /// Resize event
    /// </summary>
    /// <param name="e"></param>
    /// <param name="orientation"></param>
    protected virtual void OnScaleAction(MouseEventArgs e, int orientation = 0) => ViewService?.OnScale(new ViewModel
    {
      IsMove = e.Buttons == 1,
      Data = new DataModel
      {
        X = e.OffsetX,
        Y = e.OffsetY
      }
    }, orientation);

    /// <summary>
    /// Double clck event in the view area
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnMouseDownAction(MouseEventArgs e) => ViewService?.OnMouseDown(new ViewModel
    {
      IsControl = e.CtrlKey
    });
  }
}
