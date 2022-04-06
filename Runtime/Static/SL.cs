using System;
using HelvestSL;
using UnityEngine;

/// <summary>
/// Static Service Locator Class
/// </summary>
public static class SL
{

	#region ServiceLocator

	public static readonly ServiceLocator sl = new ServiceLocator();

	#endregion ServiceLocator

	#region Add

	/// <summary>
	/// Add the component instance to the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public static bool Add<T>(T instance) where T : MonoBehaviour
	{
		return sl.Add(instance);
	}

	/// <summary>
	/// Add or replace the component instance to the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public static bool AddOrReplace<T>(T instance) where T : MonoBehaviour
	{
		return sl.AddOrReplace(instance);
	}

	/// <summary>
	/// Add the component instance to the singleton dictionary or destroy is GameObject
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public static bool AddOrDestroy<T>(T instance) where T : MonoBehaviour
	{
		return sl.AddOrDestroy(instance);
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
		return sl.Remove<T>();
	}

	/// <summary>
	/// Remove all the component's instances of type T from the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>Return true if the instance was in the dictionary</returns>
	public static bool Remove<T>(T instance) where T : MonoBehaviour
	{
		return sl.Remove(instance);
	}

	/// <summary>
	/// Remove the component instance from the singleton dictionary and destroy is GameObject
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>Return true if the instance was in the dictionary</returns>
	public static bool RemoveAndDestroy<T>(T instance) where T : MonoBehaviour
	{
		return sl.RemoveAndDestroy(instance);
	}

	#endregion Remove

	#region Get

	/// <summary>
	/// Get a instance of type T from the singleton dictionnary
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return instance or default</returns>
	public static T Get<T>(Action<T> callback = null) where T : class
	{
		return sl.Get(callback);
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
		return sl.TryGet(out instance, callback);
	}

	/// <summary>
	/// If null, get component instance of type T from the singleton dictionnary
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="instance">MonoBehaviour instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGetIfNull<T>(ref T instance, Action<T> callback = null) where T : class
	{
		return sl.TryGetIfNull(ref instance, callback);
	}

	#endregion Get

	#region GetOrFindComponent

	/// <summary>
	/// Get a component instance of type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return instance or default</returns>
	public static T GetOrFindComponent<T>(Action<T> callback = null) where T : MonoBehaviour
	{
		return sl.GetOrFindComponent(callback);
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
		return sl.TryGetOrFindComponent(out instance, callback);
	}

	/// <summary>
	/// If null, get a component instance of type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="instance">MonoBehaviour instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGetOrFindComponentIfNull<T>(ref T instance, Action<T> callback = null) where T : MonoBehaviour
	{
		return sl.TryGetOrFindComponentIfNull(ref instance, callback);
	}

	#endregion GetOrFindComponent

	#region GetOrFindInterface

	/// <summary>
	/// Get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return instance or default</returns>
	public static T GetOrFindInterface<T>(Action<T> callback = null) where T : class
	{
		return sl.GetOrFindInterface(callback);
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
		return sl.TryGetOrFindInterface(out instance, callback);
	}

	/// <summary>
	/// If null, get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="instance">MonoBehaviour instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGetOrFindInterfaceIfNull<T>(ref T instance, Action<T> callback = null) where T : class
	{
		return sl.TryGetOrFindInterfaceIfNull(ref instance, callback);
	}

	#endregion GetOrFindInterface

	#region Reset

	/// <summary>
	/// Remove all component instance from the singleton dictionnary
	/// </summary>
	public static void ResetSL()
	{
		sl.ResetSL();
	}

	/// <summary>
	/// Remove all component instance from the singleton dictionnary and destroy their GameObject
	/// </summary>
	public static void ResetAndDestroy()
	{
		sl.ResetAndDestroy();
	}

	#endregion Reset

	#region Callback

	public static void UnsubscribeCallback<T>(Action<T> callback) where T : class
	{
		sl.UnsubscribeCallback(callback);
	}

	#endregion Callback

	#region DebugLog

#if UNITY_EDITOR

	public static bool useDebugLog
	{
		get { return sl.useDebugLog; }
		set { sl.useDebugLog = value; }
	}

#endif

	#endregion DebugLog

}
