using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.ShapeSpace
{
  public interface IShape : ICloneable
  {
    /// <summary>
    /// Data
    /// </summary>
    DataModel? Data { get; set; }

    /// <summary>
    /// Shape
    /// </summary>
    ComponentModel? Component { get; set; }

    /// <summary>
    /// Reference to panel
    /// </summary>
    IEngine Engine { get; set; }

    /// <summary>
    /// Reference to composer
    /// </summary>
    IComposer Composer { get; set; }

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
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    IDictionary<string, IList<double>> GetSeries(DataModel coordinates, DataModel values);

    /// <summary>
    /// Get series values
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    IList<double> GetSeriesValues(DataModel coordinates, DataModel values);

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
    /// Data
    /// </summary>
    public virtual DataModel? Data { get; set; }

    /// <summary>
    /// Shape
    /// </summary>
    public virtual ComponentModel? Component { get; set; }

    /// <summary>
    /// Reference to panel
    /// </summary>
    public virtual IEngine Engine { get; set; }

    /// <summary>
    /// Reference to composer
    /// </summary>
    public virtual IComposer Composer { get; set; }

    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual double[] GetDomain(int index, string name, IList<IShape> items)
    {
      var currentModel = GetItem(index, name, items);

      if (currentModel?.Data?.Y is null)
      {
        return null;
      }

      return new double[]
      {
        currentModel.Data.Value.Y,
        currentModel.Data.Value.Y,
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
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual IDictionary<string, IList<double>> GetSeries(DataModel coordinates, DataModel values)
    {
      return new Dictionary<string, IList<double>>
      {
        [nameof(Composer.Domain)] = GetSeriesValues(coordinates, values)
      };
    }

    /// <summary>
    /// Get series values
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual IList<double> GetSeriesValues(DataModel coordinates, DataModel values)
    {
      return new double[] { Data?.Y ?? 0 };
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
