using Uno;
using Uno.Collections;

namespace Uno.UX
{
	public interface IPropertyListener
	{
		void OnPropertyChanged(PropertyObject obj, Selector property);
	}

	public class PropertyObject
	{
		// TODO: Make weakref
		List<IPropertyListener> _propListeners;

		public void AddPropertyListener(IPropertyListener listener)
		{
			if (_propListeners == null)
			{
				_propListeners = new List<IPropertyListener>();
			}
			_propListeners.Add(listener);
		}

		public void RemovePropertyListener(IPropertyListener listener)
		{
			_propListeners.Remove(listener);
			if (_propListeners.Count == 0) _propListeners = null;
		}

		protected virtual void OnPropertyChanged(Selector property, IPropertyListener origin)
		{
			if (_propListeners != null)
			{
				for (int i = 0; i < _propListeners.Count; i++)
				{
					var listener = _propListeners[i];
					if (listener != origin)
						listener.OnPropertyChanged(this, property);
				}
			}
		}

		protected void OnPropertyChanged(Selector property)
		{
			OnPropertyChanged(property, this as IPropertyListener);
		}

		public static void EmulatePropertyChanged(PropertyObject obj, Selector sel, IPropertyListener origin)
		{
			obj.OnPropertyChanged(sel, origin);
		}

		Dictionary<string, object> _simulatedProps;

		internal void SetSimulatedProperty(string name, object value, IPropertyListener origin)
		{
			if (_simulatedProps == null)
				_simulatedProps = new Dictionary<string, object>();

			object oldValue;
			if (_simulatedProps.TryGetValue(name, out oldValue)) 
			{
				if (value == null && oldValue == null) return;
				if (value != null && oldValue != null && (value == oldValue || value.Equals(oldValue))) return;
			}

			_simulatedProps[name] = value;

			EmulatePropertyChanged(this, name, origin);
		}

		internal object GetSimulatedProperty(string name)
		{
			if (_simulatedProps == null) return null;

			object res;
			if (_simulatedProps.TryGetValue(name, out res)) return res;

			return null;
		}
	}

	/** Interface used by the preview engine to simulate properties on an object. */
	extern(PREVIEW) public static class SimulatedProperties
	{
		public static void Set(PropertyObject obj, string name, object value, IPropertyListener origin)
		{
			obj.SetSimulatedProperty(name, value, origin);
		}

		public static object Get(PropertyObject obj, string name)
		{
			return obj.GetSimulatedProperty(name);
		}
	}
}