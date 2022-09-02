using Canvas.Core.EnumSpace;
using Canvas.Core.ModelSpace;
using SkiaSharp;
using System;

namespace Canvas.Core.DecoratorSpace
{
  public class LabelsDecorator : BaseDecorator, IDecorator
  {
    /// <summary>
    /// Custom label renderer
    /// </summary>
    public virtual Func<double, string> ShowIndex { get; set; }

    /// <summary>
    /// Custom label renderer
    /// </summary>
    public virtual Func<double, string> ShowValue { get; set; }

    /// <summary>
    /// Reusable shape model
    /// </summary>
    protected IComponentModel _shape = null;

    /// <summary>
    /// Reusable points model
    /// </summary>
    protected IItemModel _point = null;

    /// <summary>
    /// Constructor
    /// </summary>
    public LabelsDecorator()
    {
      _point = new ItemModel
      {
        X = 0,
        Y = 0
      };

      _shape = new ComponentModel
      {
        Size = 10,
        Location = LocationEnum.Center,
        Color = new SKColor(50, 50, 50)
      };
    }

    /// <summary>
    /// Create component
    /// </summary>
    public override void CreateDelegate()
    {
      CreateIndex();
      CreateValue();
    }

    /// <summary>
    /// Update component
    /// </summary>
    public override void UpdateDelegate()
    {
      CreateIndex();
      CreateValue();
    }

    /// <summary>
    /// Create index labels
    /// </summary>
    protected virtual void CreateIndex()
    {
      ShowIndex ??= Composer.ShowIndex;

      var min = Composer.MinIndex;
      var max = Composer.MaxIndex;
      var count = Composer.IndexCount;
      var step = (Area.IndexSize - Composer.IndexSpace * 2) / count;
      var change = (Composer.MaxIndex - Composer.MinIndex) / count;

      for (var i = 1; i < count; i++)
      {
        var index = Composer.IndexSpace + step * i;
        var content = ShowIndex(min + i * change);

        // T

        _point.X = index;
        _point.Y = Composer.ValueSpace - Composer.ValueSpace * 2;
        _shape.Location = LocationEnum.Center;

        Area.CreateLabel(_point, _shape, content);

        // B

        _point.X = index;
        _point.Y = Area.ValueSize - Composer.ValueSpace + _shape.Size + Composer.ValueSpace * 2;
        _shape.Location = LocationEnum.Center;

        Area.CreateLabel(_point, _shape, content);
      }
    }

    /// <summary>
    /// Create index labels
    /// </summary>
    protected virtual void CreateValue()
    {
      ShowValue ??= Composer.ShowValue;

      var min = Composer.MinValue;
      var max = Composer.MaxValue;
      var count = Composer.ValueCount;
      var step = (Area.ValueSize - Composer.ValueSpace * 2) / count;
      var change = (Composer.MaxValue - Composer.MinValue) / count;

      for (var i = 1; i < count; i++)
      {
        var value = Composer.ValueSpace + step * i + _shape.Size / 2;
        var content = ShowValue(min + (count - i) * change);

        // L

        _point.X = Composer.IndexSpace - Composer.IndexSpace * 2;
        _point.Y = value;
        _shape.Location = LocationEnum.R;

        Area.CreateLabel(_point, _shape, content);

        // R

        _point.X = Area.IndexSize - Composer.IndexSpace + Composer.IndexSpace * 2;
        _point.Y = value;
        _shape.Location = LocationEnum.L;

        Area.CreateLabel(_point, _shape, content);
      }
    }
  }
}
