using System.Collections.Generic;

namespace Canvas.Core.ModelSpace
{
  public class AreaGroupModel : GroupModel, IGroupModel
  {
    /// <summary>
    /// Render the shape
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override void CreateShape(int index, string name, IList<IItemModel> items)
    {
      var currentModel = Value;
      var previousModel = GetItem(index - 1, name, items);

      if (currentModel?.Point is null || previousModel?.Point is null)
      {
        return;
      }

      var coordinates = new IItemModel[]
      {
        Composer.GetPixels(Engine, index - 1, previousModel.Point),
        Composer.GetPixels(Engine, index, currentModel.Point),
        Composer.GetPixels(Engine, index, 0.0),
        Composer.GetPixels(Engine, index - 1, 0.0),
        Composer.GetPixels(Engine, index - 1, previousModel.Point)
      };

      Color = currentModel.Color ?? Color;

      Engine.CreateShape(coordinates, this);
    }
  }
}
