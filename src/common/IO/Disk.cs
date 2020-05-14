using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Uno.Compiler;
using Uno.Diagnostics;
using Uno.Logging;

namespace Uno.IO
{
    public class Disk : LogObject
    {
        public static readonly Disk Default = new Disk(Log.Default);

        readonly HashSet<string> _files = new HashSet<string>();
        readonly bool _trackFiles;

        public Disk(Log log, bool trackFiles = false)
            : base(log)
        {
            _trackFiles = trackFiles;
        }

        public void CreateDirectory(string path)
        {
            try
            {
                if (path.Length > 1 && !Directory.Exists(path))
                {
                    CreateDirectory(Path.GetDirectoryName(path));

                    MarkFile(path);
                    Log.Event(IOEvent.MkDir, path);
                    var di = Directory.CreateDirectory(path);

                    // Hide directory when starts with a dot
                    if (di.Name.StartsWith('.'))
                        di.Attributes |= FileAttributes.Hidden;
                }
            }
            catch (Exception e)
            {
                Log.Error(path.ToRelativePath() + ": " + e.Message);
            }
        }

        public void CopyFile(SourceValue src, string dst, bool skipIfNewer = true)
        {
            CopyFile(src.GetFullPath(), dst, skipIfNewer, src.Source);
        }

        public void CopyFile(string src, string dst, bool skipIfNewer = true, Source errorSource = null)
        {
            try
            {
                if (Directory.Exists(dst) || dst.EndsWith('/', '\\'))
                    dst = Path.Combine(dst, Path.GetFileName(src));

                MarkFile(dst);

                if (skipIfNewer)
                {
                    var srci = new FileInfo(src);
                    var dsti = new FileInfo(dst);
                    if (srci.LastWriteTime <= dsti.LastWriteTime &&
                        srci.Length == dsti.Length)
                        return;
                }

                Log.Event(IOEvent.Write, dst);

                var dir = Path.GetDirectoryName(dst);
                if (!string.IsNullOrEmpty(dir))
                    CreateDirectory(dir);

                File.Copy(src, dst, true);
            }
            catch (Exception e)
            {
                Log.Error(errorSource ?? new Source(new SourceFile(dst)), null, e.Message);
            }
        }

        public void CopyDirectory(SourceValue src, string dst, bool skipIfNewer = true)
        {
            CopyDirectory(src.GetFullPath(), dst, skipIfNewer, src.Source);
        }

        public void CopyDirectory(string src, string dst, bool skipIfNewer = true, Source errorSource = null)
        {
            try
            {
                if (dst.EndsWith('/', '\\'))
                    dst = Path.Combine(dst, Path.GetFileName(src));

                CreateDirectory(dst);

                foreach (var srcf in Directory.EnumerateFiles(src))
                {
                    var dstf = Path.Combine(dst, Path.GetFileName(srcf));
                    CopyFile(srcf, dstf, skipIfNewer, errorSource);
                }

                foreach (var srcd in Directory.EnumerateDirectories(src))
                {
                    var dstd = Path.Combine(dst, Path.GetFileName(srcd));
                    CopyDirectory(srcd, dstd, skipIfNewer, errorSource);
                }
            }
            catch (Exception e)
            {
                Log.Error(errorSource ?? new Source(dst), null, e.Message);
            }
        }

        public void DeleteFile(string filename, bool knownToExist = false)
        {
            if (!knownToExist && !File.Exists(filename))
                return;

            try
            {
                Log.Event(IOEvent.Rm, filename);
                File.Delete(filename);
            }
            catch (Exception e)
            {
                Log.Warning(filename.ToRelativePath() + ": " + e.Message);
            }
        }

        public void DeleteDirectory(string dirName, bool knownToExist = false)
        {
            if (!knownToExist && !Directory.Exists(dirName))
                return;

            try
            {
                Log.Event(IOEvent.RmDir, dirName);
                Directory.Delete(dirName, true);
            }
            catch (Exception e)
            {
                Log.Warning(dirName.ToRelativePath() + ": " + e.Message);
            }
        }

        public void MakeExecutable(string filename)
        {
            if (PlatformDetection.IsWindows)
                return;

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = "+x \"" + filename + "\"",
                    UseShellExecute = false,
                });
            }
            catch
            {
                Log.Warning("'chmod +x \"" + filename + "\"' failed");
            }
        }

        public void WriteAllText(string filename, string text)
        {
            using (var w = CreateBufferedText(filename))
                w.Write(text);
        }

        public FileStream CreateFile(string filename)
        {
            CreateDirectory(Path.GetDirectoryName(filename));
            Log.Event(IOEvent.Write, filename);
            return File.Create(filename);
        }

        public TextWriter CreateBufferedText(string filename, NewLine newline = NewLine.Lf)
        {
            MarkFile(filename);
            return new StreamWriter(new BufferedFile(filename, this)) {NewLine = newline == NewLine.CrLf ? "\r\n" : "\n"};
        }

        public BinaryWriter CreateBufferedBinary(string filename)
        {
            MarkFile(filename);
            return new BinaryWriter(new BufferedFile(filename, this));
        }

        public TextWriter CreateText(string filename, NewLine newline = NewLine.Lf)
        {
            MarkFile(filename);
            CreateDirectory(Path.GetDirectoryName(filename));
            Log.Event(IOEvent.Write, filename);
            return new StreamWriter(filename) {NewLine = newline == NewLine.CrLf ? "\r\n" : "\n"};
        }

        public BinaryWriter CreateBinary(string filename)
        {
            MarkFile(filename);
            CreateDirectory(Path.GetDirectoryName(filename));
            Log.Event(IOEvent.Write, filename);
            return new BinaryWriter(File.OpenWrite(filename));
        }

        public bool GetFullPath(Source src, ref string path, PathFlags flags = 0)
        {
            return GetFullPath(src, src.FullPath.IsValidPath() ? Path.GetDirectoryName(src.FullPath) : null, ref path, flags);
        }

        public bool GetFullPath(Source src, string dir, ref string path, PathFlags flags = 0)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (flags.HasFlag(PathFlags.AllowNonExistingPath))
                    return true;

                Log.Error(src, null, "Expected non-empty path");
                return false;
            }

            if (!path.IsValidPath())
            {
                Log.Error(src, null, "Invalid path " + path.Quote());
                return false;
            }

            var isAbsolute = Path.IsPathRooted(path);

            if (!isAbsolute && path.Contains('\\'))
                Log.Warning(src, null, "Inconsistent separators in path " + path.Quote() + ". In Uno, paths should be separated using '/'");

            if (isAbsolute && !flags.HasFlag(PathFlags.AllowAbsolutePath))
                Log.Warning(src, null, "Absolute path " + path.Quote() + ". In Uno, paths should be relative to specifying file");

            path = path.StartsWith("@//", StringComparison.InvariantCulture)
                ? path.Substring(3).UnixToNative().ToFullPath(src.Package.SourceDirectory)
                : path.UnixToNative().ToFullPath(dir);

            if (flags.HasFlag(PathFlags.AllowNonExistingPath))
                return true;

            if (!flags.HasFlag(PathFlags.IsDirectory) && !File.Exists(path) ||
                flags.HasFlag(PathFlags.IsDirectory) && !Directory.Exists(path))
            {
                var msg = (flags.HasFlag(PathFlags.IsDirectory) ? "Directory" : "File") + " " + path.Quote() + " does not exist";

                if (flags.HasFlag(PathFlags.WarnIfNonExistingPath))
                    Log.Warning(src, null, msg);
                else
                    Log.Error(src, null, msg);

                return false;
            }

            return true;
        }

        public static bool HasValidCache(string filename, string cache, uint magic)
        {
            return !(IsNewer(filename, cache) || !HasMagicNumber(cache, magic));
        }

        public static bool HasMagicNumber(string filename, uint magic)
        {
            try
            {
                using (var f = new BinaryReader(File.OpenRead(filename)))
                    return f.ReadUInt32() == magic;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNewer(string src, string dst)
        {
            try
            {
                return File.GetLastWriteTime(src) > File.GetLastWriteTime(dst);
            }
            catch
            {
                return true;
            }
        }

        public bool HasFile(string filename)
        {
            return _files.Contains(filename.UnixToNative().ToUpperInvariant());
        }

        public void MarkFile(string filename)
        {
            if (_trackFiles)
                _files.Add(filename.UnixToNative().ToUpperInvariant());
        }

        public void DeleteOutdatedFiles(string dir)
        {
            if (!_trackFiles)
                throw new InvalidOperationException("Can't delete outdated files when " + nameof(_trackFiles) + " is disabled");

            if (Directory.Exists(dir))
            {
                foreach (var f in Directory.EnumerateDirectories(dir))
                {
                    DeleteOutdatedFiles(f);
                    if (!HasFile(f) &&
                            !Directory.EnumerateFileSystemEntries(f).Any())
                        DeleteDirectory(f, true);
                }

                foreach (var f in Directory.EnumerateFiles(dir))
                    if (!HasFile(f))
                        DeleteFile(f, true);
            }
        }
    }
}
