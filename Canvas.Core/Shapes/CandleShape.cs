using Canvas.Core.ModelSpace;
using System;
using System.Collections.Generic;

namespace Canvas.Core.ShapeSpace
{
  public class CandleShape : GroupShape, IGroupShape
  {
    /// <summary>
    /// Open
    /// </summary>
    public double? Low { get; set; }

    /// <summary>
    /// Open
    /// </summary>
    public double? High { get; set; }

    /// <summary>
    /// Open
    /// </summary>
    public double? Open { get; set; }

    /// <summary>
    /// Open
    /// </summary>
    public double? Close { get; set; }

    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override double[] GetDomain(int index, string name, IList<IShape> items)
    {
      if (Low is null || High is null)
      {
        return null;
      }

      return new double[]
      {
        Low.Value,
        High.Value
      };
    }

    /// <summary>
    /// Get series values
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public override IList<double> GetSeriesValues(DataModel coordinates, DataModel values)
    {
      var L = Low ?? 0;
      var H = High ?? 0;
      var O = Open ?? 0;
      var C = Close ?? 0;

      return new double[] { O, H, L, C };
    }

    /// <summary>
    /// Render the shape
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override void CreateShape(int index, string name, IList<IShape> items)
    {
      if (Low is null || High is null || Open is null || Close is null)
      {
        return;
      }

      var L = Low ?? 0;
      var H = High ?? 0;
      var O = Open ?? 0;
      var C = Close ?? 0;
      var size = Composer.Shape.Size.Value / 2.0;
      var downSide = Math.Min(O, C);
      var upSide = Math.Max(O, C);

      var coordinates = new DataModel[]
      {
        Composer.GetPixels(Engine, index - size, upSide),
        Composer.GetPixels(Engine, index + size, downSide)
      };

      var rangeCoordinates = new DataModel[]
      {
        Composer.GetPixels(Engine, index, L),
        Composer.GetPixels(Engine, index, H),
      };

      Engine.CreateBox(coordinates, Component ?? Composer.Shape);
      Engine.CreateLine(rangeCoordinates, Component ?? Composer.Shape);
    }
  }
}
