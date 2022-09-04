using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using SkiaSharp;
using System;

namespace Canvas.Core.DecoratorSpace
{
  public class LinesDecorator : BaseDecorator, IDecorator
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public LinesDecorator()
    {
      CreateIndex = CreateIndexAction;
      CreateValue = CreateValueAction;
    }

    /// <summary>
    /// Create index
    /// </summary>
    /// <param name="engine"></param>
    protected virtual void CreateIndexAction(IEngine engine)
    {
      var count = Composer.ValueCount;
      var step = engine.Y / count;
      var points = new IItemModel[2];
      var shape = new ComponentModel
      {
        Size = 1,
        Color = new SKColor(200, 200, 200)
      };

      for (var i = 0; i < count; i++)
      {
        points[0] = new ItemModel { X = 0, Y = step * i };
        points[1] = new ItemModel { X = engine.X, Y = step * i };

        engine.CreateLine(points, shape);
      }

      points[0] = new ItemModel { X = 0, Y = engine.Y - 1 };
      points[1] = new ItemModel { X = engine.X, Y = engine.Y - 1 };

      engine.CreateLine(points, shape);
    }

    /// <summary>
    /// Create value
    /// </summary>
    /// <param name="engine"></param>
    protected virtual void CreateValueAction(IEngine engine)
    {
      var count = Composer.IndexCount;
      var step = engine.X / count;
      var points = new IItemModel[2];
      var shape = new ComponentModel
      {
        Size = 1,
        Color = new SKColor(200, 200, 200)
      };

      for (var i = 0; i < count; i++)
      {
        points[0] = new ItemModel { X = step * i, Y = 0 };
        points[1] = new ItemModel { X = step * i, Y = engine.Y };

        engine.CreateLine(points, shape);
      }

      points[0] = new ItemModel { X = engine.X - 1, Y = 0 };
      points[1] = new ItemModel { X = engine.X - 1, Y = engine.Y };

      engine.CreateLine(points, shape);
    }
  }
}
