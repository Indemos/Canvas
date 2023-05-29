using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.Models
{
  public struct DomainModel
  {
    /// <summary>
    /// Index axis
    /// </summary>
    public IList<int> IndexDomain { get; set; }
    public IList<int> AutoIndexDomain { get; set; }
    public int MinIndex => IndexDomain?.ElementAtOrDefault(0) ?? AutoIndexDomain?.ElementAtOrDefault(0) ?? 0;
    public int MaxIndex => IndexDomain?.ElementAtOrDefault(1) ?? AutoIndexDomain?.ElementAtOrDefault(1) ?? 0;

    /// <summary>
    /// Value axis
    /// </summary>
    public IList<double> ValueDomain { get; set; }
    public IList<double> AutoValueDomain { get; set; }
    public double MinValue => ValueDomain?.ElementAtOrDefault(0) ?? AutoValueDomain?.ElementAtOrDefault(0) ?? 0.0;
    public double MaxValue => ValueDomain?.ElementAtOrDefault(1) ?? AutoValueDomain?.ElementAtOrDefault(1) ?? 0.0;
  }
}
