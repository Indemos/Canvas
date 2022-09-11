using Canvas.Core.DecoratorSpace;

namespace Canvas.Views.Web.Views
{
  public partial class GridView
  {
    protected virtual GridDecorator Decorator { get; set; }

    protected override void UpdateView()
    {
      Decorator ??= new GridDecorator
      {
        Composer = Composer
      };

      Decorator.CreateIndex(Engine);
      Decorator.CreateValue(Engine);
    }
  }
}
