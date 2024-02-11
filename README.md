# Canvas - Financial Charts

The fastest charting web control targeting primarily Blazor, both Server Side and Web Assembly, and to some extent ASP.NET MVC. 
This charting library was designed for Web, but it can also be used in Desktop apps via Web View. 

The main purpose of this library is to be used as a real-time charting tool for financial applications that require frequent updates, e.g. backtesters for trading strategies. 

Here is [the most comprehensive guide](https://github.com/swharden/Csharp-Data-Visualization) dedicated to charting in .NET that I have seen so far. 
Nevertheless, trying various options from that guide I wasn't able to find anything fast and flexible enough for my needs, so created my own. 

- Samples are [here](https://github.com/Indemos/Canvas/tree/main/Samples/Pages) 
- Example of usage in real life is a trading terminal [here](https://github.com/Indemos/Terminal) 

# Status 

```
Install-Package Canvas.Views.Web
```

![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/Indemos/Canvas/dotnet.yml?event=push)
![GitHub](https://img.shields.io/github/license/Indemos/Canvas)
![GitHub](https://img.shields.io/badge/system-Windows%20%7C%20Linux%20%7C%20Mac-blue)

# Implementations 

Currently available controls.

* Engine - base control exposing drawing context for various frameworks, like `GDI` or `SkiaSharp`  
* CanvasEngine - a wrapper around [SkiaSharp](https://github.com/mono/SkiaSharp) and Open GL 

To add different view types, e.g. `GDI+`, `Direct 2D`, `Win UI`, `Open GL`, implement `IEngine` interface.

# Chart Types 

At the moment, the library supports the following chart types. 

* Line - line 
* Bar - polygon
* Area - polygon
* Arrow - polygon
* Candle - OHLC box, a mix of a line and a box
* HeatMap - box 

To add new chart types, e.g. `Error Bars` or `Bubbles`, implement `IShape` interface. 

# Sample

The simplest data format is a list of `IShape` models with a `X` and `Y` properties. 

```C#

<CanvasView @ref="View"></CanvasView>

@code
{
  public CanvasView View { get; set; }

  protected override async Task OnAfterRenderAsync(bool setup)
  {
    if (setup)
    {
      var generator = new Random();
      var points = Enumerable.Range(1, 1000).Select(i => new BarShape 
      { 
        X = i, 
        Y = generator.Next(-5000, 5000) 
     
     }).ToList();
      
      var composer = new Composer
      {
        Name = "Demo",
        Items = points
      };

      await View.Create<CanvasEngine>(engine => composer);

      composer.Update();
    }

    await base.OnAfterRenderAsync(setup);
  }
}
```

By default, the axis X is used as an index that picks data points from the source list and axis Y is a value that represents the actual value of each data point on the vertical scale. 

# Preview 

![](Screens/Preview.gif)

# Synchronization 

To simplify synchronization, you can use `IGroupShape` model instead of simple `IShape`. 
This model allows grouping series for each chart by single timestamp, so you could display candles, lines, and other series on the same chart. 

```C#
Item = new 
{
  Groups = new GroupShape
  {
    ["Price Area"] = new Dictionary<string, GroupShape>
    {
      Groups = new GroupShape
      {
        ["Price Series"] = new CandleShape(),
        ["Arrow Series"] = new ArrowShape()
      }
    },
    ["Indicator Area"] = new Dictionary<string, GroupShape>
    {
      Groups = new GroupShape
      { 
        ["Bar Series"] = new BarShape() 
      }
    }
  }
}
```

# Pan and Zoom 

The chart is data-centric, thus in order to scale the chart you need to change the data source. 
By default, the chart displays last 100 data points, as defined in `IndexCount` property. 

```C#
MinIndex = Items.Count - IndexCount
MaxIndex = Items.Count
```

To pan the chart to the left, subtract arbitrary value from both `MinIndex` and `MaxIndex`. 

```C#
MinIndex -= 1
MaxIndex -= 1
```

To pan the chart to the right, do the opposite. 

```C#
MinIndex += 1
MaxIndex += 1
```

To zoom in, increase `MinIndex` and decrease `MaxIndex` to decrease number of visible points. 

```C#
MinIndex += 1
MaxIndex -= 1
```

To zoom out, do the opposite. 

```C#
MinIndex -= 1
MaxIndex += 1
```

# Roadmap 

To increase performance, the chart is split into pieces and each piece is using its own thread, so UI is never blocked even while rendering 100K samples. 
To increase performance even further, downsampling could be implemented, e.g. when number of points is greater that width of the screen in pixels, because all points wouldn't fit on the screen anyway. 
