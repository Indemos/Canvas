using Canvas.Core.EnumSpace;
using Canvas.Core.ModelSpace;
using System;
using System.Collections.Generic;

namespace Canvas.Core.ShapeSpace
{
  public class CandleShape : GroupShape, IGroupShape
  {
    /// <summary>
    /// Low
    /// </summary>
    public double? L { get; set; }

    /// <summary>
    /// High
    /// </summary>
    public double? H { get; set; }

    /// <summary>
    /// Open
    /// </summary>
    public double? O { get; set; }

    /// <summary>
    /// Close
    /// </summary>
    public double? C { get; set; }

    /// <summary>
    /// Options
    /// </summary>
    public virtual ComponentModel? Box { get; set; }

    /// <summary>
    /// Options
    /// </summary>
    public virtual ComponentModel? Line { get; set; }

    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override double[] GetDomain(int index, string name, IList<IShape> items)
    {
      if (L is null || H is null)
      {
        return null;
      }

      return new double[]
      {
        L.Value,
        H.Value
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
      return new double[]
      {
        O ?? 0,
        H ?? 0,
        L ?? 0,
        C ?? 0
      };
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
      if (L is null || H is null || O is null || C is null)
      {
        return;
      }

      var open = O ?? 0;
      var close = C ?? 0;
      var size = Composer.Size / 2.0;
      var downSide = Math.Min(open, close);
      var upSide = Math.Max(open, close);
      var coordinates = new DataModel[]
      {
        Composer.GetPixels(Engine, index - size, upSide),
        Composer.GetPixels(Engine, index + size, downSide)
      };

      var rangeCoordinates = new DataModel[]
      {
        Composer.GetPixels(Engine, index, L ?? 0),
        Composer.GetPixels(Engine, index, H ?? 0),
      };

      Engine.CreateLine(rangeCoordinates, Line ?? Component ?? Composer.Components[ComponentEnum.Shape]);
      Engine.CreateBox(coordinates, Box ?? Component ?? Composer.Components[ComponentEnum.Shape]);
    }
  }
}
