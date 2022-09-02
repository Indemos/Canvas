using Canvas.Core;
using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using Canvas.Views.Web;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Canvas.Client.Pages
{
  public partial class Charts : IAsyncDisposable
  {
    protected Timer _interval = new(1);
    protected Random _generator = new();
    protected double _pointValue = 0;
    protected DateTime _pointTime = DateTime.Now;
    protected List<IItemModel> _points = new();
    protected Dictionary<string, ScreenView> _views = new()
    {
      ["Candles"] = null,
      ["Bars"] = null,
      ["Lines"] = null,
      ["Areas"] = null,
      ["Deltas"] = null
    };

    protected override async Task OnAfterRenderAsync(bool setup)
    {
      if (setup)
      {
        await InvokeAsync(StateHasChanged);

        var sources = new List<TaskCompletionSource>();

        foreach (var view in _views)
        {
          var source = new TaskCompletionSource();
          var composer = new GroupComposer
          {
            Name = view.Key,
            Items = _points
          };

          sources.Add(source);

          await view.Value.Create<CanvasEngine>(message =>
          {
            source.TrySetResult();
            return composer;
          });

          //view.Value.OnMessage = message => _views.ForEach(o =>
          //{
          //  if (Equals(o.Value.Composer.Name, message.Name) is false)
          //  {
          //    o.Value.Composer.IndexDomain = message.IndexDomain;
          //    o.Value.Update();
          //  }
          //});
        }

        await Task.WhenAll(sources.Select(o => o.Task));

        _interval.Enabled = true;
        _interval.Elapsed += (o, e) =>
        {
          lock (this)
          {
            _interval.Enabled = _points.Count < 150;
            Counter();
          }
        };
      }

      await base.OnAfterRenderAsync(setup);
    }

    /// <summary>
    /// On timer event
    /// </summary>
    protected void Counter()
    {
      var candle = CreatePoint();
      var point = candle.Close;
      var pointDelta = _generator.Next(2000, 5000);
      var pointMirror = _generator.Next(2000, 5000);
      var arrow = candle.Close;

      if (IsNextFrame())
      {
        _pointValue = candle.Close.Value;
        _pointTime = DateTime.UtcNow;
        _points.Add(new GroupModel
        {
          X = _pointTime.Ticks,
          Groups = new Dictionary<string, IGroupModel>
          {
            ["Bars"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new BarItemModel { Y = point } } },
            ["Areas"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new AreaItemModel { Y = point } } },
            ["Deltas"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new BarItemModel { Y = pointDelta } } },
            ["Lines"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new LineItemModel { Y = point }, ["V2"] = new LineItemModel { Y = pointMirror } } },
            ["Candles"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = candle, ["V2"] = new ArrowItemModel { Y = arrow, Direction = 1 } } }
          }
        });
      }

      var grp = _points.Last() as IGroupModel;
      var currentDelta = grp.Groups["Deltas"].Groups["V1"];
      var currentCandle = grp.Groups["Candles"].Groups["V1"] as CandleItemModel;

      currentCandle.Low = candle.Low;
      currentCandle.High = candle.High;
      currentCandle.Close = candle.Close;
      currentCandle.Color = currentCandle.Close > currentCandle.Open ? SKColors.LimeGreen : SKColors.OrangeRed;

      currentDelta.Y = currentCandle.Close > currentCandle.Open ? candle.Close : -candle.Close;
      currentDelta.Color = currentCandle.Color;

      _views.ForEach(panel =>
      {
        //var composer = panel.Value.Composer;
        //composer.Items = _points;
        //composer.UpdateIndexDomain(new[] { composer.Items.Count - 100, composer.Items.Count });
        //panel.Value.Update();
      });
    }

    /// <summary>
    /// Generate candle
    /// </summary>
    protected CandleItemModel CreatePoint()
    {
      var open = (double)_generator.Next(1000, 5000);
      var close = (double)_generator.Next(1000, 5000);
      var shadow = (double)_generator.Next(500, 1000);
      var candle = new CandleItemModel
      {
        Low = Math.Min(open, close) - shadow,
        High = Math.Max(open, close) + shadow,
        Open = _pointValue,
        Close = close
      };

      return candle;
    }

    /// <summary>
    /// Create new bar when it's time
    /// </summary>
    /// <returns></returns>
    protected bool IsNextFrame()
    {
      return _points.Count == 0 || DateTime.UtcNow.Ticks - _pointTime.Ticks >= TimeSpan.FromMilliseconds(100).Ticks;
    }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <returns></returns>
    public ValueTask DisposeAsync()
    {
      _interval.Dispose();

      return new ValueTask(Task.CompletedTask);
    }
  }
}
