using Canvas.Core.Models;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Views.Web.Views
{
  public partial class CanvasView
  {
    protected virtual string Name { get; set; } = $"{Guid.NewGuid():N}";
    protected virtual PositionModel? Cursor { get; set; }
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

      ViewService?.OnMouseMove(message);
      OnMouseMove(message);
    }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseLeaveAction(MouseEventArgs e)
    {
      Cursor = null;
      ViewService?.OnMouseLeave(default);
      OnMouseLeave(default);
    }

    /// <summary>
    /// Get cursor position
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    protected PositionModel? GetDelta(MouseEventArgs e)
    {
      if (Engine?.Instance is null)
      {
        return null;
      }

      var coordinates = new DataModel
      {
        X = e.OffsetX,
        Y = e.OffsetY
      };

      var values = Composer.GetItemValue(Engine, coordinates);
      var item = Composer.Items.ElementAtOrDefault((int)Math.Round(values.X));

      Series = null;

      if (item is not null)
      {
        var view = new DataModel
        {
          X = Engine.X,
          Y = Engine.Y
        };

        item.Composer = Composer;
        Series = item.GetSeries(view, coordinates);
      }

      return new PositionModel
      {
        Data = coordinates,
        X = Composer.ShowMarkerIndex(values.X),
        Y = Composer.ShowMarkerValue(values.Y)
      };
    }
  }
}
