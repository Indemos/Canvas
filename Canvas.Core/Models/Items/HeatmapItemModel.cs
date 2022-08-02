using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.ModelSpace
{
  public class HeatmapItemModel : GroupModel, IGroupModel
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
      if (Points is null)
      {
        return null;
      }

      return new double[] { Points.Min(o => o.Y ?? 0), Points.Max(o => o.Y ?? 0) };
    }

    /// <summary>
    /// Get series values
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public override IList<double> GetSeriesValues(IItemModel coordinates, IItemModel values)
    {
      if (Equals(Points.Count, 0))
      {
        return new double[] { 0 };
      }

      var percentage = coordinates.Y / Composer.Engine.ValueSize;
      var position = Points.Count * percentage;
      var point = Points.ElementAtOrDefault((int)position);

      return new double[] { point?.Z ?? 0 };
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
      var currentModel = Y;
      var pointsCount = Points?.Count ?? 0;

      if (Equals(pointsCount, 0))
      {
        return;
      }

      var coordinate = 0.0;
      var coordinateStep = Engine.ValueSize / pointsCount;

      foreach (var point in Points)
      {
        var open = Composer.GetPixels(Engine, index, 0.0);
        var close = Composer.GetPixels(Engine, index + 1, 0.0);

        open.Y = coordinate;
        close.Y = coordinate + coordinateStep;
        coordinate += coordinateStep;

        var coordinates = new IItemModel[] { open, close };

        Color = Composer?.ColorService?.GetColor(point.Z.Value) ?? Color;

        Engine.CreateBox(coordinates, this);
      }
    }
  }
}
