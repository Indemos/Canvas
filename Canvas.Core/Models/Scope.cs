using System.Collections.Generic;

namespace Canvas.Core.Models
{
  public struct Scope
  {
    public IList<Mark> Values { get; set; }
    public IList<Mark> Indices { get; set; }
  }
}
