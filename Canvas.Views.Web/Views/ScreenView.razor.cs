namespace Canvas.Views.Web.Views
{
  public partial class ScreenView : BaseView
  {
    public override void UpdateView() => Composer.UpdateItems(Engine);
  }
}
