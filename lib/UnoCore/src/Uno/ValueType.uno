using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.ValueType")]
    [extern(CPLUSPLUS) Set("TypeName", "uObject*")]
    public abstract class ValueType
    {
        public override int GetHashCode()
        {
            if defined(CPLUSPLUS)
            @{
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

        public override bool Equals(object o)
        {
            if defined(CPLUSPLUS)
            @{
                return $$ == $0 || (
                        $0 != nullptr && (
                            $0->__type == $$->__type || (
                                $0->__type->Type == uTypeTypeEnum &&
                                $0->__type->Base == $$->__type
                            )
                        ) &&
                        memcmp((const uint8_t*)$$ + sizeof(uObject),
                               (const uint8_t*)$0 + sizeof(uObject),
                               $$->__type->ValueSize) == 0
                    );
            @}
            else
                build_error;
        }
    }
}
