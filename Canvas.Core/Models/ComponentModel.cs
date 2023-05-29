using Canvas.Core.Enums;
using SkiaSharp;

namespace Canvas.Core.Models
{
  public struct ComponentModel
  {
    /// <summary>
    /// Index
    /// </summary>
    public double Size { get; set; }

    /// <summary>
    /// Color
    /// </summary>
    public SKColor Color { get; set; }

    /// <summary>
    /// Background
    /// </summary>
    public SKColor Background { get; set; }

    /// <summary>
    /// Position and alignment
    /// </summary>
    public PositionEnum Position { get; set; }

    /// <summary>
    /// Format and density 
    /// </summary>
    public CompositionEnum Composition { get; set; }
  }
}
