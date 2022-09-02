using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using SkiaSharp;
using System;

namespace Canvas.Core.DecoratorSpace
{
  public interface IDecorator : IDisposable
  {
    /// <summary>
    /// Create shape
    /// </summary>
    Action<IEngine> Create { get; set; }

    /// <summary>
    /// Update shape
    /// </summary>
    Action<IEngine> Update { get; set; }

    /// <summary>
    /// Composer
    /// </summary>
    IComposer Composer { get; set; }
  }

  public abstract class BaseDecorator : IDecorator
  {
    /// <summary>
    /// Create shape
    /// </summary>
    public virtual Action<IEngine> Create { get; set; }

    /// <summary>
    /// Update shape
    /// </summary>
    public virtual Action<IEngine> Update { get; set; }

    /// <summary>
    /// Composer
    /// </summary>
    public virtual IComposer Composer { get; set; }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
    }
  }
}
