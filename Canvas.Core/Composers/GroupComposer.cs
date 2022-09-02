using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
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
    public override void UpdateItems(IEngine engine)
    {
      foreach (var i in GetEnumerator())
      {
        var group = Items.ElementAtOrDefault(i) as IGroupModel;

        if (group?.Groups is null || group.Groups.TryGetValue(Name, out IGroupModel seriesGroup) is false)
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
    /// Create Min and Max domain 
    /// </summary>
    /// <returns></returns>
    public override IList<double> GetValueDomain()
    {
      var average = 0.0;
      var min = double.MaxValue;
      var max = double.MinValue;
      var domain = new[] { 0.0, 0.0 };

      foreach (var i in GetEnumerator())
      {
        var group = Items.ElementAtOrDefault(i) as IGroupModel;

        if (group?.Groups is null || group.Groups.TryGetValue(Name, out IGroupModel series) is false)
        {
          continue;
        }

        foreach (var shape in series.Groups)
        {
          shape.Value.Composer = this;

          var itemDomain = shape.Value.CreateDomain(i, shape.Key, Items);

          if (itemDomain is not null)
          {
            min = Math.Min(min, itemDomain[0]);
            max = Math.Max(max, itemDomain[1]);
            average += max - min;
          }
        }
      }

      if (min > max)
      {
        return null;
      }

      if (min == max)
      {
        domain[0] = Math.Min(0, min);
        domain[1] = Math.Max(0, max);

        return domain;
      }

      if (min < 0 && max > 0)
      {
        var extreme = Math.Max(Math.Abs(min), Math.Abs(max));

        domain[0] = -extreme;
        domain[1] = extreme;

        return domain;
      }

      domain[0] = min;
      domain[1] = max;

      return domain;
    }
  }
}
