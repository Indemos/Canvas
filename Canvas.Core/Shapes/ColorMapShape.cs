using Canvas.Core.Composers;
using Canvas.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.Shapes
{
  public class ColorMapShape : Shape
  {
    /// <summary>
    /// Points
    /// </summary>
    public virtual IList<ComponentModel?> Points { get; set; }

    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override double[] GetDomain(int index, string name, IList<IShape> items) => [0, Points.Count];

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
      var value = item.Points.ElementAtOrDefault(item.Points.Count - valuePosition - 1)?.Size ?? 0;

      return [value];
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
      var dimension = (Composer as MapComposer).Dimension;
      var range = Composer.Domain.MaxValue - Composer.Domain.MinValue;
      var previous = Points.FirstOrDefault();
      var step = range / dimension;

      for (var i = 0; i < dimension; i++)
      {
        var open = Composer.GetItemPosition(index, i);
        var close = Composer.GetItemPosition(index + 1, i + step);
        var points = new DataModel[] { open, close };

        previous = Points.ElementAtOrDefault(i) ?? previous;

        Composer.Engine.CreateBox(points, previous.Value);
      }
    }
  }
}
