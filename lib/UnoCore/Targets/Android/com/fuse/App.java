package com.fuse;

import android.os.Build;
import android.util.Log;
import android.os.Bundle;
import android.os.Looper;
import android.view.View;
import java.util.ArrayList;
import android.view.Window;
import android.app.Activity;
import android.view.Surface;
import android.view.KeyEvent;
import android.content.Intent;
import android.app.AlertDialog;
import java.lang.reflect.Field;
import android.widget.TextView;
import android.widget.ScrollView;
import android.widget.FrameLayout;
import android.view.Choreographer;
import android.app.NativeActivity;
import android.view.WindowManager;
import android.widget.FrameLayout;
import android.annotation.TargetApi;
import android.view.ViewTreeObserver;
import android.content.DialogInterface;
import android.graphics.SurfaceTexture;
import android.R.color;
import android.annotation.SuppressLint;
import android.content.res.AssetManager;
import android.content.res.Configuration;
import androidx.core.app.ActivityCompat;
import androidx.appcompat.app.AppCompatActivity;
import android.view.View.OnLayoutChangeListener;
import com.fuse.App;
import com.fuse.Activity.ActivityListener;


public class App {

    static
    {
        // load c++ libs needed by uno
        com.uno.CppManager.LoadLibraries();
        // http://stackoverflow.com/a/37239813/574033  we use wildcard for `appcompat` so do same for `design`
        androidx.appcompat.app.AppCompatDelegate.setCompatVectorFromResourcesEnabled(true);
    }

    private static App _appState;

    public AppCompatActivity RootActivity;
    public boolean HasCreated = false;
    public final ActivityState activityState; // one per activity

    private App(AppCompatActivity activity)
    {
        activityState = new ActivityState(HasCreated);
        RootActivity = activity;
    }

    public static App Create(AppCompatActivity activity)
    {
        if (_appState==null) _appState = new App(activity);

        _appState.RootActivity = activity;
        return _appState;
    }

    public static App getCurrent()
    {
        return _appState;
    }

    public static void TerminateNow()
    {
        _appState.activityState.Destroyed = true;
        android.os.Process.killProcess(android.os.Process.myPid());
    }

    //------------------------------------------------------------
    // OnCreate

    public void onCreate(Bundle savedInstanceState)
    {
        // start main loop
        ActivityNativeEntryPoints.cppOnStartMainLoop(activityState.Resurrected);

        // ensure arrival of any intent
        onNewIntent(RootActivity.getIntent());

        // call c++ for setup
        ActivityNativeEntryPoints.cppOnCreate(RootActivity);
        
        // reset window background for app switcher to be able to take a screenshot of app content
#if @(Project.Android.Splash.Enabled:Test(1, 0))
        RootActivity.getWindow().setBackgroundDrawableResource(android.R.color.transparent);
#endif

        // Finish
        HasCreated = true;
    }

    //------------------------------------------------------------

    @SuppressLint("NewApi")
    @SuppressWarnings("deprecation")
    public void onDestroyPre() {
        activityState.Destroyed = true;
        com.fuse.Activity.onDestroy();
    }

    @SuppressLint("NewApi")
    @SuppressWarnings("deprecation")
    public void onDestroyPost() {
        if (AppRuntimeSettings.KillActivityOnDestroy) android.os.Process.killProcess(android.os.Process.myPid());
    }

    //------------------------------------------------------------

    public void onPause() {
        focusOnPause();
        ActivityNativeEntryPoints.cppOnPause();
        com.fuse.Activity.onPause();
    }

    private void focusOnPause()
    {
        if (!activityState.Destroyed) {
            if (currentlyInteractive) {
                currentlyInteractive = false;
                cachedForResume = false;
                ActivityNativeEntryPoints.cppOnWindowFocusChanged(false);
            }
        }
        allowedToChangeFocus = false;
    }

    //------------------------------------------------------------

    public void onResume() {
        ActivityNativeEntryPoints.cppOnResume();
        com.fuse.Activity.onResume();
        focusOnResume();

        if (!pendingURI.equals(""))
        {
            ActivityNativeEntryPoints.cppOnReceiveURI(pendingURI);
            pendingURI = "";
        }
    }

    private void focusOnResume()
    {
        if (!activityState.Destroyed) {
            if (currentlyInteractive) {
                Log.e(AppRuntimeSettings.AppName, "State Error: Java backend asserts interactive pre-resume.");
            }
            if (currentlyInteractive) {
                Log.e(AppRuntimeSettings.AppName, "State Error: Java backend asserts allow interact state change pre-resume.");
            }
            if (cachedForResume) {
                currentlyInteractive = true;
                ActivityNativeEntryPoints.cppOnWindowFocusChanged(true);
            }
            cachedForResume = false;
            allowedToChangeFocus = true;
        }
    }

    //------------------------------------------------------------

    public void onRestart() {
        ActivityNativeEntryPoints.cppOnRestart();
    }

    //------------------------------------------------------------

    public void onStart() {
        com.fuse.Activity.onStart();
    }

    //------------------------------------------------------------

    public void onStop() {
        com.fuse.Activity.onStop();
    }

    //------------------------------------------------------------

    public void onConfigurationChanged(Configuration arg0) {
        com.fuse.Activity.onConfigurationChanged(arg0);
        if (!activityState.Destroyed) {
            @(Activity.OnConfigurationChanged.Declaration:Join())
        }
    }

    //------------------------------------------------------------

    public void onLowMemory() {
        if (!activityState.Destroyed) {
            ActivityNativeEntryPoints.cppOnLowMemory();
            @(Activity.OnLowMemory.Declaration:Join())
        }
    }

    //------------------------------------------------------------

    public void onActivityResult (int arg0, int arg1, android.content.Intent arg2)
    {
        if (!HasCreated) return;
        com.fuse.Activity._onActivityResult(arg0, arg1, arg2);
        @(Activity.OnActivityResult.Declaration:Join())
    }

    //------------------------------------------------------------

    private String pendingURI = "";

    public void onNewIntent (Intent intent)
    {
#if @(Project.Mobile.UriScheme:IsSet)
        if (intent!=null && intent.getScheme()!=null && intent.getScheme().equals("@(Project.Mobile.UriScheme)"))
        {
            pendingURI = intent.getDataString();
        }
#endif

#if @(Project.Android.AssociatedDomains:IsSet)
        String[] associatedDomains = new String[]{@(Project.Android.AssociatedDomains:SplitAndJoin(',','"','"'))}; 

        for (int i = 0; i < associatedDomains.length; i++) {

            if (intent!=null && intent.getScheme()!=null && intent.getScheme().contains("http") 
                && intent.getDataString().toLowerCase().contains(new String(associatedDomains[i]).toLowerCase())) { 
                pendingURI = intent.getDataString();
                break;
            }
        }
#endif
        com.fuse.Activity._onActivityIntent(intent);
        @(Activity.OnNewIntent.Declaration:Join())
    }

    //------------------------------------------------------------

    public boolean onKeyUp(int keyCode, KeyEvent event)
    {
        if (keyCode == KeyEvent.KEYCODE_MENU) {
            ActivityNativeEntryPoints.cppOnKeyUp(200);
            return true;
        }
        if (keyCode == KeyEvent.KEYCODE_BACK) {
            return ActivityNativeEntryPoints.cppOnKeyUp(201);
        }
        @(Activity.OnKeyUp.Declaration:Join())
        {
            return false;
        }
    }

    public boolean onKeyDown(int keyCode, KeyEvent event)
    {
        if (keyCode == KeyEvent.KEYCODE_MENU) {
            ActivityNativeEntryPoints.cppOnKeyDown(200);
            return true;
        }
        if (keyCode == KeyEvent.KEYCODE_BACK) {
            return ActivityNativeEntryPoints.cppOnKeyDown(201);
        }
        @(Activity.OnKeyDown.Declaration:Join())
        {
            return false;
        }
    }

    //------------------------------------------------------------

    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults)
    {
        @(Activity.OnPermissionsResult:Join())
    }

    //------------------------------------------------------------
    // Normalize Focus Changed Events.
    // Force the focus changed events to be have as a part of the regular
    // lifecycle model, only firing between resume & pause.

    private boolean cachedForResume = false;
    private boolean currentlyInteractive = false;
    private boolean allowedToChangeFocus = false;

    public void onWindowFocusChanged(boolean arg0) {
        if (!activityState.Destroyed) {
            if (arg0!=currentlyInteractive) {
                if (allowedToChangeFocus) {
                    currentlyInteractive = arg0;
                    ActivityNativeEntryPoints.cppOnWindowFocusChanged(arg0);
                } else if (arg0=true) {
                    cachedForResume = true;
                }
            }
            com.fuse.Activity.onWindowFocusChanged(arg0);
        }
    }



}
