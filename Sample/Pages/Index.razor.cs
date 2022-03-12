using Control;
using Core;
using System.Reactive.Subjects;

namespace Client.Pages
{
  public partial class Index
  {
    protected CanvasView CanvasControl { get; set; }

    protected override void OnAfterRender(bool setup)
    {
      if (setup)
      {
        CanvasControl.Name = "Demo";
        CanvasControl.Composer = new Composer();
        CanvasControl.Domains = new Subject<Composer>();
      }

      base.OnAfterRender(setup);
    }
  }
}
