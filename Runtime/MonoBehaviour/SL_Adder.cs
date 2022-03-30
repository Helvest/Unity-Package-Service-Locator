using System.Collections.Generic;
using UnityEngine;

namespace UnitySL
{
	[DefaultExecutionOrder(-1)]
	public class SL_Adder : MonoBehaviour
	{
		private ServiceLocator _serviceLocator = default;

		[SerializeField]
		private bool _useGlobalSL = false;

		[SerializeField]
		private List<MonoBehaviour> _services = new List<MonoBehaviour>();

		private void Awake()
		{
			if (_useGlobalSL)
			{
				_serviceLocator = SL.sl;
			}
			else if (!TryGetComponent(out ISL _serviceLocatorHandler))
			{
				_serviceLocator = _serviceLocatorHandler.SL;
			}
		}

		private void OnEnable()
		{
			if (_serviceLocator == null)
			{
				return;
			}

			foreach (var service in _services)
			{
				if (service != null)
				{
					_serviceLocator.Add(service);
				}
			}
		}

		private void OnDisable()
		{
			if (_serviceLocator == null)
			{
				return;
			}

			foreach (var service in _services)
			{
				if (service != null)
				{
					_serviceLocator.Remove(service);
				}
			}
		}
	}
}
