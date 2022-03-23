using System.Collections.Generic;

namespace Core.ModelSpace
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
  }
}
