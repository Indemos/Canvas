using Canvas.Core.Composers;
using Canvas.Core.Engines;
using Canvas.Core.Models;

namespace Canvas.Core.Services
{
  public class EventService
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
      var domain = Composer.Domain;

      switch (true)
      {
        case true when e.Data.Y > 0: domain.IndexDomain = isZoom ? Composer.ZoomIndex(Engine, -1) : Composer.PanIndex(Engine, 1); break;
        case true when e.Data.Y < 0: domain.IndexDomain = isZoom ? Composer.ZoomIndex(Engine, 1) : Composer.PanIndex(Engine, -1); break;
      }

      Composer.Update(domain, Composer.Name);
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
        var domain = Composer.Domain;

        switch (true)
        {
          case true when deltaX > 0: domain.IndexDomain = Composer.PanIndex(Engine, 1); break;
          case true when deltaX < 0: domain.IndexDomain = Composer.PanIndex(Engine, -1); break;
        }

        Composer.Update(domain, Composer.Name);
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
        var domain = Composer.Domain;
        var source = Composer.Name;

        switch (orientation > 0)
        {
          case true when deltaX > 0: domain.IndexDomain = Composer.ZoomIndex(Engine, -1); break;
          case true when deltaX < 0: domain.IndexDomain = Composer.ZoomIndex(Engine, 1); break;
        }

        switch (orientation < 0)
        {
          case true when deltaY > 0: domain.ValueDomain = Composer.ZoomValue(Engine, -1); source = null; break;
          case true when deltaY < 0: domain.ValueDomain = Composer.ZoomValue(Engine, 1); source = null; break;
        }

        Composer.Update(domain, source);
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
        var domain = Composer.Domain;

        domain.ValueDomain = null;

        Composer.Update(domain);
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
  }
}
