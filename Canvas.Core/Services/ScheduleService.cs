using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Canvas.Core.Services
{
  public class ScheduleService : IDisposable
  {
    protected virtual CancellationTokenSource Cancellation { get; set; }
    protected virtual BlockingCollection<(Action, TaskCompletionSource)> Queue { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public ScheduleService()
    {
      Queue = new();
      Cancellation = new CancellationTokenSource();

      Task.Factory.StartNew(() =>
      {
        foreach (var (o, com) in Queue.GetConsumingEnumerable())
        {
          o();
          com.TrySetResult();
        }
      },
      Cancellation.Token,
      TaskCreationOptions.LongRunning,
      TaskScheduler.Current).ContinueWith(o => Queue.Dispose());
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public virtual void Dispose() => Cancellation?.Cancel();

    /// <summary>
    /// Schedule
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual Task Schedule(Action action)
    {
      var completion = new TaskCompletionSource();

      if (Queue.Count > 1)
      {
        Queue.TryTake(out var o);
      }

      Queue.TryAdd((action, completion));

      return completion.Task;
    }
  }
}
