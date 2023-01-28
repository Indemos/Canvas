# Canvas - Financial Charts

The fastest charting web control targeting primarily Blazor, both Server Side and Web Assembly, and even ASP.NET MVC. 
This charting library was designed for Web, but it can also be used in Desktop apps via Web View. 
The main purpose of this library is to be used as a charting tool for real-time financial applications, e.g. backtesters for trading strategies. 
Here is [the most comprehensive guide](https://github.com/swharden/Csharp-Data-Visualization) dedicated to charting in .NET that I have seen so far. 
Nevertheless, trying various options from that guide I wasn't able to find anything fast and flexible enough for my needs, so created my own. 

- Examples can be found [here](https://github.com/Indemos/Canvas/tree/main/Samples/Pages) 
- Possible application of this library is [here](https://github.com/Indemos/Terminal) 

# Package 

```
Install-Package Canvas.Views.Web
```

![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/Indemos/Canvas/dotnet.yml?event=push)
![GitHub](https://img.shields.io/github/license/Indemos/Canvas)
![GitHub](https://img.shields.io/badge/system-Windows%20%7C%20Linux%20%7C%20Mac-blue)

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
* Candle - OHLC box, a mix of a line and a polygon
* HeatMap - polygon 

To add new chart types, e.g. `Error Bars` or `Bubbles`, implement `IShape` interface. 

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
      
      } as IItemModel).ToList();
      
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

In case when charts have to be synchronized or overlapped within the same viewport, data source should have format of a list where each entry point has a time stamp and a set of `Areas` and `Series` that will be rendered in the relevant viewport. 

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

- **Area** is a viewport, an actual chart, each viewport can show several types of series, e.g. a mix of candles and lines.
- **Series** is a single chart type to be displayed in the viewport, e.g. lines. 
- **Shape** is a data point of `dynamic` type, can accept different type of inputs, e.g. double or OHLC box.

At this moment, `Canvas` supports only horizontal orientation, so the axis X is used as an index scale that picks data points from the source list and axis Y is a value scale that represents the actual value of each data point. 

# Preview 

![](Screens/Preview.gif)

# Roadmap 

Each chart of type `CanvasView` consists of many composable pieces for the grid, scales, main screen. 
To improve performance, each piece uses its own thread. 
To increase performance, downsampling could be implemented, e.g. when number of points is greater that width of the screen in pixels, because all points wouldn't fit on the screen anyway. 
