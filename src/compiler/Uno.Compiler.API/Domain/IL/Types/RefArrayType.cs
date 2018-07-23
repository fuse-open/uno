using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Types
{
    public sealed class RefArrayType : ArrayType
    {
        public static RefArrayType CreateMaster(DataType intType, DataType objectType)
        {
            var result = new RefArrayType(objectType.Source, objectType, objectType);
            var length = new Property(objectType.Source, null, Modifiers.Public | Modifiers.Extern | Modifiers.Generated, "Length", result, intType);
            length.CreateGetMethod(objectType.Source, length.Modifiers);
            result.Properties.Add(length);
            return result;
        }

        public static RefArrayType Create(RefArrayType master, DataType elmType)
        {
            var result = new RefArrayType(elmType.Source, elmType, master.Base);
            result.SetMasterDefinition(master);
            var length = new Property(elmType.Source, null, Modifiers.Public | Modifiers.Extern | Modifiers.Generated, "Length", result, master.Properties[0].ReturnType);
            length.CreateGetMethod(elmType.Source, length.Modifiers);
            length.SetMasterDefinition(master.Properties[0]);
            result.Properties.Add(length);
            return result;
        }

        public override TypeType TypeType => TypeType.RefArray;
        public override string FullName => ElementType + "[]";

        RefArrayType(Source src, DataType elementType, DataType objectType)
            : base(src, "array", elementType, objectType)
        {
        }

        public override int GetHashCode()
        {
            return ElementType.GetHashCode() * 13 + 359;
        }

        public override bool Equals(object obj)
        {
            return obj is RefArrayType && ElementType.Equals((obj as RefArrayType).ElementType);
        }
    }
}