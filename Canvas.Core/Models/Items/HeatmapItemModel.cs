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
    public override double[] GetDomain(int index, string name, IList<IItemModel> items)
    {
      if (Items is null)
      {
        return null;
      }

      return new double[] { Items.Min(o => o.Y ?? 0), Items.Max(o => o.Y ?? 0) };
    }

    /// <summary>
    /// Get series values
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public override IList<double> GetSeriesValues(IItemModel coordinates, IItemModel values)
    {
      if (Equals(Items.Count, 0))
      {
        return new double[] { 0 };
      }

      var item = values;
      var min = Composer.Domain.MinValue;
      var points = Composer.Items.ElementAt((int)values.X) as HeatmapItemModel;

      foreach (var point in points.Items)
      {
        if (values.Y - point.Y <= values.Y - min && point.Y <= values.Y)
        {
          min = point.Y.Value;
          item = point;
        }
      }

      return new double[] { item?.Z ?? 0 };
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
      var pointsCount = Items?.Count ?? 0;

      if (Equals(pointsCount, 0))
      {
        return;
      }

      var step = (Composer.Domain.MaxValue - Composer.Domain.MinValue + 1) / pointsCount;

      foreach (var point in Items)
      {
        var open = Composer.GetPixels(Engine, index, point.Y.Value);
        var close = Composer.GetPixels(Engine, index + 1, point.Y.Value + step);
        var points = new IItemModel[] { open, close };

        Color = point.Color ?? Color;

        Engine.CreateBox(points, this);
      }
    }
  }
}
