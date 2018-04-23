using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL
{
    public interface IMemberReference : ITypeReference
    {
        Member ReferencedMember { get; }
    }
}