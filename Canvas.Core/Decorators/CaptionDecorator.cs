using Canvas.Core.EngineSpace;
using Canvas.Core.EnumSpace;
using Canvas.Core.ModelSpace;
using System;

namespace Canvas.Core.DecoratorSpace
{
  public class CaptionDecorator : BaseDecorator, IDecorator
  {
    /// <summary>
    /// Location on the chart
    /// </summary>
    public PositionEnum Position { get; set; }

    /// <summary>
    /// Create index
    /// </summary>
    /// <param name="engine"></param>
    public virtual void CreateIndex(IEngine engine)
    {
      var shape = Composer.Caption.Clone() as IComponentModel;
      var space = shape.Size;
      var count = Composer.IndexCount;
      var step = engine.X / count;
      var range = (Composer.Domain.MaxIndex - Composer.Domain.MinIndex) / count;
      var point = new ItemModel();

      for (var i = 1; i < count; i++)
      {
        var index = step * i;
        var content = Composer.ShowIndex(Composer.Domain.MinIndex + i * range);

        switch (Position)
        {
          case PositionEnum.T: point.Y = engine.Y - space; break;
          case PositionEnum.B: point.Y = Math.Floor(shape.Size.Value + space.Value); break;
        }

        point.X = index;
        shape.Position = PositionEnum.Center;

        engine.CreateCaption(point, shape, content);
      }
    }

    /// <summary>
    /// Create value
    /// </summary>
    /// <param name="engine"></param>
    public virtual void CreateValue(IEngine engine)
    {
      var shape = Composer.Caption.Clone() as IComponentModel;
      var space = shape.Size;
      var count = Composer.ValueCount;
      var step = engine.Y / count;
      var range = (Composer.Domain.MaxValue - Composer.Domain.MinValue) / count;
      var point = new ItemModel();

      for (var i = 1; i < count; i++)
      {
        var value = step * i + shape.Size / 2;
        var content = Composer.ShowValue(Composer.Domain.MinValue + (count - i) * range);

        switch (Position)
        {
          case PositionEnum.L: point.X = engine.X - space; shape.Position = PositionEnum.R; break;
          case PositionEnum.R: point.X = space; shape.Position = PositionEnum.L; break;
        }

        point.Y = value;

        engine.CreateCaption(point, shape, content);
      }
    }
  }
}
