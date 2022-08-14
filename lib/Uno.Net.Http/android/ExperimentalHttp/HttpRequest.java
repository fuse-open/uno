package com.fuse.ExperimentalHttp;

import java.io.BufferedInputStream;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.net.CookieHandler;
import java.net.CookieManager;
import java.net.CookiePolicy;
import java.nio.ByteBuffer;
import java.util.HashMap;
import android.util.Log;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.net.http.HttpResponseCache;
import android.os.AsyncTask;
import android.os.Build;

public abstract class HttpRequest {
	static boolean _defaultCookieManagerSet = false;

	public static final int HttpResponseTypeNone = -1;
	public static final int HttpResponseTypeString = 0;
	public static final int HttpResponseTypeByteArray = 1;

	Activity _activity;
    String _url;
    String _method;
    int _timeout = 0;
    boolean _useCaching = false;
    boolean _verifyHost = true;
    HashMap<String,String> _uploadHeaders = new HashMap<String, String>();
    int _responseType = 0;

    boolean _aborted = false;
    boolean _errored = false;
    UploadTask _uploadTask;
    DownloadTask _downloadTask;

    BufferedInputStream _responseStream;
    String _responseString;
    HashMap<String,String> _responseHeaders = new HashMap<String, String>();
    String _responseMessage;
    int _responseStatusCode = 0;

    static long _cacheSize;
    public static boolean InstallCache(Activity activity, long cacheSize)
    {
    	try {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.ICE_CREAM_SANDWICH) {
    			File httpCacheDir = new File(activity.getCacheDir(), "http");
    			HttpResponseCache.install(httpCacheDir, cacheSize);
                _cacheSize = cacheSize;
    			return true;
    		} else {
                _cacheSize = 0;
    			return false;
    		}
    	} catch (IOException e) {
            _cacheSize = 0;
            return false;
        }
    }
    void CacheRenew(Activity activity) {
        CacheDelete();
        if (_cacheSize>0) InstallCache(activity, _cacheSize);
    }
    void CacheClose() {
        // Uninstalls the cache and releases any active resources.
        HttpResponseCache cache = HttpResponseCache.getInstalled();
        try {
            if (cache != null) cache.close();
        } catch (IOException e) {}
    }

    void CacheDelete() {
        //Uninstalls the cache and deletes all of its stored contents.
        HttpResponseCache cache = HttpResponseCache.getInstalled();
        try {
            if (cache != null) cache.delete();
        } catch (IOException e) {}
    }

    void CacheFlush() {
        //Force buffered operations to the filesystem.
        HttpResponseCache cache = HttpResponseCache.getInstalled();
        if (cache != null) cache.flush();
    }

    public HttpRequest(Activity activity, String url, String method)
    {
    	if (_defaultCookieManagerSet) {
			CookieManager cookieManager = new CookieManager(null, CookiePolicy.ACCEPT_ALL);
	        CookieHandler.setDefault(cookieManager);
		}
    	_activity = activity;
    	_url = url;
    	_method = method.toUpperCase();
    }

    public abstract void OnDataReceived(byte[] chunk, int read);
	public abstract void OnAborted();
	public abstract void OnError(String platformspesificErrorMessage);
	public abstract void OnTimeout();
	public abstract void OnDone();
	public abstract void OnHeadersReceived();
	public abstract void OnProgress(int current, int total, boolean hasTotal);

	public final void SetResponseType(int responseType)
	{
		_responseType = responseType;
	}

    public final void SetHeader(String name, String value) throws Exception {
        _uploadHeaders.put(name, value);
    }

    public final void SetTimeout(int timeoutInMilliseconds) throws Exception {
        _timeout = timeoutInMilliseconds;
    }

    public final void SetCaching(boolean set) throws Exception {
        _useCaching = set;
    }

    public final void CacheResponseString(String content)
    {
    	_responseString = content;
    }

    public final String GetResponseString()
    {
    	return _responseString;
    }

    public final void SendAsync() throws Exception {
    	SendAsyncBuf((ByteBuffer)null);
    }

    public final void SendAsyncBuf(final ByteBuffer data) throws Exception {
        final HttpRequest request = this;
        try
        {
            _activity.runOnUiThread(new Runnable() {
    				@SuppressLint("InlinedApi")
    				public void run() {
    					UploadTask task = new UploadTask();
    					_uploadTask = task;
    					if (Build.VERSION.SDK_INT>=Build.VERSION_CODES.HONEYCOMB) {
    						task.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, _url, _method, _uploadHeaders, (Integer)_timeout, data, request, (Boolean)_verifyHost, (Boolean)_useCaching);
    					}
    					else {
    						task.execute(_url, _method, _uploadHeaders, (Integer)_timeout, data, request, (Boolean)_verifyHost, (Boolean)_useCaching);
    					}
    				}});
        } catch (Exception e) {
            this.OnError("Unable to build Async Http Request: "+e.getLocalizedMessage());
        }
    }

    public final void SendAsyncStr(final String data) throws Exception {
        final HttpRequest request = this;
        try
        {
            _activity.runOnUiThread(new Runnable() {
    				@SuppressLint("InlinedApi")
    				public void run() {
    					UploadTask task = new UploadTask();
    					_uploadTask = task;
    					ByteBuffer dataBuffer = null;
    					if (data!=null) dataBuffer = ByteBuffer.wrap(data.getBytes());
    					if (Build.VERSION.SDK_INT>=Build.VERSION_CODES.HONEYCOMB) {
    						task.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, _url, _method, _uploadHeaders, (Integer)_timeout, dataBuffer, request, (Boolean)_verifyHost, (Boolean)_useCaching);
    					}
    					else {
    						task.execute(_url, _method, _uploadHeaders, (Integer)_timeout, dataBuffer, request, (Boolean)_verifyHost, (Boolean)_useCaching);
    					}
    				}});
        } catch (Exception e) {
            this.OnError("Unable to build Async Http Request: "+e.getLocalizedMessage());
        }
    }

    public final void UploadDone(BufferedInputStream responseStream, HashMap<String,String> responseHeaders, int responseCode, String responseMessage) {
    	_responseStream = responseStream;
    	_responseHeaders = responseHeaders;
    	_responseMessage = responseMessage;
    	_responseStatusCode = responseCode;
    	_uploadTask = null;
        OnHeadersReceived();
        try {
            StartDownload(_responseStream);
        } catch (Exception e) {
            this.OnError("Unable to start download: "+e.getLocalizedMessage());
        }
    }

    public final void DownloadDone()
    {
    	_responseStream = null;
        OnDone();
        CacheFlush();
    }

	public final void StartDownload(final InputStream stream) throws Exception
    {
		if (_responseStream!=null) {
			final HttpRequest request = this;
			_activity.runOnUiThread(new Runnable() { @SuppressLint("NewApi")
			public void run() {
				DownloadTask task = new DownloadTask();
				_downloadTask = task;
				if (Build.VERSION.SDK_INT > 10) {
					task.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, stream, request);
				} else {
					task.execute(stream, request);
				}
			}});
		} else if (_responseStream==null) {
			throw new Exception("HttpRequest->PullContentArray(): In correct state to pull content array but have null contentHandle");
		}
    }

    public final void Abort() {
        _aborted = true;
        if (_uploadTask!=null) {
            _uploadTask.cancel(true);
        }
        if (_downloadTask!=null) {
            _downloadTask.cancel(true);
        }
    }

    public final int GetResponseStatus() throws Exception {
        return _responseStatusCode;
    }

    public final String GetResponseHeader(String name) throws Exception {
        return _responseHeaders.get(name);
    }

    public final String GetResponseHeaders() throws Exception {
        String headers = "";
        for (String key : _responseHeaders.keySet()) {
            headers += "\n";
            headers += key + ": "+_responseHeaders.get(key);
        }
        return headers;
    }
}
