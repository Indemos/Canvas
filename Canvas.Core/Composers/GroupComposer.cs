using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using Canvas.Core.ShapeSpace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.ComposerSpace
{
  public class GroupComposer : Composer
  {
    /// <summary>
    /// Update items
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="domain"></param>
    /// <returns></returns>
    public override void UpdateItems(IEngine engine, DomainModel domain)
    {
      for (var i = domain.MinIndex; i < domain.MaxIndex; i++)
      {
        var group = Items.ElementAtOrDefault(i) as IGroupShape;

        if (group?.Groups is null || group.Groups.TryGetValue(Name, out IGroupShape seriesGroup) is false)
        {
          continue;
        }

        foreach (var series in seriesGroup.Groups)
        {
          series.Value.Engine = engine;
          series.Value.Composer = this;
          series.Value.CreateShape(i, series.Key, Items);
        }
      }
    }

    /// <summary>
    /// Calculate min and max for value domain
    /// </summary>
    /// <param name="i"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="average"></param>
    /// <returns></returns>
    protected override (double, double, double) GetExtremes(int i, double min, double max, double average)
    {
      var group = Items.ElementAtOrDefault(i) as IGroupShape;

      if (group?.Groups is null || group.Groups.TryGetValue(Name, out IGroupShape series) is false)
      {
        return (min, max, average);
      }

      foreach (var shape in series.Groups)
      {
        shape.Value.Composer = this;

        var domain = shape.Value.GetDomain(i, shape.Key, Items);

        if (domain is not null)
        {
          min = Math.Min(min, domain[0]);
          max = Math.Max(max, domain[1]);
          average += max - min;
        }
      }

      return (min, max, average);
    }
  }
}
