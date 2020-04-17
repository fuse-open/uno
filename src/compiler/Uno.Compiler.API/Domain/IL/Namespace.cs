using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL
{
    public class Namespace : Namescope, IEntity
    {
        public readonly List<Block> Blocks = new List<Block>();
        public readonly List<DataType> Types = new List<DataType>();
        public readonly List<Namespace> Namespaces = new List<Namespace>();
        public readonly HashSet<SourcePackage> Packages = new HashSet<SourcePackage>();

        // Optimizing
        public readonly List<DataType> StrippedTypes = new List<DataType>();
        public readonly List<Namespace> StrippedNamespaces = new List<Namespace>();

        public override NamescopeType NamescopeType => NamescopeType.Namespace;

        public Namespace()
            : this(null, "-")
        {
        }

        public Namespace(Namespace parent, string name)
            : base(Source.Unknown, parent, name)
        {
        }

        public void Visit(Pass p)
        {
            if (!p.Begin(this))
                return;

            var old = p.Namespace;
            p.Namespace = this;

            foreach (var child in Namespaces)
                child.Visit(p);

            for (int i = 0; i < Types.Count; i++)
                Types[i].Visit(p);

            p.Namespace = old;
            p.End(this);
        }

        public bool IsAccessibleFrom(Source src)
        {
            foreach (var p in Packages)
                if (p.IsAccessibleFrom(src.Package))
                    return true;

            return false;
        }

        Source IEntity.Source => Source;
        SourcePackage IEntity.Package => SourcePackage.Unknown;
        IEntity IEntity.MasterDefinition => this;
        DataType IEntity.DeclaringType { get { throw new InvalidOperationException(); } }
        DataType IEntity.ReturnType { get { throw new InvalidOperationException(); } }
        MemberType IEntity.MemberType => MemberType.Other;
        NewObject[] IEntity.Attributes => AttributeList.Empty;
        string IEntity.DocComment => null;
        bool IEntity.HasRefCount => true;
        bool IEntity.IsStripped => false;
        bool IEntity.IsPublic => false;
        bool IEntity.IsProtected => false;
        bool IEntity.IsPrivate => false;
        bool IEntity.IsInternal => false;
        bool IEntity.IsPartial => false;
        bool IEntity.IsStatic => false;
        bool IEntity.IsGenerated => false;
        bool IEntity.IsAbstract => false;
        bool IEntity.IsVirtual => false;
        bool IEntity.IsVirtualBase => false;
        bool IEntity.IsVirtualOverride => false;
        bool IEntity.IsSealed => false;
        bool IEntity.IsExtern => false;
        bool IEntity.IsImplicitCast => false;
        bool IEntity.IsExplicitCast => false;
    }
}
