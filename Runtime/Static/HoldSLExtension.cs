using System;
using UnityEngine;

public static class HoldSLExtension
{

	#region Add

	/// <summary>
	/// Add the component instance to the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public static bool Add<T>(this IHoldSL hold, T instance) where T : MonoBehaviour
	{
		return hold.SL.Add(instance);
	}

	/// <summary>
	/// Add or replace the component instance to the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public static bool AddOrReplace<T>(this IHoldSL hold, T instance) where T : MonoBehaviour
	{
		return hold.SL.AddOrReplace(instance);
	}

	/// <summary>
	/// Add the component instance to the singleton dictionary or destroy is GameObject
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public static bool AddOrDestroy<T>(this IHoldSL hold, T instance) where T : MonoBehaviour
	{
		return hold.SL.AddOrDestroy(instance);
	}

	#endregion Add

	#region Remove

	/// <summary>
	/// Remove all the component's instances of type T from the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>Return true if the type was in the dictionary</returns>
	public static bool Remove<T>(this IHoldSL hold) where T : class
	{
		return hold.SL.Remove<T>();
	}

	/// <summary>
	/// Remove all the component's instances of this type from the service dictionary
	/// </summary>
	/// <param name="type">Type to remove</param>
	/// <returns>Return true if the type was in the dictionary</returns>
	public static bool Remove(this IHoldSL hold, Type type)
	{
		return hold.SL.Remove(type);
	}

	/// <summary>
	/// Remove all the component's instances of type T from the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>Return true if the instance was in the dictionary</returns>
	public static bool Remove<T>(this IHoldSL hold, T instance) where T : MonoBehaviour
	{
		return hold.SL.Remove(instance);
	}

	/// <summary>
	/// Remove the component instance from the singleton dictionary and destroy is GameObject
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>Return true if the instance was in the dictionary</returns>
	public static bool RemoveAndDestroy<T>(this IHoldSL hold, T instance) where T : MonoBehaviour
	{
		return hold.SL.RemoveAndDestroy(instance);
	}

	#endregion Remove

	#region Contains

	public static bool ContainsKey<T>(this IHoldSL hold, T instance, bool searchParent = true) where T : class
	{
		return hold.SL.ContainsKey(instance, searchParent);
	}

	public static bool ContainsKey<T>(this IHoldSL hold, bool searchParent = true) where T : class
	{
		return hold.SL.ContainsKey<T>(searchParent);
	}

	public static bool ContainsType(this IHoldSL hold, Type type, bool searchParent = true)
	{
		return hold.SL.ContainsType(type, searchParent);
	}

	public static bool ContainsValue<T>(this IHoldSL hold, T instance, bool searchParent = true) where T : class
	{
		return hold.SL.ContainsValue(instance, searchParent);
	}

	#endregion Contains

	#region Get

	/// <summary>
	/// Get a instance of type T from the singleton dictionnary
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return instance or default</returns>
	public static T Get<T>(this IHoldSL hold, Action<T> callback = null) where T : class
	{
		return hold.SL.Get(callback);
	}

	/// <summary>
	/// Get component instance of type T from the singleton dictionnary
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="instance">MonoBehaviour instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGet<T>(this IHoldSL hold, out T instance, Action<T> callback = null) where T : class
	{
		return hold.SL.TryGet(out instance, callback);
	}

	/// <summary>
	/// If null, get component instance of type T from the singleton dictionnary
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="instance">MonoBehaviour instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGetIfNull<T>(this IHoldSL hold, ref T instance, Action<T> callback = null) where T : class
	{
		return hold.SL.TryGetIfNull(ref instance, callback);
	}

	#endregion Get

	#region GetOrFindComponent

	/// <summary>
	/// Get a component instance of type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return instance or default</returns>
	public static T GetOrFindComponent<T>(this IHoldSL hold, Action<T> callback = null) where T : MonoBehaviour
	{
		return hold.SL.GetOrFindComponent(callback);
	}

	/// <summary>
	/// Get a component instance of type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="instance">MonoBehaviour instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGetOrFindComponent<T>(this IHoldSL hold, out T instance, Action<T> callback = null) where T : MonoBehaviour
	{
		return hold.SL.TryGetOrFindComponent(out instance, callback);
	}

	/// <summary>
	/// If null, get a component instance of type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="instance">MonoBehaviour instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGetOrFindComponentIfNull<T>(this IHoldSL hold, ref T instance, Action<T> callback = null) where T : MonoBehaviour
	{
		return hold.SL.TryGetOrFindComponentIfNull(ref instance, callback);
	}

	#endregion GetOrFindComponent

	#region GetOrFindInterface

	/// <summary>
	/// Get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return instance or default</returns>
	public static T GetOrFindInterface<T>(this IHoldSL hold, Action<T> callback = null) where T : class
	{
		return hold.SL.GetOrFindInterface(callback);
	}

	/// <summary>
	/// Get a component instance of interface type T in the singleton dictionnary or find it first instance in the scene
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="instance">MonoBehaviour instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGetOrFindInterface<T>(this IHoldSL hold, out T instance, Action<T> callback = null) where T : class
	{
		return hold.SL.TryGetOrFindInterface(out instance, callback);
	}

	/// <summary>
	/// If null, get a component instance of interface type T in the singleton dictionnary or find it first instance in the scenepublic static void
	/// </summary>
	/// <typeparam name="T">MonoBehaviour class or interface</typeparam>
	/// <param name="instance">MonoBehaviour instance</param>
	/// <param name="callback">if the instance of type T is not found, this Action will be call when is added to the singleton dictionnary and then automatically unsubscribe</param>
	/// <returns>Return true if the instance parameter is set with a none default value</returns>
	public static bool TryGetOrFindInterfaceIfNull<T>(this IHoldSL hold, ref T instance, Action<T> callback = null) where T : class
	{
		return hold.SL.TryGetOrFindInterfaceIfNull(ref instance, callback);
	}

	#endregion GetOrFindInterface

	#region Reset

	/// <summary>
	/// Remove all component instance from the singleton dictionnary
	/// </summary>
	public static void ResetSL(this IHoldSL hold)
	{
		hold.SL.ResetSL();
	}

	/// <summary>
	/// Remove all component instance from the singleton dictionnary and destroy their GameObject
	/// </summary>
	public static void ResetAndDestroy(this IHoldSL hold)
	{
		hold.SL.ResetAndDestroy();
	}

	#endregion Reset

	#region Callback

	public static void UnsubscribeCallback<T>(this IHoldSL hold, Action<T> callback) where T : class
	{
		hold.SL.UnsubscribeCallback(callback);
	}

	#endregion Callback

}
