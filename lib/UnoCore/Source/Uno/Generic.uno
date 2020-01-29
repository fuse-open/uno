using Uno.Compiler.ExportTargetInterop;
using System.Collections.Generic;

namespace Uno
{
    public static class Generic
    {
        public static bool Equals<T>(T left, T right)
        {
            if defined(CPLUSPLUS)
            @{
                uType* type = @{T:TypeOf};
                return U_IS_OBJECT(type)
                        ? (uObject*)$0 == (uObject*)$1 || (
                                (uObject*)$0 &&
                                (uObject*)$1 &&
                                ((uObject*)$0)->Equals((uObject*)$1))
                        : memcmp($0, $1, type->ValueSize) == 0;
            @}
            else if defined(DOTNET)
                return EqualityComparer<T>.Default.Equals(left, right);
            else
                build_error;
        }

        public static int GetHashCode<T>(T obj)
        {
            if defined(DOTNET)
                return EqualityComparer<T>.Default.GetHashCode(obj);
            else
                return obj.GetHashCode();
        }

        public static string ToString<T>(T obj)
        {
            return obj.ToString();
        }
    }
}

namespace System.Collections.Generic
{
    [DotNetType]
    extern(DOTNET) abstract class EqualityComparer<T>
    {
        public static extern EqualityComparer<T> Default { get; }

        public abstract bool Equals(T left, T right);
        public abstract int GetHashCode(T obj);
    }
}
