using Canvas.Core.Models;
using System;
using System.Collections.Generic;

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
      var distance = View.Engine.X / IndexCount;
      var stepValue = (maxIndex - minIndex) / IndexCount;
      var items = new List<MarkerModel>();

      for (var i = 1; i <= IndexCount; i++)
      {
        items.Add(new MarkerModel
        {
          Line = 0,
          Marker = distance * i - distance / 2.0,
          Caption = ShowIndex(i - 1, minIndex + i * stepValue - stepValue / 2.0)
        });
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
      var distance = View.Engine.Y / ValueCount;
      var stepValue = (maxValue - minValue) / ValueCount;
      var items = new List<MarkerModel>();

      for (var i = 1; i <= ValueCount; i++)
      {
        items.Add(new MarkerModel
        {
          Line = 0,
          Marker = distance * i - distance / 2.0,
          Caption = ShowValue(i - 1, minValue + (ValueCount - i) * stepValue - stepValue / 2.0)
        });
      }

      return items;
    }
  }
}
