using System.IO;

namespace Uno.Disasm.ILView
{
    public static class ILIconInfo
    {
        public static string GetManifestResourceName(ILIcon icon)
        {
            return "Uno.Disasm.ILView.Icons." + icon + ".png";
        }

        public static Stream OpenRead(ILIcon icon)
        {
            return typeof(ILIconInfo).Assembly.GetManifestResourceStream(GetManifestResourceName(icon));
        }
    }
}