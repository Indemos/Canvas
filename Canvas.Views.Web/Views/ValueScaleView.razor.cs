using Canvas.Core.DecoratorSpace;

namespace Canvas.Views.Web.Views
{
  public partial class ValueScaleView : BaseView
  {
    protected virtual CaptionDecorator Decorator { get; set; }

    protected override void UpdateView()
    {
      Decorator ??= new CaptionDecorator
      {
        Position = Position,
        Composer = Composer
      };

      Decorator.CreateValue(Engine);
    }
  }
}
