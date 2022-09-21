using Canvas.Core;
using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
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
    public override async Task<IView> Create<EngineType>(Func<IEngine, IComposer> action)
    {
      if (Composer is not null)
      {
        Composer.OnDomain -= T.Update;
        Composer.OnDomain -= B.Update;
        Composer.OnDomain -= L.Update;
        Composer.OnDomain -= R.Update;
        Composer.OnDomain -= Grid.Update;
        Composer.OnDomain -= Screen.Update;
      }

      Screen.OnMouseMove -= Board.OnScreenMove;
      Screen.OnMouseLeave -= Board.OnScreenLeave;

      Composer = action(Screen.Engine);

      await T.Create<EngineType>(engine => Composer);
      await B.Create<EngineType>(engine => Composer);
      await L.Create<EngineType>(engine => Composer);
      await R.Create<EngineType>(engine => Composer);
      await Grid.Create<EngineType>(engine => Composer);
      await Board.Create<EngineType>(engine => Composer);
      await Screen.Create<EngineType>(engine => Composer);

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
  }
}
