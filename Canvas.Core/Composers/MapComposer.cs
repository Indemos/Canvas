using Canvas.Core.Engines;
using Canvas.Core.Models;
using Canvas.Core.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.Composers
{
  public class MapComposer : Composer
  {
    /// <summary>
    /// Enumerate indices
    /// </summary>
    protected override IList<MarkerModel> GetIndices()
    {
      var minIndex = Domain.MinIndex;
      var maxIndex = Domain.MaxIndex;
      var range = maxIndex - minIndex + 0.0;
      var stepSize = View.Engine.X / Items.Count;
      var step = Math.Round(range / Math.Min(IndexCount, range), MidpointRounding.ToEven);
      var items = new List<MarkerModel>();

      void createItem(double i)
      {
        if (i >= minIndex && i < maxIndex)
        {
          var position = GetItemPosition(View.Engine, i, 0).X;

          items.Add(new MarkerModel
          {
            Line = 0,
            Marker = position + stepSize / 2.0,
            Caption = ShowIndex(Math.Round(i, MidpointRounding.ToZero))
          });
        }
      }

      for (var i = 0.0; i < IndexCount / 2.0; i++)
      {
        createItem(minIndex + i * step);
        createItem(maxIndex - i * step - 1.0);
      }

      return items;
    }

    /// <summary>
    /// Enumerate values
    /// </summary>
    protected override IList<MarkerModel> GetValues()
    {
      var minValue = Domain.MinValue;
      var maxValue = Domain.MaxValue;
      var pointsCount = Items.Max(o => (o as ColorMapShape).Points.Count + 0.0);
      var stepSize = View.Engine.Y / pointsCount;
      var step = Math.Round(pointsCount / Math.Min(ValueCount, pointsCount), MidpointRounding.ToEven);
      var items = new List<MarkerModel>();

      void createItem(double i)
      {
        if (i >= minValue && i < maxValue)
        {
          var position = GetItemPosition(View.Engine, 0, i).Y;

          items.Add(new MarkerModel
          {
            Line = 0,
            Marker = position - stepSize / 2.0,
            Caption = ShowValue(Math.Round(i, MidpointRounding.ToZero))
          });
        }
      }

      for (var i = 0.0; i < ValueCount / 2.0; i++)
      {
        createItem(minValue + i * step);
        createItem(maxValue - i * step - 1.0);
      }

      return items;
    }

    /// <summary>
    /// Value scale
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="delta"></param>
    public override IList<double> ZoomValue(IEngine engine, int delta) => Domain.ValueDomain;

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="delta"></param>
    public override IList<int> ZoomIndex(IEngine engine, int delta) => Domain.IndexDomain;

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="delta"></param>
    public override IList<int> PanIndex(IEngine engine, int delta) => Domain.IndexDomain;
  }
}
