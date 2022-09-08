namespace Canvas.Core.MessageSpace
{
  public class ViewMessage
  {
    public virtual bool IsSnap { get; set; }
    public virtual bool IsShape { get; set; }
    public virtual bool IsControl { get; set; }
    public virtual double X { get; set; }
    public virtual double Y { get; set; }
    public virtual string ValueX { get; set; }
    public virtual string ValueY { get; set; }
  }
}
