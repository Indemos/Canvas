using Core.EngineSpace;
using Core.ModelSpace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
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
    public virtual IList<IPointModel> Points { get; set; }
    public virtual IList<IShapeModel> Shapes { get; set; }

    /// <summary>
    /// Index axis
    /// </summary>
    public virtual int IndexCount { get; set; }
    public virtual int IndexLabelCount { get; set; }
    public virtual IList<int> IndexDomain { get; set; }
    public virtual Func<dynamic, dynamic> ShowIndexAction { get; set; }
    public virtual int MinIndex => IndexDomain?.ElementAtOrDefault(0) ?? AutoIndexDomain?.ElementAtOrDefault(0) ?? 0;
    public virtual int MaxIndex => IndexDomain?.ElementAtOrDefault(1) ?? AutoIndexDomain?.ElementAtOrDefault(1) ?? IndexCount;

    /// <summary>
    /// Value axis
    /// </summary>
    public virtual int ValueCount { get; set; }
    public virtual int ValueLabelCount { get; set; }
    public virtual IList<double> ValueDomain { get; set; }
    public virtual Func<dynamic, dynamic> ShowValueAction { get; set; }
    public virtual double MinValue => ValueDomain?.ElementAtOrDefault(0) ?? AutoValueDomain?.ElementAtOrDefault(0) ?? 0.0;
    public virtual double MaxValue => ValueDomain?.ElementAtOrDefault(1) ?? AutoValueDomain?.ElementAtOrDefault(1) ?? ValueCount;

    /// <summary>
    /// Internals
    /// </summary>
    public virtual IList<int> AutoIndexDomain { get; protected set; }
    public virtual IList<double> AutoValueDomain { get; protected set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public Composer()
    {
      IndexCount = 100;
      IndexLabelCount = 10;

      ValueCount = 6;
      ValueLabelCount = 3;

      Points = new List<IPointModel>();
      Shapes = new List<IShapeModel>();
    }

    /// <summary>
    /// Update delegate
    /// </summary>
    public virtual void Update()
    {
      CreateIndexDomain();
      CreateValueDomain();
      UpdatePoints();
      UpdateShapes();
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
    /// <param name="input"></param>
    public virtual IPointModel GetPixels(IEngine engine, IPointModel input)
    {
      // Convert to device pixels

      var index = Equals(MaxIndex, MinIndex) ? 1.0 : (input.Index - MinIndex) / (MaxIndex - MinIndex);
      var value = Equals(MaxValue, MinValue) ? 1.0 : (input.Value - MinValue) / (MaxValue - MinValue);

      // Percentage to pixels, Y is inverted

      input.Index = engine.IndexSize * index.Value;
      input.Value = engine.ValueSize - engine.ValueSize * value;

      return input;
    }

    /// <summary>
    /// Transform coordinates
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IPointModel GetPixels(IEngine engine, double index, double value)
    {
      return GetPixels(engine, new PointModel { Index = index, Value = value });
    }

    /// <summary>
    /// Convert canvas coordinates to values
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="input"></param>
    public virtual IPointModel GetValues(IEngine engine, IPointModel input)
    {
      // Convert to values

      var index = input.Index / engine.IndexSize;
      var value = input.Value / engine.ValueSize;

      // Percentage to values, Y is inverted

      input.Index = MinIndex + (MaxIndex - MinIndex) * index;
      input.Value = MaxValue - (MaxValue - MinValue) * value;

      return input;
    }

    /// <summary>
    /// Format index label
    /// </summary>
    public virtual string ShowIndex(dynamic input)
    {
      return ShowIndexAction is null ? $"{input:0.00}" : ShowIndexAction(input);
    }

    /// <summary>
    /// Format value label
    /// </summary>
    public virtual string ShowValue(dynamic input)
    {
      return ShowValueAction is null ? $"{input:0.00}" : ShowValueAction(input);
    }

    /// <summary>
    /// Get specific group by position and name
    /// </summary>
    /// <param name="position"></param>
    /// <param name="name"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual dynamic GetPoint(int position, string name, IList<IPointModel> items)
    {
      return items.ElementAtOrDefault(position)?.Value;
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

      ValueDomain ??= new List<double>(AutoValueDomain);

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

      IndexDomain ??= new List<int>(AutoIndexDomain);

      var increment = IndexLabelCount / 2 * delta;
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

      IndexDomain ??= new List<int>(AutoIndexDomain);

      var increment = IndexLabelCount / 2 * delta;

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
      AutoIndexDomain[1] = Math.Max(Points.Count, IndexCount);

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
        var point = Points.ElementAtOrDefault(i);

        if (point?.Value?.Point is null)
        {
          continue;
        }

        point.Value.Composer = this;
        min = Math.Min(min, point.Value.Point);
        max = Math.Max(max, point.Value.Point);
        average += max - min;
      }

      if (min > max)
      {
        return AutoValueDomain = null;
      }

      AutoValueDomain ??= new double[2];
      AutoValueDomain[0] = min;
      AutoValueDomain[1] = max;

      if (min < 0 && max > 0)
      {
        var domain = Math.Max(Math.Abs(max), Math.Abs(min));

        AutoValueDomain[0] = -domain;
        AutoValueDomain[1] = domain;
      }

      return AutoValueDomain;
    }

    /// <summary>
    /// Update series and collections
    /// </summary>
    protected virtual void UpdatePoints()
    {
      foreach (var i in GetEnumerator())
      {
        var point = Points.ElementAtOrDefault(i);

        if (point?.Value?.Point is null)
        {
          continue;
        }

        point.Engine = Engine;
        point.Composer = this;
        point.CreateShape(i, null, Points);
      }
    }

    /// <summary>
    /// Update static shapes
    /// </summary>
    protected virtual void UpdateShapes()
    {
      //foreach (var shape in Shapes)
      //{
      //  shape.Panel.Clear();
      //  shape.UpdateShape();
      //}
    }

    /// <summary>
    /// Update levels
    /// </summary>
    protected virtual void UpdateLevels(IList<int> indexLevels, IList<double> valueLevels)
    {
      //var levels = Shapes.Where(o => o is LineShapeModel);

      //foreach (LineShapeModel level in levels)
      //{
      //  level.IndexLevels = indexLevels ?? new int[0];
      //  level.ValueLevels = valueLevels ?? new double[0];
      //}

      //UpdateShapes();
    }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void Dispose()
    {
      Engine?.Dispose();
    }
  }
}
