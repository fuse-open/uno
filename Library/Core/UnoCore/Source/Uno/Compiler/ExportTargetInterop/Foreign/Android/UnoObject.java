package com.uno;

public class UnoObject // implements com.Bindings.UnoWrapped
{
    public long _retainedUnoPtr;

    public long _GetRetainedUnoPtr() { return _retainedUnoPtr; }

    public UnoObject(long arg0) {
        super();
        _retainedUnoPtr = arg0;
    }

    public void finalize() { try { Finalize(_retainedUnoPtr); } finally { try { super.finalize(); } catch (Throwable e) {} } }

    public static native void Finalize(long arg0);
}
