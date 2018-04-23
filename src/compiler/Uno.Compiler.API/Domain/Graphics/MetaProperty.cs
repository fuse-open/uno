using System;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Logging;

namespace Uno.Compiler.API.Domain.Graphics
{
    public sealed class MetaProperty : BlockMember
    {
        public readonly BlockBase Parent;
        public MetaVisibility Visibility;
        public DataType ReturnType;
        public string Name;

        public override BlockMemberType Type => BlockMemberType.MetaProperty;
        public bool IsInvalid => ReturnType.IsInvalid;
        public bool IsPublic => Visibility == MetaVisibility.Public;
        public MetaDefinition[] Definitions { get; private set; }

        // Internal use:
        public Action<MetaProperty> Compiling;

        public MetaProperty(Source src, BlockBase parent, DataType type, string name, MetaVisibility visibility)
            : base(src)
        {
            ReturnType = type;
            Name = name;
            Parent = parent;
            Visibility = visibility;
        }

        public void SetDefinitions(params MetaDefinition[] defs)
        {
            Definitions = defs;
        }

        public void Visit(Pass p)
        {
            if (!p.Begin(this)) return;

            var old = p.MetaProperty;
            p.MetaProperty = this;

            foreach (var def in Definitions)
            {
                if (def.Value is Expression)
                {
                    Expression e = def.Value as Expression;
                    p.VisitNullable(ref e);
                    def.Value = e;
                }
                else
                {
                    p.VisitNullable(ref def.Value);
                }
            }

            p.MetaProperty = old;
            p.End(this);
        }

        public override string ToString()
        {
            return Parent + "." + Name;
        }

        public bool IsAccessibleFrom(Namescope scope)
        {
            switch (Visibility)
            {
                case MetaVisibility.Public:
                    return true;

                default:
                    {
                        var scopeBlockBase = scope as BlockBase;

                        // Should never happen
                        if (scopeBlockBase == null)
                            throw new FatalException(Source, ErrorCode.I0000, "Meta property scope was not a block");

                        var scopeBlock = scopeBlockBase.TryFindBlockParent();
                        var parentBlock = Parent.TryFindBlockParent();
                        return scopeBlock == parentBlock;
                    }
            }
        }
    }
}