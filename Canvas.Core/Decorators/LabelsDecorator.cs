using Canvas.Core.EngineSpace;
using Canvas.Core.EnumSpace;
using Canvas.Core.ModelSpace;
using SkiaSharp;

namespace Canvas.Core.DecoratorSpace
{
  public class LabelsDecorator : BaseDecorator, IDecorator
  {
    /// <summary>
    /// Location on the chart
    /// </summary>
    public PositionEnum Position { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public LabelsDecorator()
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
      var space = Composer.IndexSpace;
      var count = Composer.IndexCount;
      var step = engine.X / count;
      var range = (Composer.MaxIndex - Composer.MinIndex) / count;
      var point = new ItemModel();
      var shape = new ComponentModel
      {
        Size = 10,
        Location = PositionEnum.Center,
        Color = new SKColor(50, 50, 50)
      };

      for (var i = 1; i < count; i++)
      {
        var index = step * i;
        var content = Composer.ShowIndex(Composer.MinIndex + i * range);

        switch (Position)
        {
          case PositionEnum.T: point.Y = engine.Y - space; break;
          case PositionEnum.B: point.Y = shape.Size + space; break;
        }

        point.X = index;
        shape.Location = PositionEnum.Center;

        engine.CreateLabel(point, shape, content);
      }
    }

    /// <summary>
    /// Create value
    /// </summary>
    /// <param name="engine"></param>
    protected virtual void CreateValueAction(IEngine engine)
    {
      var space = Composer.ValueSpace;
      var count = Composer.ValueCount;
      var step = engine.Y / count;
      var range = (Composer.MaxValue - Composer.MinValue) / count;
      var point = new ItemModel();
      var shape = new ComponentModel
      {
        Size = 10,
        Location = PositionEnum.Center,
        Color = new SKColor(50, 50, 50)
      };

      for (var i = 1; i < count; i++)
      {
        var value = step * i + shape.Size / 2;
        var content = Composer.ShowValue(Composer.MinValue + (count - i) * range);

        switch (Position)
        {
          case PositionEnum.L: point.X = engine.X - space; shape.Location = PositionEnum.R; break;
          case PositionEnum.R: point.X = space; shape.Location = PositionEnum.L; break;
        }

        point.Y = value;

        engine.CreateLabel(point, shape, content);
      }
    }
  }
}
