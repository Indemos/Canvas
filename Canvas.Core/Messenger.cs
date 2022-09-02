using Canvas.Core.MessageSpace;

namespace Canvas.Core
{
  public interface IMessenger
  {
    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    void Update(DomainMessage message = null);
  }
}
