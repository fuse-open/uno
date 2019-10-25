using Uno.Compiler.ExportTargetInterop;
using Android.Base.Primitives;
using Android.Base.Wrappers;
namespace Android.com.fuse.ExperimentalHttp
{
    [TargetSpecificImplementation, NativeClass("Java", "HttpRequest")]
    [Require("AndroidManifest.ApplicationElement", "<uses-library android:name=\"org.apache.http.legacy\" android:required=\"false\" />")]
    public extern(ANDROID) abstract  class HttpRequest : Android.Base.Wrappers.JWrapper
    {
        internal static new ujclass _javaClass;
        internal static new ujclass _javaProxyClass;

        [TargetSpecificImplementation]
        public static extern new void _InitProxy();
        [TargetSpecificImplementation]
        public static extern new bool _IsThisType(Android.Base.Wrappers.IJWrapper obj);
        [TargetSpecificImplementation]
        public static extern new void _Init();
        protected HttpRequest(Android.Base.Primitives.ujobject obj, Uno.Type utype, bool hasFallbackClass, bool resolveType) : base(obj, utype, hasFallbackClass, resolveType) { }
        public static int HttpResponseTypeNone
        {
            get { return (int)HttpRequest.HttpResponseTypeNone_GET_44276(); }
        }

        public static int HttpResponseTypeString
        {
            get { return (int)HttpRequest.HttpResponseTypeString_GET_44277(); }
        }

        public static int HttpResponseTypeByteArray
        {
            get { return (int)HttpRequest.HttpResponseTypeByteArray_GET_44278(); }
        }

        public static bool InstallCache(Android.Base.Wrappers.IJWrapper arg0, long arg1)
        {
            return InstallCache_IMPL_44279(arg0, arg1);
        }

        public static void CacheRenew(Android.Base.Wrappers.IJWrapper arg0)
        {
            CacheRenew_IMPL_44280(arg0);
        }

        public static void CacheClose()
        {
            CacheClose_IMPL_44281();
        }

        public static void CacheDelete()
        {
            CacheDelete_IMPL_44282();
        }

        public static void CacheFlush()
        {
            CacheFlush_IMPL_44283();
        }

        protected  HttpRequest(Android.Base.Wrappers.IJWrapper arg0, Android.Base.Wrappers.IJWrapper arg1, Android.Base.Wrappers.IJWrapper arg2) : base (Android.Base.JNI.GetDefaultObject(),Android.Base.JNI.GetDefaultType(), false, false)
        {
            _subclassed = _IsThisType(this);
            var wrapper = _subclassed ? this : null;
            _javaObject = HttpRequest_IMPL_44284(wrapper, arg0, arg1, arg2);

        }

        public abstract void OnDataReceived(Android.Base.Wrappers.IJWrapper arg0, int arg1) ;

        public abstract void OnAborted() ;

        public abstract void OnError(Android.Base.Wrappers.IJWrapper arg0) ;

        public abstract void OnTimeout() ;

        public abstract void OnDone() ;

        public abstract void OnHeadersReceived() ;

        public abstract void OnProgress(int arg0, int arg1, bool arg2) ;

        public virtual void SetResponseType(int arg0)
        {
            SetResponseType_IMPL_44292(_subclassed, _javaObject, arg0);
        }

        public virtual void SetHeader(Android.Base.Wrappers.IJWrapper arg0, Android.Base.Wrappers.IJWrapper arg1)
        {
            SetHeader_IMPL_44293(_subclassed, _javaObject, arg0, arg1);
        }

        public virtual void SetTimeout(int arg0)
        {
            SetTimeout_IMPL_44294(_subclassed, _javaObject, arg0);
        }

        public virtual void SetCaching(bool arg0)
        {
            SetCaching_IMPL_44295(_subclassed, _javaObject, arg0);
        }

        public virtual void CacheResponseString(Android.Base.Wrappers.IJWrapper arg0)
        {
            CacheResponseString_IMPL_44296(_subclassed, _javaObject, arg0);
        }

        public virtual Android.Base.Wrappers.IJWrapper GetResponseString()
        {
            return GetResponseString_IMPL_44297(_subclassed, _javaObject);
        }

        public virtual void SendAsync()
        {
            SendAsync_IMPL_44299(_subclassed, _javaObject);
        }

        public virtual void SendAsyncBuf(Android.Base.Wrappers.IJWrapper arg0)
        {
            SendAsyncBuf_IMPL_44300(_subclassed, _javaObject, arg0);
        }

        public virtual void SendAsyncStr(Android.Base.Wrappers.IJWrapper arg0)
        {
            SendAsyncStr_IMPL_44301(_subclassed, _javaObject, arg0);
        }

        public virtual void UploadDone(Android.Base.Wrappers.IJWrapper arg0, Android.Base.Wrappers.IJWrapper arg1, int arg2, Android.Base.Wrappers.IJWrapper arg3)
        {
            UploadDone_IMPL_44302(_subclassed, _javaObject, arg0, arg1, arg2, arg3);
        }

        public virtual void DownloadDone()
        {
            DownloadDone_IMPL_44303(_subclassed, _javaObject);
        }

        public virtual void StartDownload(Android.Base.Wrappers.IJWrapper arg0)
        {
            StartDownload_IMPL_44304(_subclassed, _javaObject, arg0);
        }

        public virtual void Abort()
        {
            Abort_IMPL_44305(_subclassed, _javaObject);
        }

        public virtual int GetResponseStatus()
        {
            return GetResponseStatus_IMPL_44306(_subclassed, _javaObject);
        }

        public virtual Android.Base.Wrappers.IJWrapper GetResponseHeader(Android.Base.Wrappers.IJWrapper arg0)
        {
            return GetResponseHeader_IMPL_44307(_subclassed, _javaObject, arg0);
        }

        public virtual Android.Base.Wrappers.IJWrapper GetResponseHeaders()
        {
            return GetResponseHeaders_IMPL_44308(_subclassed, _javaObject);
        }

        static jfieldID HttpResponseTypeNone_44276_ID;
        [TargetSpecificImplementation]
        public static extern int HttpResponseTypeNone_GET_44276();

        static jfieldID HttpResponseTypeString_44277_ID;
        [TargetSpecificImplementation]
        public static extern int HttpResponseTypeString_GET_44277();

        static jfieldID HttpResponseTypeByteArray_44278_ID;
        [TargetSpecificImplementation]
        public static extern int HttpResponseTypeByteArray_GET_44278();

        static jmethodID InstallCache_44279_ID;
        [TargetSpecificImplementation]
        public static extern bool InstallCache_IMPL_44279(Android.Base.Wrappers.IJWrapper arg0, long arg1);
        static jmethodID CacheRenew_44280_ID;
        [TargetSpecificImplementation]
        public static extern void CacheRenew_IMPL_44280(Android.Base.Wrappers.IJWrapper arg0);
        static jmethodID CacheClose_44281_ID;
        [TargetSpecificImplementation]
        public static extern void CacheClose_IMPL_44281();
        static jmethodID CacheDelete_44282_ID;
        [TargetSpecificImplementation]
        public static extern void CacheDelete_IMPL_44282();
        static jmethodID CacheFlush_44283_ID;
        [TargetSpecificImplementation]
        public static extern void CacheFlush_IMPL_44283();
        static jmethodID HttpRequest_44284_ID_c;
        static jmethodID HttpRequest_44284_ID_c_prox;
        [TargetSpecificImplementation]
        public static extern Android.Base.Primitives.ujobject HttpRequest_IMPL_44284(Android.Base.Wrappers.IJWrapper arg0, Android.Base.Wrappers.IJWrapper arg1, Android.Base.Wrappers.IJWrapper arg2, Android.Base.Wrappers.IJWrapper arg3);
        static jmethodID SetResponseType_44292_ID;
        [TargetSpecificImplementation]
        public static extern void SetResponseType_IMPL_44292(bool arg0, Android.Base.Primitives.ujobject arg1, int arg2);
        static jmethodID SetHeader_44293_ID;
        [TargetSpecificImplementation]
        public static extern void SetHeader_IMPL_44293(bool arg0, Android.Base.Primitives.ujobject arg1, Android.Base.Wrappers.IJWrapper arg2, Android.Base.Wrappers.IJWrapper arg3);
        static jmethodID SetTimeout_44294_ID;
        [TargetSpecificImplementation]
        public static extern void SetTimeout_IMPL_44294(bool arg0, Android.Base.Primitives.ujobject arg1, int arg2);
        static jmethodID SetCaching_44295_ID;
        [TargetSpecificImplementation]
        public static extern void SetCaching_IMPL_44295(bool arg0, Android.Base.Primitives.ujobject arg1, bool arg2);
        static jmethodID CacheResponseString_44296_ID;
        [TargetSpecificImplementation]
        public static extern void CacheResponseString_IMPL_44296(bool arg0, Android.Base.Primitives.ujobject arg1, Android.Base.Wrappers.IJWrapper arg2);
        static jmethodID GetResponseString_44297_ID;
        [TargetSpecificImplementation]
        public static extern Android.Base.Wrappers.IJWrapper GetResponseString_IMPL_44297(bool arg0, Android.Base.Primitives.ujobject arg1);
        static jmethodID SendAsync_44299_ID;
        [TargetSpecificImplementation]
        public static extern void SendAsync_IMPL_44299(bool arg0, Android.Base.Primitives.ujobject arg1);
        static jmethodID SendAsyncBuf_44300_ID;
        [TargetSpecificImplementation]
        public static extern void SendAsyncBuf_IMPL_44300(bool arg0, Android.Base.Primitives.ujobject arg1, Android.Base.Wrappers.IJWrapper arg2);
        static jmethodID SendAsyncStr_44301_ID;
        [TargetSpecificImplementation]
        public static extern void SendAsyncStr_IMPL_44301(bool arg0, Android.Base.Primitives.ujobject arg1, Android.Base.Wrappers.IJWrapper arg2);
        static jmethodID UploadDone_44302_ID;
        [TargetSpecificImplementation]
        public static extern void UploadDone_IMPL_44302(bool arg0, Android.Base.Primitives.ujobject arg1, Android.Base.Wrappers.IJWrapper arg2, Android.Base.Wrappers.IJWrapper arg3, int arg4, Android.Base.Wrappers.IJWrapper arg5);
        static jmethodID DownloadDone_44303_ID;
        [TargetSpecificImplementation]
        public static extern void DownloadDone_IMPL_44303(bool arg0, Android.Base.Primitives.ujobject arg1);
        static jmethodID StartDownload_44304_ID;
        [TargetSpecificImplementation]
        public static extern void StartDownload_IMPL_44304(bool arg0, Android.Base.Primitives.ujobject arg1, Android.Base.Wrappers.IJWrapper arg2);
        static jmethodID Abort_44305_ID;
        [TargetSpecificImplementation]
        public static extern void Abort_IMPL_44305(bool arg0, Android.Base.Primitives.ujobject arg1);
        static jmethodID GetResponseStatus_44306_ID;
        [TargetSpecificImplementation]
        public static extern int GetResponseStatus_IMPL_44306(bool arg0, Android.Base.Primitives.ujobject arg1);
        static jmethodID GetResponseHeader_44307_ID;
        [TargetSpecificImplementation]
        public static extern Android.Base.Wrappers.IJWrapper GetResponseHeader_IMPL_44307(bool arg0, Android.Base.Primitives.ujobject arg1, Android.Base.Wrappers.IJWrapper arg2);
        static jmethodID GetResponseHeaders_44308_ID;
        [TargetSpecificImplementation]
        public static extern Android.Base.Wrappers.IJWrapper GetResponseHeaders_IMPL_44308(bool arg0, Android.Base.Primitives.ujobject arg1);

    }
}
