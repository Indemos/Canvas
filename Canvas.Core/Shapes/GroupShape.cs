using Canvas.Core.Models;
using Distribution.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.Shapes
{
  public class GroupShape : Shape
  {
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

      group.Groups.TryGetValue(Composer?.Name ?? string.Empty, out IShape series);

      if (series?.Groups is null)
      {
        return null;
      }

      series.Groups.ForEach(o => groups[o.Key] = o.Value?.GetSeriesValues(view, coordinates));

      return groups;
    }

    /// <summary>
    /// Grouping implementation
    /// </summary>
    /// <param name="currentGroup"></param>
    /// <returns></returns>
    public override IGroup Combine(IGroup currentGroup)
    {
      var group = ((currentGroup ?? this) as IShape).Clone() as IShape;

      foreach (var sourceArea in Groups)
      {
        group.Groups[sourceArea.Key] = group.Groups.TryGetValue(sourceArea.Key, out IShape _) ?
          group.Groups[sourceArea.Key] :
          new GroupShape();

        foreach (var sourceSeries in sourceArea.Value.Groups)
        {
          var area = sourceArea.Key;
          var series = sourceSeries.Key;
          var shape = sourceSeries.Value;

          group.Groups[area].Groups[series] = shape.Combine(group.Groups[area].Groups[series]) as IShape;
        }
      }

      return group;
    }
  }
}
