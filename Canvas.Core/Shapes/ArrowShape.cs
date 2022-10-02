using Canvas.Core.ModelSpace;
using System.Collections.Generic;

namespace Canvas.Core.ShapeSpace
{
  public class ArrowShape : GroupShape, IGroupShape
  {
    /// <summary>
    /// Direction
    /// </summary>
    public int Direction { get; set; }

    /// <summary>
    /// Render the shape
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override void CreateShape(int index, string name, IList<IShape> items)
    {
      var currentModel = Data?.Y;

      if (currentModel is null)
      {
        return;
      }

      var size = Composer.Shape.Size / 2.0;

      var coordinates = new DataModel[]
      {
        Composer.GetPixels(Engine, index, currentModel.Value),
        Composer.GetPixels(Engine, index + size, currentModel.Value),
        Composer.GetPixels(Engine, index - size, currentModel.Value)
      };

      coordinates[0].Y -= (coordinates[1].X - coordinates[2].X) * Direction / 2;

      Engine.CreateShape(coordinates, Component ?? Composer.Shape);
    }
  }
}
