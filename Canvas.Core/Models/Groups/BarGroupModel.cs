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
    public override void CreateShape(int position, string name, IList<IPointModel> items)
    {
      var currentModel = Composer.GetPoint(position, name, items);

      if (currentModel?.Point is null)
      {
        return;
      }

      var size = 1.0 / 4.0;

      var points = new IPointModel[]
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
