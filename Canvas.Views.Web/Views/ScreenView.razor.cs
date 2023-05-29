using Canvas.Core.Models;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Views.Web.Views
{
  public partial class ScreenView
  {
    protected virtual PositionModel? Cursor { get; set; }
    protected virtual IDictionary<string, IList<double>> Series { get; set; }

    /// <summary>
    /// Render
    /// </summary>
    protected override void Render() => Composer.UpdateItems(Engine, Composer.Domain);

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
    /// Get index step
    /// </summary>
    public virtual double IndexStep => (Engine?.X ?? 0) / (Composer?.IndexCount ?? 1);

    /// <summary>
    /// Get value step
    /// </summary>
    public virtual double ValueStep => (Engine?.Y ?? 0) / (Composer?.ValueCount ?? 1);

    /// <summary>
    /// Enumerate indices
    /// </summary>
    public virtual IEnumerable<(double, string)> GetIndices()
    {
      if (Engine is not null)
      {
        var count = (double)Composer.IndexCount;
        var distance = (double)Engine.X / count;
        var stepValue = (double)(Composer.Domain.MaxIndex - Composer.Domain.MinIndex) / count;

        if (Composer.ShowIndex is not null)
        {
          for (var i = 1; i < count; i++)
          {
            yield return
            (
              distance * i,
              Composer.ShowIndex(i - 1, Composer.Domain.MinIndex + i * stepValue)
            );
          }
        }

        if (Composer.ShowCellIndex is not null)
        {
          for (var i = 1; i <= count; i++)
          {
            yield return
            (
              distance * i - distance / 2.0,
              Composer.ShowCellIndex(i - 1, Composer.Domain.MinIndex + i * stepValue - stepValue / 2.0)
            );
          }
        }
      }
    }

    /// <summary>
    /// Enumerate values
    /// </summary>
    public virtual IEnumerable<(double, string)> GetValues()
    {
      if (Engine is not null)
      {
        var count = (double)Composer.ValueCount;
        var distance = (double)Engine.Y / count;
        var stepValue = (double)(Composer.Domain.MaxValue - Composer.Domain.MinValue) / count;

        if (Composer.ShowValue is not null)
        {
          for (var i = 1; i < count; i++)
          {
            yield return
            (
              distance * i,
              Composer.ShowValue(i - 1, Composer.Domain.MinValue + (count - i) * stepValue)
            );
          }
        }

        if (Composer.ShowCellValue is not null)
        {
          for (var i = 1; i <= count; i++)
          {
            yield return
            (
              distance * i - distance / 2.0,
              Composer.ShowCellValue(i - 1, Composer.Domain.MinValue + (count - i) * stepValue - stepValue / 2.0)
            );
          }
        }
      }
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

      var values = Composer.GetValues(Engine, coordinates);
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
        ValueX = Composer.ShowMarkerIndex(values.X),
        ValueY = Composer.ShowMarkerValue(values.Y)
      };
    }
  }
}
