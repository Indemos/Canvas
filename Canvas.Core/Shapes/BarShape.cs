using Canvas.Core.ModelSpace;
using System.Collections.Generic;

namespace Canvas.Core.ShapeSpace
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
      var currentModel = Data?.Y;

      if (currentModel is null)
      {
        return;
      }

      var size = Composer.Shape.Size / 2.0;

      var coordinates = new DataModel[]
      {
        Composer.GetPixels(Engine, index - size, 0.0),
        Composer.GetPixels(Engine, index + size, currentModel.Value)
      };

      Engine.CreateBox(coordinates, Component ?? Composer.Shape);
    }
  }
}
