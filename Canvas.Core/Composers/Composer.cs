using Canvas.Core.EngineSpace;
using Canvas.Core.EnumSpace;
using Canvas.Core.MessageSpace;
using Canvas.Core.ModelSpace;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Min index
    /// </summary>
    int MinIndex { get; }

    /// <summary>
    /// Max index
    /// </summary>
    int MaxIndex { get; }

    /// <summary>
    /// Min value
    /// </summary>
    double MinValue { get; }

    /// <summary>
    /// Max value
    /// </summary>
    double MaxValue { get; }

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
    /// Views
    /// </summary>
    IList<IView> Views { get; set; }

    /// <summary>
    /// Items
    /// </summary>
    IList<IItemModel> Items { get; set; }

    /// <summary>
    /// Format indices
    /// </summary>
    Func<double, string> ShowIndex { get; set; }

    /// <summary>
    /// Format values
    /// </summary>
    Func<double, string> ShowValue { get; set; }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    Task Update(DomainMessage message = null);

    /// <summary>
    /// Update items
    /// </summary>
    Task UpdateItems(IEngine engine);

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
    IList<double> ZoomValueScale(IEngine engine, int delta);

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="delta"></param>
    IList<int> ZoomIndexScale(IEngine engine, int delta);

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="delta"></param>
    IList<int> PanIndexScale(IEngine engine, int delta);

    /// <summary>
    /// Create Min and Max domain 
    /// </summary>
    /// <returns></returns>
    IList<int> GetDomainX();

    /// <summary>
    /// Create Min and Max domain 
    /// </summary>
    /// <returns></returns>
    IList<double> GetDomainY();

    /// <summary>
    /// Enumerate
    /// </summary>
    /// <returns></returns>
    IEnumerable<int> GetEnumerator();
  }

  public class Composer : IComposer
  {
    /// <summary>
    /// Domain
    /// </summary>
    protected virtual Domain Domain { get; set; }

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
    /// Views
    /// </summary>
    public virtual IList<IView> Views { get; set; }

    /// <summary>
    /// Items
    /// </summary>
    public virtual IList<IItemModel> Items { get; set; }

    /// <summary>
    /// Min index
    /// </summary>
    public virtual int MinIndex => Domain.MinIndex;

    /// <summary>
    /// Max index
    /// </summary>
    public virtual int MaxIndex => Domain.MaxIndex;

    /// <summary>
    /// Min value
    /// </summary>
    public virtual double MinValue => Domain.MinValue;

    /// <summary>
    /// Max value
    /// </summary>
    public virtual double MaxValue => Domain.MaxValue;

    /// <summary>
    /// Format indices
    /// </summary>
    public virtual Func<double, string> ShowIndex { get; set; } = input => $"{input:0.00}";

    /// <summary>
    /// Format values
    /// </summary>
    public virtual Func<double, string> ShowValue { get; set; } = input => $"{input:0.00}";

    /// <summary>
    /// Constructor
    /// </summary>
    public Composer()
    {
      ValueCount = 4;
      IndexCount = 10;

      Domain = new Domain();

      Item = new ComponentModel
      {
        Size = 0.5,
        Color = new SKColor(50, 50, 50)
      };

      Line = new ComponentModel
      {
        Size = 1,
        Color = new SKColor(200, 200, 200)
      };

      Board = new ComponentModel
      {
        Size = 1,
        Composition = CompositionEnum.Dashes,
        Background = new SKColor(200, 200, 200),
        Color = new SKColor(50, 50, 50)
      };

      Caption = new ComponentModel
      {
        Size = 10,
        Position = PositionEnum.Center,
        Background = new SKColor(200, 200, 200),
        Color = new SKColor(50, 50, 50)
      };

      Views = new List<IView>();
      Items = new List<IItemModel>();
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="name"></param>
    public virtual Task Update(DomainMessage message = null)
    {
      Domain.AutoDomainX = GetDomainX();
      Domain.AutoDomainY = GetDomainY();

      if (message?.IndexDomain is not null || message?.IndexUpdate is true)
      {
        Domain.DomainX = message.IndexDomain;
      }

      if (message?.ValueDomain is not null || message?.ValueUpdate is true)
      {
        Domain.DomainY = message.ValueDomain;
      }

      foreach (var view in Views)
      {
        if (Equals(message?.Id, view.Id) is false)
        {
          view.Update();
        }
      }

      return Task.FromResult(0);
    }

    /// <summary>
    /// Update items
    /// </summary>
    /// <param name="engine"></param>
    public virtual Task UpdateItems(IEngine engine)
    {
      foreach (var i in GetEnumerator())
      {
        var item = Items.ElementAtOrDefault(i);
        var domain = item?.CreateDomain(i, null, Items);

        if (domain is null)
        {
          continue;
        }

        item.Engine = engine;
        item.Composer = this;
        item.CreateShape(i, null, Items);
      }

      return Task.FromResult(0);
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
    public virtual IList<double> ZoomValueScale(IEngine engine, int delta)
    {
      var minY = Domain.MinValue;
      var maxY = Domain.MaxValue;
      var domain = new List<double> { minY, maxY };

      if (Equals(maxY, minY))
      {
        return domain;
      }

      domain ??= Domain.AutoDomainY.ToList();

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
    public virtual IList<int> ZoomIndexScale(IEngine engine, int delta)
    {
      var minX = Domain.MinIndex;
      var maxX = Domain.MaxIndex;
      var domain = new List<int> { minX, maxX };

      if (Equals(minX, maxX))
      {
        return domain;
      }

      domain ??= Domain.AutoDomainX.ToList();

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
    public virtual IList<int> PanIndexScale(IEngine engine, int delta)
    {
      var minX = Domain.MinIndex;
      var maxX = Domain.MaxIndex;
      var domain = new List<int> { minX, maxX };

      if (Equals(minX, maxX))
      {
        return domain;
      }

      domain ??= Domain.AutoDomainX.ToList();

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
    /// Create Min and Max domain 
    /// </summary>
    /// <returns></returns>
    public virtual IList<int> GetDomainX()
    {
      return new[] { 0, Math.Max(Items.Count, IndexCount) };
    }

    /// <summary>
    /// Create Min and Max domain 
    /// </summary>
    /// <returns></returns>
    public virtual IList<double> GetDomainY()
    {
      var average = 0.0;
      var min = double.MaxValue;
      var max = double.MinValue;
      var domain = new[] { 0.0, 0.0 };

      foreach (var i in GetEnumerator())
      {
        var item = Items.ElementAtOrDefault(i);
        var itemDomain = item?.CreateDomain(i, null, Items);

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
        domain[0] = Math.Min(0, min);
        domain[1] = Math.Max(0, max);

        return domain;
      }

      if (min < 0 && max > 0)
      {
        var extreme = Math.Max(Math.Abs(min), Math.Abs(max));

        domain[0] = -extreme;
        domain[1] = extreme;

        return domain;
      }

      domain[0] = min;
      domain[1] = max;

      return domain;
    }

    /// <summary>
    /// Enumerate
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<int> GetEnumerator()
    {
      var min = Domain.MinIndex;
      var max = Domain.MaxIndex;

      for (var i = min; i < max; i++)
      {
        yield return i;
      }
    }
  }
}
