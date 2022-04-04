using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;
using System.Linq;
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Canvas.Views.Web
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
    public async Task<StreamServer> Create()
    {
      var server = WebApplication.CreateBuilder();

      server.WebHost.ConfigureKestrel(options =>
      {
        options.ListenAnyIP(0, serverOptions =>
        {
          serverOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        });
      });

      Route = "/source";
      Stream = Channel.CreateUnbounded<byte[]>();
      Application = server.Build();
      Application.Use((context, next) => OnRoute(context, next));

      await Application.StartAsync();

      if (Application.Urls.Any())
      {
        var source = Application.Urls.FirstOrDefault();
        var sourceProps = new UriBuilder(source);

        sourceProps.Host = $"{ IPAddress.Loopback }";
        sourceProps.Path = Route;

        Source = $"{ sourceProps }";
      }

      return this;
    }

    /// <summary>
    /// Process route
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    protected virtual Task OnRoute(HttpContext context, Func<Task> next)
    {
      if (context.Request.Path.Value.Contains(Route))
      {
        return Task.Run(async () =>
        {
          var response = new StreamResponse();

          response.SetHeader(context);

          await foreach (var content in Stream.Reader.ReadAllAsync())
          {
            await response.SetDocument(context, content);
          }
        });
      }

      return next();
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
      if (Stream.Writer.TryComplete())
      {
        Stream.Reader.Completion.ContinueWith(async o =>
        {
          await Application.StopAsync();

          Application = null;
          Stream = null;
          Source = null;

        }).Unwrap();
      }
    }
  }
}
