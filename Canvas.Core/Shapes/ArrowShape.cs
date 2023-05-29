using Canvas.Core.Enums;
using Canvas.Core.Models;
using System.Collections.Generic;

namespace Canvas.Core.Shapes
{
  public class ArrowShape : GroupShape, IGroupShape
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
      var current = Y;

      if (current is null)
      {
        return;
      }

      var size = Composer.Size;
      var coordinates = new DataModel[]
      {
        Composer.GetPixels(Engine, index, current.Value),
        Composer.GetPixels(Engine, index + size, current.Value),
        Composer.GetPixels(Engine, index - size, current.Value)
      };

      coordinates[0].Y -= (coordinates[1].X - coordinates[2].X) * Direction / 2;

      Engine.CreateShape(coordinates, Component ?? Composer.Components[nameof(ComponentEnum.Shape)]);
    }
  }
}
