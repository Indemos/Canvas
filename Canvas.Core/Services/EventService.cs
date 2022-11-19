using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using System;

namespace Canvas.Core.ServiceSpace
{
  public class EventService : IDisposable
  {
    public virtual IView View { get; set; }

    protected virtual ViewModel? Position { get; set; }
    protected virtual ViewModel? ScreenPosition { get; set; }
    protected virtual IComposer Composer => View?.Composer;
    protected virtual IEngine Engine => View?.Engine;

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnWheel(ViewModel e)
    {
      var isZoom = e.IsShape;
      var message = Composer.Domain;

      switch (true)
      {
        case true when e.Data.Y > 0: message.IndexDomain = isZoom ? Composer.ZoomIndex(Engine, -1) : Composer.PanIndex(Engine, 1); break;
        case true when e.Data.Y < 0: message.IndexDomain = isZoom ? Composer.ZoomIndex(Engine, 1) : Composer.PanIndex(Engine, -1); break;
      }

      Composer.Update(message, Composer.Name);
    }

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnMouseMove(ViewModel e)
    {
      ScreenPosition ??= e;

      if (e.IsMove)
      {
        var deltaX = ScreenPosition?.Data.X - e.Data.X;
        var deltaY = ScreenPosition?.Data.Y - e.Data.Y;
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
    public virtual void OnScale(ViewModel e, int orientation = 0)
    {
      Position ??= e;

      if (e.IsMove)
      {
        var deltaX = Position?.Data.X - e.Data.X;
        var deltaY = Position?.Data.Y - e.Data.Y;
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
    public virtual void OnMouseDown(ViewModel e)
    {
      if (e.IsControl)
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
    public virtual void OnMouseLeave(ViewModel e)
    {
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
