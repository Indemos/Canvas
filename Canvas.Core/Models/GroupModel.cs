using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.ModelSpace
{
  public interface IGroupModel : IComponentModel
  {
    /// <summary>
    /// Shape groups
    /// </summary>
    IDictionary<string, IGroupModel> Groups { get; set; }
  }

  public class GroupModel : ComponentModel, IGroupModel
  {
    /// <summary>
    /// Shape groups
    /// </summary>
    public virtual IDictionary<string, IGroupModel> Groups { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public GroupModel()
    {
      Groups = new Dictionary<string, IGroupModel>(); 
    }

    /// <summary>
    /// Get series
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public override IDictionary<string, IList<double>> GetSeries(IItemModel coordinates, IItemModel values)
    {
      var group = this;
      var groups = new Dictionary<string, IList<double>>();

      if (group?.Groups?.Count <= 0)
      {
        return base.GetSeries(coordinates, values);
      }

      group.Groups.TryGetValue(Composer?.Name ?? string.Empty, out IGroupModel series);

      if (series?.Groups is null)
      {
        return null;
      }

      series.Groups.ForEach(o => groups[o.Key] = o.Value?.GetSeriesValues(coordinates, values));

      return groups;
    }

    /// <summary>
    /// Get specific group by position and name
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override IItemModel GetItem(int position, string name, IList<IItemModel> items)
    {
      if (name is null)
      {
        return base.GetItem(position, null, items);
      }

      var group = items.ElementAtOrDefault(position) as IGroupModel;

      if (group?.Groups is null)
      {
        return null;
      }

      group.Groups.TryGetValue(Composer.Name, out IGroupModel series);

      if (series?.Groups is null)
      {
        return null;
      }

      series.Groups.TryGetValue(name, out IGroupModel shape);

      return shape;
    }

    /// <summary>
    /// Clone
    /// </summary>
    /// <returns></returns>
    public override object Clone()
    {
      var clone = MemberwiseClone() as IGroupModel;

      clone.Groups = Groups.ToDictionary(o => o.Key, o => o.Value.Clone() as IGroupModel);

      return clone;
    }
  }
}
