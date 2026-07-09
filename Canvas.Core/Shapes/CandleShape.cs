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
    public virtual Section? Box { get; set; }

    /// <summary>
    /// Options
    /// </summary>
    public virtual Section? Line { get; set; }

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

      return
      [
        L.Value,
        H.Value
      ];
    }

    /// <summary>
    /// Get series values
    /// </summary>
    /// <param name="view"></param>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public override IList<double> GetSeriesValues(Unit view, Unit coordinates)
    {
      return
      [
        O ?? 0,
        H ?? 0,
        L ?? 0,
        C ?? 0
      ];
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

      var x = Composer.GetShapePixels(index, Composer.Size);
      var pxO = Composer.GetItemPosition(index, O.Value).Y;
      var pxC = Composer.GetItemPosition(index, C.Value).Y;
      var pxH = Composer.GetItemPosition(index, H.Value).Y;
      var pxL = Composer.GetItemPosition(index, L.Value).Y;

      var box = new Unit[]
      {
        new() { X = x.L, Y = Math.Min(pxO, pxC) },
        new() { X = x.R, Y = Math.Max(pxO, pxC) }
      };

      var line = new Unit[]
      {
        new() { X = x.Center, Y = pxH },
        new() { X = x.Center, Y = pxL }
      };

      Composer.Engine.CreateLine(line, Line ?? Component ?? Composer.Components[nameof(ComponentEnum.Shape)]);
      Composer.Engine.CreateBox(box, Box ?? Component ?? Composer.Components[nameof(ComponentEnum.Shape)]);
    }

    /// <summary>
    /// Grouping implementation
    /// </summary>
    /// <param name="previous"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    public static IShape Update(IShape previous, double? update)
    {
      var item = (previous?.Clone() ?? new CandleShape()) as CandleShape;
      var price = update ?? item.Y ?? item.C.Value;

      item.Y = item.C = price;
      item.O = item.O ?? price;
      item.L = Math.Min(item.L ?? price, price);
      item.H = Math.Max(item.H ?? price, price);

      return item;
    }
  }
}
