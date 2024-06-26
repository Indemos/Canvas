using Canvas.Core.Composers;
using Canvas.Core.Engines;
using Canvas.Core.Models;
using Canvas.Core.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Canvas.Views.Web.Views
{
  public partial class CanvasGroupView : IDisposable
  {
    /// <summary>
    /// Item
    /// </summary>
    public IShape Item { get; set; }

    /// <summary>
    /// Views
    /// </summary>
    public IDictionary<string, CanvasView> Views { get; set; } = new Dictionary<string, CanvasView>();

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="item"></param>
    public async Task<IList<IComposer>> CreateViews<EngineType>() where EngineType : IEngine, new()
    {
      var composers = new List<IComposer>();
      var sources = new List<TaskCompletionSource>();

      Item.Groups.ForEach(view => Views[view.Key] = null);

      await InvokeAsync(StateHasChanged);

      foreach (var view in Views)
      {
        var group = Item.Groups[view.Key];
        var source = new TaskCompletionSource();
        var composer = group.Composer = new GroupComposer
        {
          Name = view.Key,
          View = view.Value
        };

        sources.Add(source);
        composers.Add(composer);

        await view.Value.Create<EngineType>(() =>
        {
          source.TrySetResult();
          return composer;
        });

        composer.OnDomain += (domain, source) => Views.ForEach(o =>
        {
          if (source is not null && Equals(composer.Name, o.Value.Composer.Name) is false)
          {
            domain.ValueDomain = o.Value.Composer.Domain.ValueDomain;
            o.Value.Composer.Update(domain);
          }
        });
      }

      await Task.WhenAll(sources.Select(o => o.Task));

      return composers;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual void Update(DomainModel message, IList<IShape> items = null)
    {
      Views.ForEach(o =>
      {
        if (items is not null)
        {
          o.Value.Composer.Items = items;
        }

        o.Value.Composer.Update(message);
      });
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public override void Dispose()
    {
      base.Dispose();
      Views?.ForEach(o => o.Value?.Dispose());
    }
  }
}
