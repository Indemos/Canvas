using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.MessageSpace;
using System;

namespace Canvas.Core.ServiceSpace
{
  public class ViewService : IDisposable
  {
    public virtual IView View { get; set; }

    protected virtual ViewMessage? Position { get; set; }
    protected virtual ViewMessage? ScreenPosition { get; set; }
    protected virtual IComposer Composer => View?.Composer;
    protected virtual IEngine Engine => View?.Engine;

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
      var message = Composer.Domain;

      switch (true)
      {
        case true when e?.Y > 0: message.IndexDomain = isZoom ? Composer.ZoomIndex(Engine, -1) : Composer.PanIndex(Engine, 1); break;
        case true when e?.Y < 0: message.IndexDomain = isZoom ? Composer.ZoomIndex(Engine, 1) : Composer.PanIndex(Engine, -1); break;
      }

      Composer.Update(message, Composer.Name);
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
        var message = Composer.Domain;

        switch (true)
        {
          case true when deltaX > 0: message.IndexDomain = Composer.PanIndex(Engine, 1); break;
          case true when deltaX < 0: message.IndexDomain = Composer.PanIndex(Engine, -1); break;
        }

        Composer.Update(message, Composer.Name);
      }

      ScreenPosition = e;
    }

    /// <summary>
    /// Resize event
    /// </summary>
    /// <param name="e"></param>
    /// <param name="orientation"></param>
    public virtual void OnScale(ViewMessage? e, int orientation = 0)
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
        var message = Composer.Domain;
        var source = Composer.Name;

        switch (orientation > 0)
        {
          case true when deltaX > 0: message.IndexDomain = Composer.ZoomIndex(Engine, -1); break;
          case true when deltaX < 0: message.IndexDomain = Composer.ZoomIndex(Engine, 1); break;
        }

        switch (orientation < 0)
        {
          case true when deltaY > 0: message.ValueDomain = Composer.ZoomValue(Engine, -1); source = null; break;
          case true when deltaY < 0: message.ValueDomain = Composer.ZoomValue(Engine, 1); source = null; break;
        }

        Composer.Update(message, source);
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
        var message = Composer.Domain;

        message.ValueDomain = null;

        Composer.Update(message);
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

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
    }
  }
}
