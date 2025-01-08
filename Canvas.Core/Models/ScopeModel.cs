using System.Collections.Generic;

namespace Canvas.Core.Models
{
  public struct ScopeModel
  {
    public IList<MarkerModel> Values { get; set; }
    public IList<MarkerModel> Indices { get; set; }
  }
}
