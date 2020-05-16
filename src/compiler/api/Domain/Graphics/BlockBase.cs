using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Graphics
{
    public abstract class BlockBase : Namescope
    {
        public abstract BlockType BlockType { get; }

        public readonly List<BlockMember> Members = new List<BlockMember>();

        // Internal use:
        public Action<BlockBase> Populating;
        public void Populate() { ActionQueue.Dequeue(ref Populating, this); }

        protected BlockBase(Source src, Namescope parent, string name)
            : base(src, parent, name)
        {
        }

        public sealed override NamescopeType NamescopeType => NamescopeType.BlockBase;

        public void Visit(Pass p)
        {
            if (!p.Begin(this)) return;

            var old = p.Block;
            p.Block = this;

            var db = this as DrawBlock;
            if (db?.Drawables != null)
            {
                foreach (var dp in db.Drawables)
                {
                    dp.DrawState.VertexShader.Visit(p);
                    dp.DrawState.PixelShader.Visit(p);
                }
            }

            for (int i = 0; i < Members.Count; i++)
            {
                var m = Members[i];

                switch (m.Type)
                {
                    case BlockMemberType.Apply:
                    {
                        var apply = m as Apply;
                        p.OnApply(apply);
                        break;
                    }
                    case BlockMemberType.MetaProperty:
                    {
                        var mp = m as MetaProperty;
                        mp.Visit(p);
                        break;
                    }
                    case BlockMemberType.Node:
                    {
                        var node = m as Node;
                        node.Block.Visit(p);
                        break;
                    }
                }
            }

            p.Block = old;
            p.End(this);
        }
    }
}