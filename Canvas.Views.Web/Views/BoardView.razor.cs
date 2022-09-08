using Canvas.Core;
using Canvas.Core.DecoratorSpace;
using Canvas.Core.MessageSpace;
using SkiaSharp;
using System;

namespace Canvas.Views.Web.Views
{
  public partial class BoardView : BaseView
  {
    public virtual IView T { get; set; }
    public virtual IView B { get; set; }
    public virtual IView L { get; set; }
    public virtual IView R { get; set; }
    public virtual IView Screen { get; set; }

    protected virtual BoardDecorator Decorator { get; set; }

    public void OnScreenMove(ViewMessage e)
    {
      if (Screen?.Engine?.GetInstance() is null)
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

      Engine.Clear();
      Decorator.Create(Engine, e);

      Route = "data:image/webp;base64," + Convert.ToBase64String(Engine.Encode(SKEncodedImageFormat.Webp, 100));

      StateHasChanged();
    }

    public void OnScreenLeave(ViewMessage e)
    {
      if (Screen?.Engine?.GetInstance() is null)
      {
        return;
      }

      Engine.Clear();

      Route = "data:image/webp;base64," + Convert.ToBase64String(Engine.Encode(SKEncodedImageFormat.Webp, 100));

      StateHasChanged();
    }
  }
}
