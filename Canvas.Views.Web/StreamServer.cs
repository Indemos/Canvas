using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Canvas.Views.Web
{
  public class StreamServer : IDisposable
  {
    public virtual string Source { get; protected set; }
    public virtual WebApplication Application { get; protected set; }
    public virtual IDictionary<string, Channel<byte[]>> Streams { get; protected set; }

    /// <summary>
    /// Create
    /// </summary>
    public async Task<StreamServer> Create(IList<string> routes)
    {
      Streams = new Dictionary<string, Channel<byte[]>>();

      routes.ForEach(route => Streams[route] = Channel.CreateUnbounded<byte[]>());

      var server = WebApplication.CreateBuilder();

      server.WebHost.ConfigureKestrel(options =>
        options.ListenAnyIP(0, serverOptions =>
          serverOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3));

      Application = server.Build();
      Application.Use((context, next) => OnRoute(routes, context, next));

      await Application.StartAsync();

      if (Application.Urls.Any())
      {
        var source = Application.Urls.FirstOrDefault();
        var sourceProps = new UriBuilder(source);

        sourceProps.Host = $"{ IPAddress.Loopback }";

        Source = $"{ sourceProps }".Trim('/');
      }

      return this;
    }

    /// <summary>
    /// Process route
    /// </summary>
    /// <param name="routes"></param>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    protected virtual Task OnRoute(IList<string> routes, HttpContext context, Func<Task> next)
    {
      foreach (var route in routes)
      {
        if (context.Request.Path.Value.Equals(route))
        {
          return Task.Run(async () =>
          {
            var response = new StreamResponse();

            response.SendHeader(context);

            await foreach (var content in Streams[route].Reader.ReadAllAsync())
            {
              await response.SendDocument(context, content);
            }
          });
        }
      }

      return next();
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
      Streams.ForEach(stream =>
      {
        if (stream.Value.Writer.TryComplete())
        {
          stream.Value.Reader.Completion.ContinueWith(async o =>
          {
            await Application.StopAsync();

            Application = null;
            Source = null;

          }).Unwrap();
        }
      });

      Streams.Clear();
    }
  }
}
