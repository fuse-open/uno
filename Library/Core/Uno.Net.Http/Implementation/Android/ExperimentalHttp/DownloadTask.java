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

package com.fuse.ExperimentalHttp;

import java.io.IOException;
import java.io.InputStream;
import android.os.AsyncTask;

public class DownloadTask extends AsyncTask<Object, Void, Void> {
	public HttpRequest request;
    StringBuilder stringResult;
	@Override
	protected Void doInBackground(Object... params) {
		request = (HttpRequest)params[1];

		try {
			InputStream stream = (InputStream)params[0];

			int nRead;
			int runningTotal = 0;
			int progressThreshold = 30000;
			int steps = 1;
			byte[] data = new byte[16384];
            stringResult = new StringBuilder();

			while ((nRead = stream.read(data, 0, data.length)) != -1) {
				if (request._responseType == HttpRequest.HttpResponseTypeByteArray) {
					request.OnDataReceived(data, nRead);
				} else {
                    stringResult.append(new String(data,0,nRead));
				}
				runningTotal+=nRead;
				if (runningTotal/progressThreshold > steps)
				{
					steps = runningTotal/progressThreshold;
					request.OnProgress(runningTotal, 0, false);
				}
			}
		} catch (IOException e) {
			request.OnError("IOException (AsyncInputStreamToBytesTask): "+e.getLocalizedMessage());
		}
		return null;
	}
	@Override
	protected void onPostExecute(Void ignore)
	{
		if (request._responseType == HttpRequest.HttpResponseTypeByteArray) {
			request.OnDataReceived(null, -1);
		} else {
			request.CacheResponseString(stringResult.toString());
		}
		request.DownloadDone();
	}
}
