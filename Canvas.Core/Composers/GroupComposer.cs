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
      foreach (var i in GetEnumerator(domain))
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
    /// Get min and max values
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    protected override IList<double> ComposeValueDomain(DomainModel domain)
    {
      var average = 0.0;
      var min = double.MaxValue;
      var max = double.MinValue;
      var response = new[] { 0.0, 0.0 };

      foreach (var i in GetEnumerator(domain))
      {
        var group = Items.ElementAtOrDefault(i) as IGroupShape;

        if (group?.Groups is null || group.Groups.TryGetValue(Name, out IGroupShape series) is false)
        {
          continue;
        }

        foreach (var shape in series.Groups)
        {
          shape.Value.Composer = this;

          var itemDomain = shape.Value.GetDomain(i, shape.Key, Items);

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
        response[0] = Math.Min(0, min);
        response[1] = Math.Max(0, max);

        return response;
      }

      if (min < 0 && max > 0)
      {
        var extreme = Math.Max(Math.Abs(min), Math.Abs(max));

        response[0] = -extreme;
        response[1] = extreme;

        return response;
      }

      response[0] = min;
      response[1] = max;

      return response;
    }
  }
}
