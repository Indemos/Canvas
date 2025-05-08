using Canvas.Core;
using Canvas.Core.Composers;
using Canvas.Core.Engines;
using Canvas.Core.Models;
using Distribution.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Canvas.Controls;
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
    protected virtual ScriptService ScriptService { get; set; }
    protected virtual ScheduleService ScheduleService { get; set; }
    protected virtual ElementReference Container { get; set; }

    /// <summary>
    /// Composer
    /// </summary>
    public virtual IComposer Composer { get; set; }

    /// <summary>
    /// Labels
    /// </summary>
    public virtual IList<MarkerModel> Values { get; set; } = [];
    public virtual IList<MarkerModel> Indices { get; set; } = [];

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual Task Update(DimensionModel? domain = null, string source = null)
    {
      try
      {
        return Schedule(() =>
        {
          if (Composer?.Engine?.Instance is null)
          {
            return Task.CompletedTask;
          }

          var scope = Composer.Render(domain ?? Composer.Dimension);

          Values = scope.Values;
          Indices = scope.Indices;

          Route = "data:image/webp;base64," + Convert.ToBase64String(Composer.Engine.Encode(SKEncodedImageFormat.Webp, 100));

          if (source is not null)
          {
            Composer.OnAction(domain ?? Composer.Dimension);
          }

          return InvokeAsync(StateHasChanged);
        });
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }

      return Task.CompletedTask;
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

      Composer?.Dispose();
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
      var message = new ViewModel
      {
        IsShape = e.ShiftKey,
        Data = new DataModel
        {
          X = e.DeltaX,
          Y = e.DeltaY
        }
      };

      var domain = Composer?.OnWheel(message);

      await Update(domain, Composer?.Name);
    }

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="e"></param>
    protected virtual async void OnMouseMoveAction(MouseEventArgs e)
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

      var domain = Composer?.OnMouseMove(message);

      await Update(domain, Composer?.Name);
    }

    /// <summary>
    /// Resize event
    /// </summary>
    /// <param name="e"></param>
    /// <param name="orientation"></param>
    protected virtual async void OnScaleAction(MouseEventArgs e, int orientation = 0)
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

      var domain = Composer?.OnScale(message, orientation);

      await Update(domain, Composer?.Name);
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
        await Update(domain);
      }
    }

    /// <summary>
    /// Dedicated process
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    protected virtual Task Schedule(Func<Task> action) => ScheduleService?.Send(action)?.Task ?? Task.CompletedTask;
  }
}
