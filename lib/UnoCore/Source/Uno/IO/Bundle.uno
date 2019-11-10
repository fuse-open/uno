using Android.Base;
using Android.Base.Wrappers;
using System;
using System.Reflection;
using Uno.Collections;
using Uno.Compiler;
using Uno.Compiler.ExportTargetInterop;
using Uno.Runtime.Implementation;
using Uno.Threading;
using Uno.Platform;
using Uno.Text;

namespace System
{
    [extern(DOTNET) DotNetType("System.AppDomain")]
    extern(DOTNET)
    public class AppDomain
    {
        public static extern AppDomain CurrentDomain { get; }
        public extern Assembly[] GetAssemblies();
    }
}

namespace Uno.IO
{
    public class BundleFile
    {
        extern(PREVIEW) byte[] _bytes;

        // Listen for when preview updates a file
        public event Action<BundleFile> Changed;

        [WeakReference]
        public Bundle Bundle { get; private set; }
        public string Name { get { return Path.GetFileName(SourcePath); } }
        public string DirectoryName { get { return Path.GetDirectoryName(SourcePath); } }
        public string SourcePath { get; private set; }
        public string BundlePath { get; private set; }
        public bool IsFile { get; private set; }

        public string NativeBundlePath
        {
            get
            {
                if defined(CPLUSPLUS && !ANDROID)
                    return Path.Combine(BundleDirectory, BundlePath);
                else
                    return BundlePath;
            }
        }

        extern(CPLUSPLUS && !ANDROID)
        static readonly string BundleDirectory = GetBundleDirectory();

        extern(CPLUSPLUS && !ANDROID)
        static string GetBundleDirectory()
        {
            var i = 0;
            for (var parent = Directory.GetBaseDirectory();;
                     parent = Path.GetDirectoryName(parent))
            {
                // Give up after four levels. This is enough to find BUILD_DIR/data, for example when the executable is located
                // in any of the following trees, or similar: BUILD_DIR, BUILD_DIR/bin/Debug or BUILD_DIR/bin/x64/Debug.
                if (string.IsNullOrEmpty(parent) || i++ == 4)
                    throw new FileNotFoundException("Bundle not found: " + @(BundleDirectory), @(BundleDirectory));

                var test = Path.Combine(parent, @(BundleDirectory));
                if (Directory.Exists(test))
                    return test;
            }
        }

        internal BundleFile(Bundle bundle, string sourcePath, string bundlePath = null)
        {
            Bundle = bundle;
            SourcePath = sourcePath;
            BundlePath = bundlePath;

            if defined(CPLUSPLUS && !ANDROID)
                IsFile = bundlePath != null;
            else
                IsFile = false;
        }

        public Stream OpenRead()
        {
            if defined(PREVIEW)
            {
                if (_bytes != null)
                    return new MemoryStream(_bytes, false);
            }

            if (BundlePath == null)
                throw new ArgumentNullException(nameof(BundlePath));

            if defined(ANDROID)
                return new AAssetStream(BundlePath);
            else if defined(CPLUSPLUS)
                return File.OpenRead(NativeBundlePath);
            else if defined(DOTNET)
            {
                var asm = Assembly;

                foreach (var name in asm.GetManifestResourceNames())
                    if (name == BundlePath)
                        return asm.GetManifestResourceStream(BundlePath);

                throw new FileNotFoundException("Manifest resource not found: " + BundlePath, BundlePath);
            }
            else
                throw new NotImplementedException();
        }

        extern(DOTNET)
        static Dictionary<String, Assembly> _assemblyMap = new Dictionary<String, Assembly>();

        extern(DOTNET)
        static Assembly Assembly
        {
            get
            {
                if (CoreApp.Current != null)
                    return CoreApp.Current.GetType().Assembly;

                // Nasty hack to find the user code assembly when Uno.Application.Current is not set
                var e = Assembly.GetEntryAssembly();
                var p = Path.Combine(Path.GetDirectoryName(e.Location), Path.GetFileNameWithoutExtension(e.Location) + ".dll").ToUpper();

                if (_assemblyMap.ContainsKey(p))
                {
                    return _assemblyMap[p];
                }

                // Not all assemblies have locations.
                // We don't care about the ones that don't.
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (asm.Location.ToUpper() == p){
                            _assemblyMap[p] = asm;
                            return asm;
                        }
                    }
                    catch (NotSupportedException ex)
                    { }
                }

                throw new ArgumentNullException(nameof(Assembly));
            }
        }

        public byte[] ReadAllBytes()
        {
            using (var stream = OpenRead())
            {
                var result = new byte[(int)stream.Length];
                stream.Read(result, 0, result.Length);
                return result;
            }
        }

        public string ReadAllText()
        {
            var bytes = defined(PREVIEW)
                ? _bytes ?? ReadAllBytes()
                : ReadAllBytes();
            var count = bytes.Length;
            var index = 0;

            // Strip Byte Order Mark for UTF-8.
            // This is consistent with File.ReadAllText().
            if (count > 2 && 
                bytes[0] == 0xEF &&
                bytes[1] == 0xBB &&
                bytes[2] == 0xBF)
            {
                index = 3;
                count -= 3;
            }

            return Utf8.GetString(bytes, index, count);
        }

        /** Must be called from Main-thread */
        extern(PREVIEW)
        public void Update(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            BundlePath = null;
            IsFile = false;
            _bytes = bytes;

            if (Changed != null)
                Changed(this);
        }

        public override string ToString()
        {
            return SourcePath;
        }
    }

    public class Bundle
    {
        // Listen for when preview creates a file
        public event Action<BundleFile> Created;

        /** Must be called from Main-thread */
        extern(PREVIEW)
        public BundleFile CreateFile(string path, byte[] bytes)
        {
            var file = new BundleFile(this, path);
            file.Update(bytes);

            if (Created != null)
                Created(file);

            lock (_files)
                _files.Add(file);
            lock (_allFiles)
                _allFiles.Add(file);

            return file;
        }

        static readonly Dictionary<string, Bundle> _bundles = new Dictionary<string, Bundle>();
        static readonly List<BundleFile> _allFiles = new List<BundleFile>();

        static Bundle()
        {
            // Load index file from dummy bundle
            foreach (var line in new BundleFile(
                        new Bundle(null),
                        "bundle",
                        "bundle")
                    .ReadAllText()
                    .Split('\n'))
            {
                var parts = line.Split(':');
                var bundle = new Bundle(parts[0]);
                _bundles[parts[0]] = bundle;

                for (int i = 1; i < parts.Length; i += 2)
                {
                    var file = new BundleFile(bundle, parts[i], parts[i + 1]);
                    bundle._files.Add(file);
                    _allFiles.Add(file);
                }
            }
        }

        public static Bundle Get([CallerPackageName] string package = "")
        {
            return _bundles[package];
        }

        // should be IReadOnlyList, but missing in Uno
        public static IEnumerable<BundleFile> AllFiles
        {
            get { return _allFiles; }
        }

        // should be IReadOnlyList, but missing in Uno
        public static IEnumerable<Bundle> Bundles
        {
            get { return _bundles.Values; }
        }

        readonly string _packageName;
        readonly List<BundleFile> _files = new List<BundleFile>();

        Bundle(string packageName)
        {
            _packageName = packageName;
        }

        public string PackageName
        {
            get { return _packageName; }
        }

        public IEnumerable<BundleFile> Files
        {
            get { return _files; }
        }

        public BundleFile GetFile(string filename)
        {
            foreach (var f in _files)
                if (f.SourcePath == filename || f.BundlePath == filename)
                    return f;

            throw new FileNotFoundException("BundleFile not found: " + filename, filename);
        }

        public override string ToString()
        {
            return _packageName;
        }
    }

    [TargetSpecificType]
    [Set("TypeName", "::AAsset*")]
    [Set("Include", "android/asset_manager.h")]
    extern(ANDROID) struct AAssetPtr
    {
    }

    [TargetSpecificType]
    [Set("TypeName", "::AAssetManager*")]
    [Set("Include", "android/asset_manager.h")]
    extern(ANDROID) struct AAssetManagerPtr
    {
    }

    extern(ANDROID) static class AAssetManager
    {
        public static AAssetManagerPtr Ptr = GetPtr();

        public static AAssetPtr OpenOrThrow(string filename)
        {
            var retval = extern<AAssetPtr> "AAssetManager_open(@{Ptr}, uCString($0).Ptr, AASSET_MODE_STREAMING)";
            if (extern<bool> "!retval")
                throw new FileNotFoundException("Asset not found: " + filename, filename);
            return retval;
        }

        [Require("Source.Include", "android/asset_manager_jni.h")]
        static AAssetManagerPtr GetPtr()
        {
            var env = JNI.GetEnvPtr();
            var jobject = ((IJWrapper) GetJavaObject())._GetJavaObject();
            return extern<AAssetManagerPtr>(env, jobject) "AAssetManager_fromJava($@)";
        }

        [Foreign(Language.Java)]
        static Java.Object GetJavaObject()
        @{
            return com.fuse.Activity.getRootActivity().getAssets();
        @}
    }
    
    extern(ANDROID) class AAssetStream : Stream
    {
        AAssetPtr _fp;

        public AAssetStream(string filename)
        {
            _fp = AAssetManager.OpenOrThrow(filename);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override long Length
        {
            get
            @{
                @{$$.CheckDisposed():Call()};
                return AAsset_getLength(@{$$._fp});
            @}
        }

        public override long Position
        {
            get
            @{
                @{$$.CheckDisposed():Call()};
                return AAsset_getLength(@{$$._fp}) - AAsset_getRemainingLength(@{$$._fp});
            @}
            set
            @{
                @{$$.CheckDisposed():Call()};
                AAsset_seek(@{$$._fp}, $0, SEEK_SET);
            @}
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] dst, int byteOffset, int byteCount)
        {
            if (dst == null)
                throw new ArgumentNullException(nameof(dst));

            CheckDisposed();
            return extern<int> "AAsset_read(@{$$._fp}, (uint8_t*) $0->Ptr() + $1, $2)";
        }

        public override void Write(byte[] src, int byteOffset, int byteCount)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long byteOffset, SeekOrigin origin)
        @{
            @{$$.CheckDisposed():Call()};

            switch ($1)
            {
            case @{SeekOrigin.Begin}:
                AAsset_seek(@{$$._fp}, $0, SEEK_SET);
                break;
            case @{SeekOrigin.Current}:
                AAsset_seek(@{$$._fp}, $0, SEEK_CUR);
                break;
            case @{SeekOrigin.End}:
                AAsset_seek(@{$$._fp}, $0, SEEK_END);
                break;
            }

            return AAsset_getLength(@{$$._fp}) - AAsset_getRemainingLength(@{$$._fp});
        @}

        public override void Flush()
        {
        }

        public override void Dispose(bool disposing)
        @{
            if (!@{$$._fp})
                return;
            AAsset_close(@{$$._fp});
            @{$$._fp} = NULL;
        @}

        void CheckDisposed()
        {
            if (!extern<bool> "@{$$._fp}")
                throw new ObjectDisposedException("The asset stream was closed");
        }
    }
}
