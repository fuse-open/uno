using Uno;
using Uno.IO;

namespace BundleApp
{
    public class Main : Uno.Application
    {
        public Main()
        {
            BundleLib.Test.Assert();
            SubProject.Test.Assert();
            Assert("BundleFile.pdf");
            Assert("js/SubProjectBundleFile.js");
            Assert("js/BundleLibBundleFile.js");

            // This should not throw
            Bundle.Get();
        }

        public static void Assert(string filepath)
        {
            foreach (var f in Bundle.AllFiles)
            {
                if (f.SourcePath == filepath)
                {
                    debug_log filepath + " Success!";
                    return;
                }
            }

            debug_log filepath + " Failed!";
        }
    }
}
