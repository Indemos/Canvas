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
      var stepSize = View.Engine.X / Items.Count;
      var center = Math.Round(minIndex + (maxIndex - minIndex) / 2.0, MidpointRounding.ToZero);
      var step = Math.Round((0.0 + maxIndex - minIndex) / IndexCount, MidpointRounding.ToZero);
      var items = new List<MarkerModel>();

      void createItem(double i)
      {
        var position = GetItemPosition(View.Engine, i, 0).X;

        if (i >= minIndex && i < maxIndex)
        {
          items.Add(new MarkerModel
          {
            Line = 0,
            Marker = position + stepSize / 2.0,
            Caption = ShowIndex(i)
          });
        }
      }

      for (var i = 0; i < IndexCount; i++)
      {
        createItem(center - i * step);
        createItem(center + i * step);
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
      var pointsCount = Items.Max(o => (o as ColorMapShape).Points.Count);
      var stepSize = View.Engine.Y / pointsCount;
      var center = Math.Round(minValue + (maxValue - minValue) / 2.0, MidpointRounding.ToZero);
      var step = Math.Round((maxValue - minValue) / ValueCount, MidpointRounding.ToZero);
      var items = new List<MarkerModel>();

      void createItem(double i)
      {
        var position = GetItemPosition(View.Engine, 0, i).Y;

        if (i >= minValue && i < maxValue)
        {
          items.Add(new MarkerModel
          {
            Line = 0,
            Marker = position - stepSize / 2.0,
            Caption = ShowValue(i)
          });
        }
      }

      for (var i = 0; i < ValueCount; i++)
      {
        createItem(center - i * step);
        createItem(center + i * step);
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
