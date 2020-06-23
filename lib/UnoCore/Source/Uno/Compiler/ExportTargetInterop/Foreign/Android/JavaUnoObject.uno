using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Uno.Internal;
using Android.Base.Primitives;
using Android.Base;

namespace Uno.Compiler.ExportTargetInterop.Foreign.Android
{

    [TargetSpecificImplementation]
    public extern(Android) static class JavaUnoObject
    {
        static ujclass _unoObjectClass;
        static jmethodID _unoObjectConstructor;
        static jmethodID _unoObjectGetRetainedUnoPtr;

        static ujclass _unoBoolArrayClass;
        static jmethodID _unoBoolArrayFactoryMethod;

        static ujclass _unoByteArrayClass;
        static jmethodID _unoByteArrayFactoryMethod;

        static ujclass _unoShortArrayClass;
        static jmethodID _unoShortArrayFactoryMethod;

        static ujclass _unoIntArrayClass;
        static jmethodID _unoIntArrayFactoryMethod;

        static ujclass _unoLongArrayClass;
        static jmethodID _unoLongArrayFactoryMethod;

        static ujclass _unoFloatArrayClass;
        static jmethodID _unoFloatArrayFactoryMethod;

        static ujclass _unoDoubleArrayClass;
        static jmethodID _unoDoubleArrayFactoryMethod;

        static ujclass _unoStringArrayClass;
        static jmethodID _unoStringArrayFactoryMethod;

        static ujclass _unoObjectArrayClass;
        static jmethodID _unoObjectArrayFactoryMethod;

        static void EnsureInitialized()
        {
            if (extern<bool>"uEnterCriticalIfNull(&@{_unoObjectClass})")
            {
                _unoObjectClass = JNI.NewGlobalRef(JNI.LoadClass("com/uno/UnoObject"));
                _unoObjectConstructor = JNI.GetMethodID(_unoObjectClass, "<init>", "(J)V");
                _unoObjectGetRetainedUnoPtr = JNI.GetMethodID(_unoObjectClass, "_GetRetainedUnoPtr", "()J");

                _unoBoolArrayClass = JNI.NewGlobalRef(JNI.LoadClass("com/uno/BoolArray"));
                _unoBoolArrayFactoryMethod = JNI.GetStaticMethodID(_unoBoolArrayClass, "InitFromUnoPtr", "(J)Lcom/uno/BoolArray;");

                _unoByteArrayClass = JNI.NewGlobalRef(JNI.LoadClass("com/uno/ByteArray"));
                _unoByteArrayFactoryMethod = JNI.GetStaticMethodID(_unoByteArrayClass, "InitFromUnoPtr", "(JZ)Lcom/uno/ByteArray;");

                _unoShortArrayClass = JNI.NewGlobalRef(JNI.LoadClass("com/uno/ShortArray"));
                _unoShortArrayFactoryMethod = JNI.GetStaticMethodID(_unoShortArrayClass, "InitFromUnoPtr", "(J)Lcom/uno/ShortArray;");

                _unoIntArrayClass = JNI.NewGlobalRef(JNI.LoadClass("com/uno/IntArray"));
                _unoIntArrayFactoryMethod = JNI.GetStaticMethodID(_unoIntArrayClass, "InitFromUnoPtr", "(J)Lcom/uno/IntArray;");

                _unoLongArrayClass = JNI.NewGlobalRef(JNI.LoadClass("com/uno/LongArray"));
                _unoLongArrayFactoryMethod = JNI.GetStaticMethodID(_unoLongArrayClass, "InitFromUnoPtr", "(J)Lcom/uno/LongArray;");

                _unoFloatArrayClass = JNI.NewGlobalRef(JNI.LoadClass("com/uno/FloatArray"));
                _unoFloatArrayFactoryMethod = JNI.GetStaticMethodID(_unoFloatArrayClass, "InitFromUnoPtr", "(J)Lcom/uno/FloatArray;");

                _unoDoubleArrayClass = JNI.NewGlobalRef(JNI.LoadClass("com/uno/DoubleArray"));
                _unoDoubleArrayFactoryMethod = JNI.GetStaticMethodID(_unoDoubleArrayClass, "InitFromUnoPtr", "(J)Lcom/uno/DoubleArray;");

                _unoStringArrayClass = JNI.NewGlobalRef(JNI.LoadClass("com/uno/StringArray"));
                _unoStringArrayFactoryMethod = JNI.GetStaticMethodID(_unoStringArrayClass, "InitFromUnoPtr", "(J)Lcom/uno/StringArray;");

                _unoObjectArrayClass = JNI.NewGlobalRef(JNI.LoadClass("com/uno/ObjectArray"));
                _unoObjectArrayFactoryMethod = JNI.GetStaticMethodID(_unoObjectArrayClass, "InitFromUnoPtr", "(J)Lcom/uno/ObjectArray;");

                extern "JNINativeMethod nativeFunc = {(char* const)\"Finalize\", (char* const)\"(J)V\", (void *)&__JavaUnoObject_Finalizer}";
                var attached = extern<int>(JNI.GetEnvPtr(), _unoObjectClass) "$0->RegisterNatives($1, &nativeFunc, 1)";

                if (attached<0)
                    throw new Exception("Could not register the finalizer callback for JavaUnoObject");

                extern "uExitCritical();";
            }
        }

        public static ujobject Box(object unoObject)
        {
            if (unoObject==null) return ujobject.Null;
            EnsureInitialized();
            extern "uRetain($0)";
            var longPtr = extern<long> "(@{long})$0";
            var boxed = extern<ujobject>(JNI.GetEnvPtr(),_unoObjectClass, _unoObjectConstructor,longPtr) "$0->NewObject($1,$2,$3)";
            JNI.CheckException();
            return boxed;
        }

        public static ujobject Box(sbyte[] unoArray)
        {
            if (unoArray==null) return ujobject.Null;
            EnsureInitialized();
            extern "uRetain($0)";
            var longPtr = extern<long> "(@{long})$0";
            var boxed = extern<ujobject>(JNI.GetEnvPtr(),_unoByteArrayClass,_unoByteArrayFactoryMethod,longPtr,false) "$0->CallStaticObjectMethod($1,$2,$3,$4)";
            JNI.CheckException();
            return boxed;
        }

        public static ujobject Box(byte[] unoArray)
        {
            if (unoArray==null) return ujobject.Null;
            EnsureInitialized();
            extern "uRetain($0)";
            var longPtr = extern<long> "(@{long})$0";
            var boxed = extern<ujobject>(JNI.GetEnvPtr(),_unoByteArrayClass,_unoByteArrayFactoryMethod,longPtr,true) "$0->CallStaticObjectMethod($1,$2,$3,$4)";
            JNI.CheckException();
            return boxed;
        }

        public static ujobject Box(bool[] unoArray)
        {
            if (unoArray==null) return ujobject.Null;
            EnsureInitialized();
            extern "uRetain($0)";
            var longPtr = extern<long> "(@{long})$0";
            var boxed = extern<ujobject>(JNI.GetEnvPtr(),_unoBoolArrayClass, _unoBoolArrayFactoryMethod,longPtr) "$0->CallStaticObjectMethod($1,$2,$3)";
            JNI.CheckException();
            return boxed;
        }

        public static ujobject Box(short[] unoArray)
        {
            if (unoArray==null) return ujobject.Null;
            EnsureInitialized();
            extern "uRetain($0)";
            var longPtr = extern<long> "(@{long})$0";
            var boxed = extern<ujobject>(JNI.GetEnvPtr(),_unoShortArrayClass, _unoShortArrayFactoryMethod,longPtr) "$0->CallStaticObjectMethod($1,$2,$3)";
            JNI.CheckException();
            return boxed;
        }

        public static ujobject Box(int[] unoArray)
        {
            if (unoArray==null) return ujobject.Null;
            EnsureInitialized();
            extern "uRetain($0)";
            var longPtr = extern<long> "(@{long})$0";
            var boxed = extern<ujobject>(JNI.GetEnvPtr(),_unoIntArrayClass, _unoIntArrayFactoryMethod,longPtr) "$0->CallStaticObjectMethod($1,$2,$3)";
            JNI.CheckException();
            return boxed;
        }

        public static ujobject Box(long[] unoArray)
        {
            if (unoArray==null) return ujobject.Null;
            EnsureInitialized();
            extern "uRetain($0)";
            var longPtr = extern<long> "(@{long})$0";
            var boxed = extern<ujobject>(JNI.GetEnvPtr(),_unoLongArrayClass, _unoLongArrayFactoryMethod,longPtr) "$0->CallStaticObjectMethod($1,$2,$3)";
            JNI.CheckException();
            return boxed;
        }

        public static ujobject Box(float[] unoArray)
        {
            if (unoArray==null) return ujobject.Null;
            EnsureInitialized();
            extern "uRetain($0)";
            var longPtr = extern<long> "(@{long})$0";
            var boxed = extern<ujobject>(JNI.GetEnvPtr(),_unoFloatArrayClass, _unoFloatArrayFactoryMethod,longPtr) "$0->CallStaticObjectMethod($1,$2,$3)";
            JNI.CheckException();
            return boxed;
        }

        public static ujobject Box(double[] unoArray)
        {
            if (unoArray==null) return ujobject.Null;
            EnsureInitialized();
            extern "uRetain($0)";
            var longPtr = extern<long> "(@{long})$0";
            var boxed = extern<ujobject>(JNI.GetEnvPtr(),_unoDoubleArrayClass, _unoDoubleArrayFactoryMethod,longPtr) "$0->CallStaticObjectMethod($1,$2,$3)";
            JNI.CheckException();
            return boxed;
        }

        public static ujobject Box(string[] unoArray)
        {
            if (unoArray==null) return ujobject.Null;
            EnsureInitialized();
            extern "uRetain($0)";
            var longPtr = extern<long> "(@{long})$0";
            var boxed = extern<ujobject>(JNI.GetEnvPtr(),_unoStringArrayClass, _unoStringArrayFactoryMethod,longPtr) "$0->CallStaticObjectMethod($1,$2,$3)";
            JNI.CheckException();
            return boxed;
        }

        public static ujobject Box(object[] unoArray)
        {
            if (unoArray==null) return ujobject.Null;
            EnsureInitialized();
            extern "uRetain($0)";
            var longPtr = extern<long> "(@{long})$0";
            var boxed = extern<ujobject>(JNI.GetEnvPtr(),_unoObjectArrayClass, _unoObjectArrayFactoryMethod,longPtr) "$0->CallStaticObjectMethod($1,$2,$3)";
            JNI.CheckException();
            return boxed;
        }

        public static ujobject BoxDelegate(object delegateObj, ConstCharPtr javaClassName)
        {
            if (delegateObj==null) return ujobject.Null;
            EnsureInitialized();

            var delegateJavaClass = JNI.LoadClass(JNI.GetEnvPtr(), javaClassName);
            var constructor = JNI.GetMethodID(_unoObjectArrayClass, "<init>", "(J)V");

            extern "uRetain($0)";
            var longPtr = extern<long> "(@{long})$0";
            var boxed = extern<ujobject>(JNI.GetEnvPtr(),delegateJavaClass,constructor,longPtr) "$0->NewObject($1,$2,$3)";

            JNI.DeleteLocalRef(delegateJavaClass);
            JNI.CheckException();
            return boxed;
        }

        public static object UnBox(ujobject javaObject)
        {
            if (JNI.IsSameObject(javaObject, ujobject.Null)) return null;
            var longPtr = extern<long>(JNI.GetEnvPtr(),javaObject,_unoObjectGetRetainedUnoPtr) "(@{long})$0->CallLongMethod($1,$2)";
            JNI.CheckException();
            return extern<object>(longPtr) "(@{object})$0";
        }

        public static object UnBoxFreeingLocalRef(ujobject javaObject)
        {
            if (JNI.IsSameObject(javaObject, ujobject.Null)) return null;
            var longPtr = extern<long>(JNI.GetEnvPtr(),javaObject,_unoObjectGetRetainedUnoPtr) "(@{long})$0->CallLongMethod($1,$2)";
            JNI.CheckException();
            if (JNI.GetRefType(javaObject) == JNI.RefType.Local)
                JNI.DeleteLocalRef(javaObject);
            return extern<object>(longPtr) "(@{object})$0";
        }
    }
}
