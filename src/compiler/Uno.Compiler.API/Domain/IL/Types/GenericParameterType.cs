using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Types
{
    public class GenericParameterType : DataType
    {
        public GenericConstraintType ConstraintType { get; private set; }

        public GenericParameterType(Source source, DataType parameterOwner, string name)
            : base(source, parameterOwner, null, Modifiers.Public, name)
        {
        }

        public bool IsGenericMethodParameter
        {
            get
            {
                var p = (DataType)Parent;
                if (p.Methods.Count == 1)
                {
                    var m = p.Methods[0];
                    return m.IsGenericDefinition && m.GenericType == p;
                }

                return false;
            }
        }

        public bool IsGenericTypeParameter => !IsGenericMethodParameter;

        public Method GenericMethodParent => ((DataType)Parent).Methods[0];

        public DataType GenericTypeParent => (DataType)Parent;

        public void SetConstraintType(GenericConstraintType type)
        {
            ConstraintType = type;
        }

        public override TypeType TypeType => TypeType.GenericParameter;

        public override bool Equals(object obj)
        {
            return obj is GenericParameterType && ReferenceEquals(MasterDefinition, (obj as GenericParameterType).MasterDefinition);
        }

        public override int GetHashCode()
        {
            return MasterDefinition.Name.GetHashCode();
        }

        public override string ToString()
        {
            return MasterDefinition.Name;
        }
    }
}
