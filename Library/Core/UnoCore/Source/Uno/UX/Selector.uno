using Uno;
using Uno.Collections;
using Uno.Threading;

namespace Uno.UX
{
	static class SelectorRegistry
	{
		static object _mutex = new object();
		static Dictionary<string, int> _stringToHandle = new Dictionary<string, int>();
		static Dictionary<int, string> _handleToString = new Dictionary<int, string>();
		static int _counter = 1;

		internal static int GetHandle(string value)
		{
			lock (_mutex)
			{
				int handle;
				if (!_stringToHandle.TryGetValue(value, out handle))
				{
					handle = _counter++;
					_stringToHandle.Add(value, handle);
					_handleToString.Add(handle, value);
				}
				return handle;
			}
		}

		public static string GetValue(int handle)
		{
			lock (_mutex)
			{
				return _handleToString[handle];
			}
		}
	}

	public struct Selector
	{
		readonly int _handle;
        internal int Handle
        {
            get  { return _handle; }
        }

		public Selector(string value)
		{
			_handle = SelectorRegistry.GetHandle(value);
		}

		public bool IsNull
		{
			get { return _handle == 0; }
		}

		public override int GetHashCode()
		{
			return _handle;
		}

		public override bool Equals(object obj)
		{
			if (obj is Selector)
			{
				var sel = (Selector)obj;
				return sel._handle == _handle;
			}

			var s = obj as string;
			if (s != null)
			{
				return Equals(new Selector(s));
			}

			return false;
		}

		public static implicit operator Selector(string s)
		{
			return new Selector(s);
		}

		public static implicit operator string(Selector s)
		{
			return s.ToString();
		}

		public static bool operator == (Selector a, Selector b)
		{
			return a._handle == b._handle;
		}

		public static bool operator != (Selector a, Selector b)
		{
			return a._handle != b._handle;
		}

		public override string ToString()
		{
			if (_handle == 0) return null;
			return SelectorRegistry.GetValue(_handle);
		}
	}
}
