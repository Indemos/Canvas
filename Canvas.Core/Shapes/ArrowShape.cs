using Canvas.Core.Enums;
using Canvas.Core.Models;
using System;
using System.Collections.Generic;

namespace Canvas.Core.Shapes
{
  public class ArrowShape : Shape
  {
    /// <summary>
    /// Direction
    /// </summary>
    public virtual int Direction { get; set; }

    /// <summary>
    /// Render the shape
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override void CreateShape(int index, string name, IList<IShape> items)
    {
      if (Y is null)
      {
        return;
      }

      var x = Composer.GetShapePixels(index, Composer.Size);
      var y = Composer.GetItemPosition(index, Y.Value).Y;
      var coordinates = new Unit[]
      {
        new() { X = x.Center, Y = y },
        new() { X = x.R, Y = y },
        new() { X = x.L, Y = y }
      };

      var center = (x.R - x.L) / 2.0;

      coordinates[0].Y -= center * Math.Sign(Direction);

      Composer.Engine.CreateShape(coordinates, Component ?? Composer.Components[nameof(ComponentEnum.Shape)]);
    }
  }
}
