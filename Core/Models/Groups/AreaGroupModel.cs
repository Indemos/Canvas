using System.Collections.Generic;

namespace Core.ModelSpace
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
    public override void CreateShape(int position, string name, IList<IGroupModel> items)
    {
      var currentModel = Composer.GetGroup(position, name, items);
      var previousModel = Composer.GetGroup(position - 1, name, items);

      if (currentModel?.Point is null || previousModel?.Point is null)
      {
        return;
      }

      var points = new IPointModel[]
      {
        Composer.GetPixels(View, position - 1, previousModel.Point),
        Composer.GetPixels(View, position, currentModel.Point),
        Composer.GetPixels(View, position, 0.0),
        Composer.GetPixels(View, position - 1, 0.0),
        Composer.GetPixels(View, position - 1, previousModel.Point)
      };

      Color = currentModel.Color ?? Color;

      View.CreateShape(points, this);
    }
  }
}
