using Canvas.Core.Enums;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.Services
{
  public class ColorService
  {
    /// <summary>
    /// Min value
    /// </summary>
    public virtual double Min { get; set; }

    /// <summary>
    /// Max value
    /// </summary>
    public virtual double Max { get; set; }

    /// <summary>
    /// Color mode
    /// </summary>
    public virtual ShadeEnum Mode { get; set; } = ShadeEnum.Mirror;

    /// <summary>
    /// Color map
    /// </summary>
    public virtual IList<SKColor> Map { get; set; } = [
      SKColors.Blue,
      SKColors.Red
    ];

    /// <summary>
    /// Create color for value
    /// </summary>
    /// <param name="value"></param>
    public virtual SKColor GetColor(double input)
    {
      var percentage = (input - Min) / (Max - Min);
      var color = SKColors.Transparent;
      var opacityRatio = 1.0;

      if (Equals(Mode, ShadeEnum.Intensity))
      {
        color = Map.First();
        opacityRatio = percentage;
      }

      if (Equals(Mode, ShadeEnum.Mirror))
      {
        opacityRatio = 0.0;
        color = Map[(int)Math.Round(percentage, MidpointRounding.ToEven)];

        switch (true)
        {
          case true when percentage > 0.5: opacityRatio = percentage; break;
          case true when percentage < 0.5: opacityRatio = 1.0 - percentage; break;
        }
      }

      return new SKColor(color.Red, color.Green, color.Blue, (byte)(color.Alpha * opacityRatio));
    }
  }
}
