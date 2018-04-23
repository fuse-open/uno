using Uno.Compiler.API.Domain.IL;

namespace Uno.Disasm
{
    public static class Extensions
    {
        public static string GetNestedName(this DataType dt)
        {
            var result = dt.Tag as string;
            if (result == null)
            {
                var p = dt.ParentType;
                result = p != null
                    ? p.GetNestedName() + "." + dt.Name + dt.GenericSuffix
                    : dt.Name + dt.GenericSuffix;
            }
            return result;
        }
    }
}