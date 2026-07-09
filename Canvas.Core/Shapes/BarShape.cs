using Canvas.Core.Composers;
using Canvas.Core.Enums;
using Canvas.Core.Models;
using System;
using System.Collections.Generic;

namespace Canvas.Core.Shapes
{
  public class BarShape : Shape
  {
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
      var y0 = Composer.GetItemPosition(index, 0.0).Y;
      var y1 = Composer.GetItemPosition(index, Y.Value).Y;

      var coordinates = new Unit[]
      {
        new() { X = x.L, Y = Math.Min(y0, y1) },
        new() { X = x.R, Y = Math.Max(y0, y1) }
      };

      Composer.Engine.CreateBox(coordinates, Component ?? Composer.Components[nameof(ComponentEnum.Shape)]);
    }
  }
}
