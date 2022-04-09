# Canvas and Open GL Stock and Financial Charts

The fastest charting web control targeting primarily Blazor, both Server Side and Web Assembly, and even ASP.NET MVC. 
This charting library was designed for Web, but it can also be used in Desktop apps via Web View. 
The main purpose of this library is to be used as a charting tool for real-time financial applications, e.g. backtesters for trading strategies. 
Here is [the most comprehensive guide](https://github.com/swharden/Csharp-Data-Visualization) dedicated to charting in .NET that I have seen so far. 
Nevertheless, trying various options from that guide I wasn't able to find anything fast and flexible enough for my needs, so created my own. 

- Examples can be found [here](https://github.com/Indemos/Canvas/tree/main/Samples/Pages) 
- Possible application of this library is [here](https://github.com/Indemos/Terminal-Desktop) 

# Nuget 

```
Install-Package Canvas.Views.Web -Version 1.1.5-prerelease
```

# Drawing Methods 

Currently available controls.

* Engine - abstract base `Canvas` control exposing drawing context of various frameworks, like `GDI` or `SkiaSharp`  
* CanvasEngine - a wrapper around [SkiaSharp](https://github.com/mono/SkiaSharp) and Open GL 

To add different view types, e.g. `GDI+`, `Direct 2D`, `Win UI`, `Open GL`, implement `IEngine` interface.

# Chart Types 

At the moment, there are four built-in chart types. 

* Line - line 
* Bar - polygon
* Area - polygon
* Arrow - polygon
* Candle - OHLC box, a mix of a line and a rectangle polygon

To add new chart types, e.g. `Error Bars` or `Bubbles`, implement `IGroupModel` interface. 

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

# Data source structure

The simplest format used by the library is a list of models with a single `Point` property. 

```C#

<CanvasWebView @ref="ViewControl"></CanvasWebView>

@code
{
  public CanvasWebView ViewControl { get; set; }

  protected override async Task OnAfterRenderAsync(bool setup)
  {
    var points = Enumerable.Range(1, 100).Select(i => new BarGroupModel
    {
      Index = i,
      Value = new Model { ["Point"] = new Random().Next(-5000, 5000) }
    } as IPointModel);

    ViewControl.Composer = new Composer
    {
      Name = name,
      Points = points.ToList(),
      Engine = new CanvasEngine(1000, 500)
    };

    ViewControl.Update();
  }
}
```

In case when charts have to be synchronized or overlapped within the same viewport, data source should have format of a list where each entry point has a time stamp and a set of `Areas` and `Series` that will be rendered in the relevant viewport. 

```C#
[
  DateTime
  {
    Area A
    {
      Line Series => double,
      Candle Series => OHLC
    },
    Area B 
    {
      Line Series => double,
      Line Series => double
    },
    Area C 
    {
      Bar Series => double
    }
  }, 
  DateTime { ... },
  DateTime { ... },
  DateTime { ... }
]

```

- **Area** is a viewport, an actual chart, each viewport can show several types of series, e.g. a mix of candles and lines.
- **Series** is a single chart type to be displayed in the viewport, e.g. lines. 
- **Model** is a data point of `dynamic` type, can accept different type of inputs, e.g. double or OHLC box.

At this moment, `Canvas` supports only horizontal orientation, so the axis X is used as an index scale that picks data points from the source list and axis Y is a value scale that represents the actual value of each data point. 

# Preview 

![](Screens/Preview.png)

# Roadmap 

The current version is already the fastest .NET charting library, but it can be even faster if instead of rerendering HTML elements Blazor will simply show or hide them using CSS. 