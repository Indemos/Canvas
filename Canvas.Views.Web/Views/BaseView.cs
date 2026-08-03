using Canvas.Controls;
using Canvas.Core;
using Canvas.Core.Composers;
using Canvas.Core.Engines;
using Canvas.Core.Models;
using Canvas.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Canvas.Views.Web.Views
{
  public abstract class BaseView : ComponentBase, IView
  {
    [Inject] protected virtual IJSRuntime RuntimeService { get; set; }

    protected virtual string Route { get; set; }
    protected virtual ScriptService ScriptService { get; set; }
    protected virtual SchedulerService SchedulerService { get; set; }
    protected virtual ElementReference Container { get; set; }

    protected object sync = new();

    /// <summary>
    /// Composer
    /// </summary>
    public virtual IComposer Composer { get; set; }

    /// <summary>
    /// Labels
    /// </summary>
    public virtual IList<Mark> Values { get; set; } = [];
    public virtual IList<Mark> Indices { get; set; } = [];

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    public abstract Task<IView> Create<T>(Func<IComposer> action) where T : IEngine, new();

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="propagation"></param>
    public virtual async Task Render(Dimension? domain = null, string propagation = null)
    {
      if (SchedulerService is null)
      {
        return;
      }

      await SchedulerService?.Send(() => Update(domain, propagation));
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="propagation"></param>
    public virtual Task Update(Dimension? domain = null, string propagation = null)
    {
      lock (sync)
      {
        if (Composer?.Engine?.Instance is null)
        {
          return Task.CompletedTask;
        }

        var scope = Composer.Render(domain ?? Composer.Dimension);

        Values = scope.Values;
        Indices = scope.Indices;

        Route = "data:image/webp;base64," + Convert.ToBase64String(Composer.Engine.Encode(SKEncodedImageFormat.Webp, 100));

        if (propagation is not null)
        {
          Composer.OnAction(domain ?? Composer.Dimension);
        }
      }

      return InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
      Values?.Clear();
      Indices?.Clear();

      ScriptService?.Dispose();
      SchedulerService?.Dispose();
      Composer?.Dispose();

      ScriptService = null;
      SchedulerService = null;
    }

    /// <summary>
    /// Get information about event
    /// </summary>
    protected virtual async Task<Transition> GetBounds()
    {
      var bounds = await ScriptService.GetElementBounds(Container);

      return new Transition
      {
        Data = new Unit
        {
          X = bounds?.X ?? 0,
          Y = bounds?.Y ?? 0
        }
      };
    }

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="e"></param>
    protected virtual async void OnWheelAction(WheelEventArgs e)
    {
      var message = new Transition
      {
        IsShape = e.ShiftKey,
        Data = new Unit
        {
          X = e.DeltaX,
          Y = e.DeltaY
        }
      };

      var domain = Composer?.OnWheel(message);

      await Render(domain, Composer?.Name);
    }

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="e"></param>
    protected virtual async void OnMouseMoveAction(MouseEventArgs e)
    {
      var message = new Transition
      {
        IsMove = e.Buttons == 1,
        Data = new Unit
        {
          X = e.OffsetX,
          Y = e.OffsetY
        }
      };

      var domain = Composer?.OnMouseMove(message);

      await Render(domain, Composer?.Name);
    }

    /// <summary>
    /// Resize event
    /// </summary>
    /// <param name="e"></param>
    /// <param name="orientation"></param>
    protected virtual async void OnScaleAction(MouseEventArgs e, int orientation = 0)
    {
      var message = new Transition
      {
        IsMove = e.Buttons == 1,
        Data = new Unit
        {
          X = e.OffsetX,
          Y = e.OffsetY
        }
      };

      var domain = Composer?.OnScale(message, orientation);

      await Render(domain, Composer?.Name);
    }

    /// <summary>
    /// Double clck event in the view area
    /// </summary>
    /// <param name="e"></param>
    protected virtual async Task OnMouseDownAction(MouseEventArgs e)
    {
      if (e.CtrlKey)
      {
        var domain = Composer.Dimension;
        domain.ValueDomain = null;
        await Render(domain);
      }
    }
  }
}
