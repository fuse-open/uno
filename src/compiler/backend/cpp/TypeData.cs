using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Backends.CPlusPlus
{
    class TypeData
    {
        public ReferenceType? Reference;
        public string Declaration;
        public string Include;
        public CppType Type;

        public static TypeData Get(DataType dt)
        {
            return dt.Tag as TypeData ?? (TypeData)(dt.Tag = new TypeData());
        }
    }
}