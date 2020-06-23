using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Logging;

namespace Uno.Compiler.API.Backends
{
    public static class PInvokeHelper
    {
        public static bool IsPInvokable(this Function f, IEssentials essentials, Log log)
        {
            foreach (var attr in f.Attributes)
            {
                if (attr.ReferencedType == essentials.ForeignAttribute &&
                    essentials.Language.Literals[(int)attr.Arguments[0].ConstantValue].Name == "CPlusPlus")
                {
                    // FIXME: Move validation code to different module?
                    if (!f.IsStatic)
                        log.Error(f.Source, ErrorCode.E0000, "Foreign CPlusPlus methods must be static.");
                    return true;
                }
            }
            return false;
        }

        public static bool IsPInvokable(this DelegateType dt, IEssentials essentials)
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
