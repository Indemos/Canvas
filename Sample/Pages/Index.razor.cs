using Core;
using Core.EngineSpace;
using Core.ModelSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using View;

namespace Client.Pages
{
  public partial class Index
  {
    protected Timer _interval = new(100);
    protected Random _generator = new();
    protected double _pointValue = 0;
    protected DateTime _pointTime = DateTime.Now;
    protected IList<IGroupModel> _points = new List<IGroupModel>();
    protected IDictionary<string, CanvasWebView> _views = new Dictionary<string, CanvasWebView>();

    public CanvasWebView ViewBars { get; set; }
    public CanvasWebView ViewLines { get; set; }
    public CanvasWebView ViewAreas { get; set; }
    public CanvasWebView ViewDeltas { get; set; }
    public CanvasWebView ViewCandles { get; set; }

    protected override async Task OnAfterRenderAsync(bool setup)
    {
      if (setup)
      {
        var sourceBars = new TaskCompletionSource();
        var sourceLines = new TaskCompletionSource();
        var sourceAreas = new TaskCompletionSource();
        var sourceDeltas = new TaskCompletionSource();
        var sourceCandles = new TaskCompletionSource();

        ViewBars.OnSize = ViewBars.OnCreate = message => OnCreate(nameof(ViewBars), message, sourceBars);
        ViewLines.OnSize = ViewLines.OnCreate = message => OnCreate(nameof(ViewLines), message, sourceLines);
        ViewAreas.OnSize = ViewAreas.OnCreate = message => OnCreate(nameof(ViewAreas), message, sourceAreas);
        ViewDeltas.OnSize = ViewDeltas.OnCreate = message => OnCreate(nameof(ViewDeltas), message, sourceDeltas);
        ViewCandles.OnSize = ViewCandles.OnCreate = message => OnCreate(nameof(ViewCandles), message, sourceCandles);

        await Task.WhenAll(
          sourceBars.Task,
          sourceLines.Task,
          sourceAreas.Task,
          sourceDeltas.Task,
          sourceCandles.Task);

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
      message?.View?.Composer?.Dispose();

      _views[name] = message.View;
      _views[name].Composer = new Composer
      {
        Name = name,
        Groups = _points,
        Engine = new CanvasEngine(message.Width, message.Height)
      };

      _views[name].Update();
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
            [nameof(ViewBars)] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new BarGroupModel { Value = point } } },
            [nameof(ViewAreas)] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new AreaGroupModel { Value = point } } },
            [nameof(ViewDeltas)] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new BarGroupModel { Value = pointDelta } } },
            [nameof(ViewLines)] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new LineGroupModel { Value = point }, ["V2"] = new LineGroupModel { Value = pointMirror }}},
            [nameof(ViewCandles)] = new GroupModel { Groups = new Dictionary<string, IGroupModel> { ["V1"] = new CandleGroupModel { Value = candle }, ["V2"] = new ArrowGroupModel { Value = arrow }}}
          }
        });
      }

      var currentDelta = _points.Last().Groups[nameof(ViewDeltas)].Groups["V1"];
      var currentCandle = _points.Last().Groups[nameof(ViewCandles)].Groups["V1"];

      currentCandle.Value.Low = candle.Low;
      currentCandle.Value.High = candle.High;
      currentCandle.Value.Close = candle.Close;
      currentCandle.Color = currentCandle.Value.Close > currentCandle.Value.Open ? SKColors.LimeGreen : SKColors.OrangeRed;

      currentDelta.Value.Point = currentCandle.Value.Close > currentCandle.Value.Open ? candle.Close : -candle.Close;
      currentDelta.Color = currentCandle.Color;

      _views.ForEach(panel =>
      { 
        var composer = panel.Value.Composer;
        composer.Groups = _points;
        composer.IndexDomain ??= new int[2];
        composer.IndexDomain[0] = composer.Groups.Count - composer.IndexCount;
        composer.IndexDomain[1] = composer.Groups.Count;
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
  }
}
