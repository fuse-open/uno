namespace Uno.Compiler.Backends.JavaScript
{
    public static class JsHelper
    {
        public static bool IsValidJsIdentifier(this string s)
        {
            if (s.Length == 0 || char.IsNumber(s[0]))
                return false;

            foreach (var c in s)
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '$')
                    return false;

            return true;
        }
    }
}