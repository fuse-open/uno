
using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Android.Base.Primitives;
using Android.Base.Wrappers;

namespace Android.Base
{
    [TargetSpecificImplementation]
    public extern(ANDROID) static class AndroidBindingMacros {} // See cpp.uxl for macros

    [TargetSpecificImplementation]
    public extern(ANDROID) static class JNI
    {
        static bool _inited;
        static jmethodID Activity_getClassLoader;
        static jmethodID ClassLoader_loadClass;
        static jmethodID _getUnoRefMid;
        static ujclass exceptionClass;
        static ujclass _helperCls;
        static JWrapper _rootActivityWrapped;

        public static void Init(ujobject activityObject)
        {
            if (!_inited)
            {
                _inited = true;
                var jni = GetEnvPtr();

                Activity_getClassLoader = extern<jmethodID>(jni) "$0->GetMethodID($0->FindClass(\"android/app/NativeActivity\"), \"getClassLoader\", \"()Ljava/lang/ClassLoader;\")";
                ClassLoader_loadClass = extern<jmethodID>(jni) "$0->GetMethodID($0->FindClass(\"java/lang/ClassLoader\"), \"loadClass\", \"(Ljava/lang/String;)Ljava/lang/Class;\")";
                CheckException(jni);

                extern(jni) "jstring jname = $0->NewStringUTF(\"com.foreign.UnoHelper\")";
                ujclass classLoader = extern<ujclass>(jni, activityObject, Activity_getClassLoader) "(jclass)$0->CallObjectMethod($1, $2)";
                var hcls = extern<ujclass>(jni, classLoader, ClassLoader_loadClass) "(jclass)$0->CallObjectMethod($1, $2, jname)";
                extern (jni, classLoader) "$0->DeleteLocalRef($1)";
                extern (jni) "$0->DeleteLocalRef(jname)";
                CheckException(jni);
                if (hcls == ujclass.Null)
                    throw new Exception("Could not cache class for UnoHelper");
                _helperCls = NewGlobalRef(hcls);

                exceptionClass = extern<ujclass>(jni) "NewGlobalRef($0->FindClass(\"java/lang/RuntimeException\"))";
                //exceptionClass = NewGlobalRef(LoadClass("java/lang/RuntimeException"));

                _getUnoRefMid = extern<jmethodID>(GetEnvPtr(), _helperCls)
                    "$0->GetStaticMethodID($1,\"GetUnoObjectRef\",\"(Ljava/lang/Object;)J\")";

                if (extern<bool>(_getUnoRefMid)"!$0")
                    throw new Exception("Could not cache getUnoRefMid");
            }
        }

        [TargetSpecificImplementation]
        public static extern JNIEnvPtr GetEnvPtr();

        [TargetSpecificImplementation]
        public static extern JavaVMPtr GetVM();

        [Foreign(Language.Java)]
        static Java.Object GetActivityObjectInner()
        @{
            return com.fuse.Activity.getRootActivity();
        @}

        [Foreign(Language.Java)]
        static Java.Object GetActivityClassInner()
        @{
            return @(Activity.Package).@(Activity.Name).class;
        @}

        public static extern ujclass GetActivityClass()
        {
            return ((IJWrapper)GetActivityClassInner())._GetJavaObject();
        }

        public static extern ujobject GetActivityObject()
        {
            return ((IJWrapper)GetActivityObjectInner())._GetJavaObject();
        }

        public static JWrapper GetWrappedActivityObject()
        {
            return (JWrapper)GetActivityObjectInner();
        }


        public static ujclass LoadClass(JNIEnvPtr jni, ConstCharPtr name)
        {
            return LoadClass(jni, name, false);
        }

        public enum RefType { Invalid=0, Local=1, Global=2, WeakGlobal=3 }

        public static JNI.RefType GetRefType(JNIEnvPtr jni, ujobject obj)
        {
            return (JNI.RefType)extern<int>(jni, obj) "(int)$0->GetObjectRefType($1)";
        }

        public static JNI.RefType GetRefType(ujobject obj)
        {
            return GetRefType(GetEnvPtr(), obj);
        }

        public static ujclass LoadClass(JNIEnvPtr jni, ConstCharPtr name, bool systemClass)
        {
            assert _inited;
            ujclass result;
            extern "jstring jname = $0->NewStringUTF($1)";
            if (systemClass) {
                result = extern<ujclass>(jni, name) "$0->FindClass($1)";
            } else {
                ujclass classLoader = extern<ujclass>(jni, GetActivityObject(), Activity_getClassLoader)
                    "(jclass)$0->CallObjectMethod($1, $2)";
                result = extern<ujclass>(jni, classLoader, ClassLoader_loadClass)
                    "(jclass)$0->CallObjectMethod($1, $2, jname)";
                extern (jni, classLoader) "$0->DeleteLocalRef($1)";
            }
            extern (jni) "$0->DeleteLocalRef(jname)";
            CheckException(jni);
            return result;
        }

        public static extern ujclass LoadClass(JNIEnvPtr jni, string name, bool systemClass=false)
        {
            var cname = extern<ConstCharPtr>(name) "uAllocCStr($0)";
            var result = LoadClass(GetEnvPtr(), cname, systemClass);
            extern (cname) "free(const_cast<char*>($0))";
            return result;
        }

        public static extern ujclass LoadClass(string name, bool systemClass=false)
        {
            return LoadClass(GetEnvPtr(), name, systemClass);
        }

        public static extern ujobject NewGlobalRef(ujobject obj)
        @{
            return reinterpret_cast<jobject>(@{GetEnvPtr():Call()}->NewGlobalRef($0));
        @}


        public static extern ujclass NewGlobalRef(ujclass obj)
        {
            return extern<ujclass>(GetEnvPtr(),obj) "reinterpret_cast<jclass>($0->NewGlobalRef($1))";
        }

        public static extern ujstring NewGlobalRef(ujstring obj)
        {
            return extern<ujstring>(GetEnvPtr(),obj) "reinterpret_cast<jstring>($0->NewGlobalRef($1))";
        }

        public static extern ujobject LocalToGlobalRef(ujobject obj)
        {
            var res = NewGlobalRef(obj);
            DeleteLocalRef(obj);
            return res;
        }
        public static extern ujstring LocalToGlobalRef(ujstring obj)
        {
            var res = NewGlobalRef(obj);
            DeleteLocalRef(obj);
            return res;
        }
        public static extern ujclass LocalToGlobalRef(ujclass obj)
        {
            var res = NewGlobalRef(obj);
            DeleteLocalRef(obj);
            return res;
        }

        [TargetSpecificImplementation]
        public static extern ujobject NewWeakGlobalRef(ujobject obj);

        [TargetSpecificImplementation]
        public static extern void DeleteGlobalRef(ujobject obj);

        [TargetSpecificImplementation]
        public static extern void DeleteWeakGlobalRef(ujobject obj);

        [TargetSpecificImplementation]
        public static extern bool IsSameObject(ujobject objA, ujobject objB);

        public static Exception TryGetException(JNIEnvPtr jni, string appendMessage=null)
        {
            extern(jni) "jthrowable err = $0->ExceptionOccurred()";
            if (extern<bool>"(err != nullptr)")
            {
                extern(jni) "$0->ExceptionDescribe()";
                extern(jni) "$0->ExceptionClear()";
                extern(jni) "jmethodID toString = $0->GetMethodID($0->FindClass(\"java/lang/Object\"), \"toString\", \"()Ljava/lang/String;\")";
                var estring = extern<ujobject>(jni)"(jobject)$0->CallObjectMethod(err, toString)";
                var x = Android.Base.Types.String.JavaToUno(estring);
                if (appendMessage!=null)
                    x += (x + "\n" + appendMessage);
                return new Exception(x);
            }
            return null;
        }

        public static extern void CheckException(JNIEnvPtr jni)
        {
            CheckException(jni, null);
        }

        public static extern void CheckException(JNIEnvPtr jni, string appendMessage=null)
        {
            var excep = TryGetException(jni, appendMessage);
            if (excep!=null)
                throw excep;
        }

        public static extern void CheckException()
        {
            CheckException(GetEnvPtr());
        }

        [TargetSpecificImplementation]
        public static extern void ThrowNewException(string message);

        public static jmethodID GetMethodID(ujclass cls, string methodName, string methodSig)
        {
            extern(methodName) "char* cMethodName = uAllocCStr($0)";
            extern(methodSig) "char* cMethodSig = uAllocCStr($0)";
            var mid = extern<jmethodID>(GetEnvPtr(),cls) "$0->GetMethodID($1,cMethodName,cMethodSig)";
            extern "free(cMethodName)";
            extern "free(cMethodSig)";
            JNI.CheckException();
            if (extern<bool>(mid) "$0 == 0")
                throw new Exception("Java method id for " + methodName + " is null");
            return mid;
        }
        public static jfieldID GetFieldID(ujclass cls, string fieldName, string fieldSig)
        {
            var env = GetEnvPtr();
            extern(fieldName) "char* cFieldName = uAllocCStr($0)";
            extern(fieldSig) "char* cFieldSig = uAllocCStr($0)";
            var fid = extern<jfieldID>(env,cls) "$0->GetFieldID($1,cFieldName,cFieldSig)";
            extern "free(cFieldName)";
            extern "free(cFieldSig)";
            CheckException(env);
            return fid;
        }

        public static jmethodID GetStaticMethodID(ujclass cls, string methodName, string methodSig)
        {
            extern(methodName) "char* cMethodName = uAllocCStr($0)";
            extern(methodSig) "char* cMethodSig = uAllocCStr($0)";
            var mid = extern<jmethodID>(GetEnvPtr(),cls) "$0->GetStaticMethodID($1,cMethodName,cMethodSig)";
            extern "free(cMethodName)";
            extern "free(cMethodSig)";
            return mid;
        }
        public static jfieldID GetStaticFieldID(ujclass cls, string fieldName, string fieldSig)
        {
            var env = GetEnvPtr();
            extern(fieldName) "char* cFieldName = uAllocCStr($0)";
            extern(fieldSig) "char* cFieldSig = uAllocCStr($0)";
            var fid = extern<jfieldID>(env,cls) "$0->GetStaticFieldID($1,cFieldName,cFieldSig)";
            extern "free(cFieldName)";
            extern "free(cFieldSig)";
            CheckException(env);
            return fid;
        }

        public static void DeleteLocalRef(ujobject obj)
        {
            extern "JNIEnv* jni = @{GetEnvPtr():Call()}";
            extern(obj) "jni->DeleteLocalRef($0)";
        }

        public static void DeleteLocalRef(ujclass obj)
        {
            extern "JNIEnv* jni = @{GetEnvPtr():Call()}";
            extern(obj) "jni->DeleteLocalRef($0)";
        }

        public static void DeleteLocalRef(ujstring obj)
        {
            extern "JNIEnv* jni = @{GetEnvPtr():Call()}";
            extern(obj) "jni->DeleteLocalRef($0)";
        }


        [TargetSpecificImplementation]
        public static extern ujobject GetDefaultObject();

        [TargetSpecificImplementation]
        public static extern Uno.Type GetDefaultType();


        public static extern long GetUnoRef(ujobject obj)
        {
            assert _inited;
            if (obj != ujobject.Null)
                return extern<long>(obj, _helperCls, _getUnoRefMid, Android.Base.JNI.GetEnvPtr())
                    "(@{long})$3->CallStaticLongMethod($1, $2, $0)";
            else
                return 0;
        }

        [TargetSpecificImplementation]
        public static extern int GetArrayLength(ujobject array);
        [TargetSpecificImplementation]
        public static extern ujobject NewBooleanArray (int len);
        [TargetSpecificImplementation]
        public static extern ujobject NewByteArray (int len);
        [TargetSpecificImplementation]
        public static extern ujobject NewCharArray (int len);
        [TargetSpecificImplementation]
        public static extern ujobject NewShortArray (int len);
        [TargetSpecificImplementation]
        public static extern ujobject NewIntArray (int len);
        [TargetSpecificImplementation]
        public static extern ujobject NewLongArray (int len);
        [TargetSpecificImplementation]
        public static extern ujobject NewFloatArray (int len);
        [TargetSpecificImplementation]
        public static extern ujobject NewDoubleArray (int len);
        [TargetSpecificImplementation]
        public static extern ujobject NewObjectArray (ujclass cls, int len);
        [TargetSpecificImplementation]
        public static extern bool GetBooleanArrayElement (IJWrapper obj, int i);
        [TargetSpecificImplementation]
        public static extern sbyte GetByteArrayElement (IJWrapper obj, int i);
        [TargetSpecificImplementation]
        public static extern char GetCharArrayElement (IJWrapper obj, int i);
        [TargetSpecificImplementation]
        public static extern short GetShortArrayElement (IJWrapper obj, int i);
        [TargetSpecificImplementation]
        public static extern int GetIntArrayElement (IJWrapper obj, int i);
        [TargetSpecificImplementation]
        public static extern long GetLongArrayElement (IJWrapper obj, int i);
        [TargetSpecificImplementation]
        public static extern float GetFloatArrayElement (IJWrapper obj, int i);
        [TargetSpecificImplementation]
        public static extern double GetDoubleArrayElement (IJWrapper obj, int i);
        [TargetSpecificImplementation]
        public static extern ujobject GetObjectArrayElement (IJWrapper obj, int i);
        [TargetSpecificImplementation]
        public static extern void SetBooleanArrayElement (IJWrapper obj, int i, bool val);
        [TargetSpecificImplementation]
        public static extern void SetByteArrayElement (IJWrapper obj, int i, sbyte val);
        [TargetSpecificImplementation]
        public static extern void SetByteArrayElement (IJWrapper obj, int i, byte val);
        [TargetSpecificImplementation]
        public static extern void SetCharArrayElement (IJWrapper obj, int i, char val);
        [TargetSpecificImplementation]
        public static extern void SetShortArrayElement (IJWrapper obj, int i, short val);
        [TargetSpecificImplementation]
        public static extern void SetIntArrayElement (IJWrapper obj, int i, int val);
        [TargetSpecificImplementation]
        public static extern void SetLongArrayElement (IJWrapper obj, int i, long val);
        [TargetSpecificImplementation]
        public static extern void SetFloatArrayElement (IJWrapper obj, int i, float val);
        [TargetSpecificImplementation]
        public static extern void SetDoubleArrayElement (IJWrapper obj, int i, double val);
        [TargetSpecificImplementation]
        public static extern void SetObjectArrayElement (IJWrapper obj, int i, ujobject val);

        public static void SetBooleanArrayRegion(ujobject javaArr, bool[] unoArr, int start=0, int len=-1)
        {
            if (len==-1)
                len = Math.Min(GetArrayLength(javaArr), unoArr.Length);
            extern "JNIEnv* jni = @{GetEnvPtr():Call()}";
            extern(javaArr, start, len, unoArr) "jni->SetBooleanArrayRegion((jbooleanArray)$0,$1,$2,(@{ujboolean}*)$3->_ptr)";
        }

        public static void SetByteArrayRegion(ujobject javaArr, sbyte[] unoArr, int start=0, int len=-1)
        {
            if (len==-1)
                len = Math.Min(GetArrayLength(javaArr), unoArr.Length);
            extern "JNIEnv* jni = @{GetEnvPtr():Call()}";
            extern(javaArr, start, len, unoArr) "jni->SetByteArrayRegion((jbyteArray)$0,$1,$2,(@{ujbyte}*)$3->_ptr)";
        }

        public static void SetByteArrayRegion(ujobject javaArr, byte[] unoArr, int start=0, int len=-1)
        {
            if (len==-1)
                len = Math.Min(GetArrayLength(javaArr), unoArr.Length);
            extern "JNIEnv* jni = @{GetEnvPtr():Call()}";
            extern(javaArr, start, len, unoArr) "jni->SetByteArrayRegion((jbyteArray)$0,$1,$2,(@{ujbyte}*)$3->_ptr)";
        }

        public static void SetCharArrayRegion(ujobject javaArr, char[] unoArr, int start=0, int len=-1)
        {
            if (len==-1)
                len = Math.Min(GetArrayLength(javaArr), unoArr.Length);
            extern "JNIEnv* jni = @{GetEnvPtr():Call()}";
            extern(javaArr, start, len, unoArr) "jni->SetCharArrayRegion((jcharArray)$0,$1,$2,(@{ujchar}*)$3->_ptr)";
        }

        public static void SetShortArrayRegion(ujobject javaArr, short[] unoArr, int start=0, int len=-1)
        {
            if (len==-1)
                len = Math.Min(GetArrayLength(javaArr), unoArr.Length);
            extern "JNIEnv* jni = @{GetEnvPtr():Call()}";
            extern(javaArr, start, len, unoArr) "jni->SetShortArrayRegion((jshortArray)$0,$1,$2,(@{ujshort}*)$3->_ptr)";
        }

        public static void SetIntArrayRegion(ujobject javaArr, int[] unoArr, int start=0, int len=-1)
        {
            if (len==-1)
                len = Math.Min(GetArrayLength(javaArr), unoArr.Length);
            extern "JNIEnv* jni = @{GetEnvPtr():Call()}";
            extern(javaArr, start, len, unoArr) "jni->SetIntArrayRegion((jintArray)$0,$1,$2,(@{ujint}*)$3->_ptr)";
        }

        public static void SetLongArrayRegion(ujobject javaArr, long[] unoArr, int start=0, int len=-1)
        {
            if (len==-1)
                len = Math.Min(GetArrayLength(javaArr), unoArr.Length);
            extern "JNIEnv* jni = @{GetEnvPtr():Call()}";
            extern(javaArr, start, len, unoArr) "jni->SetLongArrayRegion((jlongArray)$0,$1,$2,(@{ujlong}*)$3->_ptr)";
        }

        public static void SetFloatArrayRegion(ujobject javaArr, float[] unoArr, int start=0, int len=-1)
        {
            if (len==-1)
                len = Math.Min(GetArrayLength(javaArr), unoArr.Length);
            extern "JNIEnv* jni = @{GetEnvPtr():Call()}";
            extern(javaArr, start, len, unoArr) "jni->SetFloatArrayRegion((jfloatArray)$0,$1,$2,(@{ujfloat}*)$3->_ptr)";
        }

        public static void SetDoubleArrayRegion(ujobject javaArr, double[] unoArr, int start=0, int len=-1)
        {
            if (len==-1)
                len = Math.Min(GetArrayLength(javaArr), unoArr.Length);
            extern "JNIEnv* jni = @{GetEnvPtr():Call()}";
            extern(javaArr, start, len, unoArr) "jni->SetDoubleArrayRegion((jdoubleArray)$0,$1,$2,(@{ujdouble}*)$3->_ptr)";
        }

        public static extern ujobject NewObject(ujclass cls, jmethodID mtd, ujvalue arg0, ujvalue arg1, ujvalue arg2, ujvalue arg3, ujvalue arg4, ujvalue arg5, ujvalue arg6, ujvalue arg7, ujvalue arg8, ujvalue arg9, ujvalue arg10)
        {
            return extern<ujobject>(GetEnvPtr(),cls,mtd,arg0,arg1,arg2,arg3,arg4,arg5,arg6,arg7,arg8,arg9,arg10) "$0->NewObject($1,$2,$3,$4,$5,$6,$7,$8,$9,$10,$11,$12,$13)";
        }
        public static extern ujobject NewObject(ujclass cls, jmethodID mtd, ujvalue arg0, ujvalue arg1, ujvalue arg2, ujvalue arg3, ujvalue arg4, ujvalue arg5, ujvalue arg6, ujvalue arg7, ujvalue arg8, ujvalue arg9)
        {
            return extern<ujobject>(GetEnvPtr(),cls,mtd,arg0,arg1,arg2,arg3,arg4,arg5,arg6,arg7,arg8,arg9) "$0->NewObject($1,$2,$3,$4,$5,$6,$7,$8,$9,$10,$11,$12)";
        }
        public static extern ujobject NewObject(ujclass cls, jmethodID mtd, ujvalue arg0, ujvalue arg1, ujvalue arg2, ujvalue arg3, ujvalue arg4, ujvalue arg5, ujvalue arg6, ujvalue arg7, ujvalue arg8)
        {
            return extern<ujobject>(GetEnvPtr(),cls,mtd,arg0,arg1,arg2,arg3,arg4,arg5,arg6,arg7,arg8) "$0->NewObject($1,$2,$3,$4,$5,$6,$7,$8,$9,$10,$11)";
        }
        public static extern ujobject NewObject(ujclass cls, jmethodID mtd, ujvalue arg0, ujvalue arg1, ujvalue arg2, ujvalue arg3, ujvalue arg4, ujvalue arg5, ujvalue arg6, ujvalue arg7)
        {
            return extern<ujobject>(GetEnvPtr(),cls,mtd,arg0,arg1,arg2,arg3,arg4,arg5,arg6,arg7) "$0->NewObject($1,$2,$3,$4,$5,$6,$7,$8,$9,$10)";
        }
        public static extern ujobject NewObject(ujclass cls, jmethodID mtd, ujvalue arg0, ujvalue arg1, ujvalue arg2, ujvalue arg3, ujvalue arg4, ujvalue arg5, ujvalue arg6)
        {
            return extern<ujobject>(GetEnvPtr(),cls,mtd,arg0,arg1,arg2,arg3,arg4,arg5,arg6) "$0->NewObject($1,$2,$3,$4,$5,$6,$7,$8,$9)";
        }
        public static extern ujobject NewObject(ujclass cls, jmethodID mtd, ujvalue arg0, ujvalue arg1, ujvalue arg2, ujvalue arg3, ujvalue arg4, ujvalue arg5)
        {
            return extern<ujobject>(GetEnvPtr(),cls,mtd,arg0,arg1,arg2,arg3,arg4,arg5) "$0->NewObject($1,$2,$3,$4,$5,$6,$7,$8)";
        }
        public static extern ujobject NewObject(ujclass cls, jmethodID mtd, ujvalue arg0, ujvalue arg1, ujvalue arg2, ujvalue arg3, ujvalue arg4)
        {
            return extern<ujobject>(GetEnvPtr(),cls,mtd,arg0,arg1,arg2,arg3,arg4) "$0->NewObject($1,$2,$3,$4,$5,$6,$7)";
        }
        public static extern ujobject NewObject(ujclass cls, jmethodID mtd, ujvalue arg0, ujvalue arg1, ujvalue arg2, ujvalue arg3)
        {
            return extern<ujobject>(GetEnvPtr(),cls,mtd,arg0,arg1,arg2,arg3) "$0->NewObject($1,$2,$3,$4,$5,$6)";
        }
        public static extern ujobject NewObject(ujclass cls, jmethodID mtd, ujvalue arg0, ujvalue arg1, ujvalue arg2)
        {
            return extern<ujobject>(GetEnvPtr(),cls,mtd,arg0,arg1,arg2) "$0->NewObject($1,$2,$3,$4,$5)";
        }
        public static extern ujobject NewObject(ujclass cls, jmethodID mtd, ujvalue arg0, ujvalue arg1)
        {
            return extern<ujobject>(GetEnvPtr(),cls,mtd,arg0,arg1) "$0->NewObject($1,$2,$3,$4)";
        }
        public static extern ujobject NewObject(ujclass cls, jmethodID mtd, ujvalue arg0)
        {
            return extern<ujobject>(GetEnvPtr(),cls,mtd,arg0) "$0->NewObject($1,$2,$3)";
        }
        public static extern ujobject NewObject(ujclass cls, jmethodID mtd)
        {
            return extern<ujobject>(GetEnvPtr(),cls,mtd) "$0->NewObject($1,$2)";
        }
    }
}
