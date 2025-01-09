using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HelvestSL
{
    public enum AddMode
    {
        AddOrNot,
        AddOrReplace,
        AddOrDestroy,
        AddOrDisable,
        AddOrSetActiveFalse
    }

    /// <summary>
    /// Service Locator Class
    /// </summary>
    public class ServiceLocator
    {
        #region Fields

        public ServiceLocator parentSL;

        public readonly Dictionary<Type, object> serviceDict = new Dictionary<Type, object>();

        public readonly Dictionary<Type, Callback> callbackDict = new Dictionary<Type, Callback>();

        #endregion Variables

        #region Constructor

        public ServiceLocator()
        {
        }

        public ServiceLocator(ServiceLocator parentSL)
        {
            if (parentSL != this)
            {
                this.parentSL = parentSL;
            }
        }

        #endregion Constructor

        #region Add

        private void _Add<T>(Type type, T instance) where T : MonoBehaviour
        {
            serviceDict.Add(type, instance);
            instance.gameObject.SetActive(true);
            _InvokeCallbacks(type, instance);
        }

        /// <summary>
        /// Add the component instance to the service dictionary
        /// </summary>
        /// <typeparam name="T">Type of your class</typeparam>
        /// <param name="instance">Instance of your class</param>
        /// <param name="addMode">Mode of how you want to add your service</param>
        /// <returns>False if a other instance of same type is already in the dictionary</returns>
        public bool Add<T>(T instance, AddMode addMode) where T : MonoBehaviour
        {
            switch (addMode)
            {
                default:
                case AddMode.AddOrNot:
                    return Add(instance);
                case AddMode.AddOrReplace:
                    return AddOrReplace(instance);
                case AddMode.AddOrDestroy:
                    return AddOrDestroy(instance);
                case AddMode.AddOrDisable:
                    return AddOrDisable(instance);
                case AddMode.AddOrSetActiveFalse:
                    return AddOrSetActiveFalse(instance);
            }
        }

        /// <summary>
        /// Add the component instance to the service dictionary
        /// </summary>
        /// <typeparam name="T">Type of your class</typeparam>
        /// <param name="instance">Instance of your class</param>
        /// <returns>False if a other instance of same type is already in the dictionary</returns>
        public bool Add<T>(T instance) where T : MonoBehaviour
        {
            var type = instance.GetType();

            if (!ContainsType(type))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("Add: add", instance);
#endif
                _Add(type, instance);

                return true;
            }
            else
            {
                bool containValue = ContainsValue(instance);

#if UNITY_EDITOR || DEVELOPMENT_BUILD

                if (containValue)
                {
                    DebugLog("Add: don't add, same instance already in", instance);
                }
                else
                {
                    DebugLog("Add: don't add, same type already in", instance);
                }
#endif

                return containValue;
            }
        }

        /// <summary>
        /// Add or replace the component instance to the local service dictionary
        /// </summary>
        /// <typeparam name="T">Type of your class</typeparam>
        /// <param name="instance">Instance of your class</param>
        /// <returns>False if a other instance of same type is already in the dictionary</returns>
        public bool AddOrReplace<T>(T instance) where T : MonoBehaviour
        {
            var type = instance.GetType();

            if (!serviceDict.ContainsKey(type))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("AddOrReplace: add", instance);
#endif

                _Add(type, instance);

                return true;
            }
            else
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("AddOrReplace: replace", instance);
#endif
                Remove<T>();

                serviceDict[type] = instance;

                return false;
            }
        }

        /// <summary>
        /// Add the component instance to the service dictionary or destroy is GameObject
        /// </summary>
        /// <typeparam name="T">Type of your class</typeparam>
        /// <param name="instance">Instance of your class</param>
        /// <returns>False if a other instance of same type is already in the dictionary</returns>
        public bool AddOrDestroy<T>(T instance) where T : MonoBehaviour
        {
            var type = instance.GetType();

            if (!ContainsType(type))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("AddOrDestroy: add", instance);
#endif
                _Add(type, instance);

                return true;
            }
            else if (ContainsValue(instance))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("AddOrDestroy: already in, don't destroy", instance);
#endif

                return true;
            }
            else
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("AddOrDestroy: destroy", instance);
#endif

                Object.Destroy(instance.gameObject);

                return false;
            }
        }

        public bool AddOrDisable<T>(T instance) where T : MonoBehaviour
        {
            var type = instance.GetType();

            if (!ContainsType(type))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("AddOrDisable: add", instance);
#endif
                _Add(type, instance);

                return true;
            }
            else if (ContainsValue(instance))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("AddOrDisable: already in, don't disable", instance);
#endif

                return true;
            }
            else
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("AddOrDisable: disable", instance);
#endif

                instance.enabled = false;

                return false;
            }
        }

        public bool AddOrSetActiveFalse<T>(T instance) where T : MonoBehaviour
        {
            var type = instance.GetType();

            if (!ContainsType(type))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("AddOrSetActiveFalse: add", instance);
#endif
                _Add(type, instance);

                return true;
            }
            else if (ContainsValue(instance))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("AddOrSetActiveFalse: already in, don't disable", instance);
#endif

                return true;
            }
            else
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("AddOrSetActiveFalse: disable", instance);
#endif

                instance.gameObject.SetActive(false);

                return false;
            }
        }

        #endregion Add

        #region Remove

        /// <summary>
        /// Remove all the component's instances of type T from the service dictionary
        /// </summary>
        /// <typeparam name="T">Type of your class</typeparam>
        /// <returns>Return true if the type was in the dictionary</returns>
        public bool Remove<T>() where T : class
        {
            return Remove(typeof(T));
        }

        /// <summary>
        /// Remove all the component's instances of this type from the service dictionary
        /// </summary>
        /// <param name="type">Type to remove</param>
        /// <returns>Return true if the type was in the dictionary</returns>
        public bool Remove(Type type)
        {
            if (!serviceDict.TryGetValue(type, out object instance))
            {
                return false;
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            DebugLog("Remove: ", type);
#endif

            serviceDict.Remove(type);

            var typeToRemove = new List<Type>();

            foreach (var pair in serviceDict)
            {
                //Remove only same instance
                if (pair.Value == instance)
                {
                    typeToRemove.Add(pair.Key);
                }
            }

            foreach (var key in typeToRemove)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("Remove: ", key, type);
#endif

                serviceDict.Remove(key);
            }

            return true;
        }

        /// <summary>
        /// Remove all the component's instances of type T from the service dictionary
        /// </summary>
        /// <typeparam name="T">Type of your class</typeparam>
        /// <param name="instance">Instance of your class</param>
        /// <returns>Return true if the instance was in the dictionary</returns>
        public bool Remove<T>(T instance) where T : MonoBehaviour
        {
            if (!serviceDict.ContainsValue(instance))
            {
                return false;
            }

            var typeToRemove = new List<Type>();

            foreach (var pair in serviceDict)
            {
                if (pair.Value == instance)
                {
                    typeToRemove.Add(pair.Key);
                }

                //Test is key Is Assignable From T
                /*if (pair.Key.IsAssignableFrom(type))
                {
                    if (instance.Equals(pair.Value))
                    {
                        typeToRemove.Add(pair.Key);
                    }
                }*/
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var type = instance.GetType();
#endif

            foreach (var key in typeToRemove)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("Remove: ", key, type);
#endif

                serviceDict.Remove(key);
            }

            return true;
        }

        /// <summary>
        /// Remove the component instance from the service dictionary and destroy is GameObject
        /// </summary>
        /// <typeparam name="T">Type of your class</typeparam>
        /// <param name="instance">Instance of your class</param>
        /// <returns>Return true if the instance was in the dictionary</returns>
        public bool RemoveAndDestroy<T>(T instance) where T : MonoBehaviour
        {
            Object.Destroy(instance.gameObject);
            return Remove(instance);
        }

        #endregion Remove

        #region Contains

        public bool ContainsKey<T>(T instance, bool searchParent = true) where T : class
        {
            return ContainsType(instance.GetType(), searchParent);
        }

        public bool ContainsKey<T>(bool searchParent = true) where T : class
        {
            return ContainsType(typeof(T), searchParent);
        }

        public bool ContainsType(Type type, bool searchParent = true)
        {
            //Search: Quick
            if (serviceDict.ContainsKey(type))
            {
                return true;
            }

            //Deeper search: Slow
            foreach (var pair in serviceDict)
            {
                //Test is key Is Assignable From T
                if (type.IsAssignableFrom(pair.Key))
                {
                    //Save type for quick search
                    serviceDict.Add(type, pair.Value);
                    return true;
                }
            }

            if (searchParent && parentSL != null)
            {
                return parentSL.ContainsType(type);
            }

            return false;
        }

        public bool ContainsValue<T>(T instance, bool searchParent = true) where T : class
        {
            //Search: Quick
            if (serviceDict.ContainsValue(instance))
            {
                return true;
            }

            if (searchParent && parentSL != null)
            {
                return parentSL.ContainsValue(instance);
            }

            return false;
        }

        #endregion Contains

        #region Get

        private bool _TryGet<T>(Type type, out T instance) where T : class
        {
            //Search: Quick
            if (serviceDict.TryGetValue(type, out object singleton))
            {
                instance = (T)singleton;
                return true;
            }

            //Deeper search: Slow
            foreach (var pair in serviceDict)
            {
                //Test is key Is Assignable From T
                if (type.IsAssignableFrom(pair.Key))
                {
                    instance = (T)pair.Value;

                    //Save type for quick search
                    serviceDict.Add(type, instance);
                    return true;
                }
            }

            if (parentSL != null)
            {
                return parentSL._TryGet(type, out instance);
            }

            instance = null;
            return false;
        }

        /// <summary>
        /// Get component instance of type T from the singleton dictionnary
        /// </summary>
        /// <typeparam name="T">MonoBehaviour class or interface</typeparam>
        /// <param name="instance">MonoBehaviour instance</param>
        /// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
        /// <returns>Return true if the instance parameter is set with a none default value</returns>
        public bool TryGet<T>(out T instance, Action<T> callback = null) where T : class
        {
            var type = typeof(T);

            if (_TryGet(type, out instance))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("TryGet: get", instance);
#endif
                callback?.Invoke(instance);
                return true;
            }
            else
            {
                _AddCallback(type, callback);
                return false;
            }
        }

        /// <summary>
        /// If null, get component instance of type T from the singleton dictionnary
        /// </summary>
        /// <typeparam name="T">MonoBehaviour class or interface</typeparam>
        /// <param name="instance">MonoBehaviour instance</param>
        /// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
        /// <returns>Return true if the instance parameter is set with a none default value</returns>
        public bool TryGetIfNull<T>(ref T instance, Action<T> callback = null) where T : class
        {
            if (instance == null)
            {
                return TryGet(out instance, callback);
            }

            callback?.Invoke(instance);
            return true;
        }

        /// <summary>
        /// Get a instance of type T from the singleton dictionnary
        /// </summary>
        /// <typeparam name="T">MonoBehaviour class or interface</typeparam>
        /// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
        /// <returns>Return instance or default</returns>
        public T Get<T>(Action<T> callback = null) where T : class
        {
            _ = TryGet(out var instance, callback);

            return instance;
        }

        #endregion Get

        #region GetOrFindComponent

        /// <summary>
        /// Get a component instance of type T in the singleton dictionnary or find it first instance in the scene
        /// </summary>
        /// <typeparam name="T">MonoBehaviour class or interface</typeparam>
        /// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
        /// <returns>Return instance or default</returns>
        public T GetOrFindComponent<T>(Action<T> callback = null) where T : MonoBehaviour
        {
            _ = TryGetOrFindComponent(out var instance, callback);

            return instance;
        }

        /// <summary>
        /// Get a component instance of type T in the singleton dictionnary or find it first instance in the scene
        /// </summary>
        /// <typeparam name="T">MonoBehaviour class or interface</typeparam>
        /// <param name="instance">MonoBehaviour instance</param>
        /// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
        /// <returns>Return true if the instance parameter is set with a none default value</returns>
        public bool TryGetOrFindComponent<T>(out T instance, Action<T> callback = null) where T : MonoBehaviour
        {
            var type = typeof(T);

            if (_TryGet(type, out instance) || _TryFindComponent(type, out instance))
            {
                callback?.Invoke(instance);
                return true;
            }
            else
            {
                _AddCallback(type, callback);
                return false;
            }
        }

        /// <summary>
        /// If null, get a component instance of type T in the singleton dictionnary or find it first instance in the scene
        /// </summary>
        /// <typeparam name="T">MonoBehaviour class or interface</typeparam>
        /// <param name="instance">MonoBehaviour instance</param>
        /// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
        /// <returns>Return true if the instance parameter is set with a none default value</returns>
        public bool TryGetOrFindComponentIfNull<T>(ref T instance, Action<T> callback = null) where T : MonoBehaviour
        {
            if (instance == null)
            {
                return TryGetOrFindComponent(out instance, callback);
            }

            callback?.Invoke(instance);
            return true;
        }

        #endregion GetOrFindComponent

        #region GetOrFindInterface

        /// <summary>
        /// Get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
        /// </summary>
        /// <typeparam name="T">MonoBehaviour class or interface</typeparam>
        /// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
        /// <returns>Return instance or default</returns>
        public T GetOrFindInterface<T>(Action<T> callback = null) where T : class
        {
            _ = TryGetOrFindInterface(out var instance, callback);

            return instance;
        }

        /// <summary>
        /// Get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
        /// </summary>
        /// <typeparam name="T">MonoBehaviour class or interface</typeparam>
        /// <param name="instance">MonoBehaviour instance</param>
        /// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
        /// <param name="includeInactive"></param>
        /// <returns>Return true if the instance parameter is set with a none default value</returns>
        public bool TryGetOrFindInterface<T>(out T instance, Action<T> callback = null, bool includeInactive = false)
            where T : class
        {
            var type = typeof(T);

            if (!type.IsInterface)
            {
                Debug.LogError($"Type {type} is no a interface");

                instance = null;
                return false;
            }

            if (_TryGet(type, out instance) || _TryFindInterface(type, out instance, includeInactive))
            {
                callback?.Invoke(instance);
                return true;
            }
            else
            {
                _AddCallback(type, callback);
                return false;
            }
        }

        /// <summary>
        /// If null, get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
        /// </summary>
        /// <typeparam name="T">MonoBehaviour class or interface</typeparam>
        /// <param name="instance">MonoBehaviour instance</param>
        /// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
        /// <returns>Return true if the instance parameter is set with a none default value</returns>
        public bool TryGetOrFindInterfaceIfNull<T>(ref T instance, Action<T> callback = null) where T : class
        {
            if (instance == null)
            {
                return TryGetOrFindInterface(out instance, callback);
            }

            callback?.Invoke(instance);
            return true;
        }

        #endregion GetOrFindInterface

        #region Find

        private bool _TryFindComponent<T>(Type type, out T instance) where T : MonoBehaviour
        {
            instance = Object.FindObjectOfType<T>();

            if (instance != null)
            {
                _Add(type, instance);
                return true;
            }

            return false;
        }

        private bool _TryFindInterface<T>(Type type, out T instance, bool includeInactive = false) where T : class
        {
            var monos = Object.FindObjectsOfType<MonoBehaviour>(includeInactive);

            foreach (var mono in monos)
            {
                if (mono is T findInstance)
                {
                    instance = findInstance;
                    _Add(type, mono);
                    return true;
                }
            }

            instance = null;
            return false;
        }

        #endregion Find

        #region Reset

        /// <summary>
        /// Remove all component instance from the singleton dictionnary
        /// </summary>
        public void ResetSL()
        {
            serviceDict.Clear();
            callbackDict.Clear();
        }

        /// <summary>
        /// Remove all component instance from the singleton dictionnary and destroy their GameObject
        /// </summary>
        public void ResetAndDestroy()
        {
            foreach (object item in serviceDict.Values)
            {
                if (item is MonoBehaviour component && component)
                {
                    Object.Destroy(component.gameObject);
                }
            }

            serviceDict.Clear();
            callbackDict.Clear();
        }

        #endregion Reset

        #region Callback

        private void _AddCallback<T>(Type type, Action<T> callback) where T : class
        {
            if (callback == null)
            {
                return;
            }

            if (callbackDict.TryGetValue(type, out var newCallback))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("_AddCallback: add", type, type);
#endif

                newCallback.Add(callback);
            }
            else
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("_AddCallback: create", type, type);
#endif

                newCallback = new Callback();
                newCallback.Create(callback);

                callbackDict.Add(type, newCallback);
            }
        }

        private void _InvokeCallbacks<T>(Type type, T instance) where T : class
        {
            var typeList = new List<Type>();

            foreach (var pair in callbackDict)
            {
                //Test is key Is Assignable From T
                if (pair.Key.IsAssignableFrom(type))
                {
                    typeList.Add(pair.Key);
                }
            }

            foreach (var key in typeList)
            {
                if (key != type)
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    DebugLog("Add: add", key, type);
#endif
                    serviceDict.Add(key, instance);
                }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("_InvokeCallbacks:", key, type);
#endif

                var callback = callbackDict[key];

                callback.Invoke(instance);

                callbackDict.Remove(key);
            }
        }

        public void UnsubscribeCallback<T>(Action<T> callback) where T : class
        {
            var type = typeof(T);

            if (callbackDict.TryGetValue(type, out var newCallback))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                DebugLog("RemoveCallback:", type, type);
#endif

                newCallback.Remove(callback);
            }
        }

        #endregion Callback

        #region DebugLog

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        public bool useDebugLog = false;

        private void DebugLog<T>(string text, T instance) where T : class
        {
            if (!useDebugLog)
            {
                return;
            }

            var targetType = typeof(T);
            var instanceType = instance.GetType();

            DebugLog(text, targetType, instanceType);
        }

        private void DebugLog(string text, Type targetType, Type instanceType)
        {
            if (!useDebugLog)
            {
                return;
            }

            if (targetType.IsInterface)
            {
                Debug.Log($"{text} '{instanceType}' of interface '{targetType}'");
            }
            else if (targetType.IsClass)
            {
                Debug.Log($"{text} '{instanceType}' of class '{targetType}'");
            }
            else
            {
                Debug.LogWarning($"{text} '{instanceType}' of '{targetType}' who is not class or interface");
            }
        }

#endif

        #endregion DebugLog
    }
}