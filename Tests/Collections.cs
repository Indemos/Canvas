using Canvas.Core.Shapes;
using Distribution.Collections;

namespace Tests
{
  public class Collections
  {
    private GroupShape GetSingleSample()
    {
      static GroupShape GetGroup() => new GroupShape { Groups = new Dictionary<string, IShape>() };

      var group = GetGroup();

      group.Groups["Assets"] = GetGroup();
      group.Groups["Assets"].Groups["Prices"] = new CandleShape();

      return group;
    }

    private IShape GetSample()
    {
      static IShape GetGroup() => new GroupShape { Groups = new Dictionary<string, IShape>() };

      var group = GetGroup();

      group.Groups["Assets"] = GetGroup();
      group.Groups["Assets"].Groups["Prices"] = new CandleShape();
      group.Groups["Assets"].Groups["Arrows"] = new ArrowShape();

      group.Groups["Indicators"] = GetGroup();
      group.Groups["Indicators"].Groups["Bars"] = new BarShape();

      group.Groups["Lines"] = GetGroup();
      group.Groups["Lines"].Groups["X"] = new LineShape();
      group.Groups["Lines"].Groups["Y"] = new LineShape();

      group.Groups["Performance"] = GetGroup();
      group.Groups["Performance"].Groups["Balance"] = new AreaShape();

      return group;
    }

    private ObservableGroupCollection<IShape> Steps(KeyValuePair<DateTime, double>[] steps)
    {
      var collection = new ObservableGroupCollection<IShape>();

      if (steps.Length > 0)
      {
        var update = GetSample();
        var stamp = steps[0].Key;
        var price = steps[0].Value;

        update.X = stamp.Ticks;
        update.Groups["Lines"].Groups["X"] = new LineShape { Y = price + 1 };
        update.Groups["Lines"].Groups["Y"] = new LineShape { Y = price + 2 };
        update.Groups["Indicators"].Groups["Bars"] = new BarShape { Y = price + 3 };
        update.Groups["Performance"].Groups["Balance"] = new AreaShape { Y = price + 4 };
        update.Groups["Assets"].Groups["Arrows"] = new ArrowShape { Y = price + 5 };
        update.Groups["Assets"].Groups["Prices"] = new CandleShape { Y = price + 6 };

        collection.Add(update, true);
      }

      if (steps.Length > 1)
      {
        var update = GetSample();
        var stamp = steps[1].Key;
        var price = steps[1].Value;

        update.X = stamp.Ticks;
        update.Groups["Lines"].Groups["X"] = new LineShape { Y = price + 1 };
        update.Groups["Lines"].Groups["Y"] = new LineShape { Y = price + 2 };
        update.Groups["Indicators"].Groups["Bars"] = new BarShape { Y = price + 3 };
        update.Groups["Performance"].Groups["Balance"] = new AreaShape { Y = price + 4 };
        update.Groups["Assets"].Groups["Arrows"] = new ArrowShape { Y = price + 5 };
        update.Groups["Assets"].Groups["Prices"] = new CandleShape { Y = price + 6 };

        collection.Add(update, true);
      }

      if (steps.Length > 2)
      {
        var update = GetSample();
        var stamp = steps[2].Key;
        var price = steps[2].Value;

        update.X = stamp.Ticks;
        update.Groups["Lines"].Groups["X"] = new LineShape { Y = price + 1 };
        update.Groups["Lines"].Groups["Y"] = new LineShape { Y = price + 2 };
        update.Groups["Indicators"].Groups["Bars"] = new BarShape { Y = price + 3 };
        update.Groups["Performance"].Groups["Balance"] = new AreaShape { Y = price + 4 };
        update.Groups["Assets"].Groups["Arrows"] = new ArrowShape { Y = price + 5 };
        update.Groups["Assets"].Groups["Prices"] = new CandleShape { Y = price + 6 };

        collection.Add(update, true);
      }

      return collection;
    }

    [Fact]
    public void Single()
    {
      var collection = new ObservableGroupCollection<IShape>();
      var sample = GetSingleSample();
      var price = 5.5;

      sample.Groups["Assets"].Groups["Prices"] = new CandleShape { Y = price };

      collection.Add(sample);

      var group = collection.Last();
      var shape = group.Groups["Assets"].Groups["Prices"] as CandleShape;

      Assert.Single(collection);
      Assert.Single(sample.Groups);
      Assert.Single(sample.Groups["Assets"].Groups);
      Assert.Equal(price, shape.Y);
      Assert.Null(shape.X);
      Assert.Null(shape.O);
      Assert.Null(shape.H);
      Assert.Null(shape.L);
      Assert.Null(shape.C);
    }

    [Fact]
    public void CombineSingle()
    {
      var price = 5.5;
      var stamp = DateTime.Now;
      var collection = Steps([KeyValuePair.Create(stamp, price)]);
      var group = collection.Last();

      Assert.Single(collection);

      Assert.Equal(4, group.Groups.Count);
      Assert.Equal(2, group.Groups["Lines"].Groups.Count);
      Assert.Equal(2, group.Groups["Assets"].Groups.Count);
      Assert.Single(group.Groups["Indicators"].Groups);
      Assert.Single(group.Groups["Performance"].Groups);

      Assert.Equal(group.X, stamp.Ticks);
      Assert.Equal(group.Groups["Lines"].Groups["X"].Y, price + 1);
      Assert.Equal(group.Groups["Lines"].Groups["Y"].Y, price + 2);
      Assert.Equal(group.Groups["Indicators"].Groups["Bars"].Y, price + 3);
      Assert.Equal(group.Groups["Performance"].Groups["Balance"].Y, price + 4);
      Assert.Equal(group.Groups["Assets"].Groups["Arrows"].Y, price + 5);
      Assert.Equal(group.Groups["Assets"].Groups["Prices"].Y, price + 6);

      var candle = group.Groups["Assets"].Groups["Prices"] as CandleShape;

      Assert.Equal(candle.O, price + 6);
      Assert.Equal(candle.H, price + 6);
      Assert.Equal(candle.L, price + 6);
      Assert.Equal(candle.C, price + 6);
    }

    [Fact]
    public void CombineTwo()
    {
      var price = 5.5;
      var priceUpdate = 10.5;
      var stamp = DateTime.Now;
      var collection = Steps([KeyValuePair.Create(stamp, price), KeyValuePair.Create(stamp, priceUpdate)]);
      var group = collection.Last();

      Assert.Single(collection);

      Assert.Equal(4, group.Groups.Count);
      Assert.Equal(2, group.Groups["Lines"].Groups.Count);
      Assert.Equal(2, group.Groups["Assets"].Groups.Count);
      Assert.Single(group.Groups["Indicators"].Groups);
      Assert.Single(group.Groups["Performance"].Groups);

      Assert.Equal(group.X, stamp.Ticks);
      Assert.Equal(group.Groups["Lines"].Groups["X"].Y, priceUpdate + 1);
      Assert.Equal(group.Groups["Lines"].Groups["Y"].Y, priceUpdate + 2);
      Assert.Equal(group.Groups["Indicators"].Groups["Bars"].Y, priceUpdate + 3);
      Assert.Equal(group.Groups["Performance"].Groups["Balance"].Y, priceUpdate + 4);
      Assert.Equal(group.Groups["Assets"].Groups["Arrows"].Y, priceUpdate + 5);
      Assert.Equal(group.Groups["Assets"].Groups["Prices"].Y, priceUpdate + 6);

      var updateCandle = group.Groups["Assets"].Groups["Prices"] as CandleShape;

      Assert.Equal(updateCandle.O, price + 6);
      Assert.Equal(updateCandle.H, priceUpdate + 6);
      Assert.Equal(updateCandle.L, price + 6);
      Assert.Equal(updateCandle.C, priceUpdate + 6);
    }

    [Fact]
    public void PostCombine()
    {
      var price = 5.5;
      var priceUpdate = 10.5;
      var priceSample = 20.5;
      var stamp = DateTime.Now;
      var collection = Steps([
        KeyValuePair.Create(stamp, price),
        KeyValuePair.Create(stamp, priceUpdate),
        KeyValuePair.Create(stamp.AddMinutes(1), priceSample)
      ]);
      var group = collection.First();
      var sample = collection.Last();

      Assert.Equal(2, collection.Count);

      Assert.Equal(4, group.Groups.Count);
      Assert.Equal(2, group.Groups["Lines"].Groups.Count);
      Assert.Equal(2, group.Groups["Assets"].Groups.Count);
      Assert.Single(group.Groups["Indicators"].Groups);
      Assert.Single(group.Groups["Performance"].Groups);

      Assert.Equal(group.X, stamp.Ticks);
      Assert.Equal(group.Groups["Lines"].Groups["X"].Y, priceUpdate + 1);
      Assert.Equal(group.Groups["Lines"].Groups["Y"].Y, priceUpdate + 2);
      Assert.Equal(group.Groups["Indicators"].Groups["Bars"].Y, priceUpdate + 3);
      Assert.Equal(group.Groups["Performance"].Groups["Balance"].Y, priceUpdate + 4);
      Assert.Equal(group.Groups["Assets"].Groups["Arrows"].Y, priceUpdate + 5);
      Assert.Equal(group.Groups["Assets"].Groups["Prices"].Y, priceUpdate + 6);

      var updateCandle = group.Groups["Assets"].Groups["Prices"] as CandleShape;

      Assert.Equal(updateCandle.O, price + 6);
      Assert.Equal(updateCandle.H, priceUpdate + 6);
      Assert.Equal(updateCandle.L, price + 6);
      Assert.Equal(updateCandle.C, priceUpdate + 6);

      Assert.Equal(sample.X, stamp.AddMinutes(1).Ticks);
      Assert.Equal(sample.Groups["Lines"].Groups["X"].Y, priceSample + 1);
      Assert.Equal(sample.Groups["Lines"].Groups["Y"].Y, priceSample + 2);
      Assert.Equal(sample.Groups["Indicators"].Groups["Bars"].Y, priceSample + 3);
      Assert.Equal(sample.Groups["Performance"].Groups["Balance"].Y, priceSample + 4);
      Assert.Equal(sample.Groups["Assets"].Groups["Arrows"].Y, priceSample + 5);
      Assert.Equal(sample.Groups["Assets"].Groups["Prices"].Y, priceSample + 6);

      var sampleCandle = sample.Groups["Assets"].Groups["Prices"] as CandleShape;

      Assert.Equal(sampleCandle.O, priceSample + 6);
      Assert.Equal(sampleCandle.H, priceSample + 6);
      Assert.Equal(sampleCandle.L, priceSample + 6);
      Assert.Equal(sampleCandle.C, priceSample + 6);
    }

    [Fact]
    public void PartialUpdate()
    {
      var price = 5.5;
      var priceUpdate = 10.5;
      var stamp = DateTime.Now;
      var collection = Steps([KeyValuePair.Create(stamp, price), KeyValuePair.Create(stamp, priceUpdate)]);
      var update = GetSample();
      var sourceCandle = new CandleShape { O = 5, H = 20, L = 5, C = 10 };

      update.X = stamp.Ticks;
      update.Groups["Assets"].Groups["Prices"] = sourceCandle;

      collection.Add(update, true);

      var group = collection.Last();

      Assert.Single(collection);

      Assert.Equal(4, group.Groups.Count);
      Assert.Equal(2, group.Groups["Lines"].Groups.Count);
      Assert.Equal(2, group.Groups["Assets"].Groups.Count);
      Assert.Single(group.Groups["Indicators"].Groups);
      Assert.Single(group.Groups["Performance"].Groups);

      Assert.Equal(group.X, stamp.Ticks);
      Assert.Equal(group.Groups["Lines"].Groups["X"].Y, priceUpdate + 1);
      Assert.Equal(group.Groups["Lines"].Groups["Y"].Y, priceUpdate + 2);
      Assert.Equal(group.Groups["Indicators"].Groups["Bars"].Y, priceUpdate + 3);
      Assert.Equal(group.Groups["Performance"].Groups["Balance"].Y, priceUpdate + 4);
      Assert.Equal(group.Groups["Assets"].Groups["Arrows"].Y, priceUpdate + 5);
      Assert.Equal(group.Groups["Assets"].Groups["Prices"].Y, priceUpdate + 6);

      var updateCandle = group.Groups["Assets"].Groups["Prices"] as CandleShape;

      Assert.Equal(updateCandle.O, price + 6);
      Assert.Equal(updateCandle.H, sourceCandle.H);
      Assert.Equal(updateCandle.L, sourceCandle.L);
      Assert.Equal(updateCandle.C, sourceCandle.C);
    }

    [Fact]
    public void ImmutableUpdate()
    {
      var price = 5.5;
      var priceUpdate = 10.5;
      var stamp = DateTime.Now;
      var collection = Steps([KeyValuePair.Create(stamp.AddMinutes(-10), price), KeyValuePair.Create(stamp, priceUpdate)]);
      var update = GetSample();
      var sourceCandle = new CandleShape { O = 5, H = 20, L = 5, C = 10, Y = 55 };
      var immutableCandle = collection.First().Groups["Assets"].Groups["Prices"].Clone() as CandleShape;

      update.X = stamp.Ticks;
      update.Groups["Assets"].Groups["Prices"] = sourceCandle;

      collection.Add(update, true);

      var group = collection.First();

      Assert.Equal(2, collection.Count);

      Assert.Equal(4, group.Groups.Count);
      Assert.Equal(2, group.Groups["Lines"].Groups.Count);
      Assert.Equal(2, group.Groups["Assets"].Groups.Count);
      Assert.Single(group.Groups["Indicators"].Groups);
      Assert.Single(group.Groups["Performance"].Groups);

      Assert.Equal(group.X, stamp.AddMinutes(-10).Ticks);
      Assert.Equal(group.Groups["Lines"].Groups["X"].Y, price + 1);
      Assert.Equal(group.Groups["Lines"].Groups["Y"].Y, price + 2);
      Assert.Equal(group.Groups["Indicators"].Groups["Bars"].Y, price + 3);
      Assert.Equal(group.Groups["Performance"].Groups["Balance"].Y, price + 4);
      Assert.Equal(group.Groups["Assets"].Groups["Arrows"].Y, price + 5);
      Assert.Equal(group.Groups["Assets"].Groups["Prices"].Y, price + 6);

      var updateCandle = group.Groups["Assets"].Groups["Prices"] as CandleShape;

      Assert.Equal(updateCandle.O, price + 6);
      Assert.Equal(updateCandle.H, immutableCandle.H);
      Assert.Equal(updateCandle.L, immutableCandle.L);
      Assert.Equal(updateCandle.C, immutableCandle.C);
    }
  }
}
