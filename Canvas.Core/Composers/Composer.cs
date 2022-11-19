using Canvas.Core.EngineSpace;
using Canvas.Core.EnumSpace;
using Canvas.Core.ModelSpace;
using Canvas.Core.ShapeSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Canvas.Core.ComposerSpace
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
    IDictionary<string, IView> Views { get; set; }

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
    Func<string, IList<double>, string> ShowBoard { get; set; }

    /// <summary>
    /// Domain update event
    /// </summary>
    Action<DomainModel, string> OnDomain { get; set; }

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
    void UpdateItems(IEngine engine, DomainModel message);

    /// <summary>
    /// Convert values to canvas coordinates
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    DataModel GetPixels(IEngine engine, DataModel item);

    /// <summary>
    /// Transform coordinates
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    DataModel GetPixels(IEngine engine, double index, double value);

    /// <summary>
    /// Convert canvas coordinates to values
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    DataModel GetValues(IEngine engine, DataModel item);

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
    public virtual DomainModel Domain { get; set; }

    /// <summary>
    /// Items
    /// </summary>
    public virtual IList<IShape> Items { get; set; }

    /// <summary>
    /// Views
    /// </summary>
    public virtual IDictionary<string, IView> Views { get; set; }

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
    public virtual Func<string, IList<double>, string> ShowBoard { get; set; }

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
      ValueCount = 4;
      IndexCount = 10;

      Domain = new DomainModel();
      Items = new List<IShape>();
      Views = new Dictionary<string, IView>();
      Components = new Dictionary<string, ComponentModel>();

      ShowIndex = (input) => $"{input:0.00}";
      ShowValue = (input) => $"{input:0.00}";
      ShowBoard = (name, values) => $"{ name }: { string.Join(", ", values.Select(o => $"{o:0.00}")) }";

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
    /// Update
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual Task Update(DomainModel? message = null, string source = null)
    {
      var domain = message ?? Domain;

      domain.AutoIndexDomain = ComposeIndexDomain();
      domain.AutoValueDomain = ComposeValueDomain(domain);

      OnDomain(Domain = domain, source);

      return Task.WhenAll(Views.Values.Select(o => o.Update(domain, source)));
    }

    /// <summary>
    /// Update items
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="domain"></param>
    /// <returns></returns>
    public virtual void UpdateItems(IEngine engine, DomainModel domain)
    {
      foreach (var i in GetEnumerator(domain))
      {
        var item = Items.ElementAtOrDefault(i);

        if (item is null)
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
    public virtual DataModel GetPixels(IEngine engine, DataModel item)
    {
      var minX = Domain.MinIndex;
      var maxX = Domain.MaxIndex;
      var minY = Domain.MinValue;
      var maxY = Domain.MaxValue;

      // Convert to device pixels

      var index = Equals(minX, maxX) ? 1.0 : (item.X - minX) / (maxX - minX);
      var value = Equals(minY, maxY) ? 1.0 : (item.Y - minY) / (maxY - minY);

      // Percentage to pixels, Y is inverted

      item.X = engine.X * index;
      item.Y = engine.Y - engine.Y * value;

      return item;
    }

    /// <summary>
    /// Transform coordinates
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual DataModel GetPixels(IEngine engine, double index, double value)
    {
      return GetPixels(engine, new DataModel { X = index, Y = value });
    }

    /// <summary>
    /// Convert canvas coordinates to values
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public virtual DataModel GetValues(IEngine engine, DataModel item)
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

      var increment = (maxY - minY) / 10;
      var isInRange = maxY - minY > increment * 2;

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

      var increment = 100 / IndexCount / 2 * delta;
      var isInRange = maxX - minX > increment * 2;

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

      var increment = IndexCount / 2 * delta;

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
    /// Get min and max indices
    /// </summary>
    /// <returns></returns>
    protected virtual IList<int> ComposeIndexDomain()
    {
      return new[] { 0, Math.Max(Items.Count, IndexCount) };
    }

    /// <summary>
    /// Get min and max values
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    protected virtual IList<double> ComposeValueDomain(DomainModel domain)
    {
      var average = 0.0;
      var min = double.MaxValue;
      var max = double.MinValue;
      var response = new[] { 0.0, 0.0 };

      foreach (var i in GetEnumerator(domain))
      {
        var item = Items.ElementAtOrDefault(i);
        var itemDomain = item?.GetDomain(i, null, Items);

        if (itemDomain is null)
        {
          continue;
        }

        item.Composer = this;
        min = Math.Min(min, itemDomain[0]);
        max = Math.Max(max, itemDomain[1]);
        average += max - min;
      }

      if (min > max)
      {
        return null;
      }

      if (Equals(min, max))
      {
        response[0] = Math.Min(0, min);
        response[1] = Math.Max(0, max);

        return response;
      }

      if (min < 0 && max > 0)
      {
        var extreme = Math.Max(Math.Abs(min), Math.Abs(max));

        response[0] = -extreme;
        response[1] = extreme;

        return response;
      }

      response[0] = min;
      response[1] = max;

      return response;
    }

    /// <summary>
    /// Enumerate
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    protected virtual IEnumerable<int> GetEnumerator(DomainModel domain)
    {
      var min = domain.MinIndex;
      var max = domain.MaxIndex;

      for (var i = min; i < max; i++)
      {
        yield return i;
      }
    }
  }
}
