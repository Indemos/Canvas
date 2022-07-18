using Canvas.Core.EngineSpace;
using Canvas.Core.ModelSpace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core
{
  public class Composer : IDisposable
  {
    /// <summary>
    /// Name
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Engine
    /// </summary>
    public virtual IEngine Engine { get; set; }

    /// <summary>
    /// Data
    /// </summary>
    public virtual double ItemSize { get; set; }
    public virtual IList<IItemModel> Items { get; set; }
    public virtual IList<IComponentModel> Components { get; set; }

    /// <summary>
    /// Index axis
    /// </summary>
    public virtual int IndexCount { get; set; }
    public virtual IList<int> IndexDomain { get; set; }
    public virtual IList<int> AutoIndexDomain { get; protected set; }
    public virtual Func<object, string> ShowIndexAction { get; set; }
    public virtual int MinIndex => IndexDomain?.ElementAtOrDefault(0) ?? AutoIndexDomain?.ElementAtOrDefault(0) ?? 0;
    public virtual int MaxIndex => IndexDomain?.ElementAtOrDefault(1) ?? AutoIndexDomain?.ElementAtOrDefault(1) ?? IndexCount;

    /// <summary>
    /// Value axis
    /// </summary>
    public virtual int ValueCount { get; set; }
    public virtual IList<double> ValueDomain { get; set; }
    public virtual IList<double> AutoValueDomain { get; protected set; }
    public virtual Func<object, string> ShowValueAction { get; set; }
    public virtual double MinValue => ValueDomain?.ElementAtOrDefault(0) ?? AutoValueDomain?.ElementAtOrDefault(0) ?? 0.0;
    public virtual double MaxValue => ValueDomain?.ElementAtOrDefault(1) ?? AutoValueDomain?.ElementAtOrDefault(1) ?? ValueCount;

    /// <summary>
    /// Constructor
    /// </summary>
    public Composer()
    {
      ItemSize = 0.5;
      IndexCount = 9;
      ValueCount = 3;

      Items = new List<IItemModel>();
      Components = new List<IComponentModel>();

      CreateIndexDomain();
    }

    /// <summary>
    /// Update delegate
    /// </summary>
    public virtual void Update()
    {
      CreateIndexDomain();
      CreateValueDomain();
      UpdateItems();
    }

    /// <summary>
    /// Enumerate
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<int> GetEnumerator()
    {
      var min = MinIndex;
      var max = MaxIndex;

      for (var i = min; i < max; i++)
      {
        yield return i;
      }
    }

    /// <summary>
    /// Convert values to canvas coordinates
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public virtual IItemModel GetPixels(IEngine engine, IItemModel item)
    {
      // Convert to device pixels

      var index = Equals(MinIndex, MaxIndex) ? 1.0 : (item.Index - MinIndex) / (MaxIndex - MinIndex);
      var value = Equals(MinValue, MaxValue) ? 1.0 : (item.Value - MinValue) / (MaxValue - MinValue);

      // Percentage to pixels, Y is inverted

      item.Index = engine.IndexSize * index.Value;
      item.Value = engine.ValueSize - engine.ValueSize * value;

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
      return GetPixels(engine, new ItemModel { Index = index, Value = value });
    }

    /// <summary>
    /// Convert canvas coordinates to values
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public virtual IItemModel GetValues(IEngine engine, IItemModel item)
    {
      // Convert to values

      var index = item.Index / engine.IndexSize;
      var value = item.Value / engine.ValueSize;

      // Percentage to values, Y is inverted

      item.Index = MinIndex + (MaxIndex - MinIndex) * index;
      item.Value = MaxValue - (MaxValue - MinValue) * value;

      return item;
    }

    /// <summary>
    /// Format index label
    /// </summary>
    public virtual string ShowIndex(object input)
    {
      return ShowIndexAction is null ? $"{input:0.00}" : ShowIndexAction(input);
    }

    /// <summary>
    /// Format value label
    /// </summary>
    public virtual string ShowValue(object input)
    {
      return ShowValueAction is null ? $"{input:0.00}" : ShowValueAction(input);
    }

    /// <summary>
    /// Value scale
    /// </summary>
    /// <param name="delta"></param>
    public virtual int ZoomValueScale(int delta)
    {
      if (Equals(MaxValue, MinValue))
      {
        return delta;
      }

      ValueDomain ??= AutoValueDomain.ToList();

      var increment = (MaxValue - MinValue) / 10;
      var isInRange = MaxValue - MinValue > increment * 2;

      switch (true)
      {
        case true when delta > 0:
          ValueDomain[0] -= Math.Abs(increment);
          ValueDomain[1] += Math.Abs(increment);
          Update();
          break;

        case true when delta < 0 && isInRange:
          ValueDomain[0] += Math.Abs(increment);
          ValueDomain[1] -= Math.Abs(increment);
          Update();
          break;
      }

      return delta;
    }

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="delta"></param>
    public virtual int ZoomIndexScale(int delta)
    {
      if (Equals(MaxIndex, MinIndex))
      {
        return delta;
      }

      IndexDomain ??= AutoIndexDomain.ToList();

      var increment = 100 / IndexCount / 2 * delta;
      var isInRange = MaxIndex - MinIndex > increment * 2;

      if (isInRange)
      {
        IndexDomain[0] += increment;
        IndexDomain[1] -= increment;
        Update();
      }

      return delta;
    }

    /// <summary>
    /// Index scale
    /// </summary>
    /// <param name="delta"></param>
    public virtual int PanIndexScale(int delta)
    {
      if (Equals(MaxIndex, MinIndex))
      {
        return delta;
      }

      IndexDomain ??= AutoIndexDomain.ToList();

      var increment = IndexCount / 2 * delta;

      switch (true)
      {
        case true when delta > 0:
          IndexDomain[0] += Math.Abs(increment);
          IndexDomain[1] += Math.Abs(increment);
          Update();
          break;

        case true when delta < 0:
          IndexDomain[0] -= Math.Abs(increment);
          IndexDomain[1] -= Math.Abs(increment);
          Update();
          break;
      }

      return delta;
    }

    /// <summary>
    /// Create Min and Max domain 
    /// </summary>
    /// <returns></returns>
    protected virtual IList<int> CreateIndexDomain()
    {
      AutoIndexDomain ??= new int[2];
      AutoIndexDomain[0] = 0;
      AutoIndexDomain[1] = Math.Max(Items.Count, IndexCount);

      return AutoIndexDomain;
    }

    /// <summary>
    /// Create Min and Max domain 
    /// </summary>
    /// <returns></returns>
    protected virtual IList<double> CreateValueDomain()
    {
      var average = 0.0;
      var min = double.MaxValue;
      var max = double.MinValue;

      foreach (var i in GetEnumerator())
      {
        var point = Items.ElementAtOrDefault(i);
        var domain = point?.CreateDomain(i, null, Items);

        if (domain is null)
        {
          continue;
        }

        point.Value.Composer = this;
        min = Math.Min(min, domain[0]);
        max = Math.Max(max, domain[1]);
        average += max - min;
      }

      AutoValueDomain ??= new double[2];

      if (min > max)
      {
        return AutoValueDomain = null;
      }

      if (Equals(min, max))
      {
        AutoValueDomain[0] = Math.Min(0, min);
        AutoValueDomain[1] = Math.Max(0, max);

        return AutoValueDomain;
      }

      if (min < 0 && max > 0)
      {
        var extreme = Math.Max(Math.Abs(min), Math.Abs(max));

        AutoValueDomain[0] = -extreme;
        AutoValueDomain[1] = extreme;

        return AutoValueDomain;
      }

      AutoValueDomain[0] = min;
      AutoValueDomain[1] = max;

      return AutoValueDomain;
    }

    /// <summary>
    /// Update series and collections
    /// </summary>
    protected virtual void UpdateItems()
    {
      foreach (var i in GetEnumerator())
      {
        var point = Items.ElementAtOrDefault(i);
        var domain = point?.CreateDomain(i, null, Items);

        if (domain is null)
        {
          continue;
        }

        point.Engine = Engine;
        point.Composer = this;
        point.CreateShape(i, null, Items);
      }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
      Engine?.Dispose();
    }
  }
}
