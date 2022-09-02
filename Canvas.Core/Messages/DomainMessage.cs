using System.Collections.Generic;

namespace Canvas.Core.MessageSpace
{
  public class DomainMessage
  {
    public virtual string Name { get; set; }
    public virtual bool IndexUpdate { get; set; }
    public virtual bool ValueUpdate { get; set; }
    public virtual IList<int> IndexDomain { get; set; }
    public virtual IList<double> ValueDomain { get; set; }
  }
}
