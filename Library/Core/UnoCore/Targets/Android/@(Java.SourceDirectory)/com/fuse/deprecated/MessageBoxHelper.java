/*
 * Copyright (C) 2010-2014 Fusetools AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
 * associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute,
 * sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
 * NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES
 * OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

package com.fuse.deprecated;

import android.app.AlertDialog;
import android.app.Activity;
import android.content.DialogInterface;
import android.os.ConditionVariable;
import android.util.Log;


public class MessageBoxHelper {
    public static int ShowMessageBox(Activity activity, CharSequence caption, CharSequence message, int buttons, int hints)
    {
        final ConditionVariable bufferLock = new ConditionVariable();
        final AlertDialog.Builder b = new AlertDialog.Builder(activity);
        final int result[] = {-1};

        b.setTitle(caption);
        b.setMessage(message);
        b.setCancelable(false);

        switch (hints) {
        case 0:
            b.setIcon(android.R.drawable.stat_notify_error);
            break;
        case 1:
            b.setIcon(android.R.drawable.btn_star_big_on);
            break;
        case 2:
            b.setIcon(android.R.drawable.stat_sys_warning);
            break;
        }


        switch (buttons) {
        case 1:
            b.setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
                public void onClick(DialogInterface dialog, int which) { result[0] = 1; bufferLock.open(); }
            });
        case 0:
            b.setPositiveButton("OK", new DialogInterface.OnClickListener() {
                public void onClick(DialogInterface dialog, int which) { result[0] = 0; bufferLock.open(); }
            });
            break;
        case 3:
            b.setNeutralButton("Cancel", new DialogInterface.OnClickListener() {
                public void onClick(DialogInterface dialog, int which) { result[0] = 1; bufferLock.open(); }
            });
        case 2:
            b.setPositiveButton("Yes", new DialogInterface.OnClickListener() {
                public void onClick(DialogInterface dialog, int which) { result[0] = 2; bufferLock.open(); }
            });
            b.setNegativeButton("No", new DialogInterface.OnClickListener() {
                public void onClick(DialogInterface dialog, int which) { result[0] = 3; bufferLock.open(); }
            });
            break;
        case 4:
            b.setPositiveButton("Continue", new DialogInterface.OnClickListener() {
                public void onClick(DialogInterface dialog, int which) { result[0] = 5; bufferLock.open(); }
            });
            b.setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
                public void onClick(DialogInterface dialog, int which) { result[0] = 1; bufferLock.open(); }
            });
            b.setNeutralButton("Try Again", new DialogInterface.OnClickListener() {
                public void onClick(DialogInterface dialog, int which) { result[0] = 4; bufferLock.open(); }
            });
            break;
        default:
            break;
        }

        try {
             activity.runOnUiThread(new Runnable() { public void run() { AlertDialog d = b.create(); d.setCanceledOnTouchOutside(false); d.show(); }});
            bufferLock.block();
        } catch (Exception e) {
            Log.e(com.fuse.AppRuntimeSettings.AppName, e.getMessage());
        }
        return result[0];
    }
}
