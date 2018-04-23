using Uno.Compiler.ExportTargetInterop;
using Uno;

namespace HTML5
{
    [TargetSpecificType]
    public extern(JAVASCRIPT) struct Uint8Array
    {
        public static Uint8Array Create(Buffer buf)
        @{
            return $0
                ? new Uint8Array(@{$0._data}.buffer, @{$0._offset}, @{$0._sizeInBytes})
                : null;
        @}
    }
}
