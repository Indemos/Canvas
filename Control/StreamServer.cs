using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Control
{
  public class StreamServer : IDisposable
  {
    public virtual string Route { get; set; }
    public virtual string Source { get; protected set; }
    public virtual Channel<byte[]> Stream { get; protected set; }
    public virtual WebApplication Application { get; protected set; }

    /// <summary>
    /// Constructor
    /// </summary>
    private StreamServer() { }

    /// <summary>
    /// Constructor
    /// </summary>
    public static async Task<StreamServer> Create()
    {
      var server = new StreamServer();
      var app = WebApplication.CreateBuilder();

      app.WebHost.ConfigureKestrel(o =>
      {
        o.ListenAnyIP(0);
      });

      server.Route = "/source";
      server.Stream = Channel.CreateUnbounded<byte[]>();
      server.Application = app.Build();
      server.Application.MapGet(server.Route, server.OnRoute);

      await server.Application.StartAsync();

      if (server.Application.Urls.Any())
      {
        var source = server.Application.Urls.FirstOrDefault();
        var sourceProps = new UriBuilder(source);

        sourceProps.Host = $"{ IPAddress.Loopback }";
        sourceProps.Path = server.Route;
        server.Source = $"{ sourceProps }";
      }

      return server;
    }

    /// <summary>
    /// Process
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected Task OnRoute(HttpContext context)
    {
      var response = new StreamResponse();

      Task.Run(async () =>
      {
        await foreach (var content in Stream.Reader.ReadAllAsync())
        {
          await response.Stream.Writer.WriteAsync(content);
        }
      });

      return Task.FromResult(response);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
      //if (Stream.Writer.TryComplete())
      //{
      //  Stream.Reader.Completion.ContinueWith(async o =>
      //  {
      //    await Application.StopAsync();

      //    Application = null;
      //    Stream = null;
      //    Source = null;
      //  });
      //}
    }
  }
}
