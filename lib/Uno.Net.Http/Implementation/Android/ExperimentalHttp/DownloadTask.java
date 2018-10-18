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
