using Uno.Collections;

namespace Uno.UX
{
	/** Resources are objects that are identified by a string key, either globally for your app or 
		for certain pages/subtrees.

		@topic Resources
		
		## Types of resources

		Resources can be of any type, but it is most commonly used for fonts, colors, constant values,
		strings etc.

		Classes that inherit from @Node can only be rooted with one parent at a time, and is therefore 
		rarely useful as global resources. Consider creating a @ux:Class instead and instantiate multiple
		objects whenever needed.

		## Global resources (`ux:Global`)

		The @ux:Global attribute can be used on an UX node to convert it to a global resource.

		For example, a global Font resource can be defined like this:

			<Font File="arial.ttf" ux:Global="MyDefaultFont" />

		And then used like this:

			<Text Font="MyDefaultFont" />

		When the global key is used directly (with no binding syntax), the resource is resolved statically
		at build time, and you get an error message if no appropriate resource is found.

		## Dynamic resource bindings

		When you want resources to resolve at runtime and respect tree-local resources, you can also use the 
		dynamic resource binding syntax:

			<Text Font="{Resource MyDefaultFont}" />

		## Local resources (`ux:Key`)

		The `ux:Key` attribute is used to convert a node to a tree-local resource.
		
		This allows you to do local overrides in the tree, using the `ux:Key` attribute:

			<Panel>
				<Font File="verdana.ttf" ux:Key="MyDefaultFont" />
				<Panel>
					<Text Font="{Resource MyDefaultFont}" />
				</Panel>
			</Panel>

		You can think of `ux:Global` as a way to set global defaults for dynamic resource keys (`ux:Key`)

		## Resource key uniqueness

		Global resource keys must be unique for the given *type* of the resource. Multiple resources can have
		the same name as long as they are unrelated types.

		For example, Fuse defines multiple resources with the key `Blue`, and the right resource is determined 
		based on the expected type where it used. It can either be a `float4` color value or a `SolidColor` brush.

			<float4 ux:Global="Red" ux:Value="#f00" />
			<SolidColor ux:Global="Red" Color="Red" />

	*/
    public sealed class Resource
    {
        public string Key { get; private set; }
        public object Value { get; private set; }

        [UXConstructor]
        public Resource([UXParameter("Key")] string key, [UXParameter("Value")] object value)
        {
            Key = key;
            Value = value;
        }

        static readonly Dictionary<string, List<object>> _globals = new Dictionary<string, List<object>>();
        static readonly List<Action<string>> _listeners = new List<Action<string>>();

        /** Registers a static listener for a particular resource key.

        	The listener will get a callback whenever a global resource with the given key is registered
        	or removed.
        */
        public static void AddGlobalKeyListener(Action<string> listener)
        {
            lock (_listeners)
            {
                _listeners.Add(listener);
            }
        }

        /** Removes a listener that has been added with `AddGlobalKeyListener`.
        */
        public static void RemoveGlobalKeyListener(Action<string> listener)
        {
            lock (_listeners)
            {
                _listeners.Remove(listener);
            }
        }

        [UXAttachedPropertySetter("Resource.GlobalKey")]
        /** The `ux:Global` attribute creates a global resource that is accessible everywhere in UX markup.
        
			See @Resources for more information.

			The `ux:Global` attribute on an UX element creates a single object (singleton) that is globally available. 
        	Using this attribute will prevent the object from being inserted by default the object at its defined location 
        	in the UX tree. 
        */
        public static void SetGlobalKey(object obj, string key)
        {
            lock (_globals)
            {
                List<object> objs;
                if (!_globals.TryGetValue(key, out objs))
                {
                    objs = new List<object>();
                    _globals.Add(key, objs);
                }

                if (!objs.Contains(obj))
                    objs.Add(obj);
            }

            Action<string>[] listeners = null;
            lock (_listeners)
                listeners = _listeners.ToArray();
            foreach (var listener in listeners)
                listener(key);
        }

        [UXAttachedPropertyGetter("Resource.GlobalKey")]
        public static string GetGlobalKey(object obj)
        {
            lock (_globals)
            {
                foreach (var list in _globals)
                {
                    if (list.Value.Contains(obj)) return list.Key;
                }
            }
            return null;
        }

        public static IEnumerable<T> GetGlobalsOfType<T>()
        {
            var result = new List<T>();
            lock (_globals)
            {
                foreach (var entry in _globals)
                {
                    var list = entry.Value;
                    for (int i = list.Count; i-- > 0; )
                    {
                        var v = list[i];
                        if (v is T)
                        {
                            result.Add((T)v);
                        }
                    }
                }
            }
            return result;
        }

        public static bool TryFindGlobal(string key, Predicate<object> acceptor, out object res)
        {
            List<object> list;
            lock (_globals)
            {
                if (_globals.TryGetValue(key, out list))
                {
                    // Scan backwards so most recently set key takes precedence
                    for (int i = list.Count; i --> 0; )
                    {
                        var v = list[i];
                        if (acceptor == null || acceptor(v))
                        {
                            res = v;
                            return true;
                        }
                    }
                }
            }
            res = null;
            return false;
        }
    }
}
