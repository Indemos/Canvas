using Canvas.Core.EngineSpace;
using Canvas.Core.EnumSpace;
using Canvas.Core.MessageSpace;
using Canvas.Core.ModelSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
    DomainMessage Domain { get; }

    /// <summary>
    /// Item definition
    /// </summary>
    IComponentModel Item { get; set; }

    /// <summary>
    /// Shape definition
    /// </summary>
    IComponentModel Line { get; set; }

    /// <summary>
    /// Board definition
    /// </summary>
    IComponentModel Board { get; set; }

    /// <summary>
    /// Caption definition
    /// </summary>
    IComponentModel Caption { get; set; }

    /// <summary>
    /// Items
    /// </summary>
    IList<IItemModel> Items { get; set; }

    /// <summary>
    /// Observable domain
    /// </summary>
    Action<DomainMessage, string> OnDomain { get; set; }

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
    /// Update
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    void Update(DomainMessage? message = null, string source = null);

    /// <summary>
    /// Update items
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    void UpdateItems(IEngine engine, DomainMessage message);

    /// <summary>
    /// Convert values to canvas coordinates
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    IItemModel GetPixels(IEngine engine, IItemModel item);

    /// <summary>
    /// Transform coordinates
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IItemModel GetPixels(IEngine engine, double index, double value);

    /// <summary>
    /// Convert canvas coordinates to values
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    IItemModel GetValues(IEngine engine, IItemModel item);

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
    public virtual DomainMessage Domain { get; set; }

    /// <summary>
    /// Item definition
    /// </summary>
    public virtual IComponentModel Item { get; set; }

    /// <summary>
    /// Shape definition
    /// </summary>
    public virtual IComponentModel Line { get; set; }

    /// <summary>
    /// Board definition
    /// </summary>
    public virtual IComponentModel Board { get; set; }

    /// <summary>
    /// Caption definition
    /// </summary>
    public virtual IComponentModel Caption { get; set; }

    /// <summary>
    /// Items
    /// </summary>
    public virtual IList<IItemModel> Items { get; set; }

    /// <summary>
    /// View subscription
    /// </summary>
    public virtual Action<DomainMessage, string> OnDomain { get; set; } = (message, source) => { };

    /// <summary>
    /// Format indices
    /// </summary>
    public virtual Func<double, string> ShowIndex { get; set; } = input => $"{input:0.00}";

    /// <summary>
    /// Format values
    /// </summary>
    public virtual Func<double, string> ShowValue { get; set; } = input => $"{input:0.00}";

    /// <summary>
    /// Format board
    /// </summary>
    public virtual Func<string, IList<double>, string> ShowBoard { get; set; } = (name, values) => $"{ name }: { string.Join(", ", values.Select(o => $"{o:0.00}")) }";

    /// <summary>
    /// Constructor
    /// </summary>
    public Composer()
    {
      ValueCount = 4;
      IndexCount = 10;
      Domain = new DomainMessage();

      Item = new ComponentModel
      {
        Size = 0.5,
        Color = new SKColor(50, 50, 50)
      };

      Line = new ComponentModel
      {
        Size = 1,
        Color = new SKColor(50, 50, 50),
        Composition = CompositionEnum.Dashes
      };

      Board = new ComponentModel
      {
        Size = 10,
        Position = PositionEnum.L,
        Color = new SKColor(50, 50, 50),
        Background = new SKColor(230, 230, 230)
      };

      Caption = new ComponentModel
      {
        Size = 10,
        Position = PositionEnum.Center,
        Color = new SKColor(50, 50, 50),
        Background = new SKColor(200, 200, 200)
      };

      Items = new List<IItemModel>();
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public virtual void Update(DomainMessage? message = null, string source = null)
    {
      var domain = message ?? Domain;

      domain.AutoIndexDomain = GetIndexDomain();
      domain.AutoValueDomain = GetValueDomain(domain);

      OnDomain(Domain = domain, source);
    }

    /// <summary>
    /// Update items
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="domain"></param>
    /// <returns></returns>
    public virtual void UpdateItems(IEngine engine, DomainMessage domain)
    {
      foreach (var i in GetEnumerator(domain))
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
    public virtual IItemModel GetPixels(IEngine engine, IItemModel item)
    {
      var minX = Domain.MinIndex;
      var maxX = Domain.MaxIndex;
      var minY = Domain.MinValue;
      var maxY = Domain.MaxValue;

      // Convert to device pixels

      var index = Equals(minX, maxX) ? 1.0 : (item.X - minX) / (maxX - minX);
      var value = Equals(minY, maxY) ? 1.0 : (item.Y - minY) / (maxY - minY);

      // Percentage to pixels, Y is inverted

      item.X = engine.X * index.Value;
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
    public virtual IItemModel GetPixels(IEngine engine, double index, double value)
    {
      return GetPixels(engine, new ItemModel { X = index, Y = value });
    }

    /// <summary>
    /// Convert canvas coordinates to values
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public virtual IItemModel GetValues(IEngine engine, IItemModel item)
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
    protected virtual IList<int> GetIndexDomain()
    {
      return new[] { 0, Math.Max(Items.Count, IndexCount) };
    }

    /// <summary>
    /// Get min and max values
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    protected virtual IList<double> GetValueDomain(DomainMessage domain)
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
    protected virtual IEnumerable<int> GetEnumerator(DomainMessage domain)
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
