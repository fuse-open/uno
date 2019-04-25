using System;
using System.IO;
using System.Linq;
using System.Threading;
using Uno.IO;
using Uno.Logging;

namespace Uno.Build.Stuff
{
    public struct FileLock : IDisposable
    {
        static readonly Random Rand = new Random();
        readonly LockFile[] _locks;

        public FileLock(Log log, params string[] filenames)
        {
            _locks = new LockFile[filenames.Length];

            for (int i = 0; i < _locks.Length; i++)
            {
                Disk.CreateDirectory(log, Path.GetDirectoryName(filenames[i]));
                _locks[i] = new LockFile(filenames[i] + ".lock");
            }

            for (int tries = 1; !TryAcquireAll(); tries++)
            {
                if (tries == 2000)
                    throw new TimeoutException("Failed to lock " + this + "; aborting after " + tries + " tries");
                if (tries % 50 == 0)
                    log.Warning("Failed to lock " + this + "; retrying...");

                // Spin wait while another process owns the file(s),
                // Some randomness helps with race conditions
                Thread.Sleep(Rand.Next(200, 500));
            }
        }

        public void Dispose()
        {
            foreach (var e in _locks)
                e.Release();
        }

        public override string ToString()
        {
            return _locks.Length == 0
                ? "(empty)"
                : string.Join(", ", _locks.Select(x => x.Filename.ToRelativePath()));
        }

        bool TryAcquireAll()
        {
            int acquired = 0;
            foreach (var e in _locks)
                if (e.TryAcquire())
                    acquired++;

            if (acquired == _locks.Length)
                return true;

            foreach (var e in _locks)
                e.Release();

            return false;
        }

        class LockFile
        {
            internal readonly string Filename;
            FileStream _stream;

            internal LockFile(string filename)
            {
                Filename = filename;
            }

            internal bool TryAcquire()
            {
                if (_stream != null)
                    return true;

                try
                {
                    _stream = File.OpenWrite(Filename);
                    _stream.Lock(0, 0);
                    return true;
                }
                catch (IOException)
                {
                    if (_stream != null)
                    {
                        _stream.Dispose();
                        _stream = null;
                    }
                    return false;
                }
            }

            internal void Release()
            {
                if (_stream == null)
                    return;

                _stream.Dispose();
                _stream = null;

                try
                {
                    File.Delete(Filename);
                }
                catch
                {
                    // ignore
                }
            }
        }
    }
}
