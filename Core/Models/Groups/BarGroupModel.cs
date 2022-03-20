using Core.EngineSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Core.ModelSpace
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
    public override void CreateShape(int position, string name, IList<IGroupModel> items)
    {
      var currentModel = Composer.GetGroup(position, name, items);

      if (currentModel?.Point is null)
      {
        return;
      }

      var size = 1.0 / 4.0;

      var points = new IPointModel[]
      {
        Composer.GetPixels(View, position - size, currentModel.Point),
        Composer.GetPixels(View, position + size, currentModel.Point),
        Composer.GetPixels(View, position + size, 0.0),
        Composer.GetPixels(View, position - size, 0.0),
        Composer.GetPixels(View, position - size, currentModel.Point)
      };

      Color = currentModel.Color ?? Color;

      View.CreateShape(points, this);
    }
  }
}
