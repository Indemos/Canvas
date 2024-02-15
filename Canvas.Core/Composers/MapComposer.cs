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

      void createItem(double i, double correction)
      {
        var position = GetItemPosition(View.Engine, i, 0).X;

        items.Add(new MarkerModel
        {
          Line = 0,
          Marker = position + correction,
          Caption = ShowIndex(i)
        });
      }

      var isEven = IndexCount % 2 is 0;

      createItem(center, 0);

      for (var i = 1.0; i <= IndexCount / 2.0; i++)
      {
        createItem(center - i * step, stepSize / 2.0);
        createItem(center + i * step - (isEven ? 1 : 0), stepSize / 2.0);
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

      void createItem(double i, double correction)
      {
        var position = GetItemPosition(View.Engine, 0, i).Y;

        items.Add(new MarkerModel
        {
          Line = 0,
          Marker = position - correction,
          Caption = ShowValue(i)
        });
      }

      var isEven = ValueCount % 2 is 0;

      createItem(center, 0);

      for (var i = 1.0; i <= ValueCount / 2.0; i++)
      {
        createItem(center - i * step, stepSize / 2.0);
        createItem(center + i * step - (isEven ? 1 : 0), stepSize / 2.0);
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
