using Canvas.Core.ModelSpace;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.ShapeSpace
{
  public interface IGroupShape : IShape
  {
    /// <summary>
    /// Shape groups
    /// </summary>
    IDictionary<string, IGroupShape> Groups { get; set; }
  }

  public class GroupShape : Shape, IGroupShape
  {
    /// <summary>
    /// Shape groups
    /// </summary>
    public virtual IDictionary<string, IGroupShape> Groups { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public GroupShape()
    {
      Groups = new Dictionary<string, IGroupShape>(); 
    }

    /// <summary>
    /// Get series
    /// </summary>
    /// <param name="view"></param>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public override IDictionary<string, IList<double>> GetSeries(DataModel view, DataModel coordinates)
    {
      var group = this;
      var groups = new Dictionary<string, IList<double>>();

      if (group?.Groups?.Count <= 0)
      {
        return base.GetSeries(view, coordinates);
      }

      group.Groups.TryGetValue(Composer?.Name ?? string.Empty, out IGroupShape series);

      if (series?.Groups is null)
      {
        return null;
      }

      series.Groups.ForEach(o => groups[o.Key] = o.Value?.GetSeriesValues(view, coordinates));

      return groups;
    }

    /// <summary>
    /// Get specific group by position and name
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override IShape GetItem(int index, string name, IList<IShape> items)
    {
      if (name is null)
      {
        return base.GetItem(index, null, items);
      }

      var group = items.ElementAtOrDefault(index) as IGroupShape;

      if (group?.Groups is null)
      {
        return null;
      }

      group.Groups.TryGetValue(Composer.Name, out IGroupShape series);

      if (series?.Groups is null)
      {
        return null;
      }

      series.Groups.TryGetValue(name, out IGroupShape shape);

      return shape;
    }

    /// <summary>
    /// Clone
    /// </summary>
    /// <returns></returns>
    public override object Clone()
    {
      var clone = base.Clone() as IGroupShape;

      clone.Groups = Groups.ToDictionary(o => o.Key, o => o.Value.Clone() as IGroupShape);

      return clone;
    }
  }
}
