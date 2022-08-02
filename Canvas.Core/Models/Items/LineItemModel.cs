using System.Collections.Generic;

namespace Canvas.Core.ModelSpace
{
  public class LineItemModel : GroupModel, IGroupModel
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
      var currentModel = Y;
      var previousModel = GetItem(index - 1, name, items)?.Y;

      if (currentModel is null)
      {
        return;
      }

      var coordinates = new IItemModel[]
      {
        Composer.GetPixels(Engine, index, (previousModel ?? currentModel).Value),
        Composer.GetPixels(Engine, index + 1, currentModel.Value)
      };

      Engine.CreateLine(coordinates, this);
    }
  }
}
