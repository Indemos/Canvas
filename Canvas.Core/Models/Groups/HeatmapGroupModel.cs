using Canvas.Core.ServiceSpace;
using System.Collections.Generic;
using System.Linq;

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
      var points = Value?.Points as IList<double>;

      if (points is null)
      {
        return null;
      }

      return new double[] { points.Min(), points.Max() };
    }

    /// <summary>
    /// Get values
    /// </summary>
    /// <returns></returns>
    public override IList<double> GetValues()
    {
      var points = Value?.Points as IList<double>;

      if (points is null)
      {
        return null;
      }

      return new double[] { points.Count };
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

      foreach (var point in points)
      {
        var open = Composer.GetPixels(Engine, index, 0.0);
        var close = Composer.GetPixels(Engine, index + 1, 0.0);

        open.Value = coordinate;
        close.Value = coordinate + coordinateStep;
        coordinate += coordinateStep;

        var coordinates = new IItemModel[] { open, close };

        Color = currentModel.Color ?? Composer?.ColorService?.GetColor(point) ?? Color;

        Engine.CreateBox(coordinates, this);
      }
    }
  }
}
