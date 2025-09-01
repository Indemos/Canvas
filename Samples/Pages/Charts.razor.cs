using Canvas.Core.Engines;
using Canvas.Core.Models;
using Canvas.Core.Shapes;
using Canvas.Views.Web.Views;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Canvas.Client.Pages
{
  public partial class Charts
  {
    protected CanvasGroupView View { get; set; }
    protected Random Generator { get; set; } = new();
    protected DateTime Time { get; set; } = DateTime.Now;
    protected DateTime TimeGroup { get; set; } = DateTime.Now;
    protected List<IShape> Points { get; set; } = [];
    protected Dictionary<long, int> Indices { get; set; } = [];

    protected override async Task OnAfterRenderAsync(bool setup)
    {
      if (setup)
      {
        View.Item = GetShape();

        var views = await View.CreateViews<CanvasEngine>();
        var interval = new Timer(TimeSpan.FromMicroseconds(1));

        views.ForEach(o => o.ShowIndex = v => GetDateByIndex(o.Items, (int)v));

        interval.Enabled = true;
        interval.Elapsed += (o, e) =>
        {
          if (Points.Count >= 100)
          {
            interval.Stop();
          }

          OnData();
        };
      }

      await base.OnAfterRenderAsync(setup);
    }

    protected IShape GetShape()
    {
      var areas = new string[] { "Assets", "Indicators", "Lines", "Performance" };
      return new Shape { Groups = areas.ToDictionary(o => o, o => new Shape() as IShape) };
    }

    protected string GetDateByIndex(IList<IShape> items, int index)
    {
      var empty = index <= 0 ? items.FirstOrDefault()?.X : items.LastOrDefault()?.X;
      var stamp = (long)(items.ElementAtOrDefault(index)?.X ?? empty ?? DateTime.Now.Ticks);

      return $"{new DateTime(stamp):MM/dd/yyyy HH:mm}";
    }

    /// <summary>
    /// On timer event
    /// </summary>
    protected Task OnData()
    {
      var min = Generator.Next(1000, 2000);
      var max = Generator.Next(3000, 5000);
      var point = Generator.Next(min, max);
      var duration = TimeSpan.FromSeconds(5);
      var biColor = point % 2 is 0 ? SKColors.LimeGreen : SKColors.OrangeRed;
      var barColor = point % 2 is 0 ? SKColors.DeepSkyBlue : SKColors.OrangeRed;
      var direction = point % 2 is 0 ? 1 : -1;

      Time += TimeSpan.FromMinutes(1);
      TimeGroup = Time - TimeGroup > TimeSpan.FromMinutes(10) ? Time : TimeGroup;

      var isUpdate = Indices.TryGetValue(TimeGroup.Ticks, out var index);
      var group = isUpdate ? Points[index] : GetShape();

      if (isUpdate is false)
      {
        Indices[TimeGroup.Ticks] = Points.Count;
        Points.Add(group);

        group.Groups["Lines"].Groups["X"] = new LineShape();
        group.Groups["Lines"].Groups["Y"] = new LineShape();
        group.Groups["Indicators"].Groups["Bars"] = new BarShape();
        group.Groups["Assets"].Groups["Prices"] = new CandleShape();
        group.Groups["Assets"].Groups["Arrows"] = new ArrowShape();
        group.Groups["Performance"].Groups["Balance"] = new AreaShape { Component = new ComponentModel { Color = SKColors.DeepSkyBlue } };
        group.Groups["Performance"].Groups["Drawdown"] = new AreaShape { Component = new ComponentModel { Color = SKColors.OrangeRed } };
      }

      group.Groups["Lines"].Groups["X"].Y = point + max;
      group.Groups["Lines"].Groups["Y"].Y = point - min;
      group.Groups["Indicators"].Groups["Bars"].Y = point;
      group.Groups["Performance"].Groups["Balance"].Y = point;
      group.Groups["Performance"].Groups["Drawdown"].Y = -(point - min);
      group.Groups["Assets"].Groups["Prices"] = CandleShape.Update(group.Groups["Assets"].Groups["Prices"], point);
      group.Groups["Assets"].Groups["Prices"].Component = new ComponentModel { Color = biColor };
      group.Groups["Assets"].Groups["Arrows"] = new ArrowShape { Y = point, Direction = direction };

      var domain = new DimensionModel { IndexDomain = [Points.Count - 100, Points.Count] };

      return View.Update(domain, Points);
    }
  }
}
