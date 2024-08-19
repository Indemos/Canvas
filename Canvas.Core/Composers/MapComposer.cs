using Canvas.Core.Engines;
using Canvas.Core.Models;
using System;
using System.Collections.Generic;

namespace Canvas.Core.Composers
{
  public class MapComposer : Composer
  {
    /// <summary>
    /// Max number of points on Y axis
    /// </summary>
    public int Dimension { get; set; }

    /// <summary>
    /// Enumerate indices
    /// </summary>
    protected override IList<MarkerModel> GetIndices()
    {
      var minIndex = Domain.MinIndex;
      var maxIndex = Domain.MaxIndex;
      var range = 0.0 + maxIndex - minIndex;
      var count = Math.Min(IndexCount, range);
      var step = Math.Round(range / count, MidpointRounding.ToEven);
      var items = new List<MarkerModel>();

      void createItem(double i)
      {
        if (i >= minIndex && i <= maxIndex)
        {
          var position = GetItemPosition(View.Engine, i, 0).X;

          items.Add(new MarkerModel
          {
            Line = 0,
            Marker = position,
            Caption = ShowIndex(Math.Round(i, MidpointRounding.ToZero))
          });
        }
      }

      for (var i = 0; i < count / 2; i++)
      {
        createItem(maxIndex - i * step - 0.5);
        createItem(minIndex + i * step + 0.5);
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
      var range = 0.0 + maxValue - minValue;
      var count = Math.Min(ValueCount, range);
      var step = Math.Round(range / count, MidpointRounding.ToEven);
      var items = new List<MarkerModel>();

      void createItem(double i)
      {
        if (i >= minValue && i <= maxValue)
        {
          var position = GetItemPosition(View.Engine, 0, i).Y;

          items.Add(new MarkerModel
          {
            Line = 0,
            Marker = position,
            Caption = ShowValue(Math.Round(i, MidpointRounding.ToZero))
          });
        }
      }

      for (var i = 0; i < count / 2; i++)
      {
        createItem(maxValue - i * step - 0.5);
        createItem(minValue + i * step + 0.5);
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
