using System.Collections.Generic;
using HelvestSL;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class MonoServiceLocator : MonoBehaviour, IHoldSL
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
		UseVarOrGlobal,
		UseVarOrFindInHierarchyOrGlobal
	}

	#endregion

	#region Fields

	[field: SerializeField, Header("MonoServiceLocator")]
	public UseCaseLocal ForThisLocator { get; private set; } = UseCaseLocal.UseLocalAndParent;

	[field: SerializeField]
	public UseCaseParent ForThisParent { get; private set; } = UseCaseParent.UseVarOrGlobal;

	[field: SerializeField]
	public MonoServiceLocator Parent { get; private set; } = default;

	private ServiceLocator _sl = null;

	public ServiceLocator SL
	{
		get
		{
			Initialise();
			return _sl;
		}
	}

	[SerializeField]
	private List<MonoBehaviour> _services = new List<MonoBehaviour>();

	private readonly List<MonoBehaviour> _servicesAdded = new List<MonoBehaviour>();

	[Header("Prefabs")]

	[SerializeField]
	private Transform _transformParentForPrefabs = default;

	[SerializeField]
	private List<MonoBehaviour> _servicesPrefab = new List<MonoBehaviour>();

	private readonly List<MonoBehaviour> _servicesInstanciate = new List<MonoBehaviour>();

	private bool _isInitialised = false;

	#endregion Variables

	#region Init

	protected virtual void Awake()
	{
		Initialise();
	}

	protected virtual void Initialise()
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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
		if (_useDebugLog)
		{
			UseDebugLog = _useDebugLog;
		}
#endif
	}

	protected virtual ServiceLocator GetParent()
	{
		if (Parent != null)
		{
			return Parent.SL;
		}

		if (ForThisParent == UseCaseParent.UseVarOrFindInHierarchyOrGlobal)
		{
			var cachedParent = transform.parent;

			if (cachedParent != null)
			{
				var msl = cachedParent.GetComponentInParent<MonoServiceLocator>();

				if (msl != null)
				{
					return msl.SL;
				}
			}
		}

		return global::SL.sl;
	}

	protected virtual void OnEnable()
	{
		foreach (var service in _services)
		{
			if (service != null)
			{
				if (SL.Add(service, AddMode.AddOrSetActiveFalse))
				{
					_servicesAdded.Add(service);
				}
			}
		}

		foreach (var prefab in _servicesPrefab)
		{
			if (prefab != null && !SL.ContainsKey(prefab, true))
			{
				var service = Instantiate(prefab, _transformParentForPrefabs);
				SL.Add(service);
				_servicesInstanciate.Add(service);
			}
		}
	}

	protected virtual void OnDisable()
	{
		foreach (var service in _servicesAdded)
		{
			if (service != null)
			{
				SL.Remove(service);
				service.gameObject.SetActive(false);
			}
		}

		_servicesAdded.Clear();

		foreach (var service in _servicesInstanciate)
		{
			if (service != null)
			{
				SL.Remove(service);
				Destroy(service.gameObject);
			}
		}

		_servicesInstanciate.Clear();
	}

	#endregion Init

	#region DebugLog

#if UNITY_EDITOR || DEVELOPMENT_BUILD

	[Header("Debug")]
	[SerializeField]
	private bool _useDebugLog = false;

	[SerializeField]
	private bool _autoSetActiveService = false;

	[SerializeField]
	private bool _serviceActiveValueToSet = false;

	public bool UseDebugLog
	{
		get { return SL.useDebugLog; }
		set { SL.useDebugLog = _useDebugLog = value; }
	}

	protected virtual void OnValidate()
	{
		if (ForThisParent == UseCaseParent.UseVarOrFindInHierarchyOrGlobal)
		{
			if (Parent == null)
			{
				Parent = transform.parent?.GetComponentInParent<MonoServiceLocator>();
			}
		}

		if (Parent == this)
		{
			Parent = null;
		}

		if (_autoSetActiveService)
		{
			foreach (var service in _services)
			{
				if (service != null)
				{
					service.gameObject.SetActive(_serviceActiveValueToSet);
				}
			}
		}	
	}

#endif

	#endregion DebugLog

}
