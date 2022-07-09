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
    /// Get series by position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override IDictionary<string, IList<double>> GetSeries(int position, IList<IItemModel> items)
    {
      var group = items.ElementAtOrDefault(position) as IGroupModel;
      var groups = new Dictionary<string, IList<double>>();

      if (group?.Groups?.Count <= 0)
      {
        return base.GetSeries(position, items);
      }

      group.Groups.TryGetValue(Composer.Name, out IGroupModel series);

      if (series?.Groups is null)
      {
        return null;
      }

      series.Groups.ForEach(o => groups[o.Key] = o.Value?.GetValues());

      return groups;
    }

    /// <summary>
    /// Get specific group by position and name
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override dynamic GetItem(int position, string name, IList<IItemModel> items)
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

      return shape?.Value;
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
