using Canvas.Core.EngineSpace;
using System;
using System.Collections.Generic;

namespace Canvas.Core.ModelSpace
{
  public interface IPointModel : ICloneable
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
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    double[] CreateDomain(int position, string name, IList<IPointModel> items);

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    void CreateShape(int position, string name, IList<IPointModel> items);
  }

  public class PointModel : IPointModel
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
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual double[] CreateDomain(int position, string name, IList<IPointModel> items)
    {
      var currentModel = Composer.GetPoint(position, name, items);

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
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual void CreateShape(int position, string name, IList<IPointModel> items)
    {
    }

    /// <summary>
    /// Clone
    /// </summary>
    /// <returns></returns>
    public virtual object Clone() => MemberwiseClone();
  }
}
