using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.MessageSpace;
using Canvas.Core.ModelSpace;

namespace Canvas.Core
{
  public class Reactor
  {
    public virtual IView View { get; set; }
    public virtual IEngine Engine { get; set; }
    public virtual IComposer Composer { get; set; }

    protected virtual ViewMessage? Position { get; set; }
    protected virtual ViewMessage? ScreenPosition { get; set; }

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnWheel(ViewMessage? e)
    {
      if (Engine?.GetInstance() is null)
      {
        return;
      }

      var isZoom = e.Value.IsShape;
      var message = new DomainMessage
      {
        Code = View.Name
      };

      switch (true)
      {
        case true when e?.Y > 0: message.IndexDomain = isZoom ? Composer.ZoomIndexScale(Engine, -1) : Composer.PanIndexScale(Engine, 1); break;
        case true when e?.Y < 0: message.IndexDomain = isZoom ? Composer.ZoomIndexScale(Engine, 1) : Composer.PanIndexScale(Engine, -1); break;
      }

      View.Update(message);
    }

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnMouseMove(ViewMessage? e)
    {
      if (Engine?.GetInstance() is null)
      {
        return;
      }

      ScreenPosition ??= e;

      if (e.Value.IsSnap)
      {
        var deltaX = ScreenPosition?.X - e?.X;
        var deltaY = ScreenPosition?.Y - e?.Y;
        var message = new DomainMessage
        {
          Code = View.Name
        };

        switch (true)
        {
          case true when deltaX > 0: message.IndexDomain = Composer.PanIndexScale(Engine, 1); break;
          case true when deltaX < 0: message.IndexDomain = Composer.PanIndexScale(Engine, -1); break;
        }

        View.Update(message);
      }

      ScreenPosition = e;
    }

    /// <summary>
    /// Resize event
    /// </summary>
    /// <param name="e"></param>
    /// <param name="direction"></param>
    public virtual void OnScale(ViewMessage? e, int direction = 0)
    {
      if (Engine?.GetInstance() is null)
      {
        return;
      }

      Position ??= e;

      if (e.Value.IsSnap)
      {
        var deltaX = Position?.X - e?.X;
        var deltaY = Position?.Y - e?.Y;
        var message = new DomainMessage
        {
          Code = View.Name
        };

        switch (direction > 0)
        {
          case true when deltaX > 0: message.IndexDomain = Composer.ZoomIndexScale(Engine, -1); break;
          case true when deltaX < 0: message.IndexDomain = Composer.ZoomIndexScale(Engine, 1); break;
        }

        switch (direction < 0)
        {
          case true when deltaY > 0: message.ValueDomain = Composer.ZoomValueScale(Engine, -1); break;
          case true when deltaY < 0: message.ValueDomain = Composer.ZoomValueScale(Engine, 1); break;
        }

        View.Update(message);
      }

      Position = e;
    }

    /// <summary>
    /// Click event in the view area
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnMouseDown(ViewMessage? e)
    {
      if (Engine?.GetInstance() is null)
      {
        return;
      }

      if (e.Value.IsControl)
      {
        var message = new DomainMessage
        {
          Code = View.Name,
          ValueUpdate = true
        };

        View.Update(message);
      }
    }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnMouseLeave(ViewMessage? e)
    {
      if (Engine?.GetInstance() is null)
      {
        return;
      }

      ScreenPosition = null;
    }
  }
}
