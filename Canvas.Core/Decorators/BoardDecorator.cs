using Canvas.Core.EngineSpace;
using Canvas.Core.EnumSpace;
using Canvas.Core.MessageSpace;
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
    public virtual void Create(IEngine engine, ViewMessage message)
    {
      var coordinates = new ItemModel
      {
        X = message.X,
        Y = message.Y
      };

      var values = Composer.GetValues(Screen, coordinates);
      var element = Composer.Items.ElementAtOrDefault((int)values.X);

      message.ValueX = Composer.ShowIndex(values.X.Value);
      message.ValueY = Composer.ShowValue(values.Y.Value);

      if (element is not null)
      {
        CreateBoard(engine, message, element.GetSeries(coordinates, values));
      }

      CreateLines(engine, message);
      CreateMarkers(engine, message);
      CreateCaptions(engine, message);
    }

    /// <summary>
    /// Create lines
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="message"></param>
    public virtual void CreateLines(IEngine engine, ViewMessage message)
    {
      var shape = Composer.Line.Clone() as IComponentModel;
      var points = new IItemModel[2]
      {
        new ItemModel(),
        new ItemModel()
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
    public virtual void CreateMarkers(IEngine engine, ViewMessage message)
    {
      var shape = Composer.Caption.Clone() as IComponentModel;
      var DX = message.ValueX.Length * shape.Size / 2;
      var DY = shape.Size / 1.5;
      var points = new IItemModel[2]
      {
        new ItemModel(),
        new ItemModel()
      };

      points[0].Y = Math.Floor((DT + message.Y - DY).Value);
      points[1].Y = Math.Floor((DT + message.Y + DY).Value);

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
        points[0].Y = Math.Floor((DT - shape.Size - shape.Size / 2 - DY).Value);
        points[1].Y = Math.Floor((DT - shape.Size - shape.Size / 2 + DY).Value);

        engine.CreateBox(points, shape);
      }

      if (B is not null)
      {
        points[0].Y = Math.Floor((engine.Y - DB + shape.Size + shape.Size / 2 - DY).Value);
        points[1].Y = Math.Floor((engine.Y - DB + shape.Size + shape.Size / 2 + DY).Value);

        engine.CreateBox(points, shape);
      }
    }

    /// <summary>
    /// Create labels
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="message"></param>
    public virtual void CreateCaptions(IEngine engine, ViewMessage message)
    {
      var shape = Composer.Caption.Clone() as IComponentModel;
      var DY = shape.Size / 2;
      var point = new ItemModel();

      shape.Color = Composer.Caption.Background;

      if (L is not null)
      {
        point.X = DL - shape.Size;
        point.Y = DT + message.Y + DY;
        shape.Position = PositionEnum.R;

        engine.CreateCaption(point, shape, message.ValueY);
      }

      if (R is not null)
      {
        point.X = engine.X - DL + shape.Size;
        point.Y = DT + message.Y + DY;
        shape.Position = PositionEnum.L;

        engine.CreateCaption(point, shape, message.ValueY);
      }

      if (T is not null)
      {
        point.X = DL + message.X;
        point.Y = DT - shape.Size;
        shape.Position = PositionEnum.Center;

        engine.CreateCaption(point, shape, message.ValueX);
      }

      if (B is not null)
      {
        point.X = DL + message.X;
        point.Y = Math.Floor((engine.Y - DB + shape.Size + shape.Size).Value);
        shape.Position = PositionEnum.Center;

        engine.CreateCaption(point, shape, message.ValueX);
      }
    }

    /// <summary>
    /// Create board
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="message"></param>
    public virtual void CreateBoard(IEngine engine, ViewMessage message, IDictionary<string, IList<double>> series)
    {
      if (series is null)
      {
        return;
      }

      var shape = Composer.Board.Clone() as IComponentModel;
      var boardSize = series.Max(o => Composer.ShowBoard(o.Key, o.Value).Length) * shape.Size;
      var itemSize = shape.Size * 1.5;
      var points = new IItemModel[2]
      {
        new ItemModel(),
        new ItemModel()
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

        engine.CreateCaption(points[0], Composer.Board, row);
        points[0].Y += itemSize;
      }
    }
  }
}
