using Canvas.Core.ShapeSpace;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.ModelSpace
{
  public class ColorMapShape : GroupShape, IGroupShape
  {
    /// <summary>
    /// Points
    /// </summary>
    public virtual IList<IShape> Points { get; set; }

    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override double[] GetDomain(int index, string name, IList<IShape> items)
    {
      if (Points is null)
      {
        return null;
      }

      return new double[] { Points.Min(o => o.Data?.Y ?? 0), Points.Max(o => o.Data?.Y ?? 0) };
    }

    /// <summary>
    /// Get series values
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public override IList<double> GetSeriesValues(DataModel coordinates, DataModel values)
    {
      if (Equals(Points.Count, 0))
      {
        return new double[] { 0 };
      }

      var item = values;
      var min = Composer.Domain.MinValue;
      var points = Composer.Items.ElementAt((int)values.X) as ColorMapShape;

      foreach (var point in points.Points)
      {
        var data = point.Data.Value;

        if (values.Y - data.Y <= values.Y - min && data.Y <= values.Y)
        {
          min = data.Y.Value;
          item = data;
        }
      }

      return new double[] { item.Z ?? 0 };
    }

    /// <summary>
    /// Render the shape
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override void CreateShape(int index, string name, IList<IShape> items)
    {
      var pointsCount = Points?.Count ?? 0;

      if (Equals(pointsCount, 0))
      {
        return;
      }

      var step = (Composer.Domain.MaxValue - Composer.Domain.MinValue + 1) / pointsCount;

      foreach (var point in Points)
      {
        var data = point.Data.Value;
        var open = Composer.GetPixels(Engine, index, data.Y.Value);
        var close = Composer.GetPixels(Engine, index + 1, data.Y.Value + step);
        var points = new DataModel[] { open, close };

        Engine.CreateBox(points, point.Component ?? Composer.Shape);
      }
    }
  }
}
