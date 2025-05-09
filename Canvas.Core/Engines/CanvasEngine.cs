using Canvas.Core.Enums;
using Canvas.Core.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.Engines
{
  public class CanvasEngine : Engine, IEngine
  {
    protected SKPath curve;
    protected SKPaint penBox;
    protected SKPaint penLine;
    protected SKPaint penShape;
    protected SKPaint penCircle;
    protected SKPaint penCaption;
    protected SKPaint penCaptionShape;
    protected SKPathEffect dotLine;
    protected SKPathEffect dashLine;
    protected SKFont caption;
    protected SKFont captionShape;

    /// <summary>
    /// Canvas
    /// </summary>
    public virtual SKSurface Image { get; protected set; }

    /// <summary>
    /// Instance
    /// </summary>
    /// <returns></returns>
    public override IEngine Instance => Image is null ? null : this;

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public override IEngine Create(double index, double value)
    {
      Dispose();

      curve = new SKPath();
      dotLine = SKPathEffect.CreateDash([1, 3], 0);
      dashLine = SKPathEffect.CreateDash([3, 3], 0);

      penLine = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Stroke,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      penCircle = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Fill,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      penBox = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Fill,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      penShape = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Fill,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      penCaption = new SKPaint
      {
        Color = SKColors.Black,
        IsAntialias = true,
        IsStroke = false,
        IsDither = false
      };

      penCaptionShape = new SKPaint
      {
        Color = SKColors.Black,
        IsAntialias = false,
        IsStroke = false,
        IsDither = false
      };

      caption = new SKFont { Size = 10 };
      captionShape = new SKFont { Size = 10 };

      X = Math.Round(Math.Max(index, 5), MidpointRounding.ToZero);
      Y = Math.Round(Math.Max(value, 5), MidpointRounding.ToZero);

      Image = SKSurface.Create(new SKImageInfo((int)X, (int)Y));

      return this;
    }

    /// <summary>
    /// Encode as image
    /// </summary>
    /// <param name="imageType"></param>
    /// <param name="quality"></param>
    /// <returns></returns>
    public override byte[] Encode(SKEncodedImageFormat imageType, int quality)
    {
      using (var image = Image.Snapshot().Encode(imageType, quality))
      {
        return image is null ? [] : image.ToArray();
      }
    }

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public override void CreateLine(IList<DataModel> coordinates, ComponentModel shape)
    {
      penLine.Color = shape.Color;
      penLine.StrokeWidth = (float)shape.Size;

      switch (shape.Composition)
      {
        case CompositionEnum.Dots: penLine.PathEffect = dotLine; break;
        case CompositionEnum.Dashes: penLine.PathEffect = dashLine; break;
      }

      Image.Canvas.DrawLine(
        (float)coordinates[0].X,
        (float)coordinates[0].Y,
        (float)coordinates[1].X,
        (float)coordinates[1].Y,
        penLine);
    }

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="shape"></param>
    public override void CreateCircle(DataModel coordinate, ComponentModel shape)
    {
      penCircle.Color = shape.Color;

      Image.Canvas.DrawCircle(
        (float)coordinate.X,
        (float)coordinate.Y,
        (float)shape.Size,
        penCircle);
    }

    /// <summary>
    /// Create box
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public override void CreateBox(IList<DataModel> coordinates, ComponentModel shape)
    {
      penBox.Color = shape.Color;

      Image.Canvas.DrawRect(
        (float)coordinates[0].X,
        (float)coordinates[0].Y,
        (float)(coordinates[1].X - coordinates[0].X),
        (float)(coordinates[1].Y - coordinates[0].Y),
        penBox);
    }

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public override void CreateShape(IList<DataModel> coordinates, ComponentModel shape)
    {
      var origin = coordinates.ElementAtOrDefault(0);

      penShape.Color = shape.Color;

      curve.Reset();
      curve.MoveTo((float)origin.X, (float)origin.Y);

      for (var i = 1; i < coordinates.Count; i++)
      {
        curve.LineTo((float)coordinates[i].X, (float)coordinates[i].Y);
      }

      Image.Canvas.DrawPath(curve, penShape);
    }

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public override void CreateCaption(DataModel coordinate, ComponentModel shape, string content)
    {
      penCaption.Color = shape.Color;
      caption.Size = (float)shape.Size;

      var position = SKTextAlign.Center;

      switch (shape.Position)
      {
        case PositionEnum.L: position = SKTextAlign.Left; break;
        case PositionEnum.R: position = SKTextAlign.Right; break;
      }

      var space = (caption.Spacing - caption.Size) / 2;

      Image.Canvas.DrawText(
        content,
        (float)coordinate.X,
        (float)(coordinate.Y - space),
        position,
        caption,
        penCaption);
    }

    /// <summary>
    /// Draw label along the path
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public override void CreateCaptionShape(IList<DataModel> coordinates, ComponentModel shape, string content)
    {
      var origin = coordinates.ElementAtOrDefault(0);

      penCaptionShape.Color = shape.Color;
      captionShape.Size = (float)shape.Size;

      curve.Reset();
      curve.MoveTo((float)origin.X, (float)origin.Y);

      for (var i = 1; i < coordinates.Count; i++)
      {
        curve.LineTo((float)coordinates[i].X, (float)coordinates[i].Y);
      }

      penCaptionShape.Color = shape.Color;
      captionShape.Size = (float)shape.Size;

      Image.Canvas.DrawTextOnPath(content, curve, 0, captionShape.Size / 2, captionShape, penCaptionShape);
    }

    /// <summary>
    /// Measure content
    /// </summary>
    /// <param name="content"></param>
    /// <param name="size"></param>
    public override DataModel GetContentMeasure(string content, double size)
    {
      caption.Size = (float)size;

      var item = new DataModel
      {
        X = content.Length * Math.Min(caption.Metrics.MaxCharacterWidth, size),
        Y = caption.Spacing
      };

      return item;
    }

    /// <summary>
    /// Clear canvas
    /// </summary>
    public override void Clear()
    {
      Image.Canvas.Clear(SKColors.Transparent);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public override void Dispose()
    {
      Image?.Dispose();

      dotLine?.Dispose();
      dashLine?.Dispose();
      penLine?.Dispose();
      penCircle?.Dispose();
      penBox?.Dispose();
      penShape?.Dispose();
      penCaption?.Dispose();
      penCaptionShape?.Dispose();

      Image = null;
    }
  }
}
