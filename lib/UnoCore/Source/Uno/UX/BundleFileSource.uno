using Uno;
using Uno.IO;

namespace Uno.UX
{
    public class BundleFileSource : FileSource
    {
        public readonly BundleFile BundleFile;
        public BundleFileSource(BundleFile bundleFile) : base(bundleFile.SourcePath)
        {
            BundleFile = bundleFile;
            BundleFile.Changed += OnChanged;
        }

        void OnChanged(BundleFile bf)
        {
        	OnDataChanged();
        }

        public override Stream OpenRead()
        {
            return BundleFile.OpenRead();
        }

        public override byte[] ReadAllBytes()
        {
            return BundleFile.ReadAllBytes();
        }

        public override string ReadAllText()
        {
            return BundleFile.ReadAllText();
        }

        public override bool Equals(object o)
        {
            var bfs = o as BundleFileSource;
            if (bfs == null)
                return false;

            return bfs.BundleFile.Equals(BundleFile);
        }

        public override int GetHashCode()
        {
            return BundleFile.GetHashCode();
        }
    }
}
