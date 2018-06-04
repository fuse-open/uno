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
        public Object()
        {
        }

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
                if (U_IS_OBJECT($$->__type))
                {
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
                }

                const uint8_t* data = (const uint8_t*)$$ + sizeof(uObject);
                size_t size = $$->__type->ValueSize;
                int hash = 5381;

                for (size_t i = 0; i < size; i++)
                    hash = ((hash << 5) + hash) ^ data[i];

                return hash;
            @}
            else
                build_error;
        }

        public virtual bool Equals(object o)
        {
            if defined(CPLUSPLUS)
            @{
                switch ($$->__type->Type)
                {
                case uTypeTypeEnum:
                case uTypeTypeStruct:
                    return $$ == $0 || (
                            $0 != NULL && (
                                $0->__type == $$->__type || (
                                    $0->__type->Type == uTypeTypeEnum &&
                                    $0->__type->Base == $$->__type
                                )
                            ) &&
                            memcmp((const uint8_t*)$$ + sizeof(uObject), (const uint8_t*)$0 + sizeof(uObject), $$->__type->ValueSize) == 0
                        );
                default:
                    return $$ == $0;
                }
            @}
            else
                build_error;
        }

        public virtual string ToString()
        {
            if defined(CPLUSPLUS)
            @{
                return $$->__type->Type == uTypeTypeEnum
                    ? uEnum::GetString($$->__type, (uint8_t*)$$ + sizeof(uObject))
                    : uString::Const($$->__type->FullName);
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
