function ScriptModule(instance, options) {

  const sizeObserverName = "Size";

  this.eventMap = {};
  this.observerMap = {};
  this.serviceInstance = instance;

  /// <summary>
  /// Get window size
  /// </summary>
  this.getDocBounds = () => this.getElementBounds(document.body);

  /// <summary>
  /// Get element size
  /// </summary>
  /// <param name="element"></param>
  this.getElementBounds = element => {
    const bounds = element.getBoundingClientRect();
    const X = 
      element.clientWidth || 
      element.scrollWidth || 
      element.offsetWidth || 
      bounds.width || 
      0.0;
    const Y = 
      element.clientHeight || 
      element.scrollHeight || 
      element.offsetHeight || 
      bounds.height ||
      0.0;
    return { X, Y };
  };

  /// <summary>
  /// Subscribe to custom event
  /// </summary>
  /// <param name="element"></param>
  /// <param name="eventName"></param>
  /// <param name="actionName"></param>
  this.subscribe = (element, eventName, actionName) => {
    let scheduler = null;
    let done = e => {
      clearTimeout(scheduler);
      scheduler = setTimeout(() => {
        this.serviceInstance && this
          .serviceInstance
          .invokeMethodAsync("OnChange", e, actionName)
          .catch(o => this.unsubscribe(actionName));
      }, options.interval || 100);
    };
    element.addEventListener(eventName, done, false);
    this.eventMap[actionName] = { element, eventName, done };
    return actionName;
  };

  /// <summary>
  /// Unsubscribe from custom event
  /// </summary>
  /// <param name="actionName"></param>
  this.unsubscribe = actionName => {
    const o = this.eventMap[actionName];
    o && o.element.removeEventListener(o.event, o.done);
    this.eventMap[actionName] = null;
  };

  /// <summary>
  /// Subscribe to element resize
  /// </summary>
  /// <param name="element"></param>
  /// <param name="actionName"></param>
  this.subscribeToSize = (element, actionName) => {
    let scheduler = null;
    let done = e => {
      clearTimeout(scheduler);
      scheduler = setTimeout(() => {
        this.serviceInstance && this
          .serviceInstance
          .invokeMethodAsync("OnChange", e, actionName)
          .catch(o => this.unsubscribeFromSize(actionName));
      }, options.interval || 100);
    };
    this.observerMap[actionName] = new ResizeObserver(done);
    this.observerMap[actionName].observe(element);
    return actionName;
  };

  /// <summary>
  /// Unsubscribe from size observer
  /// </summary>
  /// <param name="actionName"></param>
  this.unsubscribeFromSize = actionName => {
    const o = this.observerMap[actionName];
    o && o.disconnect();
    this.observerMap[actionName] = null;
  };
};

export function getScriptModule(instance, options) {
  return new ScriptModule(instance, options);
};
