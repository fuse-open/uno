using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Enum")]
    [extern(CPLUSPLUS) Set("TypeName", "uObject*")]
    public abstract class Enum : ValueType
    {
        public override string ToString()
        {
            if defined(CPLUSPLUS)
            @{
                return uEnum::GetString($$->__type, (uint8_t*)$$ + sizeof(uObject));
            @}
            else
                build_error;
        }

        public static object Parse(Type type, string str, bool ignoreCase)
        {
            if defined(CPLUSPLUS)
            @{
                uPtr($0);
                int64_t result;
                if (uEnum::TryParse($0, $1, $2, &result))
                    return uBoxPtr($0, &result);
            @}

            throw new ArgumentException("Unable to parse enum '" + str + "'");
        }

        public static object Parse(Type type, string str)
        {
            return Parse(type, str, false);
        }

        public static bool TryParse<TEnum>(string str, bool ignoreCase, out TEnum result) where TEnum : struct
        {
            result = default(TEnum);
            if defined(CPLUSPLUS)
            @{
                return uEnum::TryParse(@{TEnum:TypeOf}, $0, $1, $2);
            @}
            else
                return false;
        }

        public static bool TryParse<TEnum>(string str, out TEnum result) where TEnum : struct
        {
            return TryParse(str, false, out result);
        }
    }
}
