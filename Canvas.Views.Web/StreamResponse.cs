using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Canvas.Views.Web
{
  public class StreamResponse : IActionResult
  {
    /// <summary>
    /// New line separator
    /// </summary>
    public virtual string Line { get; protected set; }

    /// <summary>
    /// Separator between streaming docs
    /// </summary>
    public virtual string Bounds { get; protected set; }

    /// <summary>
    /// Stream of docs
    /// </summary>
    public virtual Channel<byte[]> Stream { get; protected set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public StreamResponse()
    {
      Line = Environment.NewLine;
      Bounds = Guid.NewGuid().ToString("N");
      Stream = Channel.CreateUnbounded<byte[]>();
    }

    /// <summary>
    /// Convert strings to bytes
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public virtual byte[] Encode(string input)
    {
      return Encoding.UTF8.GetBytes(input);
    }

    /// <summary>
    /// Send stream to HTTP response 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual async Task ExecuteResultAsync(ActionContext context)
    {
      try
      {
        SetHeader(context.HttpContext);

        await foreach (var content in Stream.Reader.ReadAllAsync())
        {
          await SetDocument(context.HttpContext, content);
        }
      }
      catch (TaskCanceledException)
      {
      }
    }

    /// <summary>
    /// Set HTTP stream content type
    /// </summary>
    /// <param name="context"></param>
    public virtual void SetHeader(HttpContext context)
    {
      var response = context.Response;

      response.Clear();
      response.StatusCode = 200;
      response.Headers[HeaderNames.Expires] = "0";
      response.Headers[HeaderNames.Pragma] = "no-cache";
      response.Headers[HeaderNames.Connection] = "keep-alive";
      response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
      response.ContentType = $"multipart/x-mixed-replace;boundary=--{ Bounds }";
    }

    /// <summary>
    /// Send document to the stream
    /// </summary>
    /// <param name="context"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public virtual async Task SetDocument(HttpContext context, byte[] content)
    {
      var response = context.Response;
      var bounds = Encode($"--{ Bounds }{ Line }Content-Type: image/webp{ Line }Content-Length: { content.Length }{ Line }{ Line }");

      await response.Body.WriteAsync(bounds);
      await response.Body.WriteAsync(content);
      await response.Body.WriteAsync(Encode(Line));
    }
  }
}
