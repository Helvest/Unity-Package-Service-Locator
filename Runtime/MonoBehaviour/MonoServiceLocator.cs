using System.Collections.Generic;
using UnityEngine;
using HelvestSL;

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

	#region Variables

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

	[Header("Prefabs")]

	[SerializeField]
	private Transform _transformParentForPrefabs = default;

	[SerializeField]
	private List<MonoBehaviour> _servicesPrefab = new List<MonoBehaviour>();

	private readonly List<MonoBehaviour> _servicesInstanciate = new List<MonoBehaviour>();

	private bool _isInitialised = false;

	#endregion Variables

	#region Init

	private void Awake()
	{
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

	private void OnEnable()
	{
		foreach (var service in _services)
		{
			if (service != null)
			{
				SL.Add(service);
			}
		}

		foreach (var service in _servicesPrefab)
		{
			if (service != null && !SL.ContainsKey(service))
			{
				var go = Instantiate(service, _transformParentForPrefabs);
				SL.Add(go);
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
				SL.Remove(service);
			}
		}

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

#if UNITY_EDITOR

	[Header("Debug")]
	[SerializeField]
	private bool _useDebugLog = false;

	public bool UseDebugLog
	{
		get { return SL.useDebugLog; }
		set { SL.useDebugLog = _useDebugLog = value; }
	}

	private void OnValidate()
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
	}

#endif

	#endregion DebugLog

}
