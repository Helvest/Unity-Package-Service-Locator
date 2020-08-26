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

	#region Variables

	private static readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();

#if UNITY_EDITOR
	private static bool useDebugLog = false;
#endif

	#endregion

	#region Add

	private static void _Add<T>(T instance) where T : class
	{
#if UNITY_EDITOR
		DebugLog("_Add: add", instance);
#endif

		_singletons.Add(instance.GetType(), instance);
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

			_singletons.Add(type, instance);
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
		Type type = instance.GetType();

		if (!_singletons.ContainsKey(type))
		{
#if UNITY_EDITOR
			DebugLog("AddOrDestroy: add", instance);
#endif

			_singletons.Add(type, instance);
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
		return _singletons.ContainsValue(instance) ? _singletons.Remove(instance.GetType()) : false;
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
	/// Get component instance of type T from the singleton dictionnary
	/// </summary>
	/// <typeparam name="T">Component class or interface</typeparam>
	/// <returns>Return instance or default</returns>
	public static T Get<T>() where T : class
	{
		Get(out T instance);

		return instance;
	}

	/// <summary>
	/// Get component instance of type T from the singleton dictionnary
	/// </summary>
	/// <typeparam name="T">Component class or interface</typeparam>
	/// /// <param name="instance">Component instance</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool Get<T>(out T instance) where T : class
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

		instance = default;

		return false;
	}


	/// <summary>
	/// Get a component instance of type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">Component class or interface</typeparam>
	/// <returns>Return instance or default</returns>
	public static T GetOrFindC<T>() where T : Component
	{
		GetOrFindC(out T instance);

		return instance;
	}

	/// <summary>
	/// Get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">Component class or interface</typeparam>
	/// <returns>Return instance or default</returns>
	public static T GetOrFindI<T>() where T : class
	{
		GetOrFindI(out T instance);

		return instance;
	}

	/// <summary>
	/// Get a component instance of type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">Component class or interface</typeparam>
	/// <param name="instance">Component instance</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool GetOrFindC<T>(out T instance) where T : Component
	{
		if (Get(out instance))
		{
			return true;
		}

		instance = Object.FindObjectOfType<T>();

		if (instance)
		{
			_Add(instance);
		}

		return instance;
	}

	/// <summary>
	/// Get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">Component class or interface</typeparam>
	/// <param name="instance">Component instance</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool GetOrFindI<T>(out T instance) where T : class
	{
		Type typeT = typeof(T);

		if (!typeT.IsInterface)
		{
			Debug.LogError("Type no a interface");

			instance = null;
			return false;
		}

		if (Get(out instance))
		{
			return true;
		}

		instance = Object.FindObjectsOfType<Behaviour>().OfType<T>().FirstOrDefault();

		if (instance != null)
		{
			_Add(instance);
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
