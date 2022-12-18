using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using System;
using System.Threading.Tasks;

namespace Canvas.Core
{
  public interface IView : IAsyncDisposable
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
    Action<ViewModel> OnMouseMove { get; set; }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="e"></param>
    Action<ViewModel> OnMouseLeave { get; set; }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    Task Update(DomainModel message, string source = null);

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<IView> Create<T>(Func<IComposer> action) where T : IEngine, new();
  }
}
