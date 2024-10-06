using Canvas.Core.Enums;
using Canvas.Core.Models;
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
      var current = Y;

      if (current is null)
      {
        return;
      }

      var size = Composer.Size / 2.0;
      var coordinates = new DataModel[]
      {
        Composer.GetItemPosition(index - size, 0.0),
        Composer.GetItemPosition(index + size, current.Value)
      };

      Composer.Engine.CreateBox(coordinates, Component ?? Composer.Components[nameof(ComponentEnum.Shape)]);
    }
  }
}
