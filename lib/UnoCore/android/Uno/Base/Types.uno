using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Android.Base.Primitives;
using Android.Base.Wrappers;

namespace Android.Base.Types
{
    [TargetSpecificImplementation]
    public extern(ANDROID) static class Bridge
    {
        static bool _inited;
        static ujclass _helperCls;
        static jmethodID _registerTypeMid;
        static jmethodID _registerFallbackMid;
        static jmethodID _javaToUnoMid;
        static jmethodID _unoToJavaMid;

        [TargetSpecificImplementation]
        public static extern void Init();
        [TargetSpecificImplementation]
        public static extern void  RegisterUnoType(ConstCharPtr typeName, int nameLen, Uno.Type typePtr);
        [TargetSpecificImplementation]
        public static extern void RegisterUnoFallback(ConstCharPtr typeName, int nameLen, Uno.Type typePtr);

        public static Uno.Type JavaToUnoType(ujobject javaObj, Uno.Type fallbackType, bool typeHasFallbackClass)
        {
            if (!_inited)
                Bridge.Init();
            var jni = JNI.GetEnvPtr();
            var result = extern<Uno.Type> (javaObj, fallbackType, typeHasFallbackClass, _helperCls, _javaToUnoMid, jni)
                "(@{Uno.Type})$5->CallStaticLongMethod($3, $4, $0, (jlong)$1, (jboolean)$2)";
            var excep = JNI.TryGetException(jni);
            if (excep!=null)
            {
                extern(jni) "jmethodID grabClass = $0->GetMethodID($0->FindClass(\"java/lang/Object\"), \"getClass\", \"()Ljava/lang/Class;\")";
                extern(jni) "jmethodID toString = $0->GetMethodID($0->FindClass(\"java/lang/Object\"), \"toString\", \"()Ljava/lang/String;\")";
                extern(jni, javaObj)"jobject cls = $0->CallObjectMethod($1, grabClass)";
                var estring = extern<ujobject>(jni)"(jobject)$0->CallObjectMethod(cls, toString)";
                var x = Android.Base.Types.String.JavaToUno(estring);
                excep = new Exception (excep.Message + "\nTried to find uno class for java type " + x + "\n");
                throw excep;
            }
            return result;
        }

        public static ujclass UnoToJavaType(Uno.Type unoTypePtr)
        {
            var jni = JNI.GetEnvPtr();
            var result = extern<ujclass>(jni, unoTypePtr, _helperCls, _registerFallbackMid)
                "(jclass)$0->CallStaticObjectMethod($2, $3, (jlong)$1)";
            var excep = JNI.TryGetException(jni);
            if (excep!=null)
            {
                excep = new Exception (excep.Message +" !!!test!!!");
                throw excep;
            }
            return result;
        }

        [TargetSpecificImplementation]
        public static extern void SetWrapperType(JWrapper wrapper, ujobject obj, Uno.Type typePtr, bool typeHasFallbackClass, bool resolveType);
    }

    [TargetSpecificImplementation]
    public extern(ANDROID) static class ByteBuffer
    {
        [TargetSpecificImplementation]
        public static extern ujobject NewDirectByteBuffer(byte[] data);

        [TargetSpecificImplementation]
        public static extern ujobject NewDirectByteBuffer(sbyte[] data);

        [TargetSpecificImplementation]
        public static extern ujobject NewDirectByteBuffer(Uno.IntPtr data, long capacity);

        [TargetSpecificImplementation]
        public static extern bool ValidDirectByteBuffer(ujobject directByteBuffer);

        [TargetSpecificImplementation]
        public static extern IntPtr GetDirectBufferAddress(ujobject directByteBuffer);

        [TargetSpecificImplementation]
        public static extern long GetDirectBufferCapacity(ujobject directByteBuffer);

        public static DirectBuffer WrappedToUnoDirect(JWrapper directByteBuffer)
        {
            ujobject obj = directByteBuffer._GetJavaObject();
            var addr = GetDirectBufferAddress(obj);
            var len = GetDirectBufferCapacity(obj);
            return DirectBuffer.Create(addr, len, directByteBuffer);
        }
    }

    [TargetSpecificImplementation]
    public extern(ANDROID) static class String
    {
        public static string JavaToUno(IJWrapper jstr)
        {
            if (jstr!=null) {
                return JavaToUno(JNI.GetEnvPtr(), jstr._GetJavaObject());
            } else {
                return null;
            }
        }
        public static string JavaToUno(ujobject jstr)
        {
            if (jstr!=ujobject.Null) {
                return JavaToUno(JNI.GetEnvPtr(), jstr);
            } else {
                return null;
            }
        }
        public static ujobject UnoToJava(string str)
        {
            if (str!=null) {
                return UnoToJava(JNI.GetEnvPtr(), str);
            } else {
                return ujobject.Null;
            }
        }
        [TargetSpecificImplementation]
        public static extern string JavaToUno(JNIEnvPtr jni, ujobject jstr);
        [TargetSpecificImplementation]
        public static extern ujobject UnoToJava(JNIEnvPtr jni, string str);
    }

    [TargetSpecificImplementation]
    public extern(ANDROID) static class Arrays
    {
        public static bool[] JavaToUnoBoolArray(ujobject arr)
        {
            return JavaToUnoBoolArray(arr, JNI.GetArrayLength(arr));
        }
        public static sbyte[] JavaToUnoSByteArray(ujobject arr)
        {
            return JavaToUnoSByteArray(arr, JNI.GetArrayLength(arr));
        }
        public static byte[] JavaToUnoByteArray(ujobject arr)
        {
            return JavaToUnoByteArray(arr, JNI.GetArrayLength(arr));
        }
        public static char[] JavaToUnoCharArray(ujobject arr)
        {
            return JavaToUnoCharArray(arr, JNI.GetArrayLength(arr));
        }
        public static short[] JavaToUnoShortArray(ujobject arr)
        {
            return JavaToUnoShortArray(arr, JNI.GetArrayLength(arr));
        }
        public static int[] JavaToUnoIntArray(ujobject arr)
        {
            return JavaToUnoIntArray(arr, JNI.GetArrayLength(arr));
        }
        public static long[] JavaToUnoLongArray(ujobject arr)
        {
            return JavaToUnoLongArray(arr, JNI.GetArrayLength(arr));
        }
        public static float[] JavaToUnoFloatArray(ujobject arr)
        {
            return JavaToUnoFloatArray(arr, JNI.GetArrayLength(arr));
        }
        public static double[] JavaToUnoDoubleArray(ujobject arr)
        {
            return JavaToUnoDoubleArray(arr, JNI.GetArrayLength(arr));
        }

        [TargetSpecificImplementation]
        public static extern bool[] JavaToUnoBoolArray(ujobject arr, long len);
        [TargetSpecificImplementation]
        public static extern sbyte[] JavaToUnoSByteArray(ujobject arr, long len);
        [TargetSpecificImplementation]
        public static extern byte[] JavaToUnoByteArray(ujobject arr, long len);
        [TargetSpecificImplementation]
        public static extern char[] JavaToUnoCharArray(ujobject arr, long len);
        [TargetSpecificImplementation]
        public static extern short[] JavaToUnoShortArray(ujobject arr, long len);
        [TargetSpecificImplementation]
        public static extern int[] JavaToUnoIntArray(ujobject arr, long len);
        [TargetSpecificImplementation]
        public static extern long[] JavaToUnoLongArray(ujobject arr, long len);
        [TargetSpecificImplementation]
        public static extern float[] JavaToUnoFloatArray(ujobject arr, long len);
        [TargetSpecificImplementation]
        public static extern double[] JavaToUnoDoubleArray(ujobject arr, long len);
    }
}
