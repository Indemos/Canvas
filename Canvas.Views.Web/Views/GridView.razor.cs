using Canvas.Core.DecoratorSpace;

namespace Canvas.Views.Web.Views
{
  public partial class GridView : BaseView
  {
    protected LinesDecorator Decorator { get; set; }

    public override void CreateView()
    {
      Decorator = new LinesDecorator
      {
        Composer = Composer
      };
    }

    public override void UpdateView()
    {
      Decorator.CreateIndex(Engine);
      Decorator.CreateValue(Engine);
    }
  }
}
