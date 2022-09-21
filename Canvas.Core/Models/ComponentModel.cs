using Canvas.Core.EnumSpace;
using SkiaSharp;
using System.Collections.Generic;

namespace Canvas.Core.ModelSpace
{
  public interface IComponentModel : IItemModel
  {
    /// <summary>
    /// Index
    /// </summary>
    double? Size { get; set; }

    /// <summary>
    /// Position and alignment
    /// </summary>
    PositionEnum? Position { get; set; }

    /// <summary>
    /// Format and density 
    /// </summary>
    CompositionEnum? Composition { get; set; }

    /// <summary>
    /// Points
    /// </summary>
    IList<IItemModel> Items { get; set; }

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <returns></returns>
    void UpdateShape();
  }

  public class ComponentModel : ItemModel, IComponentModel
  {
    /// <summary>
    /// Index
    /// </summary>
    public virtual double? Size { get; set; }

    /// <summary>
    /// Position and alignment
    /// </summary>
    public virtual PositionEnum? Position { get; set; }

    /// <summary>
    /// Format and density 
    /// </summary>
    public virtual CompositionEnum? Composition { get; set; }

    /// <summary>
    /// Points
    /// </summary>
    public virtual IList<IItemModel> Items { get; set; }

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <returns></returns>
    public virtual void UpdateShape()
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public ComponentModel()
    {
      Size = 1;
      Color = SKColors.Black;
      Items = new List<IItemModel>();
    }
  }
}
