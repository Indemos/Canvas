using Canvas.Core.EnumSpace;
using Canvas.Core.ModelSpace;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.EngineSpace
{
  public class CanvasEngine : Engine, IEngine
  {
    /// <summary>
    /// Bitmap
    /// </summary>
    public virtual SKBitmap Map { get; protected set; }

    /// <summary>
    /// Canvas
    /// </summary>
    public virtual SKCanvas Canvas { get; protected set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="indexSize"></param>
    /// <param name="valueSize"></param>
    /// <returns></returns>
    public CanvasEngine(double indexSize, double valueSize)
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
    public override void CreateLine(IList<IItemModel> points, IComponentModel shape)
    {
      var pen = new SKPaint
      {
        Color = shape.Color.Value,
        Style = SKPaintStyle.Stroke,
        FilterQuality = SKFilterQuality.High,
        StrokeWidth = (float)shape.Size,
        IsAntialias = true,
        IsStroke = false,
        IsDither = false
      };

      switch (shape.Composition)
      {
        case CompositionEnum.Dots: pen.PathEffect = SKPathEffect.CreateDash(new float[] { 1, 3 }, 0); break;
        case CompositionEnum.Dashes: pen.PathEffect = SKPathEffect.CreateDash(new float[] { 3, 3 }, 0); break;
      }

      Canvas.DrawLine(
        (float)points[0].Index,
        (float)points[0].Value,
        (float)points[1].Index,
        (float)points[1].Value,
        pen);

      pen?.PathEffect?.Dispose();
      pen?.Dispose();
    }

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="point"></param>
    /// <param name="shape"></param>
    public override void CreateCircle(IItemModel point, IComponentModel shape)
    {
      var pen = new SKPaint
      {
        Color = shape.Color.Value,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.High,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      Canvas.DrawCircle(
        (float)point.Index,
        (float)point.Value,
        (float)shape.Size,
        pen);

      pen.Dispose();
    }

    /// <summary>
    /// Create box
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shape"></param>
    public override void CreateBox(IList<IItemModel> points, IComponentModel shape)
    {
      var pen = new SKPaint
      {
        Color = shape.Color.Value,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.High,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      Canvas.DrawRect(
        (float)points[0].Index,
        (float)points[0].Value,
        (float)(points[1].Index - points[0].Index),
        (float)(points[1].Value - points[0].Value),
        pen);

      pen.Dispose();
    }

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shape"></param>
    public override void CreateShape(IList<IItemModel> points, IComponentModel shape)
    {
      var origin = points.ElementAtOrDefault(0);
      var curve = new SKPath();
      var pen = new SKPaint
      {
        Color = shape.Color.Value,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.High,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      curve.MoveTo((float)origin.Index.Value, (float)origin.Value);

      for (var i = 1; i < points.Count; i++)
      {
        curve.LineTo((float)points[i].Index.Value, (float)points[i].Value);
      }

      Canvas.DrawPath(curve, pen);

      pen.Dispose();
      curve.Dispose();
    }

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="point"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public override void CreateLabel(IItemModel point, IComponentModel shape, string content)
    {
      var pen = new SKPaint
      {
        Color = shape.Color.Value,
        TextAlign = SKTextAlign.Center,
        FilterQuality = SKFilterQuality.High,
        TextSize = (float)shape.Size,
        IsAntialias = true,
        IsStroke = false,
        IsDither = false
      };

      switch (shape.Location)
      {
        case LocationEnum.L: pen.TextAlign = SKTextAlign.Left; break;
        case LocationEnum.R: pen.TextAlign = SKTextAlign.Right; break;
      }

      var space = (pen.FontSpacing - pen.TextSize) / 2;

      Canvas.DrawText(
        content,
        (float)point.Index,
        (float)(point.Value - space),
        pen);

      pen.Dispose();
    }

    /// <summary>
    /// Draw label along the path
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public override void CreateLabelShape(IList<IItemModel> points, IComponentModel shape, string content)
    {
      var origin = points.ElementAtOrDefault(0);
      var curve = new SKPath();
      var pen = new SKPaint
      {
        Color = shape.Color.Value,
        TextAlign = SKTextAlign.Center,
        FilterQuality = SKFilterQuality.High,
        TextSize = (float)shape.Size,
        IsAntialias = true,
        IsStroke = false,
        IsDither = false
      };

      curve.MoveTo((float)origin.Index.Value, (float)origin.Value);

      for (var i = 1; i < points.Count; i++)
      {
        curve.LineTo((float)points[i].Index.Value, (float)points[i].Value);
      }

      pen.Color = shape.Color.Value;
      pen.TextSize = (float)shape.Size;

      Canvas.DrawTextOnPath(content, curve, 0, pen.TextSize / 2, pen);

      pen.Dispose();
      curve.Dispose();
    }

    /// <summary>
    /// Measure content
    /// </summary>
    /// <param name="content"></param>
    /// <param name="size"></param>
    public override IItemModel GetContentMeasure(string content, double size)
    {
      var pen = new SKPaint
      {
        TextSize = (float)size
      };

      var point = new ItemModel
      {
        Index = content.Length * pen.FontMetrics.MaxCharacterWidth,
        Value = pen.FontSpacing
      };

      pen.Dispose();

      return point;
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

      Canvas = null;
      Map = null;
    }
  }
}
