using Uno;
using Uno.IO;

namespace Uno.UX
{
    public abstract class FileSource
    {
        public readonly string Name;

        public event EventHandler<EventArgs> DataChanged;

        public void OnDataChanged()
        {
            var dataChanged = DataChanged;
            if (dataChanged != null)
                dataChanged(this, EventArgs.Empty);
        }

        protected FileSource(string name)
        {
            if (name == null)
                throw new ArgumentException("File name can't be null", nameof(name));

            Name = name;
        }

        public static implicit operator FileSource(BundleFile bundleFile)
        {
            return new BundleFileSource(bundleFile);
        }

        public abstract Stream OpenRead();

        public virtual byte[] ReadAllBytes()
        {
            return new BinaryReader(OpenRead()).ReadAllBytes();
        }

        public virtual string ReadAllText()
        {
            return new StreamReader(OpenRead()).ReadToEnd();
        }
    }

    static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer;
                do
                {
                    buffer = reader.ReadBytes(bufferSize);
                    ms.Write(buffer, 0, buffer.Length);
                } while(buffer.Length == bufferSize);

                    return ms.ToArray();
                }

                throw new InvalidOperationException("Bug in Uno compiler, this case should never ever happen.");
            }

        public static byte[] ToArray(this MemoryStream memoryStream)
        {
            var buffer = memoryStream.GetBuffer();
            var bytes = new byte[(int)memoryStream.Length];
            for(var i = 0;i < memoryStream.Length;++i)
            {
                bytes[i] = buffer[i];
            }

            return bytes;
        }
    }
}
