using Canvas.Core.ShapeSpace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.ModelSpace
{
  public class ColorMapShape : GroupShape, IGroupShape
  {
    /// <summary>
    /// Points
    /// </summary>
    public virtual IList<ComponentModel> Points { get; set; }

    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override double[] GetDomain(int index, string name, IList<IShape> items) => new double[] { 0, Points.Count };

    /// <summary>
    /// Get series values
    /// </summary>
    /// <param name="view"></param>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public override IList<double> GetSeriesValues(DataModel view, DataModel coordinates)
    {
      var indexRatio = Math.Max(coordinates.X / view.X, 0.0);
      var indexPosition = (int)Math.Round(Composer.Items.Count * indexRatio, MidpointRounding.ToZero);
      var item = Composer.Items.ElementAtOrDefault(indexPosition) as ColorMapShape;

      var valueRatio = Math.Max(coordinates.Y / view.Y, 0.0);
      var valuePosition = (int)Math.Round(item.Points.Count * valueRatio, MidpointRounding.ToZero);
      var value = item.Points.ElementAtOrDefault(item.Points.Count - valuePosition - 1).Size;

      return new double[] { value };
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

      var step = (Composer.Domain.MaxValue - Composer.Domain.MinValue) / pointsCount;

      for (var i = 0; i < Points.Count; i++)
      {
        var open = Composer.GetPixels(Engine, index, i);
        var close = Composer.GetPixels(Engine, index + 1, i + step);
        var points = new DataModel[] { open, close };

        Engine.CreateBox(points, Points[i]);
      }
    }
  }
}
