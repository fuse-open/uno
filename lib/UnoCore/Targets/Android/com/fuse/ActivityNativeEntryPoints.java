package com.fuse;

import android.os.Bundle;

public class ActivityNativeEntryPoints {

	//------------------------------------------------
	// Callbacks to C++ code
	public static native void cppOnCreate(Object activityObject);

	public static native void cppOnDestroy();

	public static native void cppOnPause();

	public static native void cppOnResume();

	public static native void cppOnRestart();

	public static native void cppOnLowMemory();

	public static native void cppOnWindowFocusChanged(boolean hasFocus);

	public static native void cppOnStartMainLoop(boolean _resurrected);

	public static native boolean cppOnKeyUp(int keyCode);

	public static native boolean cppOnKeyDown(int keyCode);

    public static native void cppOnReceiveURI(String data);
}
