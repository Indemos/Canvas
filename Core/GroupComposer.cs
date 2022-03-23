using Core.EngineSpace;
using Core.ModelSpace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
  public class GroupComposer : Composer
  {
    /// <summary>
    /// Get specific group by position and name
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override dynamic GetPoint(int position, string name, IList<IPointModel> items)
    {
      var point = items.ElementAtOrDefault(position);
      var group = point as IGroupModel;

      if (group?.Groups is null)
      {
        return null;
      }

      group.Groups.TryGetValue(Name, out IGroupModel series);

      if (series?.Groups is null)
      {
        return null;
      }

      series.Groups.TryGetValue(name, out IGroupModel shape);

      return shape?.Value;
    }

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
        var group = Points.ElementAtOrDefault(i) as IGroupModel;

        if (group?.Groups is null || group.Groups.TryGetValue(Name, out IGroupModel series) is false)
        {
          continue;
        }

        foreach (var shape in series.Groups)
        {
          shape.Value.Composer = this;

          var domain = shape.Value.CreateDomain(i, shape.Key, Points);

          if (domain is not null)
          {
            min = Math.Min(min, domain[0]);
            max = Math.Max(max, domain[1]);
            average += max - min;
          }
        }
      }

      if (min > max)
      {
        return AutoValueDomain = null;
      }

      AutoValueDomain ??= new double[2];
      AutoValueDomain[0] = min;
      AutoValueDomain[1] = max;

      if (min < 0 && max > 0)
      {
        var domain = Math.Max(Math.Abs(max), Math.Abs(min));

        AutoValueDomain[0] = -domain;
        AutoValueDomain[1] = domain;
      }

      return AutoValueDomain;
    }

    /// <summary>
    /// Update series and collections
    /// </summary>
    protected override void UpdatePoints()
    {
      foreach (var i in GetEnumerator())
      {
        var group = Points.ElementAtOrDefault(i) as IGroupModel;

        if (group?.Groups is null || group.Groups.TryGetValue(Name, out IGroupModel seriesGroup) is false)
        {
          continue;
        }

        foreach (var series in seriesGroup.Groups)
        {
          series.Value.Engine = Engine;
          series.Value.Composer = this;
          series.Value.CreateShape(i, series.Key, Points);
        }
      }
    }
  }
}
