using System;
using System.Collections.Generic;

namespace Canvas.Core.ModelSpace
{
  public class CandleGroupModel : GroupModel, IGroupModel
  {
    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override double[] CreateDomain(int index, string name, IList<IItemModel> items)
    {
      var currentModel = Value;

      if (currentModel is null)
      {
        return null;
      }

      return new double[]
      {
        currentModel.Low ?? currentModel.Point,
        currentModel.High ?? currentModel.Point
      };
    }

    /// <summary>
    /// Get values
    /// </summary>
    /// <returns></returns>
    public override IList<double> GetValues()
    {
      var L = Value.Low ?? Value.Point ?? 0;
      var H = Value.High ?? Value.Point ?? 0;
      var O = Value.Open ?? Value.Point ?? 0;
      var C = Value.Close ?? Value.Point ?? 0;

      return new double[] { O, H, L, C };
    }

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

      if (currentModel is null)
      {
        return;
      }

      var L = currentModel.Low ?? currentModel.Point;
      var H = currentModel.High ?? currentModel.Point;
      var O = currentModel.Open ?? currentModel.Point;
      var C = currentModel.Close ?? currentModel.Point;
      var size = Composer.ItemSize / 2.0;
      var downSide = Math.Min(O, C);
      var upSide = Math.Max(O, C);

      var coordinates = new IItemModel[]
      {
        Composer.GetPixels(Engine, index - size, upSide),
        Composer.GetPixels(Engine, index + size, downSide)
      };

      var rangeCoordinates = new IItemModel[]
      {
        Composer.GetPixels(Engine, index, L),
        Composer.GetPixels(Engine, index, H),
      };

      Color = currentModel.Color ?? Color;

      Engine.CreateBox(coordinates, this);
      Engine.CreateLine(rangeCoordinates, this);
    }
  }
}
