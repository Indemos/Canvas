using Canvas.Core.Models;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Canvas.Views.Web.Views
{
  public partial class ScreenView
  {
    protected virtual PositionModel? Cursor { get; set; }
    protected virtual IDictionary<string, IList<double>> Series { get; set; }
    protected virtual IList<double> Indices { get; set; } = new List<double>();
    protected virtual IList<double> Values { get; set; } = new List<double>();
    protected virtual string Space { get; set; } = $"{Guid.NewGuid():N}";

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
    /// Enumerate indices
    /// </summary>
    public virtual IEnumerable<(double, string)> GetIndices()
    {
      if (Engine is not null)
      {
        var minIndex = Composer.Domain.MinIndex;
        var maxIndex = Composer.Domain.MaxIndex;
        var distance = Engine.X / Composer.IndexCount;
        var stepValue = Math.Round((maxIndex - minIndex) / Composer.IndexCount);

        if (Composer.ShowIndex is not null)
        {
          Indices = new List<double>();

          for (var i = minIndex; i <= maxIndex; i++)
          {
            if (i % stepValue == 0)
            {
              var position = Composer.GetPixels(Engine, i, 0).X;

              Indices.Add(position);

              yield return (
                position,
                Composer.ShowIndex(i - 1, i)
              );
            }
          }
        }

        if (Composer.ShowCellIndex is not null)
        {
          for (var i = 1; i <= Composer.IndexCount; i++)
          {
            yield return
            (
              distance * i - distance / 2.0,
              Composer.ShowCellIndex(i - 1, minIndex + i * stepValue - stepValue / 2.0)
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
        var minValue = Composer.Domain.MinValue;
        var maxValue = Composer.Domain.MaxValue;
        var distance = Engine.Y / Composer.ValueCount;
        var stepValue = (maxValue - minValue) / Composer.ValueCount;

        if (Composer.ShowValue is not null)
        {
          Values = new List<double>();

          for (var i = 1; i < Composer.ValueCount; i++)
          {
            Values.Add(distance * i);

            yield return
            (
              distance * i,
              Composer.ShowValue(i - 1, minValue + (Composer.ValueCount - i) * stepValue)
            );
          }
        }

        if (Composer.ShowCellValue is not null)
        {
          for (var i = 1; i <= Composer.ValueCount; i++)
          {
            yield return
            (
              distance * i - distance / 2.0,
              Composer.ShowCellValue(i - 1, minValue + (Composer.ValueCount - i) * stepValue - stepValue / 2.0)
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
