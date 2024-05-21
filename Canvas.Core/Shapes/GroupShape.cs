using Canvas.Core.Models;
using Distribution.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.Shapes
{
  public class GroupShape : Shape
  {
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
