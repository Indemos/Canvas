using Canvas.Core.EngineSpace;
using SkiaSharp;
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
    /// Color
    /// </summary>
    SKColor? Color { get; set; }

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
    /// Color
    /// </summary>
    public SKColor? Color { get; set; }

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
