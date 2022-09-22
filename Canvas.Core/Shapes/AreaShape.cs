using Canvas.Core.ModelSpace;
using System.Collections.Generic;

namespace Canvas.Core.ShapeSpace
{
  public class AreaShape : GroupShape, IGroupShape
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
      var previousModel = GetItem(index - 1, name, items)?.Data?.Y;

      if (currentModel is null)
      {
        return;
      }

      var coordinates = new DataModel[]
      {
        Composer.GetPixels(Engine, index, (previousModel ?? currentModel).Value),
        Composer.GetPixels(Engine, index + 1, currentModel.Value),
        Composer.GetPixels(Engine, index + 1, 0.0),
        Composer.GetPixels(Engine, index, 0.0),
        Composer.GetPixels(Engine, index, (previousModel ?? currentModel).Value)
      };

      Engine.CreateShape(coordinates, Component ?? Composer.Shape);
    }
  }
}
