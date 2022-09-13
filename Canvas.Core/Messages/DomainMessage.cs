using System.Collections.Generic;

namespace Canvas.Core.MessageSpace
{
  public struct DomainMessage
  {
    public int Code { get; set; }
    public bool IndexUpdate { get; set; }
    public bool ValueUpdate { get; set; }
    public IList<int> IndexDomain { get; set; }
    public IList<double> ValueDomain { get; set; }
  }
}
