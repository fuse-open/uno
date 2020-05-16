using System.Collections.Generic;

namespace Uno.Macros
{
    public class MacroCall
    {
        public string Root;
        public string Method;
        public List<string> Arguments;

        public override string ToString()
        {
            return Root + (Method != null ? ":" + Method : "") + (Arguments != null ? "(" + string.Join(", ", Arguments) + ")" : "");
        }
    }
}