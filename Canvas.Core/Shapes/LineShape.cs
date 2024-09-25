using Canvas.Core.Enums;
using Canvas.Core.Models;
using System.Collections.Generic;

namespace Canvas.Core.Shapes
{
  public class LineShape : Shape
  {
    /// <summary>
    /// Render the shape
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public override void CreateShape(int index, string name, IList<IShape> items)
    {
      Draw(index, name, items);
      Draw(index - 1, name, items);
    }

    /// <summary>
    /// Render the shape on top of the previous one
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    protected virtual void Draw(int index, string name, IList<IShape> items)
    {
      var current = GetItem(index, name, items)?.Y;
      var previous = GetItem(index - 1, name, items)?.Y;

      if (current is null)
      {
        return;
      }

      var component = Composer.Components[nameof(ComponentEnum.ShapeSection)];
      var coordinates = new DataModel[]
      {
        Composer.GetItemPosition(Engine, index, (previous ?? current).Value),
        Composer.GetItemPosition(Engine, index + 1, current.Value)
      };

      Engine.CreateLine(coordinates, Component ?? component);
    }
  }
}
