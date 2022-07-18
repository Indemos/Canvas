using System.Collections.Generic;

namespace Canvas.Core.ModelSpace
{
  public class HeatmapGroupModel : GroupModel, IGroupModel
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

      var points = currentModel.Points as IList<double>;

      return new double[] { 0, points.Count };
    }

    /// <summary>
    /// Get values
    /// </summary>
    /// <returns></returns>
    public override IList<double> GetValues()
    {
      var currentModel = Value;

      if (currentModel is null)
      {
        return null;
      }

      var points = currentModel.Points as IList<double>;

      return new double[] { 0, points.Count };
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
      var points = currentModel.Points as IList<double>;
      var pointsCount = points?.Count ?? 0;

      if (Equals(pointsCount, 0))
      {
        return;
      }

      var coordinate = 0.0;
      var coordinateStep = Engine.ValueSize / pointsCount;

      foreach (var color in points)
      {
        var open = Composer.GetPixels(Engine, index - 1, 0.0);
        var close = Composer.GetPixels(Engine, index, 0.0);

        open.Value = coordinate;
        close.Value = coordinate + coordinateStep;
        coordinate =+ coordinateStep;

        var coordinates = new IItemModel[] { open, close };

        Color = currentModel.Color ?? Color;

        Engine.CreateShape(coordinates, this);
      }
    }
  }
}
