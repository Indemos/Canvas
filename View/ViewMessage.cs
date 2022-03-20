using Core;

namespace View
{
  public class ViewMessage
  {
    public virtual string Name { get; set; }
    public virtual double Width { get; set; }
    public virtual double Height { get; set; }
    public virtual Composer Composer { get; set; }
    public virtual CanvasWebView View { get; set; }
  }
}
