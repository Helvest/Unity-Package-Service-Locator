using System;

namespace HelvestSL
{
	/// <summary>
	/// Class to save and call Action by type send 
	/// </summary>
	public class Callback
	{
		private Delegate _delegate = default;

		private Action<object> _action = default;

		public void Create<T>(Action<T> callback) where T : class
		{
			_action = (instance) =>
			{
				var actions = (Action<T>)_delegate;

				actions?.Invoke((T)instance);

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
}
