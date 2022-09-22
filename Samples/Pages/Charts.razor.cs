using Canvas.Core;
using Canvas.Core.ComposerSpace;
using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using Canvas.Core.ShapeSpace;
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
    protected List<IShape> _points = new();
    protected Dictionary<string, IView> _views = new()
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

          await view.Value.Create<CanvasEngine>(engine =>
          {
            source.TrySetResult();
            return composer;
          });

          composer.OnDomain += (message, source) => _views.ForEach(o =>
          {
            if (source is not null && Equals(composer.Name, o.Value.Composer.Name) is false)
            {
              o.Value.Composer.Update(message);
            }
          });
        }

        await Task.WhenAll(sources.Select(o => o.Task));

        _interval.Enabled = true;
        _interval.Elapsed += (o, e) =>
        {
          lock (this)
          {
            if (_interval is not null)
            {
              Counter(_interval.Enabled = _points.Count < 150);
            }
          }
        };
      }

      await base.OnAfterRenderAsync(setup);
    }

    #region Generator

    protected double _pointValue = 0;
    protected DateTime _pointTime = DateTime.Now;
    protected Timer _interval = new(1);
    protected Random _generator = new();

    /// <summary>
    /// On timer event
    /// </summary>
    protected void Counter(bool active)
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
        _points.Add(new GroupShape
        {
          Data = new DataModel { X = _pointTime.Ticks },
          Groups = new Dictionary<string, IGroupShape>
          {
            ["Bars"] = new GroupShape { Groups = new Dictionary<string, IGroupShape> { ["V1"] = new BarShape { Data = new DataModel { Y = point } } } },
            ["Areas"] = new GroupShape { Groups = new Dictionary<string, IGroupShape> { ["V1"] = new AreaShape { Data = new DataModel { Y = point } } } },
            ["Deltas"] = new GroupShape { Groups = new Dictionary<string, IGroupShape> { ["V1"] = new BarShape { Data = new DataModel { Y = pointDelta } } } },
            ["Lines"] = new GroupShape { Groups = new Dictionary<string, IGroupShape> { ["V1"] = new LineShape { Data = new DataModel { Y = point } }, ["V2"] = new LineShape { Data = new DataModel { Y = pointMirror } } } },
            ["Candles"] = new GroupShape { Groups = new Dictionary<string, IGroupShape> { ["V1"] = candle, ["V2"] = new ArrowShape { Data = new DataModel { Y = arrow }, Direction = 1 } } }
          }
        });
      }

      var grp = _points.Last() as IGroupShape;
      var currentDelta = grp.Groups["Deltas"].Groups["V1"];
      var currentCandle = grp.Groups["Candles"].Groups["V1"] as CandleShape;

      currentCandle.Low = candle.Low;
      currentCandle.High = candle.High;
      currentCandle.Close = candle.Close;
      currentCandle.Component = new ComponentModel
      {
        Size = 1,
        Color = currentCandle.Close > currentCandle.Open ? SKColors.LimeGreen : SKColors.OrangeRed
      };

      //currentDelta.Data.Y = currentCandle.Close > currentCandle.Open ? candle.Close : -candle.Close;
      //currentDelta.Color = currentCandle.Color;

      _views.ForEach(view =>
      {
        var domain = view.Value.Composer.Domain;

        domain.IndexDomain = new[]
        {
          _points.Count - 100,
          _points.Count
        };

        view.Value.Composer.Items = _points;
        view.Value.Composer.Update(domain);
      });
    }

    /// <summary>
    /// Generate candle
    /// </summary>
    protected CandleShape CreatePoint()
    {
      var open = (double)_generator.Next(1000, 5000);
      var close = (double)_generator.Next(1000, 5000);
      var shadow = (double)_generator.Next(500, 1000);
      var candle = new CandleShape
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
    public void Dispose()
    {
      _interval.Dispose();
      _interval = null;
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
