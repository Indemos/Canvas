using Canvas.Core.Engines;
using Canvas.Core.Models;
using Canvas.Core.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.Composers
{
  public class GroupComposer : Composer
  {
    /// <summary>
    /// Update items
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="domain"></param>
    /// <returns></returns>
    public override void GetItems(IEngine engine, DomainModel domain)
    {
      View.Values = GetValues();
      View.Indices = GetIndices();

      for (var i = domain.MinIndex; i < domain.MaxIndex; i++)
      {
        var group = Items.ElementAtOrDefault(i);

        if (group?.Groups is null || group.Groups.TryGetValue(Name, out IShape seriesGroup) is false)
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
    /// Update items
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="domain"></param>
    /// <returns></returns>
    public override void GetSamples(IEngine engine, DomainModel domain)
    {
      if (Rate is 0)
      {
        GetItems(engine, domain);
        return;
      }

      var min = new Dictionary<string, double>();
      var max = new Dictionary<string, double>();
      var minItem = new Dictionary<string, IShape>();
      var maxItem = new Dictionary<string, IShape>();
      var range = domain.MaxIndex - domain.MinIndex;
      var sampleRate = Math.Max(Math.Round(range / engine.X, MidpointRounding.ToZero), Rate);

      View.Values = GetValues();
      View.Indices = GetIndices();

      for (var i = domain.MinIndex; i < domain.MaxIndex; i++)
      {
        var group = Items.ElementAtOrDefault(i);

        if (group?.Groups is null || group.Groups.TryGetValue(Name, out IShape seriesGroup) is false)
        {
          continue;
        }

        foreach (var series in seriesGroup.Groups)
        {
          var name = series.Key;
          var item = series.Value;
          var itemDomain = item.GetDomain(i, name, Items);

          if (min.ContainsKey(name) is false || itemDomain[0] <= min[name])
          {
            min[name] = itemDomain[0];
            minItem[name] = item;
          }

          if (max.ContainsKey(name) is false || itemDomain[1] >= max[name])
          {
            max[name] = itemDomain[1];
            maxItem[name] = item;
          }

          if (Equals(i, domain.MinIndex) || Equals(i, domain.MaxIndex) || (i % sampleRate is 0))
          {
            switch (true)
            {
              case true when Math.Abs(min[name]) > Math.Abs(max[name]): item = minItem[name]; break;
              case true when Math.Abs(min[name]) < Math.Abs(max[name]): item = maxItem[name]; break;
            }

            item.Engine = engine;
            item.Composer = this;
            item.CreateShape(i, name, Items);

            min.Remove(name);
            max.Remove(name);
          }
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
      var group = Items.ElementAtOrDefault(i);

      if (group?.Groups is null || group.Groups.TryGetValue(Name, out IShape series) is false)
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
