using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using Canvas.Core.ShapeSpace;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Canvas.Views.Web.Views
{
  public partial class CanvasGroupView
  {
    /// <summary>
    /// Indices
    /// </summary>
    protected IDictionary<long, IShape> Indices { get; set; } = new Dictionary<long, IShape>();

    /// <summary>
    /// Series
    /// </summary>
    protected IDictionary<string, string> Series { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Item
    /// </summary>
    public IGroupShape Item { get; set; }

    /// <summary>
    /// Views
    /// </summary>
    public IDictionary<string, CanvasView> Views { get; set; } = new Dictionary<string, CanvasView>();

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="item"></param>
    public async Task CreateViews<EngineType>() where EngineType : IEngine, new()
    {
      Item.Groups.ForEach(view => Views[view.Key] = null);

      await InvokeAsync(StateHasChanged);

      var sources = new List<TaskCompletionSource>();

      foreach (var view in Views)
      {
        var group = Item.Groups[view.Key];
        var source = new TaskCompletionSource();
        var composer = group.Composer = new GroupComposer
        {
          Name = view.Key
        };

        sources.Add(source);

        await view.Value.Create<EngineType>(() =>
        {
          source.TrySetResult();
          return composer;
        });

        composer.OnDomain += (message, source) => Views.ForEach(o =>
        {
          if (source is not null && Equals(composer.Name, o.Value.Composer.Name) is false)
          {
            message.ValueDomain = o.Value.Composer.Domain.ValueDomain;
            o.Value.Composer.Update(message);
          }
        });
      }

      await Task.WhenAll(sources.Select(o => o.Task));

      Update(new DomainModel());
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

        o.Value.Update(message);
      });
    }
  }
}
