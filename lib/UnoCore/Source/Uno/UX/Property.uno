namespace Uno.UX
{
    public class ValueChangedArgs<T>: EventArgs
    {
        public T Value { get; private set; }

        public ValueChangedArgs(T value)
        {
            Value = value;
        }
    }

    public delegate void ValueChangedHandler<T>(object sender, ValueChangedArgs<T> args);

    public abstract class PropertyAccessor
    {
        public abstract Selector Name { get; }
        public abstract Type PropertyType { get; }
        public virtual bool SupportsOriginSetter { get { return false; } }
        public virtual void SetAsObject(PropertyObject obj, object value, IPropertyListener origin) { throw new NotSupportedException("Property '" + Name + "' cannot be written to."); }
        public virtual object GetAsObject(PropertyObject obj) { throw new NotSupportedException("Property '" + Name + "' cannot be read."); }
    }

    public abstract class Property: PropertyAccessor
    {
		/** This may be backed by a WeakPointer or otherwise freed during cleanup. */
        public abstract PropertyObject Object { get; }

        readonly Selector _name;
        public override Selector Name { get { return _name; } }

        protected Property(Selector name)
        {
            _name = name;
        }

        public void AddListener(IPropertyListener listener)
        {
            Object.AddPropertyListener(listener);
        }

        public void RemoveListener(IPropertyListener listener)
        {
			var obj = Object;
			//assume cleanup thus it's fine if the object isn't there (we assume it will have freed its own listeners)
			if (obj != null)
				obj.RemovePropertyListener(listener);
        }

        public override bool Equals(object obj)
        {
            var p = obj as Property;
            if (p == null) return false;
            return p.Object == Object && p.Name == _name;
        }

        public override int GetHashCode()
        {
            return Object.GetHashCode() ^ Name.Handle;
        }

        public object GetAsObject() { return GetAsObject(Object); }
        public void SetAsObject(object value, IPropertyListener origin) { SetAsObject(Object, value, origin); }
    }

    public abstract class Property<T> : Property
    {
        protected Property(Selector name) : base(name) {}

        public sealed override Type PropertyType { get { return typeof(T); } }

        public virtual T Get(PropertyObject obj) { throw new NotSupportedException(); }
        public virtual void Set(PropertyObject obj, T value, IPropertyListener origin) { throw new NotSupportedException(); }

        public T Get() { return Get(Object); }
        public void Set(T value, IPropertyListener origin) { Set(Object, value, origin); }

        public sealed override object GetAsObject(PropertyObject obj)
        {
            return Get(obj);
        }

        public sealed override void SetAsObject(PropertyObject obj, object value, IPropertyListener origin)
        {
            if (value == null && typeof(T).IsValueType) Set(obj, default(T), origin);
            else Set(obj, (T)value, origin);
        }
    }
}
