using Canvas.Core.EngineSpace;
using Canvas.Core.EnumSpace;
using Canvas.Core.ModelSpace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canvas.Core.DecoratorSpace
{
  public class BoardDecorator : BaseDecorator, IDecorator
  {
    public IEngine T { get; set; }
    public IEngine B { get; set; }
    public IEngine L { get; set; }
    public IEngine R { get; set; }
    public IEngine Screen { get; set; }

    public double DT => T?.Y ?? 0;
    public double DB => B?.Y ?? 0;
    public double DL => L?.X ?? 0;
    public double DR => R?.X ?? 0;

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="message"></param>
    public virtual void Create(IEngine engine, DataModel message)
    {
      var values = Composer.GetValues(Screen, message);
      var element = Composer.Items.ElementAtOrDefault((int)values.X);

      if (element is not null)
      {
        CreateBoard(engine, message, element.GetSeries(message, values));
      }

      CreateLines(engine, message);
      CreateMarkers(engine, message, values);
      CreateCaptions(engine, message, values);
    }

    /// <summary>
    /// Create lines
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="message"></param>
    public virtual void CreateLines(IEngine engine, DataModel message)
    {
      var shape = Composer.Options[ComponentEnum.BoardLine];
      var points = new DataModel[2]
      {
        new DataModel(),
        new DataModel()
      };

      points[0].X = DL;
      points[0].Y = message.Y + DT;
      points[1].X = engine.X - DR;
      points[1].Y = message.Y + DT;

      engine.CreateLine(points, shape);

      points[0].X = message.X + DL;
      points[0].Y = DT;
      points[1].X = message.X + DL;
      points[1].Y = engine.Y - DB;

      engine.CreateLine(points, shape);
    }

    /// <summary>
    /// Create side markers
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="message"></param>
    /// <param name="values"></param>
    public virtual void CreateMarkers(IEngine engine, DataModel message, DataModel values)
    {
      var shape = Composer.Options[ComponentEnum.BoardMarker];
      var index = Composer.ShowIndex(values.X);
      var DX = index.Length * shape.Size / 2;
      var DY = shape.Size / 1.5;
      var points = new DataModel[2]
      {
        new DataModel(),
        new DataModel()
      };

      points[0].Y = Math.Floor(DT + message.Y - DY);
      points[1].Y = Math.Floor(DT + message.Y + DY);

      if (L is not null)
      {
        points[0].X = 0;
        points[1].X = DL;

        engine.CreateBox(points, shape);
      }

      if (R is not null)
      {
        points[0].X = engine.X - DL;
        points[1].X = engine.X;

        engine.CreateBox(points, shape);
      }

      points[0].X = DL + message.X - DX;
      points[1].X = DL + message.X + DX;

      if (T is not null)
      {
        points[0].Y = Math.Floor(DT - shape.Size - shape.Size / 2 - DY);
        points[1].Y = Math.Floor(DT - shape.Size - shape.Size / 2 + DY);

        engine.CreateBox(points, shape);
      }

      if (B is not null)
      {
        points[0].Y = Math.Floor(engine.Y - DB + shape.Size + shape.Size / 2 - DY);
        points[1].Y = Math.Floor(engine.Y - DB + shape.Size + shape.Size / 2 + DY);

        engine.CreateBox(points, shape);
      }
    }

    /// <summary>
    /// Create labels
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="message"></param>
    /// <param name="values"></param>
    public virtual void CreateCaptions(IEngine engine, DataModel message, DataModel values)
    {
      var shape = Composer.Options[ComponentEnum.BoardCaption];
      var point = new DataModel();
      var index = Composer.ShowIndex(values.X);
      var value = Composer.ShowValue(values.Y);
      var DY = shape.Size / 2;

      shape.Color = shape.Background;

      if (L is not null)
      {
        point.X = DL - shape.Size;
        point.Y = DT + message.Y + DY;
        shape.Position = PositionEnum.R;

        engine.CreateCaption(point, shape, value);
      }

      if (R is not null)
      {
        point.X = engine.X - DL + shape.Size;
        point.Y = DT + message.Y + DY;
        shape.Position = PositionEnum.L;

        engine.CreateCaption(point, shape, value);
      }

      if (T is not null)
      {
        point.X = DL + message.X;
        point.Y = DT - shape.Size;
        shape.Position = PositionEnum.Center;

        engine.CreateCaption(point, shape, index);
      }

      if (B is not null)
      {
        point.X = DL + message.X;
        point.Y = Math.Floor(engine.Y - DB + shape.Size * 2);
        shape.Position = PositionEnum.Center;

        engine.CreateCaption(point, shape, index);
      }
    }

    /// <summary>
    /// Create board
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="message"></param>
    public virtual void CreateBoard(IEngine engine, DataModel message, IDictionary<string, IList<double>> series)
    {
      if (series is null)
      {
        return;
      }

      var shape = Composer.Options[ComponentEnum.Board];
      var shapeCaption = Composer.Options[ComponentEnum.Board];
      var boardSize = series.Max(o => Composer.ShowBoard(o.Key, o.Value).Length) * shape.Size;
      var itemSize = shape.Size * 1.5;
      var points = new DataModel[2]
      {
        new DataModel(),
        new DataModel()
      };

      // Border

      points[0].X = DL + shape.Size;
      points[0].Y = DT + shape.Size;
      points[1].X = points[0].X + boardSize;
      points[1].Y = points[0].Y + series.Count * itemSize + shape.Size * 0.5;

      engine.CreateBox(points, shape);

      // Box

      shape.Color = shape.Background;

      points[0].X += 1;
      points[0].Y += 1;
      points[1].X -= 1;
      points[1].Y -= 1;

      engine.CreateBox(points, shape);

      // Board

      points[0].X = DL + shape.Size * 2;
      points[0].Y = DT + shape.Size * 2.5;

      foreach (var group in series)
      {
        var row = Composer.ShowBoard(group.Key, group.Value);

        engine.CreateCaption(points[0], shapeCaption, row);
        points[0].Y += itemSize;
      }
    }
  }
}
