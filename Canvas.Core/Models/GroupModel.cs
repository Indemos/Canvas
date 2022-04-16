using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.ModelSpace
{
  public interface IGroupModel : IShapeModel
  {
    /// <summary>
    /// Shape groups
    /// </summary>
    IDictionary<string, IGroupModel> Groups { get; set; }
  }

  public class GroupModel : ShapeModel, IGroupModel
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
