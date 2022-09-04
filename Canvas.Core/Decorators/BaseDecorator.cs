using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using System;

namespace Canvas.Core.DecoratorSpace
{
  public interface IDecorator
  {
    /// <summary>
    /// Create shape
    /// </summary>
    Action<IEngine> CreateIndex { get; set; }

    /// <summary>
    /// Update shape
    /// </summary>
    Action<IEngine> CreateValue { get; set; }

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
    public virtual Action<IEngine> CreateIndex { get; set; } = o => { };

    /// <summary>
    /// Update shape
    /// </summary>
    public virtual Action<IEngine> CreateValue { get; set; } = o => { };

    /// <summary>
    /// Composer
    /// </summary>
    public virtual IComposer Composer { get; set; }
  }
}
