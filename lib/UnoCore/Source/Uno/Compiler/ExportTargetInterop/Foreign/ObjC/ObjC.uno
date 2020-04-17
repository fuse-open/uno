using Uno;
using Uno.Compiler.ExportTargetInterop;

namespace ObjC
{
    [Set("FileExtension", "mm")]
    [Set("TypeName", "::id")]
    [Set("DefaultValue", "nullptr")]
    [Set("Include", "objc/objc.h")]
    public extern(FOREIGN_OBJC_SUPPORTED) struct ID
    {
        IntPtr _dummy;
        public static ID Null { get { return extern<ID> "nil"; } }

        public override bool Equals(object o)
        {
            return o is ID && this == (ID)o;
        }

        public override int GetHashCode()
        {
            return extern<Uno.IntPtr> "(__bridge @{Uno.IntPtr})*$$".GetHashCode();
        }

        public static bool operator==(ID a, ID b)
        {
            return extern<bool> "$0 == $1";
        }

        public static bool operator!=(ID a, ID b)
        {
            return extern<bool> "$0 != $1";
        }
    }

    [Set("FileExtension", "mm")]
    public extern(FOREIGN_OBJC_SUPPORTED) class Object
    {
        public ID Handle;

        Object(ID handle)
        {
            Handle = handle;
        }

        public Object() : this(ID.Null) { }

        static Object Create(ID handle)
        {
            return handle == ID.Null ? null : new Object(handle);
        }

        ~Object()
        {
            Handle = ObjC.ID.Null;
        }

        public override bool Equals(object x)
        {
            var o = x as global::ObjC.Object;
            return o != null && o.Handle == Handle;
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        static ObjC.ID GetHandle(ObjC.Object o)
        {
            return o == null ? ObjC.ID.Null : o.Handle;
        }
    }
}
