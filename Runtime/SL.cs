using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Static Service Locator Class
/// </summary>
public static class SL
{

	#region Dictionary

	private static readonly Dictionary<Type, object> _singletonsDict = new Dictionary<Type, object>();

	private static readonly Dictionary<Type, Callback> _callbackDict = new Dictionary<Type, Callback>();

	#endregion Dictionary

	#region Add

	private static void _Add<T>(T instance) where T : class
	{
		Type type = typeof(T);

		_singletonsDict.Add(type, instance);

		_InvokeCallbacks(instance);
	}

	/// <summary>
	/// Add the component instance to the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public static bool Add<T>(T instance) where T : MonoBehaviour
	{
		Type type = instance.GetType();

		if (!_singletonsDict.ContainsKey(type))
		{
#if UNITY_EDITOR
			DebugLog("Add: add", instance);
#endif
			_Add(instance);

			return true;
		}
		else
		{
#if UNITY_EDITOR
			DebugLog("Add: don't add", instance);
#endif

			return _singletonsDict.ContainsValue(instance);
		}
	}

	/// <summary>
	/// Add or replace the component instance to the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public static bool AddOrReplace<T>(T instance) where T : MonoBehaviour
	{
		Type type = instance.GetType();

		if (!_singletonsDict.ContainsKey(type))
		{
#if UNITY_EDITOR
			DebugLog("AddOrReplace: add", instance);
#endif

			_Add(instance);

			return true;
		}
		else
		{
#if UNITY_EDITOR
			DebugLog("AddOrReplace: replace", instance);
#endif
			Remove<T>();

			_singletonsDict[type] = instance;

			return false;
		}
	}

	/// <summary>
	/// Add the component instance to the singleton dictionary or destroy is GameObject
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public static bool AddOrDestroy<T>(T instance) where T : MonoBehaviour
	{
		Type type = instance.GetType();

		if (!_singletonsDict.ContainsKey(type))
		{
#if UNITY_EDITOR
			DebugLog("AddOrDestroy: add", instance);
#endif
			_Add(instance);

			return true;
		}
		else
		{
#if UNITY_EDITOR
			DebugLog("AddOrDestroy: destroy", instance);
#endif

			Object.Destroy(instance.gameObject);

			return false;
		}
	}

	#endregion Add

	#region Remove

	/// <summary>
	/// Remove all the component's instances of type T from the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>Return true if the type was in the dictionary</returns>
	public static bool Remove<T>() where T : class
	{
		Type type = typeof(T);

		if (!_singletonsDict.ContainsKey(type))
		{
			return false;
		}

#if UNITY_EDITOR
		DebugLog("Remove: ", type);
#endif

		_singletonsDict.Remove(type);

		var typeToRemove = new List<Type>();

		foreach (KeyValuePair<Type, object> pair in _singletonsDict)
		{
			//Test is key Is Assignable From T
			if (pair.Key.IsAssignableFrom(type) || type.IsAssignableFrom(pair.Key))
			{
				typeToRemove.Add(pair.Key);
			}
		}

		foreach (Type key in typeToRemove)
		{
#if UNITY_EDITOR
			DebugLog("Remove: ", key, type);
#endif

			_singletonsDict.Remove(key);
		}

		return true;
	}

	/// <summary>
	/// Remove all the component's instances of type T from the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>Return true if the instance was in the dictionary</returns>
	public static bool Remove<T>(T instance) where T : MonoBehaviour
	{
		if (!_singletonsDict.ContainsValue(instance))
		{
			return false;
		}

#if UNITY_EDITOR
		DebugLog("Remove: ", instance);
#endif

		Type type = instance.GetType();

		_singletonsDict.Remove(type);

		var typeToRemove = new List<Type>();

		foreach (KeyValuePair<Type, object> pair in _singletonsDict)
		{
			//Test is key Is Assignable From T
			if (pair.Key.IsAssignableFrom(type))
			{
				if (instance.Equals(pair.Value))
				{
					typeToRemove.Add(pair.Key);
				}
			}
		}

		foreach (Type key in typeToRemove)
		{
#if UNITY_EDITOR
			DebugLog("Remove: ", key, type);
#endif

			_singletonsDict.Remove(key);
		}

		return true;
	}

	/// <summary>
	/// Remove the component instance from the singleton dictionary and destroy is GameObject
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>Return true if the instance was in the dictionary</returns>
	public static bool RemoveAndDestroy<T>(T instance) where T : MonoBehaviour
	{
		Object.Destroy(instance.gameObject);
		return Remove(instance);
	}

	#endregion Remove

	#region Get

	private static bool _TryGet<T>(out T instance) where T : class
	{
		Type type = typeof(T);

		//Research: Quick
		if (_singletonsDict.TryGetValue(type, out object singleton))
		{
			instance = (T)singleton;
			return true;
		}

		//Deeper research: Slow
		foreach (KeyValuePair<Type, object> pair in _singletonsDict)
		{
			//Test is key Is Assignable From T
			if (type.IsAssignableFrom(pair.Key))
			{
				instance = (T)pair.Value;

				_singletonsDict.Add(type, instance);

				return true;
			}
		}

		instance = default;

		return false;
	}

	/// <summary>
	/// Get component instance of type T from the singleton dictionnary
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="instance">MonoBehaviour instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGet<T>(out T instance, Action<T> callback = null) where T : class
	{
		if (_TryGet(out instance))
		{
#if UNITY_EDITOR
			DebugLog("TryGet: get", instance);
#endif
			callback?.Invoke(instance);

			return true;
		}
		else
		{
			_AddCallback(callback);

			return false;
		}
	}

	/// <summary>
	/// Get a instance of type T from the singleton dictionnary
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return instance or default</returns>
	public static T Get<T>(Action<T> callback = null) where T : class
	{
		TryGet(out T instance, callback);

		return instance;
	}

	/// <summary>
	/// Get a component instance of type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return instance or default</returns>
	public static T GetOrFindComponent<T>(Action<T> callback = null) where T : MonoBehaviour
	{
		TryGetOrFindComponent(out T instance, callback);

		return instance;
	}

	/// <summary>
	/// Get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return instance or default</returns>
	public static T GetOrFindInterface<T>(Action<T> callback = null) where T : class
	{
		TryGetOrFindInterface(out T instance, callback);

		return instance;
	}

	/// <summary>
	/// Get a component instance of type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="instance">MonoBehaviour instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGetOrFindComponent<T>(out T instance, Action<T> callback = null) where T : MonoBehaviour
	{
		if (_TryGet(out instance) || _TryFindComponent(out instance))
		{
			callback?.Invoke(instance);
			return true;
		}
		else
		{
			_AddCallback(callback);

			return false;
		}
	}

	/// <summary>
	/// Get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="instance">MonoBehaviour instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGetOrFindInterface<T>(out T instance, Action<T> callback = null) where T : class
	{
		Type type = typeof(T);

		if (!type.IsInterface)
		{
			Debug.LogError($"Type {type} is no a interface");

			instance = null;
			return false;
		}

		if (_TryGet(out instance) || _TryFindInterface(out instance))
		{
			callback?.Invoke(instance);
			return true;
		}
		else
		{
			_AddCallback(callback);

			return false;
		}
	}

	#endregion Get

	#region Find

	private static bool _TryFindComponent<T>(out T instance) where T : MonoBehaviour
	{
		instance = Object.FindObjectOfType<T>();

		if (instance != null)
		{
			_Add(instance);
			return true;
		}

		return false;
	}

	private static bool _TryFindInterface<T>(out T instance) where T : class
	{
		instance = Object.FindObjectsOfType<MonoBehaviour>().OfType<T>().FirstOrDefault();

		if (instance != null)
		{
			_Add(instance);
			return true;
		}

		return false;
	}

	#endregion Find

	#region Reset

	/// <summary>
	/// Remove all component instance from the singleton dictionnary
	/// </summary>
	public static void Reset()
	{
		_singletonsDict.Clear();
		_callbackDict.Clear();
	}

	/// <summary>
	/// Remove all component instance from the singleton dictionnary and destroy their GameObject
	/// </summary>
	public static void ResetAndDestroy()
	{
		foreach (object item in _singletonsDict.Values)
		{
			if (item is MonoBehaviour component && component)
			{
				Object.Destroy(component.gameObject);
			}
		}

		_singletonsDict.Clear();
		_callbackDict.Clear();
	}

	#endregion Reset

	#region Callback

	private static void _AddCallback<T>(Action<T> callback) where T : class
	{
		if (callback == null)
		{
			return;
		}

		Type type = typeof(T);

		if (_callbackDict.TryGetValue(type, out Callback newCallback))
		{
#if UNITY_EDITOR
			DebugLog("_AddCallback: add", type, type);
#endif

			newCallback.Add(callback);
		}
		else
		{
#if UNITY_EDITOR
			DebugLog("_AddCallback: create", type, type);
#endif

			newCallback = new Callback();
			newCallback.Create(callback);

			_callbackDict.Add(type, newCallback);
		}
	}

	private static void _InvokeCallbacks<T>(T instance) where T : class
	{
		Type type = instance.GetType();

		var typeList = new List<Type>();

		foreach (KeyValuePair<Type, Callback> pair in _callbackDict)
		{
			//Test is key Is Assignable From T
			if (pair.Key.IsAssignableFrom(type))
			{
				typeList.Add(pair.Key);
			}
		}

		foreach (Type key in typeList)
		{
#if UNITY_EDITOR
			DebugLog("_InvokeCallbacks:", key, type);
#endif

			Callback callback = _callbackDict[key];

			callback.Invoke(instance);

			_callbackDict.Remove(key);
		}
	}

	public static void UnsubscribeCallback<T>(Action<T> callback) where T : class
	{
		Type type = typeof(T);

		if (_callbackDict.TryGetValue(type, out Callback newCallback))
		{
#if UNITY_EDITOR
			DebugLog("RemoveCallback:", type, type);
#endif

			newCallback.Remove(callback);
		}
	}

	#endregion Callback

	#region Callback class

	private class Callback
	{
		private Delegate _delegate = default;

		private Action<object> _action = default;

		public void Create<T>(Action<T> callback) where T : class
		{
			Type type = typeof(T);

			_action = (instance) =>
			{
				var actions = (Action<T>)_delegate;

				actions((T)instance);

				_action = null;
			};

			Add(callback);
		}

		public void Add<T>(Action<T> callback) where T : class
		{
			var actions = (Action<T>)_delegate;

			actions += callback;

			_delegate = actions;
		}

		public void Remove<T>(Action<T> callback) where T : class
		{
			var actions = (Action<T>)_delegate;

			actions -= callback;

			_delegate = actions;
		}

		public void Invoke<T>(T instance) where T : class
		{
			_action?.Invoke(instance);
		}
	}

	#endregion Callback class

	#region DebugLog

#if UNITY_EDITOR

	public static bool useDebugLog = false;

	private static void DebugLog<T>(string text, T instance) where T : class
	{
		if (!useDebugLog)
		{
			return;
		}

		Type targetType = typeof(T);
		Type instanceType = instance.GetType();

		DebugLog(text, targetType, instanceType);
	}

	private static void DebugLog(string text, Type targetType, Type instanceType)
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
