package @(Activity.Package);

import android.os.Build;
import android.util.Log;
import android.os.Bundle;
import android.os.Looper;
import android.view.View;
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
import android.annotation.SuppressLint;
import android.content.res.AssetManager;
import android.content.res.Configuration;
import androidx.core.app.ActivityCompat;
import android.view.View.OnLayoutChangeListener;
import com.fuse.Activity.ActivityListener;

@(Activity.File.Declaration:Join())

public class @(Activity.Name) extends @(Activity.BaseClass) implements ActivityCompat.OnRequestPermissionsResultCallback
{
#if !@(LIBRARY:Defined)

    // state
    private static com.fuse.App fuseApp;  // lasts through all resurrections

    //
    public @(Activity.Name)()
    {
        super();
        fuseApp = com.fuse.App.Create(this);
    }

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        // mandatory call to super
        super.onCreate(savedInstanceState);
        fuseApp.onCreate(savedInstanceState);
    }

    @Override
    protected void onNewIntent (Intent intent)
    {
        fuseApp.onNewIntent(intent);
    }

    @Override
    public boolean onKeyUp(int keyCode, KeyEvent event)
    {
        return fuseApp.onKeyUp(keyCode, event);
    }
    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event)
    {
        return fuseApp.onKeyDown(keyCode, event);
    }

    @Override
    public void onActivityResult (int arg0, int arg1, android.content.Intent arg2)
    {
        fuseApp.onActivityResult(arg0, arg1, arg2);
    }

    @Override
    protected void onDestroy() {
        fuseApp.onDestroyPre();
        super.onDestroy();
        fuseApp.onDestroyPost();
    }

    @Override
    protected void onPause() {
        super.onPause();
        fuseApp.onPause();
    }

    @Override
    protected void onResume() {
        super.onResume();
        fuseApp.onResume();
    }

    @Override
    protected void onRestart() {
        super.onRestart();
        fuseApp.onRestart();
    }


    @Override
    protected void onStart() {
        super.onStart();
        fuseApp.onStart();
    }

    @Override
    protected void onStop() {
        super.onStop();
        fuseApp.onStop();
    }

    @Override
    public void onConfigurationChanged(Configuration arg0) {
        super.onConfigurationChanged(arg0);
        fuseApp.onConfigurationChanged(arg0);
    }

    @Override
    public void onLowMemory() {
        super.onLowMemory();
        fuseApp.onLowMemory();
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults)
    {
        fuseApp.onRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    @Override
    public void onWindowFocusChanged(boolean arg0) {
        super.onWindowFocusChanged(arg0);
        fuseApp.onWindowFocusChanged(arg0);
    }
#endif
    //-----------------------------------------------------------
    // Here be less favorable stuff that has tickets for cleanup
    // used by c++ and annoyingly the custom view, fix this
    private static SurfaceTexture _keepDummySurfaceTexture;
    public static Object CreateDummySurface(int texName)
    {
        // this is a temp hack. Moments like this make me want to make attribute
        // that gives bad code a halflife, after 2 months it deletes itself.
        _keepDummySurfaceTexture = new SurfaceTexture(texName);
        return new Surface(_keepDummySurfaceTexture);
    }
}
