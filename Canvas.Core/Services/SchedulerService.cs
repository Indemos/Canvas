using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace Canvas.Core.ServiceSpace
{
  public class SchedulerService : IDisposable
  {
    /// <summary>
    /// Scheduler date
    /// </summary>
    public virtual DateTimeOffset Now => DateTime.Now;

    /// <summary>
    /// Scheduler
    /// </summary>
    public virtual EventLoopScheduler Instance { get; protected set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public SchedulerService()
    {
      Instance = new EventLoopScheduler(o => new Thread(o)
      {
        IsBackground = true,
        Priority = ThreadPriority.Highest,
        Name = nameof(SchedulerService)
      });
    }

    /// <summary>
    /// Action processor
    /// </summary>
    /// <param name="action"></param>
    public virtual TaskCompletionSource<T> Send<T>(Func<T> action)
    {
      var completion = new TaskCompletionSource<T>();

      Instance.Schedule(() => completion.TrySetResult(action()));

      return completion;
    }

    /// <summary>
    /// Schedule wrapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="state"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual IDisposable Schedule<T>(T state, Func<IScheduler, T, IDisposable> action) => Instance.Schedule(state, action);

    /// <summary>
    /// Schedule wrapper
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <param name="dueTime"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action) => Instance.Schedule(state, dueTime, action);

    /// <summary>
    /// Schedule wrapper
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <param name="dueTime"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action) => Instance.Schedule(state, dueTime, action);

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
      Instance?.Dispose();
    }
  }
}
