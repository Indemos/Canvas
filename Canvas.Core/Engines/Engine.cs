using Canvas.Core.ModelSpace;
using System;
using System.Collections.Generic;

namespace Canvas.Core.EngineSpace
{
  public interface IEngine : IDisposable
  {
    /// <summary>
    /// Name
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Width
    /// </summary>
    double IndexSize { get; set; }

    /// <summary>
    /// Height
    /// </summary>
    double ValueSize { get; set; }

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="coordinate"></param>
    void CreateCircle(IItemModel coordinate, IComponentModel shape);

    /// <summary>
    /// Create box
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    void CreateBox(IList<IItemModel> coordinates, IComponentModel shape);

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    void CreateLine(IList<IItemModel> coordinates, IComponentModel shape);

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    void CreateShape(IList<IItemModel> coordinates, IComponentModel shape);

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    void CreateLabel(IItemModel coordinate, IComponentModel shape, string content);

    /// <summary>
    /// Draw label along the path
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    void CreateLabelShape(IList<IItemModel> coordinates, IComponentModel shape, string content);

    /// <summary>
    /// Measure content
    /// </summary>
    /// <param name="content"></param>
    /// <param name="size"></param>
    IItemModel GetContentMeasure(string content, double size);

    /// <summary>
    /// Clear canvas
    /// </summary>
    void Clear();
  }

  public abstract class Engine : IEngine
  {
    /// <summary>
    /// Name
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Width
    /// </summary>
    public virtual double IndexSize { get; set; }

    /// <summary>
    /// Height
    /// </summary>
    public virtual double ValueSize { get; set; }

    /// <summary>
    /// Create circle
    /// </summary>
    /// <param name="coordinate"></param>
    public virtual void CreateCircle(IItemModel coordinate, IComponentModel shape)
    {
    }

    /// <summary>
    /// Create box
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public virtual void CreateBox(IList<IItemModel> coordinates, IComponentModel shape)
    {
    }

    /// <summary>
    /// Create line
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public virtual void CreateLine(IList<IItemModel> coordinates, IComponentModel shape)
    { 
    }

    /// <summary>
    /// Create shape
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    public virtual void CreateShape(IList<IItemModel> coordinates, IComponentModel shape)
    {
    }

    /// <summary>
    /// Create label
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public virtual void CreateLabel(IItemModel coordinate, IComponentModel shape, string content)
    {
    }

    /// <summary>
    /// Draw label along the path
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="shape"></param>
    /// <param name="content"></param>
    public virtual void CreateLabelShape(IList<IItemModel> coordinates, IComponentModel shape, string content)
    {
    }

    /// <summary>
    /// Measure content
    /// </summary>
    /// <param name="content"></param>
    /// <param name="size"></param>
    public virtual IItemModel GetContentMeasure(string content, double size) => null;

    /// <summary>
    /// Clear canvas
    /// </summary>
    public virtual void Clear()
    {
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
    }
  }
}
