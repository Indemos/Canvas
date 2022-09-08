using Canvas.Core.EngineSpace;
using Canvas.Core.MessageSpace;
using System;
using System.Threading.Tasks;

namespace Canvas.Core
{
  public interface IView
  {
    /// <summary>
    /// Id
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Engine
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// Load event
    /// </summary>
    /// <param name="e"></param>
    Action<ViewMessage> OnLoad { get; set; }

    /// <summary>
    /// Mouse wheel and trackpad event
    /// </summary>
    /// <param name="e"></param>
    Action<ViewMessage> OnWheel { get; set; }

    /// <summary>
    /// Mouse move event
    /// </summary>
    /// <param name="e"></param>
    Action<ViewMessage> OnMouseMove { get; set; }

    /// <summary>
    /// Click event in the view area
    /// </summary>
    /// <param name="e"></param>
    Action<ViewMessage> OnMouseDown { get; set; }

    /// <summary>
    /// Mouse leave event
    /// </summary>
    /// <param name="e"></param>
    Action<ViewMessage> OnMouseLeave { get; set; }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    Task Update(DomainMessage message = null);
  }
}
