using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Android.Base.Primitives
{

    [TargetSpecificType]
    public extern(ANDROID) struct ConstCharPtr {}

    [TargetSpecificType]
    public extern(ANDROID) struct JavaVMPtr {}

    [TargetSpecificType]
    public extern(ANDROID) struct JNIEnvPtr {}

    [TargetSpecificType]
    public extern(ANDROID) struct jweak {}

    [TargetSpecificType]
    public extern(ANDROID) struct uweakptr {}

    [TargetSpecificType]
    public extern(ANDROID) struct ujboolean
    {
        public static implicit operator bool(ujboolean v)
        {
            return extern<bool>(v) "$0";
        }

        public static implicit operator ujboolean(bool v)
        {
            return extern<ujboolean>(v) "(jboolean)$0";
        }
    }

    [TargetSpecificType]
    public extern(ANDROID) struct ujbyte
    {
        public static implicit operator sbyte(ujbyte v)
        {
            return extern<sbyte>(v) "$0";
        }

        public static implicit operator ujbyte(sbyte v)
        {
            return extern<ujbyte>(v) "(jbyte)$0";
        }
    }

    [TargetSpecificType]
    public extern(ANDROID) struct ujchar
    {
        public static implicit operator char(ujchar v)
        {
            return extern<char>(v) "$0";
        }

        public static implicit operator ujchar(char v)
        {
            return extern<ujchar>(v) "(jchar)$0";
        }
    }

    [TargetSpecificType]
    public extern(ANDROID) struct ujshort
    {
        public static implicit operator short(ujshort v)
        {
            return extern<short>(v) "$0";
        }

        public static implicit operator ujshort(short v)
        {
            return extern<ujshort>(v) "(jshort)$0";
        }
    }

    [TargetSpecificType]
    public extern(ANDROID) struct ujint
    {
        public static implicit operator int(ujint v)
        {
            return extern<int>(v) "$0";
        }

        public static implicit operator ujint(int v)
        {
            return extern<ujint>(v) "(jint)$0";
        }
    }

    [TargetSpecificType]
    public extern(ANDROID) struct ujlong
    {
        public static implicit operator long(ujlong v)
        {
            return extern<long>(v) "$0";
        }

        public static implicit operator ujlong(long v)
        {
            return extern<ujlong>(v) "(jlong)$0";
        }
    }

    [TargetSpecificType]
    public extern(ANDROID) struct ujfloat
    {
        public static implicit operator float(ujfloat v)
        {
            return extern<float>(v) "$0";
        }

        public static implicit operator ujfloat(float v)
        {
            return extern<ujfloat>(v) "(jfloat)$0";
        }
    }

    [TargetSpecificType]
    public extern(ANDROID) struct ujdouble
    {
        public static implicit operator double(ujdouble v)
        {
            return extern<double>(v) "$0";
        }

        public static implicit operator ujdouble(double v)
        {
            return extern<ujdouble>(v) "(jdouble)$0";
        }
    }

    [TargetSpecificType]
    public extern(ANDROID) struct JNINativeMethod {}

    [TargetSpecificType]
    public extern(ANDROID) struct jmethodID
    {
    }

    [TargetSpecificType]
    public extern(ANDROID) struct jfieldID {}

    [TargetSpecificType]
    public extern(ANDROID) struct ujclass
    {
        public static ujclass Null
        {
            get { return extern<ujclass> "nullptr"; }
        }

        public static bool operator == (ujclass lhs, ujclass rhs)
        {
            return Android.Base.JNI.IsSameObject(lhs, rhs);
        }

            public static bool operator != (ujclass lhs, ujclass rhs)
        {
            return !(lhs == rhs);
        }
    }

    [TargetSpecificType]
    public extern(ANDROID) struct ujstring
    {
        public static ujstring Null
        {
            get { return extern<ujstring> "nullptr"; }
        }
    }

    [TargetSpecificType]
    public extern(ANDROID) struct ujobject
    {
        public static ujobject Null
        {
            get { return extern<ujobject> "nullptr"; }
        }

        public static implicit operator ujobject(ujclass v)
        {
            return extern<ujobject>(v) "(jobject)$0";
        }

        public static implicit operator ujclass(ujobject v)
        {
            return extern<ujclass>(v) "(jclass)$0";
        }

        public static implicit operator ujobject(ujstring v)
        {
            return extern<ujobject>(v) "(jobject)$0";
        }

        public static implicit operator ujstring(ujobject v)
        {
            return extern<ujstring>(v) "(jstring)$0";
        }

        public static bool operator == (ujobject lhs, ujobject rhs)
        {
            return Android.Base.JNI.IsSameObject(lhs, rhs);
        }

        public static bool operator != (ujobject lhs, ujobject rhs)
        {
            return !(lhs == rhs);
        }
    }


    [TargetSpecificType]
    public extern(ANDROID) struct ujvalue
    {
        public static implicit operator ujvalue(Android.Base.Wrappers.JWrapper v)
        {
            return (ujvalue)v._GetJavaObject();
        }
        public static implicit operator ujvalue(ujobject v)
        {
            extern "jvalue r;";
            extern(v) "r.l = $0";
            return extern<ujvalue> "r";
        }
        public static implicit operator ujvalue(ujclass v)
        {
            extern "jvalue r;";
            extern(v) "r.l = (jobject)$0";
            return extern<ujvalue> "r";
        }
        public static implicit operator ujvalue(ujstring v)
        {
            extern "jvalue r;";
            extern(v) "r.l = (jobject)$0";
            return extern<ujvalue> "r";
        }
        public static implicit operator ujvalue(ujboolean v)
        {
            extern "jvalue r;";
            extern(v) "r.z = $0";
            return extern<ujvalue> "r";
        }
        public static implicit operator ujvalue(ujbyte v)
        {
            extern "jvalue r;";
            extern(v) "r.b = $0";
            return extern<ujvalue> "r";
        }
        public static implicit operator ujvalue(ujchar v)
        {
            extern "jvalue r;";
            extern(v) "r.c = $0";
            return extern<ujvalue> "r";
        }
        public static implicit operator ujvalue(ujshort v)
        {
            extern "jvalue r;";
            extern(v) "r.s = $0";
            return extern<ujvalue> "r";
        }
        public static implicit operator ujvalue(ujint v)
        {
            extern "jvalue r;";
            extern(v) "r.i = $0";
            return extern<ujvalue> "r";
        }
        public static implicit operator ujvalue(ujlong v)
        {
            extern "jvalue r;";
            extern(v) "r.j = $0";
            return extern<ujvalue> "r";
        }
        public static implicit operator ujvalue(ujfloat v)
        {
            extern "jvalue r;";
            extern(v) "r.f = $0";
            return extern<ujvalue> "r";
        }
        public static implicit operator ujvalue(ujdouble v)
        {
            extern "jvalue r;";
            extern(v) "r.d = $0";
            return extern<ujvalue> "r";
        }

        public static implicit operator ujvalue(bool v)
        {
            return (ujvalue)((ujboolean)v);
        }

        public static implicit operator ujvalue(sbyte v)
        {
            return (ujvalue)((ujbyte)v);
        }

        public static implicit operator ujvalue(char v)
        {
            return (ujvalue)((ujchar)v);
        }

        public static implicit operator ujvalue(short v)
        {
            return (ujvalue)((ujshort)v);
        }

        public static implicit operator ujvalue(int v)
        {
            return (ujvalue)((ujint)v);
        }

        public static implicit operator ujvalue(long v)
        {
            return (ujvalue)((ujlong)v);
        }

        public static implicit operator ujvalue(float v)
        {
            return (ujvalue)((ujfloat)v);
        }

        public static implicit operator ujvalue(double v)
        {
            return (ujvalue)((ujdouble)v);
        }
    }
}
