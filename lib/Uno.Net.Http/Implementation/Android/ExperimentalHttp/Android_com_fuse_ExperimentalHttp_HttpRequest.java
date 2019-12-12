package com.foreign;

public class Android_com_fuse_ExperimentalHttp_HttpRequest extends com.fuse.ExperimentalHttp.HttpRequest implements com.foreign.UnoWrapped {

    public long _unoRef;
    @Override public long _GetUnoPtr() { return _unoRef; }
    public Object _implementor;

    public Android_com_fuse_ExperimentalHttp_HttpRequest(long arg0, android.app.Activity arg1, java.lang.String arg2, java.lang.String arg3) throws Exception {
        super(arg1, arg2, arg3);
        _unoRef = arg0;
    }

    public static native void __n_OnDataReceived(long arg0, byte[] arg1, int arg2, long arg3, long arg4);
    public  void OnDataReceived(byte[] arg0, int arg1)  {
        #if !@{Android.com.fuse.ExperimentalHttp.HttpRequest.OnDataReceived(Android.Base.Wrappers.IJWrapper,int):IsStripped}
        try {
            __n_OnDataReceived(_unoRef,arg0, arg1, UnoHelper.GetUnoObjectRef(arg0), -1);
        } catch (UnsatisfiedLinkError e) {
            return;
        }
        #else
            return;
        #endif
    }

    public static native void __n_OnAborted(long arg0);
    public  void OnAborted()  {
        #if !@{Android.com.fuse.ExperimentalHttp.HttpRequest.OnAborted():IsStripped}
        try {
            __n_OnAborted(_unoRef);
        } catch (UnsatisfiedLinkError e) {
            return;
        }
        #else
            return;
        #endif
    }

    public static native void __n_OnError(long arg0, java.lang.String arg1, long arg2);
    public  void OnError(java.lang.String arg0)  {
        #if !@{Android.com.fuse.ExperimentalHttp.HttpRequest.OnError(Android.Base.Wrappers.IJWrapper):IsStripped}
        try {
            __n_OnError(_unoRef,arg0, UnoHelper.GetUnoObjectRef(arg0));
        } catch (UnsatisfiedLinkError e) {
            return;
        }
        #else
            return;
        #endif
    }

    public static native void __n_OnTimeout(long arg0);
    public  void OnTimeout()  {
        #if !@{Android.com.fuse.ExperimentalHttp.HttpRequest.OnTimeout():IsStripped}
        try {
            __n_OnTimeout(_unoRef);
        } catch (UnsatisfiedLinkError e) {
            return;
        }
        #else
            return;
        #endif
    }

    public static native void __n_OnDone(long arg0);
    public  void OnDone()  {
        #if !@{Android.com.fuse.ExperimentalHttp.HttpRequest.OnDone():IsStripped}
        try {
            __n_OnDone(_unoRef);
        } catch (UnsatisfiedLinkError e) {
            return;
        }
        #else
            return;
        #endif
    }

    public static native void __n_OnHeadersReceived(long arg0);
    public  void OnHeadersReceived()  {
        #if !@{Android.com.fuse.ExperimentalHttp.HttpRequest.OnHeadersReceived():IsStripped}
        try {
            __n_OnHeadersReceived(_unoRef);
        } catch (UnsatisfiedLinkError e) {
            return;
        }
        #else
            return;
        #endif
    }

    public static native void __n_OnProgress(long arg0, int arg1, int arg2, boolean arg3, long arg4, long arg5, long arg6);
    public  void OnProgress(int arg0, int arg1, boolean arg2)  {
        #if !@{Android.com.fuse.ExperimentalHttp.HttpRequest.OnProgress(int,int,bool):IsStripped}
        try {
            __n_OnProgress(_unoRef,arg0, arg1, arg2, -1, -1, -1);
        } catch (UnsatisfiedLinkError e) {
            return;
        }
        #else
            return;
        #endif
    }



    public void finalize() {
        UnoHelper.Finalize(_unoRef);
    }



}
