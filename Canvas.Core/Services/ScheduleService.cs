using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Canvas.Core.Services
{
  public class ScheduleService : IDisposable
  {
    protected virtual Task Consumer { get; set; }
    protected virtual BlockingCollection<(Action, TaskCompletionSource)> Queue { get; set; }
    protected virtual CancellationTokenSource Cancellation { get; set; }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose()
    {
      Queue?.Dispose();
      Cancellation?.Cancel();
    }

    /// <summary>
    /// Schedule
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual Task Schedule(Action action)
    {
      Queue ??= new();
      Cancellation ??= new CancellationTokenSource();
      Consumer ??= Task.Factory.StartNew(() =>
      {
        foreach (var (o, com) in Queue.GetConsumingEnumerable())
        {
          o();
          com.TrySetResult();
        }
      },
      Cancellation.Token,
      TaskCreationOptions.LongRunning,
      TaskScheduler.Current).ContinueWith(o => Consumer.Dispose());

      if (Queue.Count > 1)
      {
        Queue.TryTake(out var o);
      }

      var completion = new TaskCompletionSource();

      Queue.TryAdd((action, completion));

      return completion.Task;
    }
  }
}
