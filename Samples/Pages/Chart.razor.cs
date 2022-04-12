using Canvas.Core;
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
  public partial class Chart : IAsyncDisposable
  {
    protected Timer _interval = new(100);
    protected Random _generator = new();
    protected double _pointValue = 0;
    protected DateTime _pointTime = DateTime.Now;
    protected List<IPointModel> _points = new();
    protected Dictionary<string, CanvasWebView> _views = new()
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
          sources.Add(new TaskCompletionSource());
          view.Value.OnSize = view.Value.OnCreate = message => OnCreate(view.Key, message, sources.Last());
          view.Value.OnUpdate = message => _views.ForEach(o =>
          {
            if (Equals(o.Value.Composer.Name, message.View.Composer.Name) is false)
            {
              o.Value.Composer.IndexDomain = message.View.Composer.IndexDomain;
              o.Value.Update();
            }
          });

          await view.Value.Create();
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
    /// On load event for web view
    /// </summary>
    /// <param name="name"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    protected void OnCreate(string name, ViewMessage message, TaskCompletionSource source)
    {
      var composer = new GroupComposer
      {
        Name = name,
        Points = _points,
        Engine = new CanvasEngine(message.X, message.Y)
      };

      if (message?.View?.Composer is not null)
      {
        composer.Points = message.View.Composer.Points;
        composer.IndexDomain = message.View.Composer.IndexDomain;
        composer.ValueDomain = message.View.Composer.ValueDomain;
        message.View.Composer.Dispose();
      }

      _views[name] = message.View;
      _views[name].Composer = composer;
      _views[name].Update();

      source.TrySetResult();
    }

    /// <summary>
    /// On timer event
    /// </summary>
    protected void Counter()
    {
      var candle = CreatePoint();
      var point = new Model { ["Point"] = candle.Close };
      var pointDelta = new Model { ["Point"] = _generator.Next(2000, 5000) };
      var pointMirror = new Model { ["Point"] = _generator.Next(2000, 5000) };
      var arrow = new Model { ["Point"] = candle.Close, ["Direction"] = 1 };

      if (IsNextFrame())
      {
        _pointValue = candle.Close;
        _pointTime = DateTime.UtcNow;
        _points.Add(new GroupModel
        {
          Index = _pointTime.Ticks,
          Groups = new Dictionary<string, IGroupModel>
          {
            ["Bars"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new BarGroupModel { Value = point } } },
            ["Areas"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new AreaGroupModel { Value = point } } },
            ["Deltas"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new BarGroupModel { Value = pointDelta } } },
            ["Lines"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new LineGroupModel { Value = point }, ["V2"] = new LineGroupModel { Value = pointMirror } } },
            ["Candles"] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new CandleGroupModel { Value = candle }, ["V2"] = new ArrowGroupModel { Value = arrow } } }
          }
        });
      }

      var grp = _points.Last() as IGroupModel;
      var currentDelta = grp.Groups["Deltas"].Groups["V1"];
      var currentCandle = grp.Groups["Candles"].Groups["V1"];

      currentCandle.Value.Low = candle.Low;
      currentCandle.Value.High = candle.High;
      currentCandle.Value.Close = candle.Close;
      currentCandle.Color = currentCandle.Value.Close > currentCandle.Value.Open ? SKColors.LimeGreen : SKColors.OrangeRed;

      currentDelta.Value.Point = currentCandle.Value.Close > currentCandle.Value.Open ? candle.Close : -candle.Close;
      currentDelta.Color = currentCandle.Color;

      _views.ForEach(panel =>
      {
        var composer = panel.Value.Composer;
        composer.Points = _points;
        composer.IndexDomain ??= new int[2];
        composer.IndexDomain[0] = composer.Points.Count - composer.IndexCount;
        composer.IndexDomain[1] = composer.Points.Count;
        panel.Value.Update();
      });
    }

    /// <summary>
    /// Generate candle
    /// </summary>
    protected dynamic CreatePoint()
    {
      var open = (double)_generator.Next(1000, 5000);
      var close = (double)_generator.Next(1000, 5000);
      var shadow = (double)_generator.Next(500, 1000);
      var candle = new Model
      {
        ["Low"] = Math.Min(open, close) - shadow,
        ["High"] = Math.Max(open, close) + shadow,
        ["Open"] = _pointValue,
        ["Close"] = close
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
