using Canvas.Core;
using Canvas.Core.ComposerSpace;
using Canvas.Core.EnumSpace;
using Canvas.Core.MessageSpace;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;

namespace Canvas.Views.Web
{
  public partial class IndexScaleView : IMessenger, IDisposable
  {
    [Parameter] public PositionEnum View { get; set; }

    [Inject] protected virtual IJSRuntime RuntimeService { get; set; }

    protected virtual string Route { get; set; }
    protected virtual IComposer Composer { get; set; }
    protected virtual ElementReference CanvasContainer { get; set; }

    /// <summary>
    /// Resize event
    /// </summary>
    /// <param name="e"></param>
    /// <param name="direction"></param>
    protected void OnScaleMove(MouseEventArgs e, int direction = 0)
    {
      //if (Engine?.GetInstance() is null)
      //{
      //  return;
      //}

      //var position = GetDelta(e);

      //Move ??= position;

      //if (e.Buttons == 1)
      //{
      //  var isZoom = e.ShiftKey;
      //  var deltaX = Move.X - position.X;
      //  var deltaY = Move.Y - position.Y;

      //  switch (direction > 0)
      //  {
      //    case true when deltaX > 0: _ = Composer.ZoomIndexScale(Engine, -1); break;
      //    case true when deltaX < 0: _ = Composer.ZoomIndexScale(Engine, 1); break;
      //  }

      //  switch (direction < 0)
      //  {
      //    case true when deltaY > 0: Composer.ZoomValueScale(Engine, -1); break;
      //    case true when deltaY < 0: Composer.ZoomValueScale(Engine, 1); break;
      //  }

      //  Update(Composer);
      //}

      //Move = position;
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    public void Update(DomainMessage message = null)
    {
    }
  }
}
