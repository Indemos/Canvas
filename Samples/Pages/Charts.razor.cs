using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using Canvas.Core.ShapeSpace;
using Canvas.Views.Web.Views;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Canvas.Client.Pages
{
  public partial class Charts : IDisposable, IAsyncDisposable
  {
    protected CanvasGroupView ViewControl { get; set; }

    protected override async Task OnAfterRenderAsync(bool setup)
    {
      if (setup)
      {
        ViewControl.Item = new GroupShape
        {
          Groups = new Dictionary<string, IGroupShape>
          {
            ["Assets"] = new GroupShape
            {
              Groups = new Dictionary<string, IGroupShape>
              {
                ["Prices"] = new CandleShape(),
                ["Arrows"] = new ArrowShape()
              }
            },
            ["Indicators"] = new GroupShape
            {
              Groups = new Dictionary<string, IGroupShape>
              {
                ["Bars"] = new BarShape()
              }
            },
            ["Lines"] = new GroupShape
            {
              Groups = new Dictionary<string, IGroupShape>
              {
                ["X"] = new LineShape(),
                ["Y"] = new LineShape()
              }
            },
            ["Performance"] = new GroupShape
            {
              Groups = new Dictionary<string, IGroupShape>
              {
                ["Balance"] = new AreaShape()
              }
            }
          }
        };

        (await ViewControl.CreateViews<CanvasEngine>()).ForEach(o =>
        {
          o.ShowIndex = (i, v) => GetDateByIndex(o.Items, (int)v);
          o.ShowMarkerIndex = v => GetDateByIndex(o.Items, (int)v);
        });

        Time = DateTime.Now;
        Price = Generator.Next(1000, 5000);
        Interval.Enabled = true;
        Interval.Elapsed += (o, e) =>
        {
          if (Points.Count < 100) Counter(true);
        };
      }

      await base.OnAfterRenderAsync(setup);
    }

    protected string GetDateByIndex(IList<IShape> items, int v)
    {
      var empty = v <= 0 ?
        items.FirstOrDefault()?.X :
        items.LastOrDefault()?.X;

      var stamp = (long)(
        items.ElementAtOrDefault(v)?.X ??
        empty ??
        DateTime.Now.Ticks);

      return $"{new DateTime(stamp)}";
    }

    #region Generator

    protected double Price { get; set; }
    protected DateTime Time { get; set; }
    protected Timer Interval { get; set; } = new(1);
    protected Random Generator { get; set; } = new();
    protected List<IShape> Points { get; set; } = new();

    /// <summary>
    /// On timer event
    /// </summary>
    protected void Counter(bool active)
    {
      var min = Generator.Next(1000, 2000);
      var max = Generator.Next(3000, 5000);
      var point = Generator.Next(min, max);

      if (IsNextFrame())
      {
        var group = (Points.LastOrDefault() ?? ViewControl.Item).Clone() as IGroupShape;
        var groupCandle = group.Groups["Assets"].Groups["Prices"] as CandleShape;

        groupCandle.L = point;
        groupCandle.H = point;
        group.X = DateTime.Now.Ticks;

        Time = DateTime.Now;
        Price = groupCandle.C ?? point;
        Points.Add(group);
      }

      var current = Points.Last() as IGroupShape;
      var currentCandle = current.Groups["Assets"].Groups["Prices"] as CandleShape;
      var color = point > Price ? SKColors.LimeGreen : SKColors.OrangeRed;
      var direction = point > Price ? 1 : -1;

      current.Groups["Lines"].Groups["X"] = new LineShape { Y = point };
      current.Groups["Lines"].Groups["Y"] = new LineShape { Y = point };
      current.Groups["Indicators"].Groups["Bars"] = new BarShape { Y = point };
      current.Groups["Performance"].Groups["Balance"] = new AreaShape { Y = point };
      current.Groups["Assets"].Groups["Arrows"] = new ArrowShape { Y = (point + Price) / 2, Direction = direction };
      current.Groups["Assets"].Groups["Prices"] = new CandleShape
      {
        O = Price,
        C = point,
        L = Math.Min(currentCandle?.L ?? point, point),
        H = Math.Max(currentCandle?.H ?? point, point),
        Box = new ComponentModel { Color = color }
      };

      var domain = new DomainModel { IndexDomain = new[] { Points.Count - 100, Points.Count } };

      ViewControl.Update(domain, Points);
    }

    /// <summary>
    /// Create new bar when it's time
    /// </summary>
    /// <returns></returns>
    protected bool IsNextFrame()
    {
      return Points.Count == 0 || DateTime.Now.Ticks - Time.Ticks >= TimeSpan.FromMilliseconds(500).Ticks;
    }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <returns></returns>
    public void Dispose()
    {
      Interval.Dispose();
      Interval = null;
    }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <returns></returns>
    public ValueTask DisposeAsync()
    {
      Dispose();
      return new ValueTask(Task.CompletedTask);
    }
  }

  #endregion
}
