namespace Canvas.Core.Models
{
  public struct Transition
  {
    public bool IsMove { get; set; }
    public bool IsShape { get; set; }
    public Unit Data { get; set; }
  }
}
