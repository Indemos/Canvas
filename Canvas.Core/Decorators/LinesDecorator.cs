using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using SkiaSharp;
using System.Collections.Generic;

namespace Canvas.Core.DecoratorSpace
{
  public class LinesDecorator : BaseDecorator, IDecorator
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public LinesDecorator()
    {
      Create = CreateAction;
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="engine"></param>
    protected void CreateAction(IEngine engine)
    {
      CreateIndex(engine);
      CreateValue(engine);
    }

    /// <summary>
    /// Create index
    /// </summary>
    /// <param name="engine"></param>
    protected virtual void CreateIndex(IEngine engine)
    {
      var count = Composer.IndexCount - 1;
      var step = engine.X / count;

      var shape = new ComponentModel
      {
        Size = 1,
        Color = new SKColor(230, 230, 230)
      };

      for (var i = 1; i < count; i++)
      {
        var points = new IItemModel[]
        {
          new ItemModel { X = 0, Y = step * i },
          new ItemModel { X = engine.Y, Y = step * i }
        };

        engine.CreateLine(points, shape);
      }
    }

    /// <summary>
    /// Create value
    /// </summary>
    /// <param name="engine"></param>
    protected virtual void CreateValue(IEngine engine)
    {
      var count = Composer.ValueCount - 1;
      var step = engine.Y / count;

      var shape = new ComponentModel
      {
        Size = 1,
        Color = new SKColor(230, 230, 230)
      };

      for (var i = 1; i < count; i++)
      {
        var points = new IItemModel[]
        {
          new ItemModel { X = step * i, Y = 0 },
          new ItemModel { X = step * i, Y = engine.X }
        };

        engine.CreateLine(points, shape);
      }
    }
  }
}
