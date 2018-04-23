using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public static class ParameterList
    {
        public static Parameter[] Empty { get; } = new Parameter[0];

        public static Parameter[] Copy(this Parameter[] pl, CopyState state)
        {
            var result = new Parameter[pl.Length];

            for (int i = 0; i < result.Length; i++)
            {
                var p = pl[i];
                result[i] = new Parameter(p.Source, p.Attributes.Copy(state), p.Modifier, state.GetType(p.Type), p.Name, p.OptionalDefault.CopyNullable(state));
            }

            return result;
        }

        public static string BuildString(this Parameter[] pl, string openParen = "(", string closeParen = ")", bool includeParamNames = false)
        {
            var s = openParen;

            for (int i = 0; i < pl.Length; i++)
            {
                var p = pl[i];

                if (i > 0)
                    s += "," + (includeParamNames ? " " : "");
                if (p.OptionalDefault != null)
                    s += "[";

                s += p.Type;

                if (p.IsReference)
                    s += "&";
                if (includeParamNames)
                    s += " " + p.UnoName;
                if (p.OptionalDefault != null)
                    s += "]";
            }

            return s + closeParen;
        }
    }
}