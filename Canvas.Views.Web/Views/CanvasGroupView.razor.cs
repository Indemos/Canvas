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

        composer.OnAction += async domain => await Task.WhenAll(Views.Values.Select(async o =>
        {
          if (Equals(composer.Name, o?.Composer?.Name) is false)
          {
            domain.ValueDomain = o.Composer.Dimension.ValueDomain;
            await o.Update(domain);
          }
        }));
      }

      await Task.WhenAll(sources.Select(o => o.Task));

      return composers;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="dimension"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual Task Update(DimensionModel? dimension, IList<IShape> items = null)
    {
      var processes = Views.Values.Select(o =>
      {
        if (items is not null)
        {
          o.Composer.Items = items;
        }

        return o.Update(dimension);
      });

      return Task.WhenAll(processes);
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
