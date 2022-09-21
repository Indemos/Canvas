using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.MessageSpace;
using System;
using System.Threading.Tasks;

namespace Canvas.Core
{
  public interface IView : IDisposable
  {
    /// <summary>
    /// Engine
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// Composer
    /// </summary>
    IComposer Composer { get; }

    /// <summary>
    /// Mouse move event
    /// </summary>
    /// <param name="e"></param>
    Action<ViewMessage?> OnMouseMove { get; set; }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="e"></param>
    Action<ViewMessage?> OnMouseLeave { get; set; }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    void Update(DomainMessage message, string source = null);

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<IView> Create<T>(Func<IEngine, IComposer> action) where T : IEngine, new();
  }
}
