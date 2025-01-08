using Canvas.Core.Engines;
using Canvas.Core.Enums;
using Canvas.Core.Models;
using Canvas.Core.Shapes;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.Composers
{
  public interface IComposer : IDisposable
  {
    /// <summary>
    /// Name
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Item size
    /// </summary>
    double Size { get; set; }

    /// <summary>
    /// Spacing in percentage
    /// </summary>
    double Space { get; set; }

    /// <summary>
    /// Index ticks
    /// </summary>
    int Indices { get; set; }

    /// <summary>
    /// Value ticks
    /// </summary>
    int Values { get; set; }

    /// <summary>
    /// Domain
    /// </summary>
    DimensionModel Dimension { get; set; }

    /// <summary>
    /// Engine
    /// </summary>
    IEngine Engine { get; set; }

    /// <summary>
    /// Items
    /// </summary>
    IList<IShape> Items { get; set; }

    /// <summary>
    /// Options
    /// </summary>
    IDictionary<string, ComponentModel> Components { get; set; }

    /// <summary>
    /// Format indices
    /// </summary>
    Func<double, string> ShowIndex { get; set; }

    /// <summary>
    /// Format values
    /// </summary>
    Func<double, string> ShowValue { get; set; }

    /// <summary>
    /// Format board
    /// </summary>
    Func<double, string> ShowBoard { get; set; }

    /// <summary>
    /// Domain update event
    /// </summary>
    Action<DimensionModel> OnAction { get; set; }

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="e"></param>
    DimensionModel OnWheel(ViewModel e);

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="e"></param>
    DimensionModel OnMouseMove(ViewModel e);

    /// <summary>
    /// Resize event
    /// </summary>
    /// <param name="e"></param>
    /// <param name="orientation"></param>
    DimensionModel OnScale(ViewModel e, int orientation = 0);

    /// <summary>
    /// Update items
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    ScopeModel Render(DimensionModel message);

    /// <summary>
    /// Update dimensions
    /// </summary>
    /// <param name="domain"></param>
    DimensionModel SetDimensions(DimensionModel domain);

    /// <summary>
    /// Convert values to canvas coordinates
    /// </summary>
    /// <param name="item"></param>
    DataModel GetItemPosition(DataModel item);

    /// <summary>
    /// Transform coordinates
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    DataModel GetItemPosition(double index, double value);

    /// <summary>
    /// Convert canvas coordinates to values
    /// </summary>
    /// <param name="item"></param>
    DataModel GetItemValue(DataModel item);

    /// <summary>
    /// Value scale
    /// </summary>
    /// <param name="delta"></param>
    IList<double> ZoomValue(int delta);

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="delta"></param>
    IList<int> ZoomIndex(int delta);

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="delta"></param>
    IList<int> PanIndex(int delta);
  }

  public partial class Composer : IComposer
  {
    protected virtual ViewModel? MoveEvent { get; set; }
    protected virtual ViewModel? ScaleEvent { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Item size
    /// </summary>
    public virtual double Size { get; set; }

    /// <summary>
    /// Spacing in percentage
    /// </summary>
    public virtual double Space { get; set; }

    /// <summary>
    /// Index ticks
    /// </summary>
    public virtual int Indices { get; set; }

    /// <summary>
    /// Value ticks
    /// </summary>
    public virtual int Values { get; set; }

    /// <summary>
    /// Domain
    /// </summary>
    public virtual DimensionModel Dimension { get; set; }

    /// <summary>
    /// Engine
    /// </summary>
    public virtual IEngine Engine { get; set; }

    /// <summary>
    /// Items
    /// </summary>
    public virtual IList<IShape> Items { get; set; }

    /// <summary>
    /// Options
    /// </summary>
    public virtual IDictionary<string, ComponentModel> Components { get; set; }

    /// <summary>
    /// Format indices
    /// </summary>
    public virtual Func<double, string> ShowIndex { get; set; }

    /// <summary>
    /// Format values
    /// </summary>
    public virtual Func<double, string> ShowValue { get; set; }

    /// <summary>
    /// Format board
    /// </summary>
    public virtual Func<double, string> ShowBoard { get; set; }

    /// <summary>
    /// Domain update event
    /// </summary>
    public virtual Action<DimensionModel> OnAction { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public Composer()
    {
      Size = 0.5;
      Space = 0;
      Values = 3;
      Indices = 9;

      Dimension = new DimensionModel();
      Components = new Dictionary<string, ComponentModel>();
      Items = [];

      ShowBoard = o => $"{o:0.00}";
      ShowIndex = o => $"{o:0.00}";
      ShowValue = o => $"{o:0.00}";

      OnAction = o => { };

      Components[nameof(ComponentEnum.Shape)] = new ComponentModel
      {
        Size = 1,
        Color = new SKColor(50, 50, 50)
      };

      Components[nameof(ComponentEnum.ShapeSection)] = new ComponentModel
      {
        Size = 1,
        Color = new SKColor(50, 50, 50)
      };

      Components[nameof(ComponentEnum.Grid)] =
      Components[nameof(ComponentEnum.BoardLine)] = new ComponentModel
      {
        Size = 1,
        Color = new SKColor(50, 50, 50),
        Composition = CompositionEnum.Dashes
      };

      Components[nameof(ComponentEnum.Board)] = new ComponentModel
      {
        Size = 10,
        Position = PositionEnum.L,
        Color = new SKColor(50, 50, 50),
        Background = new SKColor(230, 230, 230)
      };

      Components[nameof(ComponentEnum.Caption)] =
      Components[nameof(ComponentEnum.BoardMarker)] =
      Components[nameof(ComponentEnum.BoardCaption)] = new ComponentModel
      {
        Size = 10,
        Position = PositionEnum.Center,
        Color = new SKColor(50, 50, 50),
        Background = new SKColor(200, 200, 200)
      };
    }

    /// <summary>
    /// Update items
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    public virtual ScopeModel Render(DimensionModel domain)
    {
      Engine.Clear();

      SetDimensions(domain);

      for (var i = domain.MinIndex; i < domain.MaxIndex; i++)
      {
        var item = Items.ElementAtOrDefault(i);

        if (item is null)
        {
          continue;
        }

        item.Composer = this;
        item.CreateShape(i, null, Items);
      }

      return new ScopeModel
      {
        Values = GetValues(),
        Indices = GetIndices()
      };
    }

    /// <summary>
    /// Convert values to canvas coordinates
    /// </summary>
    /// <param name="item"></param>
    public virtual DataModel GetItemPosition(DataModel item)
    {
      var valueRange = Dimension.MaxValue - Dimension.MinValue;
      var minX = Dimension.MinIndex;
      var maxX = Dimension.MaxIndex;
      var minY = Dimension.MinValue - valueRange * Space;
      var maxY = Dimension.MaxValue + valueRange * Space;

      // Convert to device pixels

      var index = Equals(minX, maxX) ? 1.0 : (item.X - minX) / (maxX - minX);
      var value = Equals(minY, maxY) ? 1.0 : (item.Y - minY) / (maxY - minY);

      // Percentage to pixels, Y is inverted

      item.X = Engine.X * index;
      item.Y = Engine.Y - Engine.Y * value;

      return item;
    }

    /// <summary>
    /// Transform coordinates
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual DataModel GetItemPosition(double index, double value)
    {
      return GetItemPosition(new DataModel { X = index, Y = value });
    }

    /// <summary>
    /// Convert canvas coordinates to values
    /// </summary>
    /// <param name="item"></param>
    public virtual DataModel GetItemValue(DataModel item)
    {
      var valueRange = Dimension.MaxValue - Dimension.MinValue;
      var minX = Dimension.MinIndex;
      var maxX = Dimension.MaxIndex;
      var minY = Dimension.MinValue - valueRange * Space;
      var maxY = Dimension.MaxValue + valueRange * Space;

      // Convert to values

      var index = item.X / Engine.X;
      var value = item.Y / Engine.Y;

      // Percentage to values, Y is inverted

      item.X = minX + (maxX - minX) * index;
      item.Y = maxY - (maxY - minY) * value;

      return item;
    }

    /// <summary>
    /// Value scale
    /// </summary>
    /// <param name="delta"></param>
    public virtual IList<double> ZoomValue(int delta)
    {
      var minY = Dimension.MinValue;
      var maxY = Dimension.MaxValue;
      var domain = new List<double> { minY, maxY };

      if (Equals(maxY, minY))
      {
        return domain;
      }

      var increment = (maxY - minY) / 100;
      var isInRange = maxY - minY > increment * 2.0;

      switch (true)
      {
        case true when delta > 0:
          domain[0] -= Math.Abs(increment);
          domain[1] += Math.Abs(increment);
          break;

        case true when delta < 0 && isInRange:
          domain[0] += Math.Abs(increment);
          domain[1] -= Math.Abs(increment);
          break;
      }

      return domain;
    }

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="delta"></param>
    public virtual IList<int> ZoomIndex(int delta)
    {
      var minX = Dimension.MinIndex;
      var maxX = Dimension.MaxIndex;
      var domain = new[] { minX, maxX };

      if (Equals(minX, maxX))
      {
        return domain;
      }

      var increment = Math.Sign(delta);
      var isInRange = maxX - minX > Indices * increment * 2;

      if (isInRange)
      {
        domain[0] += increment;
        domain[1] -= increment;
      }

      return domain;
    }

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="delta"></param>
    public virtual IList<int> PanIndex(int delta)
    {
      var minX = Dimension.MinIndex;
      var maxX = Dimension.MaxIndex;
      var domain = new List<int> { minX, maxX };

      if (Equals(minX, maxX))
      {
        return domain;
      }

      var increment = Math.Sign(delta);

      switch (true)
      {
        case true when delta > 0:
          domain[0] += Math.Abs(increment);
          domain[1] += Math.Abs(increment);
          break;

        case true when delta < 0:
          domain[0] -= Math.Abs(increment);
          domain[1] -= Math.Abs(increment);
          break;
      }

      return domain;
    }

    /// <summary>
    /// Mouse wheel event
    /// </summary>
    /// <param name="e"></param>
    public virtual DimensionModel OnWheel(ViewModel e)
    {
      var isZoom = e.IsShape;
      var domain = Dimension;

      switch (true)
      {
        case true when e.Data.Y > 0: domain.IndexDomain = isZoom ? ZoomIndex(-1) : PanIndex(1); break;
        case true when e.Data.Y < 0: domain.IndexDomain = isZoom ? ZoomIndex(1) : PanIndex(-1); break;
      }

      return domain;
    }

    /// <summary>
    /// Horizontal drag and resize event
    /// </summary>
    /// <param name="e"></param>
    public virtual DimensionModel OnMouseMove(ViewModel e)
    {
      MoveEvent ??= e;

      var domain = Dimension;

      if (e.IsMove)
      {
        var deltaX = MoveEvent?.Data.X - e.Data.X;
        var deltaY = MoveEvent?.Data.Y - e.Data.Y;

        switch (true)
        {
          case true when deltaX > 0: domain.IndexDomain = PanIndex(1); break;
          case true when deltaX < 0: domain.IndexDomain = PanIndex(-1); break;
        }
      }

      MoveEvent = e;

      return domain;
    }

    /// <summary>
    /// Resize event
    /// </summary>
    /// <param name="e"></param>
    /// <param name="orientation"></param>
    public virtual DimensionModel OnScale(ViewModel e, int orientation = 0)
    {
      ScaleEvent ??= e;

      var domain = Dimension;

      if (e.IsMove)
      {
        var deltaX = ScaleEvent?.Data.X - e.Data.X;
        var deltaY = ScaleEvent?.Data.Y - e.Data.Y;

        switch (orientation > 0)
        {
          case true when deltaX > 0: domain.IndexDomain = ZoomIndex(-1); break;
          case true when deltaX < 0: domain.IndexDomain = ZoomIndex(1); break;
        }

        switch (orientation < 0)
        {
          case true when deltaY > 0: domain.ValueDomain = ZoomValue(-1); break;
          case true when deltaY < 0: domain.ValueDomain = ZoomValue(1); break;
        }
      }

      ScaleEvent = e;

      return domain;
    }

    /// <summary>
    /// Enumerate indices
    /// </summary>
    protected virtual IList<MarkerModel> GetIndices()
    {
      var minIndex = Dimension.MinIndex;
      var maxIndex = Dimension.MaxIndex;
      var range = 0.0 + maxIndex - minIndex;
      var center = Math.Round(minIndex + range / 2.0, MidpointRounding.ToEven);
      var step = Math.Round(range / Indices, MidpointRounding.ToZero);
      var items = new List<MarkerModel>();

      void createItem(double i)
      {
        if (i > minIndex && i < maxIndex)
        {
          var position = GetItemPosition(i, 0).X;

          items.Add(new MarkerModel
          {
            Line = position,
            Marker = position,
            Caption = ShowIndex(i)
          });
        }
      }

      for (var i = 0; i <= Math.Min(Indices, range); i++)
      {
        createItem(center - i * step);
        createItem(center + i * step);
      }

      return items;
    }

    /// <summary>
    /// Enumerate values
    /// </summary>
    protected virtual IList<MarkerModel> GetValues()
    {
      var minValue = Dimension.MinValue;
      var maxValue = Dimension.MaxValue;
      var range = maxValue - minValue;
      var center = minValue + range / 2.0;
      var step = range / Values;
      var items = new List<MarkerModel>();

      void createItem(double i)
      {
        if (i > minValue && i < maxValue)
        {
          var position = GetItemPosition(0, i).Y;

          items.Add(new MarkerModel
          {
            Line = position,
            Marker = position,
            Caption = ShowValue(i)
          });
        }
      }

      for (var i = 0; i <= Math.Min(Values, range); i++)
      {
        createItem(center - i * step);
        createItem(center + i * step);
      }

      return items;
    }

    /// <summary>
    /// Get min and max values
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    public virtual DimensionModel SetDimensions(DimensionModel domain)
    {
      var autoMin = 0;
      var autoMax = Items.Count;
      var response = domain;

      response.AutoValueDomain = [0.0, 0.0];
      response.AutoIndexDomain = [autoMin, autoMax];

      var average = 0.0;
      var min = double.MaxValue;
      var max = double.MinValue;

      for (var i = response.MinIndex; i < response.MaxIndex; i++)
      {
        (min, max, average) = GetExtremes(i, min, max, average);
      }

      if (min > max)
      {
        return Dimension = response;
      }

      if (Equals(min, max))
      {
        response.AutoValueDomain[0] = Math.Min(0, min);
        response.AutoValueDomain[1] = Math.Max(0, max);

        return Dimension = response;
      }

      if (min < 0 && max > 0)
      {
        var extreme = Math.Max(Math.Abs(min), Math.Abs(max));

        response.AutoValueDomain[0] = -extreme;
        response.AutoValueDomain[1] = extreme;

        return Dimension = response;
      }

      response.AutoValueDomain[0] = min;
      response.AutoValueDomain[1] = max;

      return Dimension = response;
    }

    /// <summary>
    /// Calculate min and max for value domain
    /// </summary>
    /// <param name="i"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="average"></param>
    /// <returns></returns>
    protected virtual (double, double, double) GetExtremes(int i, double min, double max, double average)
    {
      var item = Items.ElementAtOrDefault(i);
      var domain = item?.GetDomain(i, null, Items);

      if (domain is null)
      {
        return (min, max, average);
      }

      item.Composer = this;
      min = Math.Min(min, domain[0]);
      max = Math.Max(max, domain[1]);
      average += max - min;

      return (min, max, average);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() => Engine?.Dispose();
  }
}
