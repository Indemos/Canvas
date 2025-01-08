using Canvas.Core.Composers;
using Canvas.Core.Engines;
using Canvas.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Canvas.Core
{
  public interface IView : IDisposable
  {
    /// <summary>
    /// Indices
    /// </summary>
    IList<MarkerModel> Indices { get; set; }

    /// <summary>
    /// Values
    /// </summary>
    IList<MarkerModel> Values { get; set; }

    /// <summary>
    /// Composer
    /// </summary>
    IComposer Composer { get; }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    Task Update(DimensionModel? domain = null, string source = null);

    /// <summary>
    /// Create
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<IView> Create<T>(Func<IComposer> action) where T : IEngine, new();
  }
}
