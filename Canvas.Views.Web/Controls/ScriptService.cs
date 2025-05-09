using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Canvas.Controls
{
  /// <summary>
  /// Singleton service
  /// </summary>
  public class ScriptService : IDisposable
  {
    /// <summary>
    /// Script runtime
    /// </summary>
    private IJSRuntime runtime;

    /// <summary>
    /// Script reference
    /// </summary>
    private IJSObjectReference scriptModule;

    /// <summary>
    /// Script instance 
    /// </summary>
    private IJSObjectReference scriptInstance;

    /// <summary>
    /// Service instance 
    /// </summary>
    private DotNetObjectReference<ScriptService> serviceInstance;

    /// <summary>
    /// On size event
    /// </summary>
    public Dictionary<string, Action<dynamic>> Actions { get; set; } = new();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="runtime"></param>
    public ScriptService(IJSRuntime runtime) => this.runtime = runtime;

    /// <summary>
    /// Get document bounds
    /// </summary>
    /// <returns></returns>
    public async Task<ScriptMessage?> GetDocBounds()
    {
      try
      {
        if (scriptInstance is not null)
        {
          return await scriptInstance.InvokeAsync<ScriptMessage>("getDocBounds");
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }

      return null;
    }

    /// <summary>
    /// Get element bounds
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public async Task<ScriptMessage?> GetElementBounds(ElementReference element)
    {
      try
      {
        if (scriptInstance is not null)
        {
          return await scriptInstance.InvokeAsync<ScriptMessage>("getElementBounds", element);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }

      return null;
    }

    /// <summary>
    /// Subscribe to custom events
    /// </summary>
    /// <param name="element"></param>
    /// <param name="eventName"></param>
    /// <param name="actionName"></param>
    /// <returns></returns>
    public async Task<string> Subscribe(ElementReference element, string eventName, string actionName)
    {
      try
      {
        if (scriptInstance is not null)
        {
          return await scriptInstance.InvokeAsync<string>("subscribe", element, eventName, actionName);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }

      return null;
    }

    /// <summary>
    /// Subscribe to size changes
    /// </summary>
    /// <param name="element"></param>
    /// <param name="actionName"></param>
    /// <returns></returns>
    public async Task<string> SubscribeToSize(ElementReference element, string actionName)
    {
      try
      {
        if (scriptInstance is not null)
        {
          return await scriptInstance.InvokeAsync<string>("subscribeToSize", element, actionName);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }

      return null;
    }

    /// <summary>
    /// Setup script proxy under specified namespace
    /// </summary>
    /// <returns></returns>
    public async Task<ScriptService> CreateModule(IDictionary<string, dynamic> options = null)
    {
      try
      {
        await DisposeAsync();

        serviceInstance = DotNetObjectReference.Create(this);
        scriptModule = await runtime.InvokeAsync<IJSObjectReference>("import", "./_content/Canvas.Views.Web/Controls/ScriptControl.razor.js");
        scriptInstance = await scriptModule.InvokeAsync<IJSObjectReference>("getScriptModule", serviceInstance, options ?? new Dictionary<string, dynamic>());
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }

      return this;
    }

    /// <summary>
    /// Script proxy
    /// </summary>
    /// <param name="message"></param>
    /// <param name="actionName"></param>
    /// <returns></returns>
    [JSInvokable]
    public void OnChange(dynamic message, string actionName)
    {
      try
      {
        if (Actions.TryGetValue(actionName, out var action))
        {
          action(message);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <returns></returns>
    public virtual void Dispose() => DisposeAsync();

    /// <summary>
    /// Dispose
    /// </summary>
    /// <returns></returns>
    public virtual Task DisposeAsync()
    {
      try
      {
        Actions?.Clear();

        serviceInstance?.Dispose();

        var response = Task.WhenAll([
          scriptModule is null ? Task.CompletedTask : scriptModule.DisposeAsync().AsTask(),
        scriptInstance is null ? Task.CompletedTask : scriptInstance.DisposeAsync().AsTask()
        ]);

        scriptModule = null;
        scriptInstance = null;
        serviceInstance = null;

        return response;
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }

      return Task.CompletedTask;
    }
  }
}
