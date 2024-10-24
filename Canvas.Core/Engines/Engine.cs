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
    /// Update
    /// </summary>
    /// <param name="indexSize"></param>
    /// <param name="valueSize"></param>
    /// <returns></returns>
    IEngine Update(double indexSize, double valueSize);

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="coordinate"></param>
    void CreateCircle(DataModel coordinate, ComponentModel shape);

    /// <summary>
    /// Create box
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    void CreateBox(IList<DataModel> coordinates, ComponentModel shape);

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    void CreateLine(IList<DataModel> coordinates, ComponentModel shape);

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    void CreateShape(IList<DataModel> coordinates, ComponentModel shape);

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    void CreateCaption(DataModel coordinate, ComponentModel shape, string content);

    /// <summary>
    /// Draw label along the path
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    void CreateCaptionShape(IList<DataModel> coordinates, ComponentModel shape, string content);

    /// <summary>
    /// Measure content
    /// </summary>
    /// <param name="content"></param>
    /// <param name="size"></param>
    DataModel GetContentMeasure(string content, double size);

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
    /// Update
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public abstract IEngine Update(double x, double y);

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="coordinate"></param>
    public abstract void CreateCircle(DataModel coordinate, ComponentModel shape);

    /// <summary>
    /// Create box
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public abstract void CreateBox(IList<DataModel> coordinates, ComponentModel shape);

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public abstract void CreateLine(IList<DataModel> coordinates, ComponentModel shape);

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public abstract void CreateShape(IList<DataModel> coordinates, ComponentModel shape);

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public abstract void CreateCaption(DataModel coordinate, ComponentModel shape, string content);

    /// <summary>
    /// Draw label along the path
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public abstract void CreateCaptionShape(IList<DataModel> coordinates, ComponentModel shape, string content);

    /// <summary>
    /// Measure content
    /// </summary>
    /// <param name="content"></param>
    /// <param name="size"></param>
    public abstract DataModel GetContentMeasure(string content, double size);

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
