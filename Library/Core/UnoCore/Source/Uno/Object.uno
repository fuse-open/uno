using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Object")]
    [extern(CPLUSPLUS) Set("TypeName", "uObject*")]
    [extern(CPLUSPLUS) Set("BaseType", "uObject")]
    [extern(CPLUSPLUS) Set("TypeOfType", "uType")]
    [extern(CPLUSPLUS) Set("TypeOfFunction", "uObject_typeof")]
    public intrinsic class Object
    {
        public Type GetType()
        {
            if defined(CPLUSPLUS)
            @{
                return $$->__type;
            @}
            else
                build_error;
        }

        public virtual int GetHashCode()
        {
            if defined(CPLUSPLUS)
            @{
                if (sizeof(void*) > 4)
                {
                    union
                    {
                        void *ptr;
                        uint32_t data[2];
                    } u;
                    u.ptr = $$;
                    return u.data[0] ^ u.data[1];
                }
                else
                    return (int)(intptr_t)$$;
            @}
            else
                build_error;
        }

        public virtual bool Equals(object o)
        {
            if defined(CPLUSPLUS)
            @{
                return $$ == $0;
            @}
            else
                build_error;
        }

        public virtual string ToString()
        {
            if defined(CPLUSPLUS)
            @{
                return uString::Const($$->__type->FullName);
            @}
            else
                build_error;
        }

        public static bool Equals(object left, object right)
        {
            if (left == right)
                return true;

            if (left == null ||
                right == null)
                return false;

            return left.Equals(right);
        }

        public static bool ReferenceEquals(object left, object right)
        {
            return left == right;
        }
    }
}
