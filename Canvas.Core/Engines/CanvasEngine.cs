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
    /// Get instance
    /// </summary>
    /// <returns></returns>
    public override IEngine GetInstance()
    {
      if (Map is null)
      {
        return null;
      }

      return this;
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public override IEngine Create(double x, double y)
    {
      Dispose();

      X = x;
      Y = y;

      Map = new SKBitmap((int)x, (int)y);
      Canvas = new SKCanvas(Map);

      return this;
    }

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public override void CreateLine(IList<IItemModel> coordinates, IComponentModel shape)
    {
      var pen = new SKPaint
      {
        Color = shape.Color.Value,
        Style = SKPaintStyle.Stroke,
        FilterQuality = SKFilterQuality.High,
        StrokeWidth = (float)shape.Size,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      switch (shape.Composition)
      {
        case CompositionEnum.Dots: pen.PathEffect = SKPathEffect.CreateDash(new float[] { 1, 3 }, 0); break;
        case CompositionEnum.Dashes: pen.PathEffect = SKPathEffect.CreateDash(new float[] { 3, 3 }, 0); break;
      }

      Canvas.DrawLine(
        (float)coordinates[0].X,
        (float)coordinates[0].Y,
        (float)coordinates[1].X,
        (float)coordinates[1].Y,
        pen);

      pen.Dispose();
    }

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="shape"></param>
    public override void CreateCircle(IItemModel coordinate, IComponentModel shape)
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
        (float)coordinate.X,
        (float)coordinate.Y,
        (float)shape.Size,
        pen);

      pen.Dispose();
    }

    /// <summary>
    /// Create box
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public override void CreateBox(IList<IItemModel> coordinates, IComponentModel shape)
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
        (float)coordinates[0].X,
        (float)coordinates[0].Y,
        (float)(coordinates[1].X - coordinates[0].X),
        (float)(coordinates[1].Y - coordinates[0].Y),
        pen);

      pen.Dispose();
    }

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public override void CreateShape(IList<IItemModel> coordinates, IComponentModel shape)
    {
      var origin = coordinates.ElementAtOrDefault(0);
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

      curve.MoveTo((float)origin.X.Value, (float)origin.Y);

      for (var i = 1; i < coordinates.Count; i++)
      {
        curve.LineTo((float)coordinates[i].X.Value, (float)coordinates[i].Y);
      }

      Canvas.DrawPath(curve, pen);

      pen.Dispose();
      curve.Dispose();
    }

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public override void CreateLabel(IItemModel coordinate, IComponentModel shape, string content)
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
        case PositionEnum.L: pen.TextAlign = SKTextAlign.Left; break;
        case PositionEnum.R: pen.TextAlign = SKTextAlign.Right; break;
      }

      var space = (pen.FontSpacing - pen.TextSize) / 2;

      Canvas.DrawText(
        content,
        (float)coordinate.X,
        (float)(coordinate.Y - space),
        pen);

      pen.Dispose();
    }

    /// <summary>
    /// Draw label along the path
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public override void CreateLabelShape(IList<IItemModel> coordinates, IComponentModel shape, string content)
    {
      var origin = coordinates.ElementAtOrDefault(0);
      var curve = new SKPath();
      var pen = new SKPaint
      {
        Color = shape.Color.Value,
        TextAlign = SKTextAlign.Center,
        FilterQuality = SKFilterQuality.High,
        TextSize = (float)shape.Size,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      curve.MoveTo((float)origin.X.Value, (float)origin.Y);

      for (var i = 1; i < coordinates.Count; i++)
      {
        curve.LineTo((float)coordinates[i].X.Value, (float)coordinates[i].Y);
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

      var item = new ItemModel
      {
        X = content.Length * pen.FontMetrics.MaxCharacterWidth,
        Y = pen.FontSpacing
      };

      pen.Dispose();

      return item;
    }

    /// <summary>
    /// Clear canvas
    /// </summary>
    public override void Clear()
    {
      Canvas.Clear(SKColors.Transparent);
    }

    /// <summary>
    /// Encode as image
    /// </summary>
    /// <param name="imageType"></param>
    /// <param name="quality"></param>
    /// <returns></returns>
    public override byte[] Encode(SKEncodedImageFormat imageType, int quality)
    {
      using (var image = Map.Encode(imageType, quality))
      {
        return image.ToArray();
      }
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
