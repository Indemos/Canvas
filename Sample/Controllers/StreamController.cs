using Control;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace Client.Controllers
{
  [ApiController]
  public class StreamController : ControllerBase
  {
    private object ls = new object();
    private Random _generator = null;
    SKPaint exPaint;
    SKBitmap map;
    SKCanvas canvas;
    SKPaint inPaint;

    public StreamController()
    {
      _generator = new Random();

      map = new SKBitmap(250, 250);
      canvas = new SKCanvas(map);
      inPaint = new SKPaint
      {
        Color = SKColors.Black,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.High
      };
      exPaint = new SKPaint
      {
        Color = SKColors.White,
        Style = SKPaintStyle.Fill,
        FilterQuality = SKFilterQuality.Low
      };
    }

    [Route("/source")]
    public async Task<StreamResponse> Get()
    {
      var aTimer = new Timer();
      var response = new StreamResponse();

      aTimer.Interval = 1;
      aTimer.Enabled = true;
      aTimer.Elapsed += (sender, e) => 
      {
        using (var image = SKImage.FromBitmap(map))
        {
          var pos = _generator.Next(50, 150);
          canvas.DrawRect(0, 0, 250, 250, exPaint);
          canvas.DrawCircle(pos, pos, 20, inPaint);
          var data = image.Encode(SKEncodedImageFormat.Webp, 100).ToArray();
          response.Stream.Writer.WriteAsync(data);
        }
      };

      return await Task.FromResult(response);
    }

    //[Route("/source")]
    //public async Task Get()
    //{
    //  var res = new byte[1000];
    //  var boundary = Guid.NewGuid().ToString();

    //  Response.ContentType = "multipart/x-mixed-replace;boundary=" + boundary;

    //  var outputStream = Response.Body;
    //  var cancellationToken = Request.HttpContext.RequestAborted;

    //  await Task.Run(async () =>
    //  {
    //    try
    //    {
    //      while (true)
    //      {
    //        using (var image = SKImage.FromBitmap(map))
    //        {
    //          var pos = _generator.Next(50, 150);
    //          canvas.DrawRect(0, 0, 250, 250, exPaint);
    //          canvas.DrawCircle(pos, pos, 20, inPaint);
    //          res = image.Encode(SKEncodedImageFormat.Webp, 100).ToArray();
    //        }

    //        var header = $"--{ boundary }\r\nContent-Type: image/webp\r\nContent-Length: { res.Length }\r\n\r\n";
    //        var headerData = Encoding.UTF8.GetBytes(header);

    //        await outputStream.WriteAsync(headerData);
    //        await outputStream.WriteAsync(res);
    //        await outputStream.WriteAsync(Encoding.UTF8.GetBytes("\r\n"));

    //        if (cancellationToken.IsCancellationRequested) break;
    //      }
    //    }
    //    catch (TaskCanceledException) { }
    //    finally { }
    //  });
    //}

  }
}
