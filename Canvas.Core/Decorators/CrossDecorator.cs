using Canvas.Core.EnumSpace;
using Canvas.Core.ModelSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Canvas.Core.DecoratorSpace
{
  public class CrossDecorator : BaseDecorator, IDecorator
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
    /// Reusable line shape model
    /// </summary>
    protected IComponentModel _shapeLine = null;

    /// <summary>
    /// Reusable label shape model
    /// </summary>
    protected IComponentModel _shapeLabel = null;

    /// <summary>
    /// Reusable label points model
    /// </summary>
    protected IItemModel _pointLabel = null;

    /// <summary>
    /// Reusable points model
    /// </summary>
    protected IList<IItemModel> _points = null;

    /// <summary>
    /// Constructor
    /// </summary>
    public CrossDecorator()
    {
      _shapeLine = new ComponentModel
      {
        Size = 1,
        Location = LocationEnum.Center,
        LineShape = LineShapeEnum.Dashes,
        Color = new SKColor(70, 70, 70)
      };

      _shapeLabel = new ComponentModel
      {
        Size = 10,
        Location = LocationEnum.Center,
        Color = new SKColor(255, 255, 255)
      };

      _pointLabel = new ItemModel
      {
        X = 0,
        Y = 0
      };

      _points = new IItemModel[]
      {
        new ItemModel { X = 0, Y = 0 },
        new ItemModel { X = 0, Y = 0 }
      };
    }

    /// <summary>
    /// Create cross decorator
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public new void Update(double index, double value)
    {
      Area.Clear();

      var isInRange =
        index > Composer.IndexSpace &&
        index < Area.IndexSize - Composer.IndexSpace &&
        value > Composer.ValueSpace &&
        value < Area.ValueSize - Composer.ValueSpace;

      if (isInRange)
      {
        ShowIndex ??= Composer.ShowIndex;
        ShowValue ??= Composer.ShowValue;

        _shapeLine = Composer.LineShape ?? _shapeLine;
        _shapeLabel = Composer.LabelShape ?? _shapeLabel;

        _pointLabel.X = index;
        _pointLabel.Y = value;

        var scale = Composer.GetValues(Area, _pointLabel);

        CreateLines(index, value);
        CreateBoxes(index, value, scale);
        CreateLabels(index, value, scale);
      }
    }

    /// <summary>
    /// Create lines
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    protected virtual void CreateLines(double index, double value)
    {
      // Index

      _points[0].X = Composer.IndexSpace;
      _points[0].Y = value;
      _points[1].X = Area.IndexSize - Composer.IndexSpace;
      _points[1].Y = value;

      Area.CreateLine(_points, _shapeLine);

      // Value

      _points[0].X = index;
      _points[0].Y = Composer.ValueSpace;
      _points[1].X = index;
      _points[1].Y = Area.ValueSize - Composer.ValueSpace;

      Area.CreateLine(_points, _shapeLine);
    }

    /// <summary>
    /// Create lines
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="scale"></param>
    protected virtual void CreateBoxes(double index, double value, IItemModel scale)
    {
      var content = ShowIndex(scale.X);
      var contentMeasure = Area.GetContentMeasure(content, _shapeLabel.Size.Value);
      var indexSize = _shapeLabel.Size * content.Length / 2;
      var valueSize = contentMeasure.Value / 2;

      // L

      _points[0].X = 0;
      _points[0].Y = value - valueSize;
      _points[1].X = Composer.IndexSpace;
      _points[1].Y = value + valueSize;

      Area.CreateBox(_points, _shapeLine);

      // R

      _points[0].X = Area.IndexSize - Composer.IndexSpace;
      _points[0].Y = value - valueSize;
      _points[1].X = Area.IndexSize;
      _points[1].Y = value + valueSize;

      Area.CreateBox(_points, _shapeLine);

      // T

      var baseT = Composer.ValueSpace - Composer.ValueAxisSpace * 2 - _shapeLabel.Size / 2;

      _points[0].X = index - indexSize;
      _points[0].Y = baseT - valueSize;
      _points[1].X = index + indexSize;
      _points[1].Y = baseT + valueSize;

      Area.CreateBox(_points, _shapeLine);

      // B

      var baseB = Area.ValueSize - Composer.ValueSpace + Composer.ValueAxisSpace * 2 + _shapeLabel.Size / 2;

      _points[0].X = index - indexSize;
      _points[0].Y = baseB - valueSize;
      _points[1].X = index + indexSize;
      _points[1].Y = baseB + valueSize;

      Area.CreateBox(_points, _shapeLine);
    }

    /// <summary>
    /// Create labels
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="scale"></param>
    protected virtual void CreateLabels(double index, double value, IItemModel scale)
    {
      // L

      _points[0].X = Composer.IndexSpace - Composer.IndexAxisSpace * 2;
      _points[0].Y = value + _shapeLabel.Size / 2;
      _shapeLabel.Location = LocationEnum.R;

      Area.CreateLabel(_points[0], _shapeLabel, ShowValue(scale.Y));

      // R

      _points[0].X = Area.IndexSize - Composer.IndexSpace + Composer.IndexAxisSpace * 2;
      _points[0].Y = value + _shapeLabel.Size / 2;
      _shapeLabel.Location = LocationEnum.L;

      Area.CreateLabel(_points[0], _shapeLabel, ShowValue(scale.Y));

      // T 

      _points[0].X = index;
      _points[0].Y = Composer.ValueSpace - Composer.ValueAxisSpace * 2;
      _shapeLabel.Location = LocationEnum.Center;

      Area.CreateLabel(_points[0], _shapeLabel, ShowIndex(scale.X));

      // B

      _points[0].X = index;
      _points[0].Y = Area.ValueSize - Composer.ValueSpace + Composer.ValueAxisSpace * 2 + _shapeLabel.Size;
      _shapeLabel.Location = LocationEnum.Center;

      Area.CreateLabel(_points[0], _shapeLabel, ShowIndex(scale.X));
    }
  }
}
