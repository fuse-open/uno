using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL
{
    public abstract class CopyProvider
    {
        public abstract DataType TryGetType(DataType dt);
        public abstract Member TryGetMember(Member member);
    }
}