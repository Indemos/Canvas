using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Control
{
  public class StreamResponse : IActionResult
  {
    public virtual string Line { get; set; }
    public virtual string Boundary { get; set; }
    public virtual string ContentType { get; set; }
    public virtual Channel<byte[]> Stream { get; protected set; }

    public StreamResponse()
    {
      Line = Environment.NewLine;
      Boundary = Guid.NewGuid().ToString("N");
      Stream = Channel.CreateUnbounded<byte[]>();
      ContentType = $"multipart/x-mixed-replace;boundary={ Boundary }";
    }

    public virtual byte[] Encode(string input)
    {
      return Encoding.UTF8.GetBytes(input);
    }

    public virtual async Task ExecuteResultAsync(ActionContext context)
    {
      await SendAsync(context.HttpContext);
    }

    public virtual async Task SendAsync(HttpContext context)
    {
      try
      {
        var response = context.Response;

        response.ContentType = ContentType;

        await foreach (var content in Stream.Reader.ReadAllAsync())
        {
          var header = Encode($"--{ Boundary }{ Line }Content-Type: image/webp{ Line }Content-Length: { content.Length }{ Line }{ Line }");

          await response.Body.WriteAsync(header);
          await response.Body.WriteAsync(content);
          await response.Body.WriteAsync(Encode(Line));
        }
      }
      catch (TaskCanceledException)
      {
      }
    }
  }
}
