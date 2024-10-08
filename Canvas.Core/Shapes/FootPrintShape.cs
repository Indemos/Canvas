using Canvas.Core.Enums;
using Canvas.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.Shapes
{
  public class FootPrintShape : CandleShape
  {
    /// <summary>
    /// Price levels on the left side of the bar
    /// </summary>
    public virtual IList<KeyValuePair<double, double>> Ls { get; set; } = [];

    /// <summary>
    /// Price levels on the right side of the bar
    /// </summary>
    public virtual IList<KeyValuePair<double, double>> Rs { get; set; } = [];

    /// <summary>
    /// Render the shape
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override void CreateShape(int index, string name, IList<IShape> items)
    {
      if (index % 2 == 0)
      {
        return;
      }

      if (L is null || H is null || O is null || C is null)
      {
        return;
      }

      var open = O ?? 0;
      var close = C ?? 0;
      var size = Composer.Size / 3.0;
      var downSide = Math.Min(open, close);
      var upSide = Math.Max(open, close);
      var coordinates = new DataModel[]
      {
        Composer.GetItemPosition(index - size, upSide),
        Composer.GetItemPosition(index + size, downSide)
      };

      var rangeCoordinates = new DataModel[]
      {
        Composer.GetItemPosition(index, L ?? 0),
        Composer.GetItemPosition(index, H ?? 0),
      };

      Composer.Engine.CreateLine(rangeCoordinates, Line ?? Component ?? Composer.Components[nameof(ComponentEnum.Shape)]);
      Composer.Engine.CreateBox(coordinates, Box ?? Component ?? Composer.Components[nameof(ComponentEnum.Shape)]);

      var lastL = double.MaxValue;
      var lastR = double.MaxValue;
      var textSize = 10;

      var comLs = new ComponentModel
      {
        Size = textSize,
        Position = PositionEnum.R,
        Color = Composer.Components[nameof(ComponentEnum.Caption)].Color
      };

      var comRs = new ComponentModel
      {
        Size = textSize,
        Position = PositionEnum.L,
        Color = Composer.Components[nameof(ComponentEnum.Caption)].Color
      };

      Ls.ForEach(o =>
      {
        var coordinate = Composer.GetItemPosition(index - size - 0.1, o.Key);

        if (lastL - coordinate.Y > textSize)
        {
          lastL = coordinate.Y;
          Composer.Engine.CreateCaption(coordinate, comLs, $"{o.Value}");
        }
      });

      Rs.ForEach(o =>
      {
        var coordinate = Composer.GetItemPosition(index + size + 0.1, o.Key);

        if (lastR - coordinate.Y > textSize)
        {
          lastR = coordinate.Y;
          Composer.Engine.CreateCaption(coordinate, comRs, $"{o.Value}");
        }
      });
    }
  }
}
