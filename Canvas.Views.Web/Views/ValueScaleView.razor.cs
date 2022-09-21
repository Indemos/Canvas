using Canvas.Core.DecoratorSpace;

namespace Canvas.Views.Web.Views
{
  public partial class ValueScaleView
  {
    protected virtual CaptionDecorator Decorator { get; set; }

    protected override void Render()
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
