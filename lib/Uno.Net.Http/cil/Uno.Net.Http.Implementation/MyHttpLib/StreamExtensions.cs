using System;
using System.IO;
using System.Threading.Tasks;

namespace MyHttpLib
{
    internal static class StreamExtensions
    {
        public static Task<int> ReadAsync(this Stream stream, byte[] buffer)
        {
            try
            {
                return Task.Factory.FromAsync((cb, state) => stream.BeginRead(buffer, 0, buffer.Length, cb, state),
                                              ar => stream.EndRead(ar),
                                              null);
            }
            catch (Exception ex)
            {
                return TaskAsyncHelper.FromError<int>(ex);
            }
        }

        public static Task WriteAsync(this Stream stream, byte[] buffer)
        {
            try
            {
                return Task.Factory.FromAsync((cb, state) => stream.BeginWrite(buffer, 0, buffer.Length, cb, state),
                                              ar => stream.EndWrite(ar),
                                              null);
            }
            catch (Exception ex)
            {
                return TaskAsyncHelper.FromError<object>(ex);
            }
        }
    }
}