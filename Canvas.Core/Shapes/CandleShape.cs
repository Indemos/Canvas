using Canvas.Core.Enums;
using Canvas.Core.Models;
using System;
using System.Collections.Generic;

namespace Canvas.Core.Shapes
{
  public class CandleShape : Shape
  {
    /// <summary>
    /// Low
    /// </summary>
    public virtual double? L { get; set; }

    /// <summary>
    /// High
    /// </summary>
    public virtual double? H { get; set; }

    /// <summary>
    /// Open
    /// </summary>
    public virtual double? O { get; set; }

    /// <summary>
    /// Close
    /// </summary>
    public virtual double? C { get; set; }

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
    /// <param name="view"></param>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public override IList<double> GetSeriesValues(DataModel view, DataModel coordinates)
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
        Composer.GetItemPosition(Engine, index - size, upSide),
        Composer.GetItemPosition(Engine, index + size, downSide)
      };

      var rangeCoordinates = new DataModel[]
      {
        Composer.GetItemPosition(Engine, index, L ?? 0),
        Composer.GetItemPosition(Engine, index, H ?? 0),
      };

      Engine.CreateLine(rangeCoordinates, Line ?? Component ?? Composer.Components[nameof(ComponentEnum.Shape)]);
      Engine.CreateBox(coordinates, Box ?? Component ?? Composer.Components[nameof(ComponentEnum.Shape)]);
    }

    /// <summary>
    /// Grouping implementation
    /// </summary>
    /// <param name="previous"></param>
    /// <returns></returns>
    public override IShape Update(IShape previous)
    {
      var bar = previous as CandleShape;
      var price = Y ?? C ?? bar.Y ?? bar.C.Value;

      Y = C = price;
      O = bar.O ?? O ?? price;
      L = Math.Min(bar.L ?? price, L ?? price);
      H = Math.Max(bar.H ?? price, H ?? price);

      return this;
    }
  }
}
