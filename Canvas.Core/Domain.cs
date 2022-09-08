using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core
{
  public class Domain
  {
    /// <summary>
    /// Index axis
    /// </summary>
    public virtual IList<int> DomainX { get; set; }
    public virtual IList<int> AutoDomainX { get; set; }
    public virtual int MinIndex => DomainX?.ElementAtOrDefault(0) ?? AutoDomainX?.ElementAtOrDefault(0) ?? 0;
    public virtual int MaxIndex => DomainX?.ElementAtOrDefault(1) ?? AutoDomainX?.ElementAtOrDefault(1) ?? 0;

    /// <summary>
    /// Value axis
    /// </summary>
    public virtual IList<double> DomainY { get; set; }
    public virtual IList<double> AutoDomainY { get; set; }
    public virtual double MinValue => DomainY?.ElementAtOrDefault(0) ?? AutoDomainY?.ElementAtOrDefault(0) ?? 0.0;
    public virtual double MaxValue => DomainY?.ElementAtOrDefault(1) ?? AutoDomainY?.ElementAtOrDefault(1) ?? 0.0;
  }
}
