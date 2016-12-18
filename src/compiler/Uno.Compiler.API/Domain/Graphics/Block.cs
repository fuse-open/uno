using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.Graphics
{
    public sealed class Block : BlockBase, IEntity
    {
        static readonly Block[] EmptyArray = new Block[0];
        public static readonly Block Invalid = new Block(Source.Unknown, DataType.Invalid, null, 0, "<invalid>");

        public readonly string DocComment;
        public readonly Modifiers Modifiers;
        public readonly List<Block> NestedBlocks = new List<Block>();
        public NewObject[] Attributes { get; private set; } = AttributeList.Empty;

        public override BlockType BlockType => BlockType.Block;
        public Block[] UsingBlocks { get; private set; }

        public Block(Source src, Namescope parent, string comment, Modifiers modifiers, string name)
            : base(src, parent, name)
        {
            DocComment = comment;
            Modifiers = modifiers;
            UsingBlocks = EmptyArray;
        }

        public void SetAttributes(params NewObject[] attributes)
        {
            Attributes = attributes;
        }

        public void SetUsingBlocks(params Block[] usingBlocks)
        {
            UsingBlocks = usingBlocks;
        }

        public bool IsAccessibleFrom(Source src)
        {
            return Source.Package.IsAccessibleFrom(src.Package);
        }

        Source IEntity.Source => Source;
        IEntity IEntity.MasterDefinition => this;
        DataType IEntity.DeclaringType { get { throw new InvalidOperationException(); } }
        DataType IEntity.ReturnType { get { throw new InvalidOperationException(); } }
        MemberType IEntity.MemberType => MemberType.Other;
        public SourcePackage Package => Source.Package;
        NewObject[] IEntity.Attributes => Attributes;
        string IEntity.DocComment => DocComment;
        bool IEntity.HasRefCount => false;
        bool IEntity.IsStripped => true;
        public bool IsPublic => Modifiers.HasFlag(Modifiers.Public);
        bool IEntity.IsProtected => Modifiers.HasFlag(Modifiers.Protected);
        public bool IsPrivate => Modifiers.HasFlag(Modifiers.Private);
        public bool IsInternal => Modifiers.HasFlag(Modifiers.Internal);
        bool IEntity.IsPartial => Modifiers.HasFlag(Modifiers.Partial);
        bool IEntity.IsStatic => Modifiers.HasFlag(Modifiers.Static);
        bool IEntity.IsGenerated => Modifiers.HasFlag(Modifiers.Generated);
        bool IEntity.IsAbstract => Modifiers.HasFlag(Modifiers.Abstract);
        bool IEntity.IsVirtual => (Modifiers & (Modifiers.Virtual | Modifiers.Override | Modifiers.Abstract)) != 0;
        bool IEntity.IsVirtualBase => Modifiers.HasFlag(Modifiers.Virtual);
        bool IEntity.IsVirtualOverride => Modifiers.HasFlag(Modifiers.Override);
        bool IEntity.IsSealed => Modifiers.HasFlag(Modifiers.Sealed);
        bool IEntity.IsExtern => Modifiers.HasFlag(Modifiers.Extern);
        bool IEntity.IsImplicitCast => Modifiers.HasFlag(Modifiers.Implicit);
        bool IEntity.IsExplicitCast => Modifiers.HasFlag(Modifiers.Explicit);
    }
}
