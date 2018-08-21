package com.fuse;

import java.util.List;
import java.util.HashMap;
import java.util.ArrayList;
import android.content.Intent;
import android.content.res.Configuration;


public final class Activity
{
    private static final ArrayList<ActivityListener> _activityListeners = new ArrayList<ActivityListener>();

    //------------------------------------------------------------

    public static android.support.v7.app.AppCompatActivity getRootActivity()
    {
        return com.fuse.App.getCurrent().RootActivity;
    }

    //------------------------------------------------------------

    public static void SubscribeToLifecycleChange(ActivityListener listener)
    {
        _activityListeners.add(listener);
    }

    public static void UnsubscribeFromLifecycleChange(ActivityListener listener)
    {
        _activityListeners.remove(listener);
    }

    //------------------------------------------------------------

    public interface ResultListener
    {
        boolean onResult (int requestCode, int resultCode, Intent data);
    }

    private static final ArrayList<ResultListener> _resultListeners = new ArrayList<ResultListener>();

    public static void subscribeToResults(ResultListener listener)
    {
        _resultListeners.add(listener);
    }
    public static void unsubscribeFromResults(ResultListener listener)
    {
        _resultListeners.remove(listener);
    }

    public static void _onActivityResult(int requestCode, int resultCode, Intent data)
    {
        for (int i=0; i<_resultListeners.size(); i++) {
            if (_resultListeners.get(i).onResult(requestCode, resultCode, data))
                break;
        }
    }

    //------------------------------------------------------------

    public interface IntentListener
    {
        void onIntent (Intent newIntent);
    }

    private static final HashMap<String, ArrayList<IntentListener>> _intentListeners = new HashMap<String, ArrayList<IntentListener>>();

    public static void subscribeToIntents(IntentListener listener, String actionName)
    {
        if (!_intentListeners.containsKey(actionName))
            _intentListeners.put(actionName, new ArrayList<IntentListener>());
        ArrayList<IntentListener> l = _intentListeners.get(actionName);
        if (!l.contains(listener))
            l.add(listener);
        _dispatchUnhandledToListeners();
    }
    public static void unsubscribeFromIntents(IntentListener toRemove)
    {
        for (List<IntentListener> listeners : _intentListeners.values()) {
            if (listeners.contains(toRemove))
                listeners.remove(toRemove);
        }
    }

    static ArrayList<Intent> _unhandledIntents = new ArrayList<Intent>();

    public static void _onActivityIntent(Intent newIntent)
    {
        if (!_dispatchToListeners(newIntent)) {
            _unhandledIntents.add(newIntent);
        }
    }

    public static void _dispatchUnhandledToListeners()
    {
        ArrayList<Intent> toRemove = new ArrayList<Intent>();

        for (Intent i : _unhandledIntents)
            if (_dispatchToListeners(i))
                toRemove.add(i);

        for (Intent i : toRemove)
            _unhandledIntents.remove(i);
    }

    public static boolean _dispatchToListeners(Intent newIntent)
    {
        String action = newIntent.getAction();
        if (_intentListeners.containsKey(action)) {
            ArrayList<IntentListener> l = _intentListeners.get(action);
            for (IntentListener x : l) {
                x.onIntent(newIntent);
            }
            return true;
        } else {
            return false;
        }
    }

    //------------------------------------------------------------

    public interface ActivityListener {
        // Allows you to listen to a subset of the root activity's events
        void onPause();
        void onResume();
        void onStart();
        void onStop();
        void onDestroy();
        void onWindowFocusChanged(boolean hasFocus);
        void onConfigurationChanged(Configuration config);
    }

    //------------------------------------------------------------

    static void onPause() {
        for (ActivityListener activityListener : _activityListeners) {
            activityListener.onPause();
        }
    }


    static void onResume() {
        for (ActivityListener activityListener : _activityListeners) {
            activityListener.onResume();
        }
    }

    static void onStart() {
        for (ActivityListener activityListener : _activityListeners) {
            activityListener.onStart();
        }
    }

    static void onStop() {
        for (ActivityListener activityListener : _activityListeners) {
            activityListener.onStop();
        }
    }

    static void onDestroy() {
        for (ActivityListener activityListener : _activityListeners) {
            activityListener.onDestroy();
        }
    }

    static void onWindowFocusChanged(boolean hasFocus) {
        for (ActivityListener activityListener : _activityListeners) {
            activityListener.onWindowFocusChanged(hasFocus);
        }
    }

    static void onConfigurationChanged(Configuration config) {
        for (ActivityListener activityListener : _activityListeners) {
            activityListener.onConfigurationChanged(config);
        }
    }
}
