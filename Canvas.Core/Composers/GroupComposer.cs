using Canvas.Core.Extensions;
using Canvas.Core.Models;
using System;
using System.Linq;

namespace Canvas.Core.Composers
{
  public class GroupComposer : Composer
  {
    /// <summary>
    /// Update items
    /// </summary>
    /// <param name="dimension"></param>
    /// <returns></returns>
    public override Scope Render(Dimension domain)
    {
      Engine.Clear();

      var dimension = SetDimensions(domain);
      var minIndex = dimension.MinIndex;
      var maxIndex = dimension.MaxIndex;

      for (var i = minIndex; i < maxIndex; i++)
      {
        var group = Items.ElementAtOrDefault(i);
        var grouSeries = group?.Groups?.Get(Name);

        if (grouSeries is null)
        {
          continue;
        }

        foreach (var (name, shape) in grouSeries.Groups)
        {
          shape.Composer = this;
          shape.CreateShape(i, name, Items);
        }
      }

      return new Scope
      {
        Values = GetValues(),
        Indices = GetIndices()
      };
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
      var series = group?.Groups?.Get(Name);

      if (series is null)
      {
        return (min, max, average);
      }

      foreach (var (name, shape) in series.Groups)
      {
        shape.Composer = this;

        var domain = shape.GetDomain(i, name, Items);

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
