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

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.IOException;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.SocketTimeoutException;
import java.net.URL;
import java.nio.ByteBuffer;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Locale;
import java.util.Map;

import javax.net.ssl.HostnameVerifier;
import javax.net.ssl.HttpsURLConnection;
import javax.net.ssl.SSLContext;
import javax.net.ssl.SSLSession;

import android.os.AsyncTask;
import android.util.Log;

public class UploadTask extends AsyncTask<Object, Integer, Boolean> {
	HttpRequest _request;
	boolean _verifyHost;

	BufferedInputStream _responseStream;
	HashMap<String,String> _responseHeaders;
	int _responseCode;
	String _responseMessage;
	boolean _useCaching;

	@Override
	protected Boolean doInBackground(Object... params) {

		String url = (String)params[0];
		String method = (String)params[1];
		@SuppressWarnings("unchecked")
		HashMap<String,String> headers = (HashMap<String,String>)params[2];
		int timeout = (Integer)params[3];
		ByteBuffer body = (ByteBuffer)params[4];
		_request = (HttpRequest)params[5];
		_verifyHost = (Boolean)params[6];
		_useCaching = (Boolean)params[7];
		HashMap<String,String> responseHeaders;
		boolean hasUploadContent = (body != null);
		HttpURLConnection connection = null;

		if (this.isCancelled()) { _request.OnAborted(); return false; }
		try {
			connection = NewHttpConnection(url, method, hasUploadContent, timeout, _request, _verifyHost, _useCaching);
			if (connection==null) {
				_responseStream = null;
				_responseHeaders = new HashMap<String,String>();
				_responseCode = -1;
				return false;
			}
		} catch (Exception e) {
			_request.OnError("JavaError (NewHttpConnection): Could not make connection. Check Android permissions");
            return false;
		}

		//set headers
		Iterator<Map.Entry<String, String>> it = headers.entrySet().iterator();
		while (it.hasNext()) {
			Map.Entry<String, String>pair = (Map.Entry<String, String>)it.next();
			connection.addRequestProperty(pair.getKey(), pair.getValue());
		}
        // connection.addRequestProperty("Cache-Control", "max-stale=" + 60*60*12);
		// connection.addRequestProperty("Cache-Control", "max-age=" + 60*60*24);

		if (this.isCancelled()) { _request.OnAborted(); return false; }

		//set content payload
		if (hasUploadContent)
		{
			if (body!=null)
			{

				int length = body.capacity();
				int progressThreshold = Math.max((length / 100), 2048);
				int steps = 1;
				int runningTotal=0;
				int bufferSize = 2048;
				body.clear();
				byte[] block = new byte[bufferSize];

				try {
					connection.setFixedLengthStreamingMode(length);
                    connection.connect();
					BufferedOutputStream out = new BufferedOutputStream(connection.getOutputStream());

					while (runningTotal<length) {
						if (this.isCancelled()) { _request.OnAborted(); return false; }

						int thisSendSize = (int)Math.min(bufferSize, (length-runningTotal));
						body.get(block, 0, thisSendSize);
						out.write(block, 0, thisSendSize);
						if ((runningTotal / progressThreshold) > steps) {
							steps = (runningTotal / progressThreshold);
							publishProgress(runningTotal,length);
						}
						runningTotal+=bufferSize;
					}

					publishProgress(runningTotal, length);
					out.flush();
                    out.close();
                } catch(SocketTimeoutException e) {
					_request.OnTimeout();
                    return false;
				} catch(Exception e) {
					_request.OnError("Unable to upload data: "+e.getLocalizedMessage());
                    return false;
				}

				body=null;
			}
		} else {
            try {
                connection.connect();
            } catch(SocketTimeoutException e) {
                _request.OnTimeout();
                return false;
            } catch(Exception e) {
            	_request.OnError("Unable to upload data: "+e.getLocalizedMessage());
                return false;
            }
        }

		// headers
		responseHeaders = HeadersToStringArray(connection, _request);
		_responseHeaders = responseHeaders;

		// responseCode
		try {
			_responseCode = connection.getResponseCode();
        } catch(SocketTimeoutException e) {
            _request.OnTimeout();
            return false;
		} catch (IOException e) {
			_request.OnError("IOException (getresponsecode): "+e.getLocalizedMessage());
			return false;
		}

		// responseMessage
		try {
			_responseMessage = connection.getResponseMessage();
        } catch(SocketTimeoutException e) {
            _request.OnTimeout();
            return false;
		} catch (IOException e) {
			_request.OnError("IOException (getresponsemessage): "+e.getLocalizedMessage());
			return false;
		}

		//result payload
		BufferedInputStream stream_b;

		try {

			stream_b = new BufferedInputStream(connection.getInputStream());
        } catch(SocketTimeoutException e) {
            _request.OnTimeout();
            return false;
		} catch (IOException e) {

			try {
				stream_b = new BufferedInputStream(connection.getErrorStream());
			} catch (Exception e2) {
				_request.OnError("IOException (getinputstream): "+e.getLocalizedMessage());
				return false;
			}
		}

		_responseStream = stream_b;
		return true;
	}

	@Override
	protected void onProgressUpdate(Integer... progress) {
		_request.OnProgress(progress[0], progress[1], true);
	}
	@Override
	protected void onPostExecute(Boolean hasResult)
	{
		if (hasResult) {
			_request.UploadDone(_responseStream, _responseHeaders, _responseCode, _responseMessage);
		}
	}

    //[TODO] Could optimize by changing chunk mode if length known
	public static HttpURLConnection NewHttpConnection(String url, String method, boolean hasPayload, int timeout, HttpRequest request, boolean verifyHost, boolean useCaching)
    {

        URL j_url = null;
        try {
            j_url = new URL(url);
        } catch (MalformedURLException e) {
        	request.OnError("Malformed URL: "+e.getLocalizedMessage());
            return null;
        }
        HttpURLConnection urlConnection = null;

        try {
        	if (j_url.getProtocol().toLowerCase(Locale.ENGLISH).equals("https") && !verifyHost) {
    			HttpsURLConnection uc = (HttpsURLConnection)j_url.openConnection();
    	    	urlConnection = uc;
        	} else {
        		urlConnection = (HttpURLConnection)j_url.openConnection();
        	}
            if (timeout>0) {
        		urlConnection.setConnectTimeout(timeout);
        		urlConnection.setReadTimeout(timeout);
        	}
            urlConnection.setUseCaches(true);
            urlConnection.setDoOutput(hasPayload);
            // urlConnection.setDoInput(true);
            urlConnection.setRequestMethod(method);
        } catch (IOException e) {
			request.OnError("IOException (newHttpConnection): "+e.getLocalizedMessage());
            return null;
        }

        return urlConnection;
    }

	public static HashMap<String,String> HeadersToStringArray(HttpURLConnection connection, HttpRequest request)
	{
		HashMap<String,String> headers = new HashMap<String,String>();
		Map<String, List<String>> headerMap;
		try {
			headerMap = connection.getHeaderFields();
		} catch (Exception e) {
			request.OnError("Error in getHeaderFields: "+e.getLocalizedMessage());
			return new HashMap<String, String>();
		}
		if (headerMap!=null){
			try {
				for (Map.Entry<String, List<String>> entry : headerMap.entrySet()) {
					String key = entry.getKey();
					if (key==null) {
						key="null";
					}
					StringBuilder sb = new StringBuilder();
					for(String s: entry.getValue()) { sb.append(s); }
					headers.put(key, sb.toString());
				}
				return headers;
			} catch (Exception e) {
				request.OnError("Error in HeadersToStringArray: "+e.getLocalizedMessage());
			}
		}
		return new HashMap<String, String>();
    }
}
