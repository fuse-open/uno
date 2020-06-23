using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Uno.Internal;
using Android.Base.Primitives;

namespace Android.Base.Wrappers
{
    // Tells the compiler what implementation to look up when subclassing
    public extern(Android) sealed class BindingIDAttribute : Attribute
    {
        public BindingIDAttribute(string id) {}
    }
    public extern(Android) sealed class BindingSubclassAttribute : Attribute
    {
        public BindingSubclassAttribute() {}
    }

    [TargetSpecificImplementation]
    public extern(ANDROID) interface IJWrapper
    {
        ujobject _GetJavaObject();
        bool _IsSubclassed ();
    }

    [TargetSpecificImplementation]
    public extern(ANDROID) class JWrapper : global::Java.Object, IJWrapper, IDisposable
    {
        public static ujclass _jlangObjectClass;
        public static jmethodID _jlangObjectHashCodeMid;
        public static jmethodID _jlangObjectEqualsMid;

        private uweakptr _weakptr;
        private bool disposed = false;

        public ujobject _javaObject;
        public bool _subclassed;
        protected string _javaClassName;

        private void SetInternalObj(ujobject obj, bool stackArg)
        {
            if (!extern<bool>"__JWrapper_Callbacks_Registered") {
                extern "__JWrapper_Callbacks_Registered = true";
                extern "__Register_Finalizer()";
            }
            if (extern<bool>(_weakptr) "!$0") {
                extern(_weakptr, this)"uStoreWeak(&$0, $1)";
                extern(_weakptr) "uWeakStateIntercept::SetCallback($0, (uWeakStateIntercept::Callback)__JWrapper_WeakCallback)";
            }
            if (extern<bool>(obj)"$0") {
                _javaObject = JNI.NewGlobalRef(obj);
                if (!stackArg && JNI.GetRefType(obj) == JNI.RefType.Local)
                    JNI.DeleteLocalRef(obj);
            }
        }

        public JWrapper(ujobject obj, Uno.Type typePtr, bool typeHasFallbackClass, bool resolveType)
        : this (obj, typePtr, typeHasFallbackClass, resolveType, false) {}

        public JWrapper(ujobject obj, Uno.Type typePtr, bool typeHasFallbackClass, bool resolveType, bool stackArg)
        {
            extern "@{$$._weakptr} = 0";
            Android.Base.Types.Bridge.SetWrapperType(this, obj, typePtr, typeHasFallbackClass, resolveType);
            SetInternalObj(obj, stackArg);
        }

        ~JWrapper()
        {
            Dispose(false);
        }

        public static JWrapper Wrap(ujobject obj, bool resolveType=false, bool typeHasFallbackClass=false)
        {
            return new JWrapper(obj, null, typeHasFallbackClass, resolveType);
        }

        internal static ujclass _javaClass;
        internal static ujclass _javaProxyClass;
        [TargetSpecificImplementation]
        public static extern void _Init();
        public static void _InitProxy() {}
        public static bool _IsThisType(IJWrapper obj)
        {
            return false;
        }

        public ujobject _GetJavaObject ()
        {
            return _javaObject;
        }

        public bool _IsSubclassed()
        {
            return _subclassed;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    _DisposeJavaObject();
                }
                disposed = true;
            }
        }

        [TargetSpecificImplementation]
        protected extern void _DisposeJavaObject();

        public static bool operator ==(JWrapper a, JWrapper b)
        {
            if (((object)a)==null || ((object)b)==null)
            {
                return (((object)b)==((object)a));
            } else {
                return Android.Base.JNI.IsSameObject(a._GetJavaObject(), b._GetJavaObject());
            }
        }

        public static bool operator !=(JWrapper a, JWrapper b)
        {
            return !(a == b);
        }

        [TargetSpecificImplementation]
        public extern virtual bool equals(JWrapper arg0);

        [TargetSpecificImplementation]
        public extern virtual int hashCode();
    }

    public extern(android) static class JavaObjectHelper
    {
        [Require("Source.Include", "jni.h")]
        public static IJWrapper JObjectToJWrapper(ujobject tmpRes, bool stackArg)
        {
            Android.Base.JNI.CheckException();
            long unoRef = Android.Base.JNI.GetUnoRef(tmpRes);
            if (unoRef==0)
            {
                return null;
            }
            else if (unoRef==-1)
            {
                return new JWrapper(tmpRes, typeof(JWrapper), false, false, stackArg);
            }
            else if (unoRef>0)
            {
                var res = extern<JWrapper>(unoRef) "(@{JWrapper})uLoadWeak((uWeakObject*)$0)";
                JNIEnvPtr __cb_jni = Android.Base.JNI.GetEnvPtr();
                if (extern<bool>(__cb_jni, tmpRes)"$0->GetObjectRefType($1)==JNILocalRefType" && !stackArg)
                    Android.Base.JNI.DeleteLocalRef(tmpRes);
                return res;
            } else {
                throw new Exception("JObjectToJWrapper: Unknown unoRef detected: >" + unoRef + "<");
            }
        }
    }
}
