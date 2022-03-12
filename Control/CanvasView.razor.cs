using Core;
using Core.ControlSpace;
using Core.ModelSpace;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ScriptContainer;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Timers;

namespace Control
{
  public partial class CanvasView : IDisposable
  {
    protected bool Setup;
    protected StreamServer Server;
    protected ScriptControl ScaleContainer;
    protected ElementReference CanvasContainer;
    protected IDictionary<string, dynamic> Cache = new Dictionary<string, dynamic>();

    public virtual string Name { get; set; }
    public virtual Composer Composer { get; set; }
    public virtual Subject<Composer> Domains { get; set; }

    /// <summary>
    /// Load
    /// </summary>
    /// <param name="setup"></param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool setup)
    {
      if (setup)
      {
        Server ??= await StreamServer.Create();
        ScaleContainer.OnSize = async message => await GetBounds();
        ScaleContainer.OnLoad = async () => await GetBounds();
      }

      await base.OnAfterRenderAsync(setup);
    }

    /// <summary>
    /// Canvas rendering
    /// </summary>
    /// <returns></returns>
    protected async Task GetBounds()
    {
      Composer?.Dispose();

      var bounds = await ScaleContainer.GetElementBounds(CanvasContainer);
      //var canvas = new CanvasCompositeControl(bounds.Width, bounds.Height);

      //Composer = new Composer();
      //Composer.Canvas = canvas;

      //using (var image = SKImage.FromBitmap(canvas.Map))
      //{
      //  canvas.CreateCircle(
      //    new PointModel { Index = 50, Value = 50 },
      //    new ShapeModel { Size = 20, Color = SKColors.Black });

      //  var data = image.Encode(SKEncodedImageFormat.Webp, 100).ToArray();

      //  await Server.Stream.Writer.WriteAsync(data);
      //}

      var map = new SKBitmap(250, 250);
      var canvas = new SKCanvas(map);
      var pos = new Random().Next(50, 150);
      using (var image = SKImage.FromBitmap(map))
      {
        var inPaint = new SKPaint
        {
          Color = SKColors.Black,
          Style = SKPaintStyle.Fill,
          FilterQuality = SKFilterQuality.High
        };
        var exPaint = new SKPaint
        {
          Color = SKColors.White,
          Style = SKPaintStyle.Fill,
          FilterQuality = SKFilterQuality.Low
        };

        canvas.DrawRect(0, 0, 250, 250, exPaint);
        canvas.DrawCircle(pos, pos, 20, inPaint);
        var data = image.Encode(SKEncodedImageFormat.Webp, 100).ToArray();
        await Server.Stream.Writer.WriteAsync(data);
      }

      //await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    protected void Dispose(int o)
    {
      Composer?.Dispose();
      Server?.Dispose();
    }

    /// <summary>
    /// Dispose
    /// </summary>
    void IDisposable.Dispose()
    {
      Dispose(0);
    }

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="e"></param>
    protected void OnWheel(WheelEventArgs e)
    {
      if (Setup is false)
      {
        return;
      }

      //var isZoom = e.ShiftKey;

      //switch (true)
      //{
      //  case true when e.DeltaX > 0: _ = isZoom ? Composer.ZoomIndexScale(1) : Composer.PanIndexScale(1); break;
      //  case true when e.DeltaX < 0: _ = isZoom ? Composer.ZoomIndexScale(-1) : Composer.PanIndexScale(-1); break;
      //}

      //Domains.OnNext(Composer);
    }

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="e"></param>
    protected void OnMouseMove(MouseEventArgs e)
    {
      if (Setup is false)
      {
        return;
      }

      //var position = new PointModel
      //{
      //  Index = e.ClientX,
      //  Value = e.ClientY
      //};

      //if (Equals(e.Button, 0))
      //{
      //  var deltaX = position.Index;
      //  var deltaY = position.Value;
      //  var isZoom = e.ShiftKey;

      //  switch (true)
      //  {
      //    case true when deltaX > 0: _ = isZoom ? Composer.ZoomIndexScale(-1) : Composer.PanIndexScale(1); break;
      //    case true when deltaX < 0: _ = isZoom ? Composer.ZoomIndexScale(1) : Composer.PanIndexScale(-1); break;
      //  }

      //  switch (true)
      //  {
      //    case true when deltaY > 0: Composer.ZoomValueScale(-1); break;
      //    case true when deltaY < 0: Composer.ZoomValueScale(1); break;
      //  }
      //}

      //Domains.OnNext(Composer);
    }

    /// <summary>
    /// Double clck event in the view area
    /// </summary>
    /// <param name="e"></param>
    protected void OnMouseDown(MouseEventArgs e)
    {
      if (Setup is false)
      {
        return;
      }

      //Composer.ValueDomain = null;

      //Domains.OnNext(Composer);
    }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="e"></param>
    protected void OnMouseLeave(MouseEventArgs e)
    {
      if (Setup is false)
      {
        return;
      }
    }

    /// <summary>
    /// Create canvas
    /// </summary>
    protected void Create()
    {
      Dispose(0);
    }

    /// <summary>
    /// Update canvas
    /// </summary>
    protected void Update()
    {
    }
  }
}
