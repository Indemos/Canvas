using Canvas.Core.MessageSpace;
using System.Threading.Tasks;

namespace Canvas.Core
{
  public interface IMessenger
  {
    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    Task Update(DomainMessage message = null);
  }
}
