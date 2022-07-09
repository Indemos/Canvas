using System.Collections.Generic;

namespace Canvas.Core.ModelSpace
{
  public class ArrowGroupModel : GroupModel, IGroupModel
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

      var points = new ItemModel[]
      {
        Composer.GetPixels(Engine, position, currentModel.Point),
        Composer.GetPixels(Engine, position + size, currentModel.Point),
        Composer.GetPixels(Engine, position - size, currentModel.Point)
      };

      points[0].Value -= (points[1].Index - points[2].Index) * currentModel.Direction / 2;

      Color = currentModel.Color ?? Color;

      Engine.CreateShape(points, this);
    }
  }
}
