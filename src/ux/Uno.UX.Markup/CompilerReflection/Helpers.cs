using System.Linq;
using System.Xml.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.UX.Markup.Reflection
{
    static class Helpers
    {
        public static bool IsReferenceTypeExceptStringAndValue(this DataType dt)
        {
            return (dt is ClassType || dt is InterfaceType) && (dt.FullName != "string");
        }
    }
}
