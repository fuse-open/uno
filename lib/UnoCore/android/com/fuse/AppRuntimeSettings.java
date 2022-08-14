package com.fuse;

import android.os.Build;

public class AppRuntimeSettings
{
    public static final boolean KillActivityOnDestroy = @(Runtime.KillActivityOnDestroy);
    public static final String AppName = "@(Activity.Name)";
    public static int SDKVersion = Build.VERSION.SDK_INT;
}
