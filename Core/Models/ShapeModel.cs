using Core.EnumSpace;
using SkiaSharp;
using System.Collections.Generic;

namespace Core.ModelSpace
{
  public interface IShapeModel : IPointModel
  {
    /// <summary>
    /// Index
    /// </summary>
    double? Size { get; set; }

    /// <summary>
    /// Color
    /// </summary>
    SKColor? Color { get; set; }

    /// <summary>
    /// Line style
    /// </summary>
    LineShapeEnum? LineShape { get; set; }

    /// <summary>
    /// Points
    /// </summary>
    IList<IPointModel> Points { get; set; }

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <returns></returns>
    void UpdateShape();
  }

  public class ShapeModel : PointModel, IShapeModel
  {
    /// <summary>
    /// Index
    /// </summary>
    public virtual double? Size { get; set; }

    /// <summary>
    /// Color
    /// </summary>
    public virtual SKColor? Color { get; set; }

    /// <summary>
    /// Line style
    /// </summary>
    public virtual LineShapeEnum? LineShape { get; set; }

    /// <summary>
    /// Points
    /// </summary>
    public virtual IList<IPointModel> Points { get; set; }

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
    public ShapeModel()
    {
      Size = 1;
      Color = SKColors.Black;
      Points = new List<IPointModel>();
    }
  }
}
