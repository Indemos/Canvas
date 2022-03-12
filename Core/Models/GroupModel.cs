using System.Collections.Generic;
using System.Linq;

namespace Core.ModelSpace
{
  public interface IGroupModel : IShapeModel
  {
    /// <summary>
    /// Shape groups
    /// </summary>
    IDictionary<string, IGroupModel> Groups { get; set; }

    /// <summary>
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    double[] CreateDomain(int position, string name, IList<IGroupModel> items);

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    void CreateShape(int position, string name, IList<IGroupModel> items);
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
    /// Get Min and Max for the current point
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual double[] CreateDomain(int position, string name, IList<IGroupModel> items)
    {
      var currentModel = Composer.GetGroup(position, name, items);

      if (currentModel?.Point is null)
      {
        return null;
      }

      return new double[]
      {
        currentModel.Point,
        currentModel.Point
      };
    }

    /// <summary>
    /// Create the shape
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual void CreateShape(int position, string name, IList<IGroupModel> items)
    {
    }
  }
}
