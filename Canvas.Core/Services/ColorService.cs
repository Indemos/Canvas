using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Canvas.Core.ServiceSpace
{
  public class ColorService
  {
    /// <summary>
    /// Color map
    /// </summary>
    public IList<SKColor> Map { get; set; } = new SKColor[] {
      SKColors.Black,
      SKColors.Blue,
      SKColors.Cyan,
      SKColors.Green,
      SKColors.Yellow,
      SKColors.Red,
      SKColors.White
    };

    /// <summary>
    /// Create color for value
    /// </summary>
    /// <param name="input"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public SKColor GetColor(double input, double max)
    {
      var valPerc = input / max; // value %
      var colorPerc = 1d / (Map.Count - 1); // % of each block of color, the last is the 100% Color
      var blockOfColor = valPerc / colorPerc; // the integer part repersents how many block to skip
      var blockIdx = (int)Math.Truncate(blockOfColor); // Idx 
      var valPercResidual = valPerc - (blockIdx * colorPerc); // remove the part represented of block 
      var percOfColor = valPercResidual / colorPerc; // % of color of this block that will be filled

      var cTarget = Map[blockIdx];
      var cNext = Map[blockIdx + 1];

      var deltaR = cNext.Red - cTarget.Red;
      var deltaG = cNext.Green - cTarget.Green;
      var deltaB = cNext.Blue - cTarget.Blue;

      var R = cTarget.Red + (deltaR * percOfColor);
      var G = cTarget.Green + (deltaG * percOfColor);
      var B = cTarget.Blue + (deltaB * percOfColor);

      var color = Map[0];

      try
      {
        color = new SKColor((byte)R, (byte)G, (byte)B, 0xFF);
      }
      catch (Exception) {}

      return color;
    }
  }
}
