using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Canvas.Core.ModelSpace
{
  public interface IModel : IDynamicMetaObjectProvider, ICloneable, IDisposable
  {
  }

  /// <summary>
  /// Expando class that allows to extend other models in runtime
  /// </summary>
  public class Model : DynamicObject, IModel
  {
    /// <summary>
    /// Disposable resources
    /// </summary>
    protected IList<IDisposable> _disposables = new List<IDisposable>();

    /// <summary>
    /// Internal dictionary to keep dynamic properties
    /// </summary>
    protected ConcurrentDictionary<string, dynamic> _items = new ConcurrentDictionary<string, dynamic>();

    /// <summary>
    /// Redirect setter for dynamic properties to internal dictionary
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      _items[binder.Name] = value;
      return true;
    }

    /// <summary>
    /// Redirect getter for dynamic properties to internal dictionary
    /// </summary>
    /// <param name="binder"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public override bool TryGetMember(GetMemberBinder binder, out object result) => _items.TryGetValue(binder.Name, out result) || true;

    /// <summary>
    /// Get all properties for serialization
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<string> GetDynamicMemberNames() => GetType()
      .GetProperties()
      .Select(o => o.Name)
      .Concat(_items.Keys);

    /// <summary>
    /// Implement indexer for internal dictionary
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual object this[object index]
    {
      get => _items.TryGetValue(Convert.ToString(index), out object result) ? result : null;
      set => _items[Convert.ToString(index)] = value;
    }

    /// <summary>
    /// Clone
    /// </summary>
    /// <returns></returns>
    public virtual object Clone() => MemberwiseClone();

    /// <summary>
    /// Dispose implementation
    /// </summary>
    public virtual void Dispose()
    {
      foreach (var disposable in _disposables)
      {
        disposable.Dispose();
      }

      _disposables.Clear();
    }
  }
}
