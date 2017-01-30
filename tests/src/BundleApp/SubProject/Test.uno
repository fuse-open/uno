using Uno;
using Uno.IO;

namespace SubProject
{
    public class Test
    {
        public static void Assert()
        {
            foreach (var f in Bundle.AllFiles)
            {
                if(f.SourcePath == "js/SubProjectBundleFile.js")
                {
                    debug_log("SubProject Success!");
                    return;
                }
            }
            debug_log("SubProject Failed!");
        }
    }
}