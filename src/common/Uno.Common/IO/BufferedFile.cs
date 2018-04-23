using System;
using System.IO;
using System.Text;
using Uno.Logging;

namespace Uno.IO
{
    public class BufferedFile : MemoryStream
    {
        readonly string _filename;
        readonly Disk _disk;

        internal BufferedFile(string filename, Disk disk)
        {
            _disk = disk;
            _filename = filename;
        }

        public override void Close()
        {
            try
            {
                var len = (int)Length;
                var buf = GetBuffer();
                var info = new FileInfo(_filename);

                if (info.Exists)
                {
                    if (len == info.Length)
                    {
                        var file = File.ReadAllBytes(_filename);

                        for (int i = 0; i < len; i++)
                        {
                            if (file[i] != buf[i])
                            {
                                if (!_disk.Log.IsUltraVerbose)
                                    goto WRITE_FILE;

                                Diff(buf, len);
                                goto WRITE_FILE;
                            }
                        }

                        return;
                    }

                    if (!_disk.Log.IsUltraVerbose)
                        goto WRITE_FILE;

                    Diff(buf, len);
                    _disk.Log.UltraVerbose("(" + len + " bytes in buffer, " + info.Length + " bytes on disk)");
                }
                else
                    _disk.CreateDirectory(Path.GetDirectoryName(_filename));

            WRITE_FILE:
                _disk.Log.Event(IOEvent.Write, _filename);
                using (var f = new FileStream(_filename, FileMode.Create))
                    f.Write(buf, 0, len);
            }
            finally
            {
                base.Close();
            }
        }

        void Diff(byte[] buf, int len)
        {
            var bufferString = Encoding.UTF8.GetString(buf, 0, len);
            var cachedString = File.ReadAllText(_filename);

            for (int j = 0; j < Math.Min(bufferString.Length, cachedString.Length); j++)
            {
                if (bufferString[j] != cachedString[j])
                {
                    int bStart = j, bEnd = j;
                    int cStart = j, cEnd = j;
                    int bLine = 1, cLine = 1;

                    while (bStart > 0 && bufferString[bStart - 1] != '\n')
                        bStart--;

                    while (cStart > 0 && cachedString[cStart - 1] != '\n')
                        cStart--;

                    while (bEnd < bufferString.Length && bufferString[bEnd] != '\n')
                        bEnd++;

                    while (cEnd < cachedString.Length && cachedString[cEnd] != '\n')
                        cEnd++;

                    for (int k = 0; k < bStart; k++)
                        if (bufferString[k] == '\n')
                            bLine++;

                    for (int k = 0; k < cStart; k++)
                        if (cachedString[k] == '\n')
                            cLine++;

                    _disk.Log.WriteLine("buffer(" + bLine + "): " + bufferString.Substring(bStart, bEnd - bStart).Trim(), ConsoleColor.Cyan);
                    _disk.Log.WriteLine("cached(" + cLine + "): " + cachedString.Substring(cStart, cEnd - cStart).Trim(), ConsoleColor.Blue);
                    return;
                }
            }
        }
    }
}
