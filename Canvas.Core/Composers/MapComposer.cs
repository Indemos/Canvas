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
    public int Range { get; set; }

    /// <summary>
    /// Enumerate indices
    /// </summary>
    protected override IList<MarkerModel> GetIndices()
    {
      var minIndex = Dimension.MinIndex;
      var maxIndex = Dimension.MaxIndex;
      var range = 0.0 + maxIndex - minIndex;
      var count = Math.Min(IndexCount, range);
      var step = Math.Round(range / count, MidpointRounding.ToEven);
      var items = new List<MarkerModel>();

      void createItem(double i)
      {
        if (i >= minIndex && i <= maxIndex)
        {
          var position = GetItemPosition(i, 0).X;

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
      var minValue = Dimension.MinValue;
      var maxValue = Dimension.MaxValue;
      var range = 0.0 + maxValue - minValue;
      var count = Math.Min(ValueCount, range);
      var step = Math.Round(range / count, MidpointRounding.ToEven);
      var items = new List<MarkerModel>();

      void createItem(double i)
      {
        if (i >= minValue && i <= maxValue)
        {
          var position = GetItemPosition(0, i).Y;

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
    /// <param name="delta"></param>
    public override IList<double> ZoomValue(int delta) => Dimension.ValueDomain;

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="delta"></param>
    public override IList<int> ZoomIndex(int delta) => Dimension.IndexDomain;

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="delta"></param>
    public override IList<int> PanIndex(int delta) => Dimension.IndexDomain;
  }
}
