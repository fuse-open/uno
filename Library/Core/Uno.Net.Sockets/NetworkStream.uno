using Uno;
using Uno.IO;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Net.Sockets
{

    [DotNetType("System.Net.Sockets.NetworkStream")]
    public class NetworkStream : Stream
    {
        protected Socket Socket { get { return _socket; } }

        readonly Socket _socket;

        public NetworkStream(Socket socket)
        {
            _socket = socket;
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position
        {
            get { return 0; }
            set { }
        }

        public override void SetLength(long value) { }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override int Read(byte[] dst, int byteOffset, int byteCount)
        {
            return _socket.Receive(dst, byteOffset, byteCount, SocketFlags.None);
        }

        public override void Write(byte[] src, int byteOffset, int byteCount)
        {
            var sent = _socket.Send(src, byteOffset, byteCount, SocketFlags.None);

            if (sent != byteCount)
                throw new Exception("sent != byteCount");
        }

        public override long Seek(long byteOffset, SeekOrigin origin)
        {
            return 0;
        }

        public override void Flush() { }
        
        public virtual bool DataAvailable
        {
            get { return _socket.Available > 0; }
        }
    }
}
