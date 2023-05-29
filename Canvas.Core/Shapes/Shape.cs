using Canvas.Core.Composers;
using Canvas.Core.Engines;
using Canvas.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.Shapes
{
  public interface IShape : ICloneable
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

  public class Shape : IShape
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
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual double[] GetDomain(int index, string name, IList<IShape> items)
    {
      var current = GetItem(index, name, items);

      if (current?.Y is null)
      {
        return null;
      }

      return new double[]
      {
        current.Y.Value,
        current.Y.Value
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
      return new Dictionary<string, IList<double>>
      {
        [Composer.Name] = GetSeriesValues(view, coordinates)
      };
    }

    /// <summary>
    /// Get series values
    /// </summary>
    /// <param name="view"></param>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public virtual IList<double> GetSeriesValues(DataModel view, DataModel coordinates)
    {
      return new double[] { Y ?? 0 };
    }

    /// <summary>
    /// Get specific group by position and name
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual IShape GetItem(int index, string name, IList<IShape> items)
    {
      return items.ElementAtOrDefault(index);
    }

    /// <summary>
    /// Clone
    /// </summary>
    /// <returns></returns>
    public virtual object Clone() => MemberwiseClone();
  }
}
