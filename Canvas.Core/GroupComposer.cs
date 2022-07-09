using Canvas.Core.ModelSpace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core
{
  public class GroupComposer : Composer
  {
    /// <summary>
    /// Create Min and Max domain 
    /// </summary>
    /// <returns></returns>
    protected override IList<double> CreateValueDomain()
    {
      var average = 0.0;
      var min = double.MaxValue;
      var max = double.MinValue;

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
          shape.Value.Engine = Engine;

          var domain = shape.Value.CreateDomain(i, shape.Key, Items);

          if (domain is not null)
          {
            min = Math.Min(min, domain[0]);
            max = Math.Max(max, domain[1]);
            average += max - min;
          }
        }
      }

      AutoValueDomain ??= new double[2];

      if (min > max)
      {
        return AutoValueDomain = null;
      }

      if (min == max)
      {
        AutoValueDomain[0] = Math.Min(0, min);
        AutoValueDomain[1] = Math.Max(0, max);

        return AutoValueDomain;
      }

      if (min < 0 && max > 0)
      {
        var extreme = Math.Max(Math.Abs(min), Math.Abs(max));

        AutoValueDomain[0] = -extreme;
        AutoValueDomain[1] = extreme;

        return AutoValueDomain;
      }

      AutoValueDomain[0] = min;
      AutoValueDomain[1] = max;

      return AutoValueDomain;
    }

    /// <summary>
    /// Update series and collections
    /// </summary>
    protected override void UpdateItems()
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
          series.Value.Engine = Engine;
          series.Value.Composer = this;
          series.Value.CreateShape(i, series.Key, Items);
        }
      }
    }
  }
}
