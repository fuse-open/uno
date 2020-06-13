using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Backends
{
    public static class PInvokeHelper
    {
        public static bool IsPInvokable(this Backend backend, IEssentials essentials, Function f)
        {
            foreach (var attr in f.Attributes)
            {
                if (attr.ReferencedType == essentials.ForeignAttribute &&
                    essentials.Language.Literals[(int)attr.Arguments[0].ConstantValue].Name == "CPlusPlus")
                {
                    if (!f.IsStatic)
                        backend.Log.Error(f.Source, ErrorCode.E0000, "Foreign CPlusPlus methods must be static.");
                    return true;
                }
            }
            return false;
        }

        public static bool IsPInvokable(this Backend backend, IEssentials essentials, DelegateType dt)
        {
            foreach (var attr in dt.Attributes)
            {
                if (attr.ReferencedType == essentials.ForeignAttribute &&
                    essentials.Language.Literals[(int)attr.Arguments[0].ConstantValue].Name == "CPlusPlus")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
