using Canvas.Views;
using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace Canvas.Client.Controllers
{
  [ApiController]
  public class StreamController : ControllerBase
  {
    Random _generator = new Random();

    [Route("/source")]
    public Task<StreamResponse> Get()
    {
      var aTimer = new Timer();
      var response = new StreamResponse();

      aTimer.Interval = 1000;
      aTimer.Enabled = true;
      aTimer.Elapsed += async (sender, e) =>
      {
        var pos = _generator.Next(50, 150);
        var canvas = new CanvasEngine(600, 600);

        canvas.CreateBox(new List<IPointModel>
        {
          new PointModel { Index = 0, Value = 0 },
          new PointModel { Index = 600, Value = 600 }
        },
        new ShapeModel { Color = SKColors.Yellow });

        canvas.CreateCircle(
          new PointModel { Index = pos, Value = pos },
          new ShapeModel { Size = 20, Color = SKColors.Black });

        var data = canvas.Map.Encode(SKEncodedImageFormat.Webp, 100).ToArray();

        await response.Stream.Writer.WriteAsync(data);
      };

      return Task.FromResult(response);
    }

    //[Route("/source")]
    //public async Task Get()
    //{
    //  var res = new byte[1000];
    //  var boundary = Guid.NewGuid().ToString();
    //  var generator = new Random();
    //  var map = new SKBitmap(250, 250);
    //  var canvas = new SKCanvas(map);
    //  var inPaint = new SKPaint
    //  {
    //    Color = SKColors.Black,
    //    Style = SKPaintStyle.Fill,
    //    FilterQuality = SKFilterQuality.High
    //  };
    //  var exPaint = new SKPaint
    //  {
    //    Color = SKColors.White,
    //    Style = SKPaintStyle.Fill,
    //    FilterQuality = SKFilterQuality.Low
    //  };

    //  Response.ContentType = "multipart/x-mixed-replace;boundary=" + boundary;

    //  var outputStream = Response.Body;
    //  var cancellationToken = Request.HttpContext.RequestAborted;

    //  async Task drawShapes()
    //  {
    //    var pos = generator.Next(50, 150);
    //    canvas.DrawRect(0, 0, 250, 250, exPaint);
    //    canvas.DrawCircle(pos, pos, 20, inPaint);
    //    res = map.Encode(SKEncodedImageFormat.Webp, 100).ToArray();

    //    var header = $"--{ boundary }\r\nContent-Type: image/webp\r\nContent-Length: { res.Length }\r\n\r\n";
    //    var headerData = Encoding.ASCII.GetBytes(header);

    //    await outputStream.WriteAsync(headerData);
    //    await outputStream.WriteAsync(res);
    //    await outputStream.WriteAsync(Encoding.ASCII.GetBytes("\r\n"));
    //  }

    //  await Task.Run(async () =>
    //  {
    //    try
    //    {
    //      //while (true)
    //      {
    //        await drawShapes();
    //        //await drawShapes();
    //      }
    //    }
    //    catch (TaskCanceledException) { }
    //  });
    //}
  }
}
