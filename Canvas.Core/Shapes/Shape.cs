using Canvas.Core.Composers;
using Canvas.Core.Engines;
using Canvas.Core.Models;
using Distribution.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.Shapes
{
  public interface IShape : ICloneable, IGroup
  {
    /// <summary>
    /// X
    /// </summary>
    double? X { get; set; }

    /// <summary>
    /// Y
    /// </summary>
    double? Y { get; set; }

    /// <summary>
    /// Z
    /// </summary>
    double? Z { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Reference to panel
    /// </summary>
    IEngine Engine { get; set; }

    /// <summary>
    /// Reference to composer
    /// </summary>
    IComposer Composer { get; set; }

    /// <summary>
    /// Options
    /// </summary>
    ComponentModel? Component { get; set; }

    /// <summary>
    /// Shape groups
    /// </summary>
    IDictionary<string, IShape> Groups { get; set; }

    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    double[] GetDomain(int index, string name, IList<IShape> items);

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    void CreateShape(int index, string name, IList<IShape> items);

    /// <summary>
    /// Get series
    /// </summary>
    /// <param name="view"></param>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    IDictionary<string, IList<double>> GetSeries(DataModel view, DataModel coordinates);

    /// <summary>
    /// Get series values
    /// </summary>
    /// <param name="view"></param>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    IList<double> GetSeriesValues(DataModel view, DataModel coordinates);

    /// <summary>
    /// Get specific group by position and name
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    IShape GetItem(int index, string name, IList<IShape> items);
  }

  public abstract class Shape : IShape
  {
    /// <summary>
    /// X
    /// </summary>
    public virtual double? X { get; set; }

    /// <summary>
    /// Y
    /// </summary>
    public virtual double? Y { get; set; }

    /// <summary>
    /// Z
    /// </summary>
    public virtual double? Z { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Reference to panel
    /// </summary>
    public virtual IEngine Engine { get; set; }

    /// <summary>
    /// Reference to composer
    /// </summary>
    public virtual IComposer Composer { get; set; }

    /// <summary>
    /// Options
    /// </summary>
    public virtual ComponentModel? Component { get; set; }

    /// <summary>
    /// Shape groups
    /// </summary>
    public virtual IDictionary<string, IShape> Groups { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public Shape()
    {
      Groups = new Dictionary<string, IShape>();
    }

    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual double[] GetDomain(int index, string name, IList<IShape> items)
    {
      return new[]
      {
        Y.Value,
        Y.Value
      };
    }

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual void CreateShape(int index, string name, IList<IShape> items)
    {
    }

    /// <summary>
    /// Get series
    /// </summary>
    /// <param name="view"></param>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public virtual IDictionary<string, IList<double>> GetSeries(DataModel view, DataModel coordinates)
    {
      var group = this;
      var groups = new Dictionary<string, IList<double>>();

      if (group?.Groups?.Count <= 0)
      {
        return new Dictionary<string, IList<double>>
        {
          [Composer.Name] = GetSeriesValues(view, coordinates)
        };
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
    /// Get series values
    /// </summary>
    /// <param name="view"></param>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public virtual IList<double> GetSeriesValues(DataModel view, DataModel coordinates)
    {
      return new[] { Y ?? 0.0 };
    }

    /// <summary>
    /// Get specific group by position and name
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    /// <summary>
    /// Get specific group by position and name
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual IShape GetItem(int index, string name, IList<IShape> items)
    {
      if (name is null)
      {
        return items.ElementAtOrDefault(index);
      }

      var group = items.ElementAtOrDefault(index);

      if (group?.Groups is null)
      {
        return null;
      }

      group.Groups.TryGetValue(Composer.Name, out IShape series);

      if (series?.Groups is null)
      {
        return null;
      }

      series.Groups.TryGetValue(name, out IShape shape);

      return shape;
    }

    /// <summary>
    /// Clone
    /// </summary>
    /// <returns></returns>
    /// <summary>
    /// Clone
    /// </summary>
    /// <returns></returns>
    public virtual object Clone()
    {
      var clone = MemberwiseClone() as IShape;

      clone.Groups = Groups.ToDictionary(o => o.Key, o => o.Value.Clone() as IShape);

      return clone;
    }

    /// <summary>
    /// Grouping index
    /// </summary>
    /// <returns></returns>
    public virtual long GetIndex() => (long)X;

    /// <summary>
    /// Grouping implementation
    /// </summary>
    /// <param name="previous"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    public virtual IGroup Combine(IGroup previous, IGroup current)
    {
      if (current is not null)
      {
        var currentItem = (current as IShape).Clone() as IShape;
        
        currentItem.Y = Y ?? currentItem.Y;

        return currentItem;
      }

      if (previous is not null)
      {
        return (previous as IShape).Clone() as IShape;
      }

      return this;
    }
  }
}
