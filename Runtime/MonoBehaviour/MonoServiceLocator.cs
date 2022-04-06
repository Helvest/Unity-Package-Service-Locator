using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelvestSL
{

	[DefaultExecutionOrder(-10000)]
	public class MonoServiceLocator : MonoBehaviour
	{

		#region Enum

		public enum UseCaseLocal
		{
			UseLocal,
			UseLocalAndParent,
			UseParent
		}

		public enum UseCaseParent
		{
			UseParentMSLOrGlobal,
			UseParentMSLOrParentInHierarchyOrGlobal
		}

		#endregion

		#region Variables

		[field: SerializeField]
		public UseCaseLocal ForThisLocator { get; private set; } = UseCaseLocal.UseLocalAndParent;

		[field: SerializeField]
		public UseCaseParent ForThisLocatorParent { get; private set; } = UseCaseParent.UseParentMSLOrGlobal;

		[field: SerializeField]
		public MonoServiceLocator ParentMSL { get; private set; } = default;

		private ServiceLocator _sl = null;

		public ServiceLocator sl
		{
			get
			{
				Initialise();
				return _sl;
			}
		}

		[SerializeField]
		private Transform _instanceParent = default;

		[SerializeField]
		private List<MonoBehaviour> _servicesPrefab = new List<MonoBehaviour>();

		private readonly List<MonoBehaviour> _servicesInstanciate = new List<MonoBehaviour>();

		[SerializeField]
		private List<MonoBehaviour> _services = new List<MonoBehaviour>();

		private bool _isInitialised = false;

		#endregion Variables

		#region Init

		private void Awake()
		{
			if (_instanceParent == null)
			{
				TryGetComponent(out _instanceParent);
			}

			Initialise();
		}

		private void Initialise()
		{
			if (_isInitialised)
			{
				return;
			}

			_isInitialised = true;

			switch (ForThisLocator)
			{
				case UseCaseLocal.UseLocal:
					_sl = new ServiceLocator();
					break;
				case UseCaseLocal.UseLocalAndParent:
					_sl = new ServiceLocator(GetParent());
					break;
				default:
				case UseCaseLocal.UseParent:
					_sl = GetParent();
					break;
			}

			if (_useDebugLog)
			{
				UseDebugLog = _useDebugLog;
			}
		}

		private ServiceLocator GetParent()
		{
			if (ParentMSL != null)
			{
				return ParentMSL.sl;
			}

			if (ForThisLocatorParent == UseCaseParent.UseParentMSLOrParentInHierarchyOrGlobal)
			{
				var cachedParent = transform.parent;

				if (cachedParent != null)
				{
					var msl = cachedParent.GetComponentInParent<MonoServiceLocator>();

					if (msl != null)
					{
						return msl.sl;
					}
				}
			}

			return SL.sl;
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

			foreach (var service in _servicesPrefab)
			{
				if (service != null && !ContainsKey(service))
				{
					var go = Instantiate(service, _instanceParent);
					Add(go);
					_servicesInstanciate.Add(go);
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

			foreach (var service in _servicesInstanciate)
			{
				if (service != null)
				{
					Remove(service);
					Destroy(service.gameObject);
				}
			}

			_servicesInstanciate.Clear();
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

		#region Contains

		public bool ContainsKey<T>(T instance, bool searchParent = true) where T : class
		{
			return sl.ContainsKey(instance, searchParent);
		}

		public bool ContainsKey<T>(bool searchParent = true) where T : class
		{
			return sl.ContainsKey<T>(searchParent);
		}

		public bool ContainsKey<T>(Type type, bool searchParent = true) where T : class
		{
			return sl.ContainsKey<T>(type, searchParent);
		}

		public bool ContainsValue<T>(T instance, bool searchParent = true) where T : class
		{
			return sl.ContainsValue(instance, searchParent);
		}

		#endregion Contains

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

		[Header("Debug")]
		[SerializeField]
		private bool _useDebugLog = false;

		public bool UseDebugLog
		{
			get { return sl.useDebugLog; }
			set { sl.useDebugLog = _useDebugLog = value; }
		}

		private void OnValidate()
		{
			if (ForThisLocatorParent == UseCaseParent.UseParentMSLOrParentInHierarchyOrGlobal)
			{
				if (ParentMSL == null)
				{
					ParentMSL = transform.parent?.GetComponentInParent<MonoServiceLocator>();
				}
			}

			if (ParentMSL == this)
			{
				ParentMSL = null;
			}
		}

#endif

		#endregion DebugLog

	}

}