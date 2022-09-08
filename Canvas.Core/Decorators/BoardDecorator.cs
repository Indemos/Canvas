using Canvas.Core.EngineSpace;
using Canvas.Core.EnumSpace;
using Canvas.Core.MessageSpace;
using Canvas.Core.ModelSpace;
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

      if (element is not null)
      {
        element.Composer = Composer;
        //Series = element.GetSeries(coordinates, values);
      }

      message.ValueX = Composer.ShowIndex(values.X.Value);
      message.ValueY = Composer.ShowValue(values.Y.Value);

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
      var DT = T?.Y ?? 0;
      var DB = B?.Y ?? 0;
      var DL = L?.X ?? 0;
      var DR = R?.X ?? 0;
      var shape = Composer.Board;
      var points = new IItemModel[2];

      points[0] = new ItemModel { X = DL, Y = message.Y + DT };
      points[1] = new ItemModel { X = engine.X - DR, Y = message.Y + DT };

      engine.CreateLine(points, shape);

      points[0] = new ItemModel { X = message.X + DL, Y = DT };
      points[1] = new ItemModel { X = message.X + DL, Y = engine.Y - DB };

      engine.CreateLine(points, shape);
    }

    /// <summary>
    /// Create boxes
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="message"></param>
    public virtual void CreateMarkers(IEngine engine, ViewMessage message)
    {
      var DT = T?.Y ?? 0;
      var DB = B?.Y ?? 0;
      var DL = L?.X ?? 0;
      var DR = R?.X ?? 0;
      var shape = Composer.Caption;
      var space = shape.Size;
      var DX = message.ValueX.Length * shape.Size / 2;
      var DY = shape.Size / 1.5;
      var points = new IItemModel[2]
      {
        new ItemModel(),
        new ItemModel()
      };
  
      points[0].Y = DT + message.Y - DY;
      points[1].Y = DT + message.Y + DY;

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
        points[0].Y = DT - space - shape.Size / 2 - DY;
        points[1].Y = DT - space - shape.Size / 2 + DY;

        engine.CreateBox(points, shape);
      }

      if (B is not null)
      {
        points[0].Y = engine.Y - DB + space + shape.Size / 2 - DY;
        points[1].Y = engine.Y - DB + space + shape.Size / 2 + DY;

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
      var DT = T?.Y ?? 0;
      var DB = B?.Y ?? 0;
      var DL = L?.X ?? 0;
      var DR = R?.X ?? 0;
      var shape = Composer.Caption.Clone() as ComponentModel;
      var space = shape.Size;
      var DY = shape.Size / 2;
      var point = new ItemModel();

      shape.Color = Composer.Caption.Background;

      if (L is not null)
      {
        point.X = DL - space;
        point.Y = DT + message.Y + DY;
        shape.Position = PositionEnum.R;

        engine.CreateCaption(point, shape, message.ValueY);
      }

      if (R is not null)
      {
        point.X = engine.X - DL + space;
        point.Y = DT + message.Y + DY;
        shape.Position = PositionEnum.L;

        engine.CreateCaption(point, shape, message.ValueY);
      }

      if (T is not null)
      {
        point.X = DL + message.X;
        point.Y = DT - space;
        shape.Position = PositionEnum.Center;

        engine.CreateCaption(point, shape, message.ValueX);
      }

      if (B is not null)
      {
        point.X = DL + message.X;
        point.Y = engine.Y - DB + space + +shape.Size;
        shape.Position = PositionEnum.Center;

        engine.CreateCaption(point, shape, message.ValueX);
      }
    }
  }
}
