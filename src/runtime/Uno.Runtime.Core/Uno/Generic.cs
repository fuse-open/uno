// This file was generated based on Library/Core/UnoCore/Source/Uno/Generic.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class Generic
    {
        public static bool Equals<T>(T left, T right)
        {
            return global::System.Collections.Generic.EqualityComparer<T>.Default.Equals(left, right);
        }

        public static int GetHashCode<T>(T obj)
        {
            return global::System.Collections.Generic.EqualityComparer<T>.Default.GetHashCode(obj);
        }

        public static string ToString<T>(T obj)
        {
            return obj.ToString();
        }
    }
}
