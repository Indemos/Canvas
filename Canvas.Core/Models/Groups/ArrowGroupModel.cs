using System.Collections.Generic;

namespace Canvas.Core.ModelSpace
{
  public class ArrowGroupModel : GroupModel, IGroupModel
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

      if (currentModel?.Point is null)
      {
        return;
      }

      var size = Composer.ItemSize / 2.0;

      var coordinates = new ItemModel[]
      {
        Composer.GetPixels(Engine, index, currentModel.Point),
        Composer.GetPixels(Engine, index + size, currentModel.Point),
        Composer.GetPixels(Engine, index - size, currentModel.Point)
      };

      coordinates[0].Value -= (coordinates[1].Index - coordinates[2].Index) * currentModel.Direction / 2;

      Color = currentModel.Color ?? Color;

      Engine.CreateShape(coordinates, this);
    }
  }
}
