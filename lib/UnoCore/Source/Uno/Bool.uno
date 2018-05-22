using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Boolean")]
    [extern(CPLUSPLUS) Set("TypeName", "bool")]
    [extern(CPLUSPLUS) Set("DefaultValue", "0")]
    /** Represents a boolean (`true` or `false`) value */
    public intrinsic struct Bool
    {
        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            if defined(CPLUSPLUS)
            @{
                return *$$ ? 1 : 0;
            @}
            else
                return base.GetHashCode();
        }

        public override string ToString()
        {
            if defined(CPLUSPLUS)
            @{
                return uString::Const(*$$ ? "True" : "False");
            @}
            else
                return base.ToString();
        }

        public static bool Parse(string str)
        {
            if (str == null)
                throw new Uno.ArgumentNullException(nameof(str));

            var trimmedStr = str.Trim().ToLower();
            if (trimmedStr == "true")
                return true;
            if (trimmedStr == "false")
                return false;

            throw new FormatException("Unable to convert string to bool");
        }

        public static bool TryParse(string str, out bool res)
        {
            res = default(bool);
            if (!string.IsNullOrEmpty(str))
            {
                if (str.ToLower() == "true")
                {
                    res = true;
                    return true;
                }

                if (str.ToLower() == "false")
                {
                    res = false;
                    return true;
                }
            }
            return false;
        }

        public static intrinsic bool operator ! (bool a);
        public static intrinsic bool operator == (bool left, bool right);
        public static intrinsic bool operator != (bool left, bool right);
    }
}
