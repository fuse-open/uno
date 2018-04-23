using System.IO;
using System.Text;

namespace Uno.Disasm
{
    public class SynchronizedWriter : TextWriter
    {
        public readonly StringBuilder Buffer = new StringBuilder();

        public override Encoding Encoding => Encoding.Unicode;

        public override void Write(char value)
        {
            lock (Buffer)
                Buffer.Append(value);
        }

        public override void Write(string value)
        {
            lock (Buffer)
                Buffer.Append(value);
        }
    }
}