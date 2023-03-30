using Uno.Compiler.ExportTargetInterop;
using Uno;

namespace Android.Base.Types
{
    public class DirectBuffer : IDisposable
    {
        IntPtr _ptr;
        long _size;
        IDisposable _dispose;

        private DirectBuffer(IntPtr ptr, long size, IDisposable dispose)
        {
            _ptr = ptr;
            _size = size;
            _dispose = dispose;
        }

        public long Size { get { return _size; } }
        public IntPtr Handle { get { return _ptr; } }

        public static DirectBuffer Create(IntPtr ptr, long size)
        {
            return new DirectBuffer(ptr, size, null);
        }

        public static DirectBuffer Create(IntPtr ptr, long size, IDisposable dispose)
        {
            return new DirectBuffer(ptr, size, dispose);
        }

        void IDisposable.Dispose()
        {
            if (_dispose != null) {
                _dispose.Dispose();
            }
            _dispose = null;
        }
    }
}