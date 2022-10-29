using Canvas.Core.EnumSpace;
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
    /// Color mode
    /// </summary>
    public ShadeEnum Shade { get; set; } = ShadeEnum.Opacity;

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
      var colorCount = Math.Max(Map.Count, double.Epsilon);
      var inputCount = Math.Max(Max - Min, double.Epsilon);
      var colorRatio = 1.0 / colorCount;
      var inputRatio = input / inputCount;
      var index = colorCount * inputRatio;
      var color = Map[(int)index];
      var opacityRatio = 1.0;

      if (Equals(Shade, ShadeEnum.Opacity))
      {
        opacityRatio = 0.0;

        switch (true)
        {
          case true when index > colorCount / 2: opacityRatio = inputRatio / colorRatio; break;
          case true when index < colorCount / 2: opacityRatio = 1.0 - inputRatio / colorRatio; break;
        }
      }

      return new SKColor(color.Red, color.Green, color.Blue, (byte)(color.Alpha * opacityRatio));
    }
  }
}
