using Canvas.Core.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Canvas.Core.Engines
{
  public interface IEngine : IDisposable
  {
    /// <summary>
    /// Width
    /// </summary>
    double X { get; set; }

    /// <summary>
    /// Height
    /// </summary>
    double Y { get; set; }

    /// <summary>
    /// Instance
    /// </summary>
    /// <returns></returns>
    IEngine Instance { get; }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="indexSize"></param>
    /// <param name="valueSize"></param>
    /// <returns></returns>
    IEngine Create(double indexSize, double valueSize);

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="coordinate"></param>
    void CreateCircle(Unit coordinate, Section shape);

    /// <summary>
    /// Create box
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    void CreateBox(IList<Unit> coordinates, Section shape);

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    void CreateLine(IList<Unit> coordinates, Section shape);

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    void CreateShape(IList<Unit> coordinates, Section shape);

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    void CreateCaption(Unit coordinate, Section shape, string content);

    /// <summary>
    /// Draw label along the path
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    void CreateCaptionShape(IList<Unit> coordinates, Section shape, string content);

    /// <summary>
    /// Measure content
    /// </summary>
    /// <param name="content"></param>
    /// <param name="size"></param>
    Unit GetContentMeasure(string content, double size);

    /// <summary>
    /// Encode as image
    /// </summary>
    /// <param name="imageType"></param>
    /// <param name="quality"></param>
    /// <returns></returns>
    byte[] Encode(SKEncodedImageFormat imageType, int quality);

    /// <summary>
    /// Clear canvas
    /// </summary>
    void Clear();
  }

  public abstract class Engine : IEngine
  {
    /// <summary>
    /// Width
    /// </summary>
    public virtual double X { get; set; }

    /// <summary>
    /// Height
    /// </summary>
    public virtual double Y { get; set; }

    /// <summary>
    /// Instance
    /// </summary>
    /// <returns></returns>
    public virtual IEngine Instance { get; }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public abstract IEngine Create(double x, double y);

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="coordinate"></param>
    public abstract void CreateCircle(Unit coordinate, Section shape);

    /// <summary>
    /// Create box
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public abstract void CreateBox(IList<Unit> coordinates, Section shape);

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public abstract void CreateLine(IList<Unit> coordinates, Section shape);

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public abstract void CreateShape(IList<Unit> coordinates, Section shape);

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public abstract void CreateCaption(Unit coordinate, Section shape, string content);

    /// <summary>
    /// Draw label along the path
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public abstract void CreateCaptionShape(IList<Unit> coordinates, Section shape, string content);

    /// <summary>
    /// Measure content
    /// </summary>
    /// <param name="content"></param>
    /// <param name="size"></param>
    public abstract Unit GetContentMeasure(string content, double size);

    /// <summary>
    /// Encode as image
    /// </summary>
    /// <param name="imageType"></param>
    /// <param name="quality"></param>
    /// <returns></returns>
    public abstract byte[] Encode(SKEncodedImageFormat imageType, int quality);

    /// <summary>
    /// Clear canvas
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// Dispose
    /// </summary>
    public abstract void Dispose();
  }
}
