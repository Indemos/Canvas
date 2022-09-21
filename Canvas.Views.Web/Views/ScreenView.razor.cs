namespace Canvas.Views.Web.Views
{
  public partial class ScreenView
  {
    protected override void Render() => Composer.UpdateItems(Engine, Composer.Domain);
  }
}
