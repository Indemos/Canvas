using Distribution.Collections;
using System.Collections.Generic;

namespace Canvas.Core.Shapes
{
  public class GroupShape : Shape
  {
    /// <summary>
    /// Grouping implementation
    /// </summary>
    /// <param name="previous"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    public override IGroup Combine(IGroup previous, IGroup current)
    {
      var group = ((current ?? this) as IShape).Clone() as IShape;

      foreach (var sourceArea in Groups)
      {
        foreach (var sourceSeries in sourceArea.Value.Groups)
        {
          var previousArea = Map((previous as IShape)?.Groups, sourceArea.Key);
          var previousSeries = Map(previousArea?.Groups, sourceSeries.Key);
          var currentArea = group.Groups[sourceArea.Key] = Map(
            group.Groups,
            sourceArea.Key,
            previousArea?.Clone() as IShape);

          currentArea.Groups[sourceSeries.Key] = sourceSeries
            .Value
            .Combine(previousSeries, Map(currentArea.Groups, sourceSeries.Key)) as IShape;
        }
      }

      return group;
    }

    /// <summary>
    /// Get dictionary value
    /// </summary>
    /// <param name="map"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected virtual IShape Map(IDictionary<string, IShape> map, string index, IShape value = null)
    {
      return map is not null && index is not null && map.TryGetValue(index, out var o) ? o : value;
    }
  }
}
