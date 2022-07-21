using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Canvas.Core.ServiceSpace
{
  public class ColorService
  {
    /// <summary>
    /// Min value
    /// </summary>
    public double Min { get; set; }

    /// <summary>
    /// Max value
    /// </summary>
    public double Max { get; set; }

    /// <summary>
    /// Color map
    /// </summary>
    public IList<SKColor> Map { get; set; } = new SKColor[] {
      SKColors.Blue,
      SKColors.Red
    };

    /// <summary>
    /// Create color for value
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public SKColor GetColor(double input)
    {
      var mapRatio = 1.0 / Math.Max(Map.Count, double.Epsilon);
      var valueRatio = input / Math.Max(Max - Min, double.Epsilon);
      var color = Map[(int)Math.Min(Map.Count - 1, Math.Max(0, Math.Truncate(valueRatio / mapRatio)))];

      return new SKColor(color.Red, color.Green, color.Blue, (byte)(color.Alpha * valueRatio / mapRatio));
    }
  }
}
