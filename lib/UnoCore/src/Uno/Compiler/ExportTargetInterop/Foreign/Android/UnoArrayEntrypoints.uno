using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;
using Uno.Compiler.ExportTargetInterop.Android;
using Android.Base.Wrappers;

namespace Uno.Compiler.ExportTargetInterop.Foreign.Android
{
    [TargetSpecificImplementation]
    public extern(Android) static class JavaToUnoArrayEntrypoints
    {
        public static bool GetBool(object array, int index)
        {
            return ((bool[])array)[index];
        }
        public static sbyte GetByte(object array, int index)
        {
            return ((sbyte[])array)[index];
        }
        public static sbyte GetUByte(object array, int index)
        {
            return (sbyte)((byte[])array)[index];
        }
        public static char GetChar(object array, int index)
        {
            return ((char[])array)[index];
        }
        public static short GetShort(object array, int index)
        {
            return ((short[])array)[index];
        }
        public static int GetInt(object array, int index)
        {
            return ((int[])array)[index];
        }
        public static long GetLong(object array, int index)
        {
            return ((long[])array)[index];
        }
        public static float GetFloat(object array, int index)
        {
            return ((float[])array)[index];
        }
        public static double GetDouble(object array, int index)
        {
            return ((double[])array)[index];
        }
        public static string GetString(object array, int index)
        {
            return ((string[])array)[index];
        }
        public static object GetObject(object array, int index)
        {
            return ((object[])array)[index];
        }

        public static bool SetBool(object array, int index, bool val)
        {
            return ((bool[])array)[index] = val;
        }
        public static sbyte SetByte(object array, int index, sbyte val)
        {
            return ((sbyte[])array)[index] = val;
        }
        public static sbyte SetUByte(object array, int index, sbyte val)
        {
            ((byte[])array)[index] = (byte)val;
            return val;
        }
        public static char SetChar(object array, int index, char val)
        {
            return ((char[])array)[index] = val;
        }
        public static short SetShort(object array, int index, short val)
        {
            return ((short[])array)[index] = val;
        }
        public static int SetInt(object array, int index, int val)
        {
            return ((int[])array)[index] = val;
        }
        public static long SetLong(object array, int index, long val)
        {
            return ((long[])array)[index] = val;
        }
        public static float SetFloat(object array, int index, float val)
        {
            return ((float[])array)[index] = val;
        }
        public static double SetDouble(object array, int index, double val)
        {
            return ((double[])array)[index] = val;
        }
        public static string SetString(object array, int index, string val)
        {
            return ((string[])array)[index] = val;
        }
        public static object SetObject(object array, int index, object val)
        {
            return ((object[])array)[index] = val;
        }

        public static Java.Object CopyBoolArray(object array)
        {
            var env = global::Android.Base.JNI.GetEnvPtr();
            var arr = ((bool[])array);
            var jarr = extern<global::Android.Base.Primitives.ujobject>(env, arr.Length) "$0->NewBooleanArray((jsize)$1)";
            global::Android.Base.JNI.SetBooleanArrayRegion(jarr, arr);
            return global::Android.Base.Wrappers.JWrapper.Wrap(jarr);
        }

        public static Java.Object CopyByteArray(object array)
        {
            var env = global::Android.Base.JNI.GetEnvPtr();
            var arr = ((sbyte[])array);
            var jarr = extern<global::Android.Base.Primitives.ujobject>(env, arr.Length) "$0->NewByteArray((jsize)$1)";
            global::Android.Base.JNI.SetByteArrayRegion(jarr, arr);
            return global::Android.Base.Wrappers.JWrapper.Wrap(jarr);
        }

        public static Java.Object CopyUByteArray(object array)
        {
            var env = global::Android.Base.JNI.GetEnvPtr();
            var arr = ((byte[])array);
            var jarr = extern<global::Android.Base.Primitives.ujobject>(env, arr.Length) "$0->NewByteArray((jsize)$1)";
            global::Android.Base.JNI.SetByteArrayRegion(jarr, arr);
            return global::Android.Base.Wrappers.JWrapper.Wrap(jarr);
        }

        public static Java.Object CopyCharArray(object array)
        {
            var env = global::Android.Base.JNI.GetEnvPtr();
            var arr = ((char[])array);
            var jarr = extern<global::Android.Base.Primitives.ujobject>(env, arr.Length) "$0->NewCharArray((jsize)$1)";
            global::Android.Base.JNI.SetCharArrayRegion(jarr, arr);
            return global::Android.Base.Wrappers.JWrapper.Wrap(jarr);
        }

        public static Java.Object CopyShortArray(object array)
        {
            var env = global::Android.Base.JNI.GetEnvPtr();
            var arr = ((short[])array);
            var jarr = extern<global::Android.Base.Primitives.ujobject>(env, arr.Length) "$0->NewShortArray((jsize)$1)";
            global::Android.Base.JNI.SetShortArrayRegion(jarr, arr);
            return global::Android.Base.Wrappers.JWrapper.Wrap(jarr);
        }

        public static Java.Object CopyIntArray(object array)
        {
            var env = global::Android.Base.JNI.GetEnvPtr();
            var arr = ((int[])array);
            var jarr = extern<global::Android.Base.Primitives.ujobject>(env, arr.Length) "$0->NewIntArray((jsize)$1)";
            global::Android.Base.JNI.SetIntArrayRegion(jarr, arr);
            return global::Android.Base.Wrappers.JWrapper.Wrap(jarr);
        }

        public static Java.Object CopyLongArray(object array)
        {
            var env = global::Android.Base.JNI.GetEnvPtr();
            var arr = ((long[])array);
            var jarr = extern<global::Android.Base.Primitives.ujobject>(env, arr.Length) "$0->NewLongArray((jsize)$1)";
            global::Android.Base.JNI.SetLongArrayRegion(jarr, arr);
            return global::Android.Base.Wrappers.JWrapper.Wrap(jarr);
        }

        public static Java.Object CopyFloatArray(object array)
        {
            var env = global::Android.Base.JNI.GetEnvPtr();
            var arr = ((float[])array);
            var jarr = extern<global::Android.Base.Primitives.ujobject>(env, arr.Length) "$0->NewFloatArray((jsize)$1)";
            global::Android.Base.JNI.SetFloatArrayRegion(jarr, arr);
            return global::Android.Base.Wrappers.JWrapper.Wrap(jarr);
        }

        public static Java.Object CopyDoubleArray(object array)
        {
            var env = global::Android.Base.JNI.GetEnvPtr();
            var arr = ((double[])array);
            var jarr = extern<global::Android.Base.Primitives.ujobject>(env, arr.Length) "$0->NewDoubleArray((jsize)$1)";
            global::Android.Base.JNI.SetDoubleArrayRegion(jarr, arr);
            return global::Android.Base.Wrappers.JWrapper.Wrap(jarr);
        }

        static global::Android.Base.Primitives.ujclass _stringClass;

        public static Java.Object CopyStringArray(object array)
        {
            var env = global::Android.Base.JNI.GetEnvPtr();
            if (_stringClass==global::Android.Base.Primitives.ujclass.Null)
                _stringClass = global::Android.Base.JNI.NewGlobalRef(global::Android.Base.JNI.LoadClass("java/lang/String", true));

            var arr = ((string[])array);
            var jarr = extern<global::Android.Base.Primitives.ujobject>(env, _stringClass, arr.Length) "$0->NewObjectArray((jsize)$2, $1, nullptr)";
            for (int i=0; i<arr.Length; i++)
            {
                var s = global::Android.Base.Types.String.UnoToJava(arr[i]);
                extern(env, jarr, i, s) "$0->SetObjectArrayElement((jobjectArray)$1, $2, (jobject)$3)";
                global::Android.Base.JNI.DeleteLocalRef(s);
            }

            return global::Android.Base.Wrappers.JWrapper.Wrap(jarr);
        }

        static global::Android.Base.Primitives.ujclass _objectClass;

        public static Java.Object CopyObjectArray(object array)
        {
            var env = global::Android.Base.JNI.GetEnvPtr();
            if (_objectClass==global::Android.Base.Primitives.ujclass.Null)
                _objectClass = global::Android.Base.JNI.NewGlobalRef(global::Android.Base.JNI.LoadClass("java/lang/Object", true));

            var arr = ((object[])array);
            var jarr = extern<global::Android.Base.Primitives.ujobject>(env, _stringClass, arr.Length) "$0->NewObjectArray((jsize)$2, $1, nullptr)";
            for (int i=0; i<arr.Length; i++)
            {
                var o = JavaUnoObject.Box((object)arr[i]);
                extern(env, jarr, i, o) "$0->SetObjectArrayElement((jobjectArray)$1, $2, $3)";
                global::Android.Base.JNI.DeleteLocalRef(o);
            }
            return global::Android.Base.Wrappers.JWrapper.Wrap(jarr);
        }


        [Foreign(Language.Java), ForeignFixedName]
        public static bool getBool(object array, int index)
        @{
            return @{GetBool(object,int):Call(array, index)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static sbyte getByte(object array, int index)
        @{
            return @{GetByte(object,int):Call(array, index)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static sbyte getUByte(object array, int index)
        @{
            return @{GetUByte(object,int):Call(array, index)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static char getChar(object array, int index)
        @{
            return @{GetChar(object,int):Call(array, index)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static short getShort(object array, int index)
        @{
            return @{GetShort(object,int):Call(array, index)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static int getInt(object array, int index)
        @{
            return @{GetInt(object,int):Call(array, index)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long getLong(object array, int index)
        @{
            return @{GetLong(object,int):Call(array, index)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static float getFloat(object array, int index)
        @{
            return @{GetFloat(object,int):Call(array, index)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static double getDouble(object array, int index)
        @{
            return @{GetDouble(object,int):Call(array, index)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static string getString(object array, int index)
        @{
            return @{GetString(object,int):Call(array, index)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static object getObject(object array, int index)
        @{
            return @{GetObject(object,int):Call(array, index)};
        @}



        [Foreign(Language.Java), ForeignFixedName]
        public static bool setBool(object array, int index, bool val)
        @{
            return @{SetBool(object,int,bool):Call(array, index, val)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static sbyte setByte(object array, int index, sbyte val)
        @{
            return @{SetByte(object,int,sbyte):Call(array, index, val)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static sbyte setUByte(object array, int index, sbyte val)
        @{
            return @{SetUByte(object,int,sbyte):Call(array, index, val)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static char setChar(object array, int index, char val)
        @{
            return @{SetChar(object,int,char):Call(array, index, val)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static short setShort(object array, int index, short val)
        @{
            return @{SetShort(object,int,short):Call(array, index, val)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static int setInt(object array, int index, int val)
        @{
            return @{SetInt(object,int,int):Call(array, index, val)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long setLong(object array, int index, long val)
        @{
            return @{SetLong(object,int,long):Call(array, index, val)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static float setFloat(object array, int index, float val)
        @{
            return @{SetFloat(object,int,float):Call(array, index, val)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static double setDouble(object array, int index, double val)
        @{
            return @{SetDouble(object,int,double):Call(array, index, val)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static string setString(object array, int index, string val)
        @{
            return @{SetString(object,int,string):Call(array, index, val)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static object setObject(object array, int index, object val)
        @{
            return @{SetObject(object,int,object):Call(array, index, val)};
        @}


        [Foreign(Language.Java), ForeignFixedName]
        public static Java.Object copyBoolArray(object array)
        @{
            return @{CopyBoolArray(object):Call(array)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static Java.Object copyByteArray(object array)
        @{
            return @{CopyByteArray(object):Call(array)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static Java.Object copyUByteArray(object array)
        @{
            return @{CopyUByteArray(object):Call(array)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static Java.Object copyCharArray(object array)
        @{
            return @{CopyCharArray(object):Call(array)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static Java.Object copyShortArray(object array)
        @{
            return @{CopyShortArray(object):Call(array)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static Java.Object copyIntArray(object array)
        @{
            return @{CopyIntArray(object):Call(array)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static Java.Object copyLongArray(object array)
        @{
            return @{CopyLongArray(object):Call(array)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static Java.Object copyFloatArray(object array)
        @{
            return @{CopyFloatArray(object):Call(array)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static Java.Object copyDoubleArray(object array)
        @{
            return @{CopyDoubleArray(object):Call(array)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static Java.Object copyStringArray(object array)
        @{
            return @{CopyStringArray(object):Call(array)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static Java.Object copyObjectArray(object array)
        @{
            return @{CopyObjectArray(object):Call(array)};
        @}



        [Foreign(Language.Java), ForeignFixedName]
        public static long newBoolArrayPtr(int length)
        @{
            return @{NewBoolArrayPtr(int):Call(length)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long newByteArrayPtr(int length, bool unoIsUnsigned)
        @{
            return @{NewByteArrayPtr(int,bool):Call(length, unoIsUnsigned)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long newCharArrayPtr(int length)
        @{
            return @{NewCharArrayPtr(int):Call(length)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long newShortArrayPtr(int length)
        @{
            return @{NewShortArrayPtr(int):Call(length)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long newIntArrayPtr(int length)
        @{
            return @{NewIntArrayPtr(int):Call(length)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long newLongArrayPtr(int length)
        @{
            return @{NewLongArrayPtr(int):Call(length)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long newFloatArrayPtr(int length)
        @{
            return @{NewFloatArrayPtr(int):Call(length)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long newDoubleArrayPtr(int length)
        @{
            return @{NewDoubleArrayPtr(int):Call(length)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long newStringArrayPtr(int length)
        @{
            return @{NewStringArrayPtr(int):Call(length)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long newObjectArrayPtr(int length)
        @{
            return @{NewObjectArrayPtr(int):Call(length)};
        @}

        public static long NewBoolArrayPtr(int length)
        {
            var arr = new bool[length];
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long NewByteArrayPtr(int length, bool unoIsUnsigned)
        {
            object arr;
            if (unoIsUnsigned)
                arr = (object)new byte[length];
            else
                arr = (object)new sbyte[length];
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long NewCharArrayPtr(int length)
        {
            var arr = new char[length];
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long NewShortArrayPtr(int length)
        {
            var arr = new short[length];
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long NewIntArrayPtr(int length)
        {
            var arr = new int[length];
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long NewLongArrayPtr(int length)
        {
            var arr = new long[length];
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long NewFloatArrayPtr(int length)
        {
            var arr = new float[length];
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long NewDoubleArrayPtr(int length)
        {
            var arr = new double[length];
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long NewStringArrayPtr(int length)
        {
            var arr = new string[length];
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long NewObjectArrayPtr(int length)
        {
            var arr = new object[length];
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        //----------------------------------------------------------------------

        [Foreign(Language.Java), ForeignFixedName]
        public static long boolArrayToUnoArrayPtr(Java.Object arr)
        @{
            return @{BoolArrayToUnoArrayPtr(Java.Object):Call(arr)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long charArrayToUnoArrayPtr(Java.Object arr)
        @{
            return @{CharArrayToUnoArrayPtr(Java.Object):Call(arr)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long byteArrayToUnoArrayPtr(Java.Object arr)
        @{
            return @{ByteArrayToUnoArrayPtr(Java.Object):Call(arr)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long shortArrayToUnoArrayPtr(Java.Object arr)
        @{
            return @{ShortArrayToUnoArrayPtr(Java.Object):Call(arr)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long intArrayToUnoArrayPtr(Java.Object arr)
        @{
            return @{IntArrayToUnoArrayPtr(Java.Object):Call(arr)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long longArrayToUnoArrayPtr(Java.Object arr)
        @{
            return @{LongArrayToUnoArrayPtr(Java.Object):Call(arr)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long floatArrayToUnoArrayPtr(Java.Object arr)
        @{
            return @{FloatArrayToUnoArrayPtr(Java.Object):Call(arr)};
        @}

        [Foreign(Language.Java), ForeignFixedName]
        public static long doubleArrayToUnoArrayPtr(Java.Object arr)
        @{
            return @{DoubleArrayToUnoArrayPtr(Java.Object):Call(arr)};
        @}

        public static long BoolArrayToUnoArrayPtr(Java.Object jarr)
        {
            // get the jarray and length
            extern(((JWrapper)jarr)._GetJavaObject()) "jbooleanArray obj = static_cast<jbooleanArray>($0)";
            var env = global::Android.Base.JNI.GetEnvPtr();
            int len = extern<int>(env)"(int)$0->GetArrayLength(obj)";

            // make the uno array and copy in the data
            var arr = new bool[len];
            extern(env,len,arr)"$0->GetBooleanArrayRegion(obj, (jsize)0, (jsize)$1, (jboolean*)$2->_ptr)";
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long CharArrayToUnoArrayPtr(Java.Object jarr)
        {
            // get the jarray and length
            extern(((JWrapper)jarr)._GetJavaObject()) "jcharArray obj = static_cast<jcharArray>($0)";
            var env = global::Android.Base.JNI.GetEnvPtr();
            int len = extern<int>(env)"(int)$0->GetArrayLength(obj)";

            // make the uno array and copy in the data
            var arr = new char[len];
            extern(env,len,arr)"$0->GetCharArrayRegion(obj, (jsize)0, (jsize)$1, (jchar*)$2->_ptr)";
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long ByteArrayToUnoArrayPtr(Java.Object jarr)
        {
            // get the jarray and length
            extern(((JWrapper)jarr)._GetJavaObject()) "jbyteArray obj = static_cast<jbyteArray>($0)";
            var env = global::Android.Base.JNI.GetEnvPtr();
            int len = extern<int>(env)"(int)$0->GetArrayLength(obj)";

            // make the uno array and copy in the data
            var arr = new byte[len];
            extern(env,len,arr)"$0->GetByteArrayRegion(obj, (jsize)0, (jsize)$1, (jbyte*)$2->_ptr)";
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long ShortArrayToUnoArrayPtr(Java.Object jarr)
        {
            // get the jarray and length
            extern(((JWrapper)jarr)._GetJavaObject()) "jshortArray obj = static_cast<jshortArray>($0)";
            var env = global::Android.Base.JNI.GetEnvPtr();
            int len = extern<int>(env)"(int)$0->GetArrayLength(obj)";

            // make the uno array and copy in the data
            var arr = new short[len];
            extern(env,len,arr)"$0->GetShortArrayRegion(obj, (jsize)0, (jsize)$1, (jshort*)$2->_ptr)";
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long IntArrayToUnoArrayPtr(Java.Object jarr)
        {
            // get the jarray and length
            extern(((JWrapper)jarr)._GetJavaObject()) "jintArray obj = static_cast<jintArray>($0)";
            var env = global::Android.Base.JNI.GetEnvPtr();
            int len = extern<int>(env)"(int)$0->GetArrayLength(obj)";

            // make the uno array and copy in the data
            var arr = new int[len];
            extern(env,len,arr)"$0->GetIntArrayRegion(obj, (jsize)0, (jsize)$1, (jint*)$2->_ptr)";
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long LongArrayToUnoArrayPtr(Java.Object jarr)
        {
            // get the jarray and length
            extern(((JWrapper)jarr)._GetJavaObject()) "jlongArray obj = static_cast<jlongArray>($0)";
            var env = global::Android.Base.JNI.GetEnvPtr();
            int len = extern<int>(env)"(int)$0->GetArrayLength(obj)";

            // make the uno array and copy in the data
            var arr = new long[len];
            extern(env,len,arr)"$0->GetLongArrayRegion(obj, (jsize)0, (jsize)$1, (jlong*)$2->_ptr)";
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long FloatArrayToUnoArrayPtr(Java.Object jarr)
        {
            // get the jarray and length
            extern(((JWrapper)jarr)._GetJavaObject()) "jfloatArray obj = static_cast<jfloatArray>($0)";
            var env = global::Android.Base.JNI.GetEnvPtr();
            int len = extern<int>(env)"(int)$0->GetArrayLength(obj)";

            // make the uno array and copy in the data
            var arr = new float[len];
            extern(env,len,arr)"$0->GetFloatArrayRegion(obj, (jsize)0, (jsize)$1, (jfloat*)$2->_ptr)";
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }

        public static long DoubleArrayToUnoArrayPtr(Java.Object jarr)
        {
            // get the jarray and length
            extern(((JWrapper)jarr)._GetJavaObject()) "jdoubleArray obj = static_cast<jdoubleArray>($0)";
            var env = global::Android.Base.JNI.GetEnvPtr();
            int len = extern<int>(env)"(int)$0->GetArrayLength(obj)";

            // make the uno array and copy in the data
            var arr = new double[len];
            extern(env,len,arr)"$0->GetDoubleArrayRegion(obj, (jsize)0, (jsize)$1, (jdouble*)$2->_ptr)";
            extern(arr) "uRetain($0)";
            return extern<long>(arr) "(@{long})$0";
        }
    }
}
