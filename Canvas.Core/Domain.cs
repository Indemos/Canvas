using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core
{
  public class Domain
  {
    /// <summary>
    /// Index axis
    /// </summary>
    public virtual IList<int> IndexDomain { get; set; }
    public virtual IList<int> AutoIndexDomain { get; set; }
    public virtual int MinIndex => IndexDomain?.ElementAtOrDefault(0) ?? AutoIndexDomain?.ElementAtOrDefault(0) ?? 0;
    public virtual int MaxIndex => IndexDomain?.ElementAtOrDefault(1) ?? AutoIndexDomain?.ElementAtOrDefault(1) ?? 0;

    /// <summary>
    /// Value axis
    /// </summary>
    public virtual IList<double> ValueDomain { get; set; }
    public virtual IList<double> AutoValueDomain { get; set; }
    public virtual double MinValue => ValueDomain?.ElementAtOrDefault(0) ?? AutoValueDomain?.ElementAtOrDefault(0) ?? 0.0;
    public virtual double MaxValue => ValueDomain?.ElementAtOrDefault(1) ?? AutoValueDomain?.ElementAtOrDefault(1) ?? 0.0;
  }
}
