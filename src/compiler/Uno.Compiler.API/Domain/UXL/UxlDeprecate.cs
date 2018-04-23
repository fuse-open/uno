using Uno.Compiler.API.Domain.Serialization;

namespace Uno.Compiler.API.Domain.UXL
{
    public class UxlDeprecate
    {
        public readonly Source Source;
        public readonly string OldName;
        public readonly string NewName;

        public UxlDeprecate(Source src, string oldName, string newName)
        {
            Source = src;
            OldName = oldName;
            NewName = newName;
        }

        public void Write(CacheWriter f)
        {
            f.Write(Source);
            f.WriteGlobal(OldName);
            f.WriteGlobal(NewName);
        }

        public static UxlDeprecate Read(CacheReader f)
        {
            var src = f.ReadSource();
            var oldName = f.ReadGlobalString();
            var newName = f.ReadGlobalString();
            return new UxlDeprecate(src, oldName, newName);
        }
    }
}
