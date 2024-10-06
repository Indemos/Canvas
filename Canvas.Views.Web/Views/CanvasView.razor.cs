using Canvas.Core;
using Canvas.Core.Composers;
using Canvas.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ScriptContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Canvas.Views.Web.Views
{
  public partial class CanvasView : IDisposable
  {
    protected virtual string Name { get; set; } = $"{Guid.NewGuid():N}";
    protected virtual PositionModel? Cursor { get; set; }
    protected virtual ElementReference ChartContainer { get; set; }
    protected virtual IDictionary<string, IList<double>> Series { get; set; }

    /// <summary>
    /// Board values
    /// </summary>
    /// <param name="series"></param>
    /// <returns></returns>
    public virtual string ShowSeries(IList<double> series)
    {
      if (series is not null)
      {
        return string.Join(" / ", series.Select(o => Composer.ShowBoard(o)));
      }

      return "0";
    }

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseMoveAction(MouseEventArgs e)
    {
      Cursor = GetDelta(e);

      var message = new ViewModel
      {
        IsMove = e.Buttons == 1,
        Data = new DataModel
        {
          X = e.OffsetX,
          Y = e.OffsetY
        }
      };

      Composer?.OnMouseMove(message);
      OnMouseMove(message);
    }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseLeaveAction(MouseEventArgs e)
    {
      Cursor = null;
      Composer?.OnMouseLeave(default);
      OnMouseLeave(default);
    }

    /// <summary>
    /// Get cursor position
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected PositionModel? GetDelta(MouseEventArgs e)
    {
      var engine = Composer?.Engine?.Instance;

      if (engine is null)
      {
        return null;
      }

      var coordinates = new DataModel
      {
        X = e.OffsetX,
        Y = e.OffsetY
      };

      var values = Composer.GetItemValue(coordinates);
      var item = Composer.Items.ElementAtOrDefault((int)Math.Round(values.X));

      Series = null;

      if (item is not null)
      {
        var view = new DataModel
        {
          X = engine.X,
          Y = engine.Y
        };

        item.Composer = Composer;
        Series = item.GetSeries(view, coordinates);
      }

      return new PositionModel
      {
        Data = coordinates,
        X = Composer.ShowIndex(values.X),
        Y = Composer.ShowValue(values.Y)
      };
    }

    /// Create
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public override async Task<IView> Create<T>(Func<IComposer> action)
    {
      Dispose();

      ScriptService = await new ScriptService(RuntimeService).CreateModule();
      ScriptService.Actions["OnChange"] = o => Setup();

      await ScriptService.SubscribeToSize(ChartContainer, "OnChange");

      Task Setup() => Schedule(async () =>
      {
        var engine = new T();
        var bounds = await GetBounds();

        Composer?.Engine?.Dispose();
        Composer = action();
        Composer.Engine = engine.Create(bounds.Data.X, bounds.Data.Y);

        await Update(Composer.Domain);
      });

      return this;
    }
  }
}
