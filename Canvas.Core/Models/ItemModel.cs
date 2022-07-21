using Canvas.Core.EngineSpace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.ModelSpace
{
  public interface IItemModel : ICloneable
  {
    /// <summary>
    /// Index
    /// </summary>
    double? Index { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    dynamic Value { get; set; }

    /// <summary>
    /// Reference to composer
    /// </summary>
    Composer Composer { get; set; }

    /// <summary>
    /// Reference to panel
    /// </summary>
    IEngine Engine { get; set; }

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
    /// Get series by position
    /// </summary>
    /// <param name="index"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    IDictionary<string, IList<double>> GetSeries(int index, IList<IItemModel> items);

    /// <summary>
    /// Get values
    /// </summary>
    /// <returns></returns>
    IList<double> GetValues();

    /// <summary>
    /// Get specific group by position and name
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    dynamic GetItem(int index, string name, IList<IItemModel> items);
  }

  public class ItemModel : IItemModel
  {
    /// <summary>
    /// Index
    /// </summary>
    public virtual double? Index { get; set; }

    /// <summary>
    /// Model that may contain arbitrary data needed to draw the shape
    /// </summary>
    public virtual dynamic Value { get; set; }

    /// <summary>
    /// Reference to composer
    /// </summary>
    public virtual Composer Composer { get; set; }

    /// <summary>
    /// Reference to panel
    /// </summary>
    public virtual IEngine Engine { get; set; }

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

      if (currentModel?.Point is null)
      {
        return null;
      }

      return new double[]
      {
        currentModel.Point,
        currentModel.Point
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
    /// Get series by position
    /// </summary>
    /// <param name="index"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual IDictionary<string, IList<double>> GetSeries(int index, IList<IItemModel> items)
    {
      var point = GetItem(index, null, items);
      var groups = new Dictionary<string, IList<double>>();

      if (point is null)
      {
        return groups;
      }

      groups[Composer.Name] = GetValues();

      return groups;
    }

    /// <summary>
    /// Get values
    /// </summary>
    /// <returns></returns>
    public virtual IList<double> GetValues()
    {
      var currentModel = Value;

      if (currentModel is null)
      {
        return null;
      }

      return new double[] { currentModel.Point ?? 0 };
    }

    /// <summary>
    /// Get specific group by position and name
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual dynamic GetItem(int index, string name, IList<IItemModel> items)
    {
      return items.ElementAtOrDefault(index)?.Value;
    }

    /// <summary>
    /// Clone
    /// </summary>
    /// <returns></returns>
    public virtual object Clone() => MemberwiseClone();
  }
}