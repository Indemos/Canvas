using System.Collections.Generic;

namespace Canvas.Core.ModelSpace
{
  public class LineGroupModel : GroupModel, IGroupModel
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
      var previousModel = Composer.GetPoint(position - 1, name, items);

      if (currentModel?.Point is null || previousModel?.Point is null)
      {
        return;
      }

      var points = new IPointModel[]
      {
        Composer.GetPixels(Engine, position - 1, previousModel.Point),
        Composer.GetPixels(Engine, position, currentModel.Point)
      };

      Color = currentModel.Color ?? Color;

      Engine.CreateLine(points, this);
    }
  }
}
