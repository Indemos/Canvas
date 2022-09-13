using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.ModelSpace
{
  public interface IItemModel : ICloneable
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
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    double[] CreateDomain(int index, string name, IList<IItemModel> items);

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    void CreateShape(int index, string name, IList<IItemModel> items);

    /// <summary>
    /// Get series
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    IDictionary<string, IList<double>> GetSeries(IItemModel coordinates, IItemModel values);

    /// <summary>
    /// Get series values
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    IList<double> GetSeriesValues(IItemModel coordinates, IItemModel values);

    /// <summary>
    /// Get specific group by position and name
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    IItemModel GetItem(int index, string name, IList<IItemModel> items);
  }

  public class ItemModel : IItemModel
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
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual double[] CreateDomain(int index, string name, IList<IItemModel> items)
    {
      var currentModel = GetItem(index, name, items);

      if (currentModel?.Y is null)
      {
        return null;
      }

      return new double[]
      {
        currentModel.Y.Value,
        currentModel.Y.Value,
      };
    }

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual void CreateShape(int index, string name, IList<IItemModel> items)
    {
    }

    /// <summary>
    /// Get series
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual IDictionary<string, IList<double>> GetSeries(IItemModel coordinates, IItemModel values)
    {
      return new Dictionary<string, IList<double>>
      {
        [Composer?.Name ?? nameof(Composer.Name)] = GetSeriesValues(coordinates, values)
      };
    }

    /// <summary>
    /// Get series values
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual IList<double> GetSeriesValues(IItemModel coordinates, IItemModel values)
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
    public virtual IItemModel GetItem(int index, string name, IList<IItemModel> items)
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
