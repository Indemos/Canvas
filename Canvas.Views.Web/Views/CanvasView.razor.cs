using Canvas.Core;
using Canvas.Core.ComposerSpace;
using Canvas.Core.ModelSpace;
using System;
using System.Threading.Tasks;

namespace Canvas.Views.Web.Views
{
  public partial class CanvasView
  {
    public virtual ScreenView Screen { get; set; }

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="EngineType"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public override async Task<IView> Create<EngineType>(Func<IComposer> action)
    {
      Composer = action();

      await Screen.Create<EngineType>(() => Composer);

      Composer.Views[nameof(Screen)] = Screen;

      return this;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    public override Task Update(DomainModel message, string source = null) => Composer.Update(message, source);
  }
}
