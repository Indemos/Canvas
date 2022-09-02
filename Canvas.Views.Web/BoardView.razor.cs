using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

namespace Canvas.Views.Web
{
  public partial class BoardView : IDisposable
  {
    [Inject] protected virtual IJSRuntime RuntimeService { get; set; }

    /// <summary>
    /// Internals
    /// </summary>
    protected virtual string Route { get; set; }
    protected virtual ElementReference CanvasContainer { get; set; }


    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
    }
  }
}
