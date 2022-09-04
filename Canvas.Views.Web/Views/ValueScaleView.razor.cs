using Canvas.Core.DecoratorSpace;

namespace Canvas.Views.Web.Views
{
  public partial class ValueScaleView : BaseView
  {
    protected LabelsDecorator Decorator { get; set; }

    public override void CreateView()
    {
      Decorator = new LabelsDecorator
      {
        Position = View,
        Composer = Composer
      };
    }

    public override void UpdateView()
    {
      Decorator.CreateValue(Engine);
    }
  }
}
