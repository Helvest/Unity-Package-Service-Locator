using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>
/// Static Service Locator Class
/// </summary>
public static class SL
{

	#region Variables

	private static readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();

	private static readonly Dictionary<Type, Delegate> _dictOfEventAdded = new Dictionary<Type, Delegate>();

#if UNITY_EDITOR
	private static bool useDebugLog = false;
#endif

	#endregion

	#region Add

	private static void _Add<T>(Type type, T instance) where T : class
	{
		_singletons.Add(type, instance);

		if (_dictOfEventAdded.TryGetValue(type, out var value))
		{
			var action = (Action<T>)value;

			action?.Invoke(instance);

			action = null;

			_dictOfEventAdded.Remove(type);
		}
	}

	/// <summary>
	/// Add the component instance to the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public static bool Add<T>(T instance) where T : Component
	{
		Type type = instance.GetType();

		if (!_singletons.ContainsKey(type))
		{
#if UNITY_EDITOR
			DebugLog("Add: add", instance);
#endif
			_Add(type, instance);
		}

#if UNITY_EDITOR
		DebugLog("Add: don't add", instance);
#endif

		return _singletons.ContainsValue(instance);
	}

	/// <summary>
	/// Add the component instance to the singleton dictionary or destroy is GameObject
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public static bool AddOrDestroy<T>(T instance) where T : Component
	{
		Type type = typeof(T);

		if (!_singletons.ContainsKey(type))
		{
#if UNITY_EDITOR
			DebugLog("AddOrDestroy: add", instance);
#endif
			_Add(type, instance);
		}
		else if (!_singletons.ContainsValue(instance))
		{
#if UNITY_EDITOR
			DebugLog("AddOrDestroy: destroy", instance);
#endif

			Object.Destroy(instance.gameObject);

			return false;
		}

		return true;
	}

	#endregion Add

	#region Remove

	/// <summary>
	/// Remove the component instance from the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>Return true if the instance was in the dictionary</returns>
	public static bool Remove<T>(T instance) where T : Component
	{
		if (_singletons.ContainsValue(instance))
		{
			var type = typeof(T);

			_singletons.Remove(type);

			_dictOfEventAdded.Remove(type);

			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Remove the component instance from the singleton dictionary and destroy is GameObject
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>Return true if the instance was in the dictionary</returns>
	public static bool RemoveAndDestroy<T>(T instance) where T : Component
	{
		Object.Destroy(instance.gameObject);
		return Remove(instance);
	}

	#endregion Remove

	#region Get

	/// <summary>
	/// Get a instance of type T from the singleton dictionnary
	/// </summary>
	/// <typeparam name="T">Component class or interface</typeparam>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return instance or default</returns>
	public static T Get<T>(Action<T> callback = null) where T : class
	{
		TryGet(out T instance, callback);

		return instance;
	}

	/// <summary>
	/// Get component instance of type T from the singleton dictionnary
	/// </summary>
	/// <typeparam name="T">Component class or interface</typeparam>
	/// <param name="instance">Component instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGet<T>(out T instance, Action<T> callback = null) where T : class
	{
		Type type = typeof(T);

		foreach (Type key in _singletons.Keys)
		{
			//Test is key Is Assignable From T
			if (type.IsAssignableFrom(key))
			{
				instance = (T)_singletons[key];
				return true;
			}
		}

		if (callback != null)
		{
			if (_dictOfEventAdded.TryGetValue(type, out var value))
			{
				var action = (Action<T>)value;

				action += callback;
			}
			else
			{
				var action = new Action<T>(callback);

				_dictOfEventAdded.Add(type, action);
			}
		}

		instance = default;

		return false;
	}


	/// <summary>
	/// Get a component instance of type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">Component class or interface</typeparam>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return instance or default</returns>
	public static T GetOrFindComponent<T>(Action<T> callback = null) where T : Component
	{
		TryGetOrFindComponent(out T instance, callback);

		return instance;
	}

	/// <summary>
	/// Get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">Component class or interface</typeparam>
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
	/// <typeparam name="T">Component class or interface</typeparam>
	/// <param name="instance">Component instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGetOrFindComponent<T>(out T instance, Action<T> callback = null) where T : Component
	{
		if (TryGet(out instance, callback))
		{
			return true;
		}

		instance = Object.FindObjectOfType<T>();

		if (instance != null)
		{
			_Add(typeof(T), instance);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">Component class or interface</typeparam>
	/// <param name="instance">Component instance</param>
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

		if (TryGet(out instance, callback))
		{
			return true;
		}

		instance = Object.FindObjectsOfType<Behaviour>().OfType<T>().FirstOrDefault();

		if (instance != null)
		{
			_Add(type, instance);
			return true;
		}

		return false;
	}

	#endregion Get

	#region Reset

	/// <summary>
	/// Remove all component instance from the singleton dictionnary
	/// </summary>
	public static void Reset()
	{
		_singletons.Clear();
		_dictOfEventAdded.Clear();
	}

	/// <summary>
	/// Remove all component instance from the singleton dictionnary and destroy their GameObject
	/// </summary>
	public static void ResetAndDestroy()
	{
		foreach (var item in _singletons.Values)
		{
			if (item is Component component && component)
			{
				Object.Destroy(component.gameObject);
			}
		}

		_singletons.Clear();
		_dictOfEventAdded.Clear();
	}

	#endregion Reset

	#region DebugLog

#if UNITY_EDITOR

	private static void DebugLog<T>(string text, T instance)
	{
		if (!useDebugLog)
		{
			return;
		}

		var targetType = typeof(T);
		var trueType = instance.GetType();

		if (targetType.IsInterface)
		{
			Debug.Log($"{text} '{trueType}' of interface '{targetType}'");
		}
		else if (targetType.IsClass)
		{
			Debug.Log($"{text} '{trueType}' of class '{targetType}'");
		}
		else
		{
			Debug.LogWarning($"{text} '{trueType}' of '{targetType}' who is not class or interface");
		}
	}

#endif

	#endregion

}
