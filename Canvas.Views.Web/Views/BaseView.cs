using Canvas.Core;
using Canvas.Core.Composers;
using Canvas.Core.Engines;
using Canvas.Core.Models;
using Canvas.Core.Services;
using Distribution.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ScriptContainer;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Canvas.Views.Web.Views
{
  public class BaseView : ComponentBase, IView
  {
    [Inject] protected virtual IJSRuntime RuntimeService { get; set; }

    protected virtual string Route { get; set; }
    protected virtual EventService ViewService { get; set; }
    protected virtual ScriptService ScriptService { get; set; }
    protected virtual ScheduleService ScheduleService { get; set; }
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
    /// Labels
    /// </summary>
    public virtual IList<MarkerModel> Values { get; set; } = new List<MarkerModel>();
    public virtual IList<MarkerModel> Indices { get; set; } = new List<MarkerModel>();

    /// <summary>
    /// Events
    /// </summary>
    public virtual Action<ViewModel> OnMouseMove { get; set; } = o => { };
    public virtual Action<ViewModel> OnMouseLeave { get; set; } = o => { };

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public virtual async Task Update(DomainModel message)
    {
      await Schedule(() =>
      {
        if (Engine?.Instance is null)
        {
          return;
        }

        Engine.Clear();
        Composer.GetItems(Engine, Composer.Domain);
        Route = "data:image/webp;base64," + Convert.ToBase64String(Engine.Encode(SKEncodedImageFormat.Webp, 100));
      });

      await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual Task<IView> Create<T>(Func<IComposer> action) where T : IEngine, new()
    {
      return Task.FromResult<IView>(default);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
      Values?.Clear();
      Indices?.Clear();

      OnMouseMove = o => { };
      OnMouseLeave = o => { };

      ScriptService?.Dispose();
      ScheduleService?.Dispose();

      ScriptService = null;
      ScheduleService = null;
    }

    /// <summary>
    /// Get information about event
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<ViewModel> GetBounds()
    {
      var bounds = await ScriptService.GetElementBounds(Container);

      return new ViewModel
      {
        Data = new DataModel
        {
          X = bounds.Value.X,
          Y = bounds.Value.Y
        }
      };
    }

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

    /// <summary>
    /// Dedicated process
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    protected virtual Task Schedule(Action action) => (ScheduleService ??= new ScheduleService()).Send(action).Task;
  }
}
