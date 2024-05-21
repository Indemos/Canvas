using Canvas.Core.Engines;
using Canvas.Core.Enums;
using Canvas.Core.Models;
using Canvas.Core.Shapes;
using Distribution.Collections;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Canvas.Core.Composers
{
  public interface IComposer
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
    /// Index ticks
    /// </summary>
    int IndexCount { get; set; }

    /// <summary>
    /// Value ticks
    /// </summary>
    int ValueCount { get; set; }

    /// <summary>
    /// Domain
    /// </summary>
    DomainModel Domain { get; }

    /// <summary>
    /// Items
    /// </summary>
    IList<IShape> Items { get; set; }

    /// <summary>
    /// Views
    /// </summary>
    IView View { get; set; }

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
    Action<DomainModel, string> OnDomain { get; set; }

    /// <summary>
    /// Create 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task Create<T>() where T : IEngine, new();

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    Task Update(DomainModel? message = null, string source = null);

    /// <summary>
    /// Update items
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    void GetItems(IEngine engine, DomainModel message);

    /// <summary>
    /// Convert values to canvas coordinates
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    DataModel GetItemPosition(IEngine engine, DataModel item);

    /// <summary>
    /// Transform coordinates
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    DataModel GetItemPosition(IEngine engine, double index, double value);

    /// <summary>
    /// Convert canvas coordinates to values
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    DataModel GetItemValue(IEngine engine, DataModel item);

    /// <summary>
    /// Value scale
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="delta"></param>
    IList<double> ZoomValue(IEngine engine, int delta);

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="delta"></param>
    IList<int> ZoomIndex(IEngine engine, int delta);

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="delta"></param>
    IList<int> PanIndex(IEngine engine, int delta);
  }

  public class Composer : IComposer
  {
    /// <summary>
    /// Name
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Item size
    /// </summary>
    public virtual double Size { get; set; }

    /// <summary>
    /// Index ticks
    /// </summary>
    public virtual int IndexCount { get; set; }

    /// <summary>
    /// Value ticks
    /// </summary>
    public virtual int ValueCount { get; set; }

    /// <summary>
    /// Domain
    /// </summary>
    public virtual DomainModel Domain { get; protected set; }

    /// <summary>
    /// Items
    /// </summary>
    public virtual IList<IShape> Items { get; set; }

    /// <summary>
    /// Views
    /// </summary>
    public virtual IView View { get; set; }

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
    public virtual Action<DomainModel, string> OnDomain { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public Composer()
    {
      Size = 0.5;
      ValueCount = 3;
      IndexCount = 9;

      Domain = new DomainModel();
      Items = new ObservableGroupCollection<IShape>();
      Components = new Dictionary<string, ComponentModel>();

      ShowBoard = o => $"{o:0.00}";
      ShowIndex = o => $"{o:0.00}";
      ShowValue = o => $"{o:0.00}";

      OnDomain = (message, source) => { };

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
    /// Create
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual Task Create<T>() where T : IEngine, new() => View.Create<T>(() => this);

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual Task Update(DomainModel? domain = null, string source = null)
    {
      Domain = ComposeDomain(domain ?? Domain);
      OnDomain(Domain, source);

      return View.Update(Domain);
    }

    /// <summary>
    /// Update items
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="domain"></param>
    /// <returns></returns>
    public virtual void GetItems(IEngine engine, DomainModel domain)
    {
      View.Values = GetValues();
      View.Indices = GetIndices();

      for (var i = domain.MinIndex; i < domain.MaxIndex; i++)
      {
        var item = Items.ElementAtOrDefault(i);
        var itemDomain = item?.GetDomain(i, null, Items);

        if (itemDomain is null)
        {
          continue;
        }

        item.Engine = engine;
        item.Composer = this;
        item.CreateShape(i, null, Items);
      }
    }

    /// <summary>
    /// Convert values to canvas coordinates
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public virtual DataModel GetItemPosition(IEngine engine, DataModel item)
    {
      var minX = Domain.MinIndex;
      var maxX = Domain.MaxIndex;
      var minY = Domain.MinValue;
      var maxY = Domain.MaxValue;

      // Convert to device pixels

      var index = Equals(minX, maxX) ? 1.0 : (item.X - minX) / (maxX - minX);
      var value = Equals(minY, maxY) ? 1.0 : (item.Y - minY) / (maxY - minY);

      // Percentage to pixels, Y is inverted

      item.X = Math.Round(engine.X * index, MidpointRounding.ToZero);
      item.Y = Math.Round(engine.Y - engine.Y * value, MidpointRounding.ToZero);

      return item;
    }

    /// <summary>
    /// Transform coordinates
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual DataModel GetItemPosition(IEngine engine, double index, double value)
    {
      return GetItemPosition(engine, new DataModel { X = index, Y = value });
    }

    /// <summary>
    /// Convert canvas coordinates to values
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public virtual DataModel GetItemValue(IEngine engine, DataModel item)
    {
      var minX = Domain.MinIndex;
      var maxX = Domain.MaxIndex;
      var minY = Domain.MinValue;
      var maxY = Domain.MaxValue;

      // Convert to values

      var index = item.X / engine.X;
      var value = item.Y / engine.Y;

      // Percentage to values, Y is inverted

      item.X = minX + (maxX - minX) * index;
      item.Y = maxY - (maxY - minY) * value;

      return item;
    }

    /// <summary>
    /// Value scale
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="delta"></param>
    public virtual IList<double> ZoomValue(IEngine engine, int delta)
    {
      var minY = Domain.MinValue;
      var maxY = Domain.MaxValue;
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
    /// <param name="engine"></param>
    /// <param name="delta"></param>
    public virtual IList<int> ZoomIndex(IEngine engine, int delta)
    {
      var minX = Domain.MinIndex;
      var maxX = Domain.MaxIndex;
      var domain = new List<int> { minX, maxX };

      if (Equals(minX, maxX))
      {
        return domain;
      }

      var increment = Math.Sign(delta);
      var isInRange = maxX - minX > IndexCount * increment * 2;

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
    /// <param name="engine"></param>
    /// <param name="delta"></param>
    public virtual IList<int> PanIndex(IEngine engine, int delta)
    {
      var minX = Domain.MinIndex;
      var maxX = Domain.MaxIndex;
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
    /// Enumerate indices
    /// </summary>
    protected virtual IList<MarkerModel> GetIndices()
    {
      var minIndex = Domain.MinIndex;
      var maxIndex = Domain.MaxIndex;
      var center = Math.Round(minIndex + (maxIndex - minIndex) / 2.0, MidpointRounding.ToEven);
      var step = Math.Round((0.0 + maxIndex - minIndex) / IndexCount, MidpointRounding.ToEven);
      var items = new List<MarkerModel>();

      void createItem(double i)
      {
        if (i > minIndex && i < maxIndex)
        {
          var position = GetItemPosition(View.Engine, i, 0).X;

          items.Add(new MarkerModel
          {
            Line = position,
            Marker = position,
            Caption = ShowIndex(i)
          });
        }
      }

      for (var i = 0; i <= IndexCount; i++)
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
      var minValue = Domain.MinValue;
      var maxValue = Domain.MaxValue;
      var center = minValue + (maxValue - minValue) / 2.0;
      var step = (maxValue - minValue) / ValueCount;
      var items = new List<MarkerModel>();

      void createItem(double i)
      {
        if (i > minValue && i < maxValue)
        {
          var position = GetItemPosition(View.Engine, 0, i).Y;

          items.Add(new MarkerModel
          {
            Line = position,
            Marker = position,
            Caption = ShowValue(i)
          });
        }
      }

      for (var i = 0; i <= ValueCount; i++)
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
    protected virtual DomainModel ComposeDomain(DomainModel domain)
    {
      var autoMin = 0;
      var autoMax = Items.Count;
      var response = domain;

      response.AutoValueDomain = new[] { 0.0, 0.0 };
      response.AutoIndexDomain = new[] { autoMin, autoMax };

      var average = 0.0;
      var min = double.MaxValue;
      var max = double.MinValue;

      for (var i = response.MinIndex; i < response.MaxIndex; i++)
      {
        (min, max, average) = GetExtremes(i, min, max, average);
      }

      if (min > max)
      {
        return response;
      }

      if (Equals(min, max))
      {
        response.AutoValueDomain[0] = Math.Min(0, min);
        response.AutoValueDomain[1] = Math.Max(0, max);

        return response;
      }

      if (min < 0 && max > 0)
      {
        var extreme = Math.Max(Math.Abs(min), Math.Abs(max));

        response.AutoValueDomain[0] = -extreme;
        response.AutoValueDomain[1] = extreme;

        return response;
      }

      response.AutoValueDomain[0] = min;
      response.AutoValueDomain[1] = max;

      return response;
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
  }
}
