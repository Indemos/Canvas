using System.Collections.Generic;

namespace Canvas.Core.ModelSpace
{
  public class LineShapeModel : ComponentModel, IComponentModel
  {
    public virtual IList<int> IndexLevels { get; set; }
    public virtual IList<double> ValueLevels { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public LineShapeModel()
    {
      IndexLevels = new List<int>();
      ValueLevels = new List<double>();
    }

    /// <summary>
    /// Update
    /// </summary>
    public override void UpdateShape()
    {
      var pointMin = new ItemModel { X = 0, Y = 0 };
      var pointMax = new ItemModel { X = 0, Y = 0 };
      var points = new IItemModel[]
      {
        pointMin, 
        pointMax
      };

      foreach (var level in IndexLevels)
      {
        var pixelLevel = Composer.GetPixels(Engine, level, 0);

        points[0].X = pointMax.X = pixelLevel.X;
        points[0].Y = 0;
        points[1].Y = Engine.Y;

        Engine.CreateLine(points, this);
      }

      foreach (var level in ValueLevels)
      {
        var pixelLevel = Composer.GetPixels(Engine, 0, level);

        points[0].Y = pointMax.Y = pixelLevel.Y;
        points[0].X = 0;
        points[1].X = Engine.X;

        Engine.CreateLine(points, this);
      }
    }
  }
}
