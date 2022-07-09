using System.Collections.Generic;

namespace Canvas.Core.ModelSpace
{
  public class BarGroupModel : GroupModel, IGroupModel
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

      if (currentModel?.Point is null)
      {
        return;
      }

      var size = Composer.ItemSize / 2.0;

      var points = new IItemModel[]
      {
        Composer.GetPixels(Engine, position - size, currentModel.Point),
        Composer.GetPixels(Engine, position + size, currentModel.Point),
        Composer.GetPixels(Engine, position + size, 0.0),
        Composer.GetPixels(Engine, position - size, 0.0),
        Composer.GetPixels(Engine, position - size, currentModel.Point)
      };

      Color = currentModel.Color ?? Color;

      Engine.CreateShape(points, this);
    }
  }
}
