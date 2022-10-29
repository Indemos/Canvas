using Canvas.Core;
using Canvas.Core.ComposerSpace;
using Canvas.Core.ModelSpace;
using System;
using System.Threading.Tasks;

namespace Canvas.Views.Web.Views
{
  public partial class CanvasView
  {
    public virtual GridView Grid { get; set; }
    public virtual BoardView Board { get; set; }
    public virtual ScreenView Screen { get; set; }
    public virtual IndexScaleView T { get; set; }
    public virtual IndexScaleView B { get; set; }
    public virtual ValueScaleView L { get; set; }
    public virtual ValueScaleView R { get; set; }

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="EngineType"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public override async Task<IView> Create<EngineType>(Func<IComposer> action)
    {
      if (Composer is not null)
      {
        Composer.OnDomain = null;
      }

      Screen.OnMouseMove -= Board.OnScreenMove;
      Screen.OnMouseLeave -= Board.OnScreenLeave;

      Composer = action();

      await T.Create<EngineType>(() => Composer);
      await B.Create<EngineType>(() => Composer);
      await L.Create<EngineType>(() => Composer);
      await R.Create<EngineType>(() => Composer);
      await Grid.Create<EngineType>(() => Composer);
      await Board.Create<EngineType>(() => Composer);
      await Screen.Create<EngineType>(() => Composer);

      Composer.OnDomain += T.Update;
      Composer.OnDomain += B.Update;
      Composer.OnDomain += L.Update;
      Composer.OnDomain += R.Update;
      Composer.OnDomain += Grid.Update;
      Composer.OnDomain += Screen.Update;

      Board.T = T;
      Board.B = B;
      Board.L = L;
      Board.R = R;
      Board.Screen = Screen;
      Screen.OnMouseMove += Board.OnScreenMove;
      Screen.OnMouseLeave += Board.OnScreenLeave;

      return this;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    public override void Update(DomainModel message, string source = null) => Composer.Update(message, source);
  }
}
