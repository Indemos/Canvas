namespace Canvas.Views.Web.Views
{
  public partial class ScreenView : BaseView
  {
    protected override void UpdateView() => Composer.UpdateItems(Engine);
  }
}
