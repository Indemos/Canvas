using System.Collections.Generic;

namespace Canvas.Core.ModelSpace
{
  public class AreaGroupModel : GroupModel, IGroupModel
  {
    /// <summary>
    /// Render the shape
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override void CreateShape(int position, string name, IList<IItemModel> items)
    {
      var currentModel = Value;
      var previousModel = GetItem(position - 1, name, items);

      if (currentModel?.Point is null || previousModel?.Point is null)
      {
        return;
      }

      var points = new IItemModel[]
      {
        Composer.GetPixels(Engine, position - 1, previousModel.Point),
        Composer.GetPixels(Engine, position, currentModel.Point),
        Composer.GetPixels(Engine, position, 0.0),
        Composer.GetPixels(Engine, position - 1, 0.0),
        Composer.GetPixels(Engine, position - 1, previousModel.Point)
      };

      Color = currentModel.Color ?? Color;

      Engine.CreateShape(points, this);
    }
  }
}
