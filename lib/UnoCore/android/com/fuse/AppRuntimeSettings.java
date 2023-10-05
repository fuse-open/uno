package com.fuse;

import android.os.Build;

public class AppRuntimeSettings
{
    public static final boolean KillActivityOnDestroy = @(runtime.killActivityOnDestroy);
    public static final String AppName = "@(activity.name)";
    public static int SDKVersion = Build.VERSION.SDK_INT;
}
