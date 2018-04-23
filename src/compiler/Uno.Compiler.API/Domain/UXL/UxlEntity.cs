using System.Collections.Generic;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.Serialization;

namespace Uno.Compiler.API.Domain.UXL
{
    public class UxlEntity
    {
        public readonly List<CopyFile> CopyFiles = new List<CopyFile>();
        public readonly List<ImageFile> ImageFiles = new List<ImageFile>();
        public readonly List<UxlElement> Elements;

        public UxlEntity(List<UxlElement> elements = null)
        {
            Elements = elements ?? new List<UxlElement>();
        }

        protected UxlEntityFlags EntityFlags
        {
            get
            {
                UxlEntityFlags flags = 0;
                if (Elements.Count > 0)
                    flags |= UxlEntityFlags.Elements;
                if (CopyFiles.Count > 0)
                    flags |= UxlEntityFlags.CopyFiles;
                if (CopyFiles.Count > 0)
                    flags |= UxlEntityFlags.ImageFiles;

                return flags;
            }
        }

        protected void WriteEntity(CacheWriter f, UxlEntityFlags flags)
        {
            if (flags.HasFlag(UxlEntityFlags.Elements))
                f.WriteList(Elements, (w, x) => x.Write(w));
            if (flags.HasFlag(UxlEntityFlags.CopyFiles))
                f.WriteList(CopyFiles, (w, x) => w.Write(x));
            if (flags.HasFlag(UxlEntityFlags.ImageFiles))
                f.WriteList(ImageFiles, (w, x) => w.Write(x));
        }

        protected void ReadEntity(CacheReader f, UxlEntityFlags flags)
        {
            if (flags.HasFlag(UxlEntityFlags.Elements))
                f.ReadList(Elements, UxlElement.Read);
            if (flags.HasFlag(UxlEntityFlags.CopyFiles))
                f.ReadList(CopyFiles, r => f.ReadCopyFile());
            if (flags.HasFlag(UxlEntityFlags.ImageFiles))
                f.ReadList(ImageFiles, r => f.ReadImageFile());
        }
    }
}