using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL
{
    public interface IEntity
    {
        Source Source { get; }
        NewObject[] Attributes { get; }
        DataType DeclaringType { get; }
        DataType ReturnType { get; }
        string DocComment { get; }
        string FullName { get; }
        string UnoName { get; }
        string Name { get; }

        EntityType EntityType { get; }
        MemberType MemberType { get; }
        NamescopeType NamescopeType { get; }
        IEntity MasterDefinition { get; }

        bool HasRefCount { get; }
        bool IsStripped { get; }
        bool IsPublic { get; }
        bool IsProtected { get; }
        bool IsPrivate { get; }
        bool IsInternal { get; }
        bool IsPartial { get; }
        bool IsStatic { get; }
        bool IsGenerated { get; }
        bool IsAbstract { get; }
        bool IsVirtual { get; }         // abstract|override|virtual
        bool IsVirtualBase { get; }     // virtual
        bool IsVirtualOverride { get; } // override
        bool IsSealed { get; }
        bool IsExtern { get; }
        bool IsImplicitCast { get; }
        bool IsExplicitCast { get; }

        bool IsAccessibleFrom(Source src);
        SourcePackage Package { get; }
    }
}
