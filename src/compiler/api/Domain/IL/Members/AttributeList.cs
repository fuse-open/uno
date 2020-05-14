using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public static class AttributeList
    {
        public static NewObject[] Empty { get; } = new NewObject[0];

        public static NewObject[] Copy(this NewObject[] attributes, CopyState state)
        {
            if (attributes == null) return null;

            var result = new NewObject[attributes.Length];

            for (int i = 0; i < result.Length; i++)
                result[i] = (NewObject)attributes[i].CopyExpression(state);

            return result;
        }
    }
}