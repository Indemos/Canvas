using Canvas.Core;
using Canvas.Core.DecoratorSpace;
using Canvas.Core.MessageSpace;
using SkiaSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Canvas.Views.Web.Views
{
  public partial class BoardView
  {
    public virtual IView T { get; set; }
    public virtual IView B { get; set; }
    public virtual IView L { get; set; }
    public virtual IView R { get; set; }
    public virtual IView Screen { get; set; }

    protected virtual BoardDecorator Decorator { get; set; }
    protected virtual TaskCompletionSource<int> Completion { get; set; }

    public void OnScreenLeave(ViewMessage? e) => UpdateDecorator();

    public void OnScreenMove(ViewMessage? e)
    {
      if (Completion?.Task?.IsCompleted is false)
      {
        return;
      }

      Decorator ??= new BoardDecorator
      {
        T = T?.Engine,
        B = B?.Engine,
        L = L?.Engine,
        R = R?.Engine,
        Screen = Screen?.Engine,
        Composer = Composer
      };

      UpdateDecorator(() => Decorator.Create(Engine, e.Value));
    }

    protected virtual void UpdateDecorator(Action action = null)
    {
      Completion = ScheduleService.Send(() =>
      {
        if (Engine?.GetInstance() is null)
        {
          return 0;
        }

        Engine.Clear();

        if (action is not null)
        {
          action();
        }

        Route = "data:image/webp;base64," + Convert.ToBase64String(Engine.Encode(SKEncodedImageFormat.Webp, 100));

        InvokeAsync(StateHasChanged);

        return 0;
      });
    }
  }
}
