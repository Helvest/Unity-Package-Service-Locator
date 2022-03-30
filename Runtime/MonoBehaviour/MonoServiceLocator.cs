using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class MonoServiceLocator : MonoBehaviour, IHolderSL
{

	#region Variables

	[field: SerializeField]
	public bool UseGlobalSL { get; private set; } = false;

	[field: SerializeField]
	public bool UseGlobalSLAsParent { get; private set; } = false;

	[field: SerializeField]
	public MonoServiceLocator ParentMSL { get; private set; } = default;

	public ServiceLocator sl { get; private set; } = null;

	[SerializeField]
	private List<MonoBehaviour> _services = new List<MonoBehaviour>();

	#endregion Variables

	#region Init

	private void Awake()
	{
		sl = UseGlobalSL ? SL.sl : new ServiceLocator(UseGlobalSLAsParent ? SL.sl : ParentMSL != null ? ParentMSL.sl : null);
	}

	private void OnValidate()
	{
		if (ParentMSL == this)
		{
			ParentMSL = null;
		}
	}

	private void OnEnable()
	{
		foreach (var service in _services)
		{
			if (service != null)
			{
				Add(service);
			}
		}
	}

	private void OnDisable()
	{
		foreach (var service in _services)
		{
			if (service != null)
			{
				Remove(service);
			}
		}
	}

	#endregion Init

	#region Add

	/// <summary>
	/// Add the component instance to the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public bool Add<T>(T instance) where T : MonoBehaviour
	{
		return sl.Add(instance);
	}

	/// <summary>
	/// Add or replace the component instance to the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public bool AddOrReplace<T>(T instance) where T : MonoBehaviour
	{
		return sl.AddOrReplace(instance);
	}

	/// <summary>
	/// Add the component instance to the singleton dictionary or destroy is GameObject
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>False if a other instance of same type is already in the dictionary</returns>
	public bool AddOrDestroy<T>(T instance) where T : MonoBehaviour
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
	public bool Remove<T>() where T : class
	{
		return sl.Remove<T>();
	}

	/// <summary>
	/// Remove all the component's instances of type T from the singleton dictionary
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>Return true if the instance was in the dictionary</returns>
	public bool Remove<T>(T instance) where T : MonoBehaviour
	{
		return sl.Remove(instance);
	}

	/// <summary>
	/// Remove the component instance from the singleton dictionary and destroy is GameObject
	/// </summary>
	/// <typeparam name="T">Type of your class</typeparam>
	/// <param name="instance">Instance of your class</param>
	/// <returns>Return true if the instance was in the dictionary</returns>
	public bool RemoveAndDestroy<T>(T instance) where T : MonoBehaviour
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
	public T Get<T>(Action<T> callback = null) where T : class
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
	public bool TryGet<T>(out T instance, Action<T> callback = null) where T : class
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
	public bool TryGetIfNull<T>(ref T instance, Action<T> callback = null) where T : class
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
	public T GetOrFindComponent<T>(Action<T> callback = null) where T : MonoBehaviour
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
	public bool TryGetOrFindComponent<T>(out T instance, Action<T> callback = null) where T : MonoBehaviour
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
	public bool TryGetOrFindComponentIfNull<T>(ref T instance, Action<T> callback = null) where T : MonoBehaviour
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
	public T GetOrFindInterface<T>(Action<T> callback = null) where T : class
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
	public bool TryGetOrFindInterface<T>(out T instance, Action<T> callback = null) where T : class
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
	public bool TryGetOrFindInterfaceIfNull<T>(ref T instance, Action<T> callback = null) where T : class
	{
		return sl.TryGetOrFindInterfaceIfNull(ref instance, callback);
	}

	#endregion GetOrFindInterface

	#region Reset

	/// <summary>
	/// Remove all component instance from the singleton dictionnary
	/// </summary>
	public void ResetSL()
	{
		sl.ResetSL();
	}

	/// <summary>
	/// Remove all component instance from the singleton dictionnary and destroy their GameObject
	/// </summary>
	public void ResetAndDestroy()
	{
		sl.ResetAndDestroy();
	}

	#endregion Reset

	#region Callback

	public void UnsubscribeCallback<T>(Action<T> callback) where T : class
	{
		sl.UnsubscribeCallback(callback);
	}

	#endregion Callback

	#region DebugLog

#if UNITY_EDITOR

	public bool useDebugLog
	{
		get { return sl.useDebugLog; }
		set { sl.useDebugLog = value; }
	}

#endif

	#endregion DebugLog

}
