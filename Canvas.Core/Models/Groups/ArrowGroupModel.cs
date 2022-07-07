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
    public override void CreateShape(int position, string name, IList<IPointModel> items)
    {
      var currentModel = Composer.GetPoint(position, name, items);

      if (currentModel?.Point is null)
      {
        return;
      }

      var points = new PointModel[]
      {
        Composer.GetPixels(Engine, position, currentModel.Point),
        Composer.GetPixels(Engine, position + Composer.ItemSize, currentModel.Point),
        Composer.GetPixels(Engine, position - Composer.ItemSize, currentModel.Point)
      };

      points[0].Value -= (points[1].Index - points[2].Index) * currentModel.Direction / 2;

      Color = currentModel.Color ?? Color;

      Engine.CreateShape(points, this);
    }
  }
}
