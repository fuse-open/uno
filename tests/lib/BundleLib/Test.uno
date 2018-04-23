using Uno;
using Uno.IO;

namespace BundleLib
{
	public class Test
    {
		public static void Assert()
		{
			foreach (var f in Bundle.AllFiles)
			{
				if(f.SourcePath == "js/BundleLibBundleFile.js")
				{
					debug_log("BundleLib Success!");
					return;
				}
			}
			debug_log("BundleLib Failed!");
		}
    }
}