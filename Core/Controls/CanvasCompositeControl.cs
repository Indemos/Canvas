using Core.EnumSpace;
using Core.ModelSpace;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace Core.ControlSpace
{
  public class CanvasCompositeControl : CanvasControl, ICanvasControl
  {
    /// <summary>
    /// Bitmap
    /// </summary>
    public virtual SKBitmap Map { get; protected set; }

    /// <summary>
    /// Drawing surface
    /// </summary>
    public virtual SKCanvas Canvas { get; protected set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="indexSize"></param>
    /// <param name="valueSize"></param>
    /// <returns></returns>
    public CanvasCompositeControl(double indexSize, double valueSize)
    {
      IndexSize = indexSize;
      ValueSize = valueSize;

      Map = new SKBitmap(
        (int)indexSize,
        (int)valueSize);

      Canvas = new SKCanvas(Map);
    }

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shape"></param>
    public override void CreateLine(IList<IPointModel> points, IShapeModel shape)
    {
      using (var paint = new SKPaint
      {
        Color = shape.Color.Value,
        Style = SKPaintStyle.Stroke,
        FilterQuality = SKFilterQuality.High,
        StrokeWidth = (float)shape.Size,
        IsAntialias = true,
        IsStroke = false,
        IsDither = false
      })
      {
        switch (shape.LineShape)
        {
          case LineShapeEnum.Dots: paint.PathEffect = SKPathEffect.CreateDash(new float[] { 1, 3 }, 0); break;
          case LineShapeEnum.Dashes: paint.PathEffect = SKPathEffect.CreateDash(new float[] { 3, 3 }, 0); break;
        }

        Canvas.DrawLine(
          (float)points[0].Index,
          (float)points[0].Value,
          (float)points[1].Index,
          (float)points[1].Value,
          paint);
      }
    }

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="point"></param>
    /// <param name="shape"></param>
    public override void CreateCircle(IPointModel point, IShapeModel shape)
    {
      using (var paint = new SKPaint
      {
        Color = shape.Color.Value,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.High,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      })
      {
        Canvas.DrawCircle(
          (float)point.Index,
          (float)point.Value,
          (float)shape.Size,
          paint);
      }
    }

    /// <summary>
    /// Create box
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shape"></param>
    public override void CreateBox(IList<IPointModel> points, IShapeModel shape)
    {
      using (var paint = new SKPaint
      {
        Color = shape.Color.Value,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.High,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      })
      {
        Canvas.DrawRect(
          (float)points[0].Index,
          (float)points[0].Value,
          (float)(points[1].Index - points[0].Index),
          (float)(points[1].Value - points[0].Value),
          paint);
      }
    }

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shape"></param>
    public override void CreateShape(IList<IPointModel> points, IShapeModel shape)
    {
      var origin = points.ElementAtOrDefault(0);

      if (origin is null)
      {
        return;
      }

      using (var curve = new SKPath())
      {
        curve.MoveTo((float)origin.Index.Value, (float)origin.Value);

        for (var i = 1; i < points.Count; i++)
        {
          curve.LineTo((float)points[i].Index.Value, (float)points[i].Value);
        }

        using (var paint = new SKPaint
        {
          Color = shape.Color.Value,
          Style = SKPaintStyle.Fill,
          FilterQuality = SKFilterQuality.High,
          IsAntialias = false,
          IsStroke = false,
          IsDither = false
        })
        {
          Canvas.DrawPath(curve, paint);
        }
      }
    }

    /// <summary>
    /// Measure content
    /// </summary>
    /// <param name="content"></param>
    /// <param name="size"></param>
    public override IPointModel GetContentMeasure(string content, double size)
    {
      using (var paint = new SKPaint
      {
        TextSize = (float)size
      })
      {
        return new PointModel
        {
          Index = content.Length * paint.FontMetrics.MaxCharacterWidth,
          Value = paint.FontSpacing
        };
      }
    }

    /// <summary>
    /// Clear canvas
    /// </summary>
    public override void Clear()
    {
      Canvas.Clear(SKColors.Transparent);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public override void Dispose()
    {
      Canvas?.Dispose();
      Map?.Dispose();
    }
  }
}
