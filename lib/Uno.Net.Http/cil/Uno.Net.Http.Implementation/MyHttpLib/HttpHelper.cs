using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MyHttpLib
{
    public static class HttpHelper
    {
        public static TaskScheduler _scheduler = TaskScheduler.Default;

        static Task<HttpWebResponse> GetHttpResponseAsync(this HttpWebRequest request)
        {
            try
            {
                return Task.Factory.FromAsync(request.BeginGetResponse, ar =>
                {
                    try
                    {
                        return (HttpWebResponse)request.EndGetResponse(ar);
                    }
                    catch (WebException we)
                    {
                        var httpWebResponse = (HttpWebResponse) we.Response;
                        if (httpWebResponse != null)
                            return httpWebResponse;

                        throw;
                    }
                }, null);
            }
            catch (Exception ex)
            {
                return TaskAsyncHelper.FromError<HttpWebResponse>(ex);
            }
        }

        static Task<byte[]> ReadHttpResponseByteArrayAsync(this HttpWebResponse response, Action<int, int> progress)
        {
            return Task.Run(async () =>
            {
                using (var responseStream = response.GetResponseStream())
                {
                    int read = 0;
                    int offset = 0;

                    var responseBuffer = new byte[4096];

                    if (responseStream != null)
                    {
                        using (var contentStream = new MemoryStream())
                        {
                            do
                            {
                                read = await responseStream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                                await contentStream.WriteAsync(responseBuffer, 0, read);
                                offset += read;

                                progress(offset, (int) response.ContentLength);
                            } while (read != 0);
                            return contentStream.ToArray();
                        }
                    }
                    return null;
                }
            });
        }

        static Task<Stream> GetHttpRequestStreamAsync(this HttpWebRequest request)
        {
            try
            {
                return Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, null);
            }
            catch (Exception ex)
            {
                return TaskAsyncHelper.FromError<Stream>(ex);
            }
        }


        public static Task<HttpWebResponse> Send(HttpWebRequest request, byte[] buffer)
        {
            // Set the content length if the buffer is non-null
            request.ContentLength = buffer != null ? buffer.LongLength : 0;

            if (buffer == null)
            {
                // If there's nothing to be written to the request then just get the response
                return request.GetHttpResponseAsync();
            }
            else
            {
                request.ContentLength = buffer.LongLength;
            }

            // Write the post data to the request stream
            return request.GetHttpRequestStreamAsync()
                          .Then(stream => stream.WriteAsync(buffer).Then(stream.Dispose))
                          .Then(() => request.GetHttpResponseAsync());
        }

        public static void Send(HttpWebRequest request,
                                byte[] buffer,
                                Action<HttpWebResponse> onHeadersReceived,
                                Action<byte[]> onDone,
                                Action<int, int> onProgress,
                                Action<Exception> exceptionCallback)
        {
            Send(request, buffer).Then(httpWebResponse =>
            {
                if(httpWebResponse == null)
                    throw new ArgumentNullException("httpWebResponse");

                onHeadersReceived(httpWebResponse);
                httpWebResponse.ReadHttpResponseByteArrayAsync(onProgress).ContinueWith(b => onDone(b.Result));
            }).ContinueWith(task =>
            {
                if (!task.IsFaulted || task.Exception == null) return;

                foreach (var exception in task.Exception.Flatten().InnerExceptions)
                    exceptionCallback(exception);

            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}