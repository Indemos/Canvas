namespace Canvas.Core.MessageSpace
{
  public struct ViewMessage
  {
    public bool IsSnap { get; set; }
    public bool IsShape { get; set; }
    public bool IsControl { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public string ValueX { get; set; }
    public string ValueY { get; set; }
  }
}
