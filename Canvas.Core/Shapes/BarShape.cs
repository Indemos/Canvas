using Canvas.Core.Enums;
using Canvas.Core.Models;
using System.Collections.Generic;

namespace Canvas.Core.Shapes
{
  public class BarShape : GroupShape, IGroupShape
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
        Composer.GetItemPosition(Engine, index - size, 0.0),
        Composer.GetItemPosition(Engine, index + size, current.Value)
      };

      Engine.CreateBox(coordinates, Component ?? Composer.Components[nameof(ComponentEnum.Shape)]);
    }
  }
}
