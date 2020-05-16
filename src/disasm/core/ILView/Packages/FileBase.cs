using System;
using System.IO;

namespace Uno.Disasm.ILView.Packages
{
    public abstract class FileBase : ILItem
    {
        public abstract string FullName { get; }

        string _contents;
        public string Contents
        {
            get
            {
                if (_contents == null)
                {
                    var name = FullName;
                    _contents = File.Exists(name)
                        ? File.ReadAllText(FullName)
                        : "";
                }

                return _contents;
            }
        }

        public override ILIcon Icon
        {
            get
            {
                switch (Path.GetExtension(FullName).ToUpperInvariant())
                {
                    case ".UNO":
                        return ILIcon.UnoDocument;
                    case ".UX":
                    case ".UXL":
                        return ILIcon.UxlDocument;
                    case ".JPG":
                    case ".JPEG":
                    case ".PNG":
                        return ILIcon.Texture2D;
                    default:
                        return ILIcon.File;
                }
            }
        }

        public override Syntax Syntax
        {
            get
            {
                switch (Path.GetExtension(FullName).ToUpperInvariant())
                {
                    case ".CS":
                    case ".UNO":
                        return Syntax.Uno;
                    case "":
                    case ".C":
                    case ".CC":
                    case ".CPP":
                    case ".H":
                    case ".JAVA":
                    case ".JS":
                    case ".M":
                    case ".MM":
                    case ".BASH":
                    case ".SH":
                    case ".PCH":
                        return Syntax.Foreign;
                    case ".UX":
                    case ".UXL":
                    case ".XML":
                    case ".HTML":
                    case ".XHTML":
                    case ".CSPROJ":
                    case ".VCXPROJ":
                    case ".PLIST":
                    case ".IML":
                    case ".ANDROIDPROJ":
                    case ".FILTERS":
                        return Syntax.UXL;
                    default:
                        try
                        {
                            return Contents.Contains("<?xml ")
                                ? Syntax.UXL
                                : Syntax.Stuff;
                        }
                        catch (Exception)
                        {
                            return Syntax.Stuff;
                        }
                }
            }
        }
    }
}