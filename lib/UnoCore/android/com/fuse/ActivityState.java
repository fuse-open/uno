package com.fuse;

import java.util.ArrayList;
import android.content.res.Configuration;

import com.fuse.App;
import com.fuse.Activity.ActivityListener;

public class ActivityState {
    public final boolean Resurrected;
    public boolean Destroyed = false;

    public ActivityState(boolean resurrected) {
        Resurrected = resurrected;
    }
}
