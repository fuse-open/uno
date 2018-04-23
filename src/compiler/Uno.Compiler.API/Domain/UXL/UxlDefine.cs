using Uno.Compiler.API.Domain.Serialization;

namespace Uno.Compiler.API.Domain.UXL
{
    public struct UxlDefine
    {
        public readonly string Name;
        public readonly SourceValue Condition;

        public UxlDefine(string name, SourceValue cond)
        {
            Name = name;
            Condition = cond;
        }

        public void Write(CacheWriter f)
        {
            f.WriteGlobal(Name);
            f.WriteGlobal(Condition);
        }

        public static UxlDefine Read(CacheReader f)
        {
            var name = f.ReadGlobalString();
            var cond = f.ReadGlobalValue();
            return new UxlDefine(name, cond);
        }
    }
}