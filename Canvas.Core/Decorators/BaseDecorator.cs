using Canvas.Core.ComposerSpace;

namespace Canvas.Core.DecoratorSpace
{
  public interface IDecorator
  {
    /// <summary>
    /// Composer
    /// </summary>
    IComposer Composer { get; set; }
  }

  public abstract class BaseDecorator : IDecorator
  {
    /// <summary>
    /// Composer
    /// </summary>
    public virtual IComposer Composer { get; set; }
  }
}
