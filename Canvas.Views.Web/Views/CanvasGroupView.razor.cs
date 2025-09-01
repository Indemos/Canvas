using Canvas.Core.Composers;
using Canvas.Core.Engines;
using Canvas.Core.Models;
using Canvas.Core.Shapes;
using Distribution.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Canvas.Views.Web.Views
{
  public partial class CanvasGroupView : IDisposable
  {
    protected virtual ScheduleService GroupService { get; set; }

    /// <summary>
    /// Item
    /// </summary>
    public IShape Item { get; set; }

    /// <summary>
    /// Views
    /// </summary>
    public ConcurrentDictionary<string, CanvasView> Views { get; set; } = new ConcurrentDictionary<string, CanvasView>();

    /// <summary>
    /// Completion sources
    /// </summary>
    public ConcurrentDictionary<string, TaskCompletionSource> Sources { get; set; } = new ConcurrentDictionary<string, TaskCompletionSource>();

    /// <summary>
    /// Renderer
    /// </summary>
    /// <param name="name"></param>
    public void OnCanvasRender(string name)
    {
      if (Sources.TryGetValue(name, out var source))
      {
        source.TrySetResult();
      }
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="item"></param>
    public async Task<IList<IComposer>> CreateViews<EngineType>() where EngineType : IEngine, new()
    {
      Dispose();

      GroupService = new ScheduleService();

      var composers = new List<IComposer>();

      foreach (var group in Item.Groups)
      {
        Views[group.Key] = null;
        Sources[group.Key] = new TaskCompletionSource();
      }

      await InvokeAsync(StateHasChanged);
      await Task.WhenAll(Sources.Values.Select(o => o.Task));

      foreach (var view in Views)
      {
        var group = Item.Groups[view.Key];
        var composer = group.Composer = new GroupComposer
        {
          Name = view.Key
        };

        await view.Value.Create<EngineType>(() => composer);

        composers.Add(composer);
        composer.OnAction += domain => Views.Values.ForEach(async o =>
        {
          if (o?.Composer?.Name is not null && Equals(composer.Name, o.Composer.Name) is false)
          {
            domain.ValueDomain = o.Composer.Dimension.ValueDomain;
            await o.Update(domain);
          }
        });
      }

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
      try
      {
        return GroupService?.Send(() =>
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

        })?.Task ?? Task.CompletedTask;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }

      return Task.CompletedTask;
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public override void Dispose()
    {
      base.Dispose();
      GroupService?.Dispose();
      Views?.ForEach(o => o.Value?.Dispose());
    }
  }
}
