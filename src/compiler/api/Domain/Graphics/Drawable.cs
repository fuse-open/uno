using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Graphics
{
    public class Drawable
    {
        string _suffix;
        public string Suffix => _suffix ?? (_suffix = (DrawBlock + "[" + PathIndex + "]").GetHashCode().ToString("x"));
        public Source Source => DrawBlock.Source;

        public readonly DrawState DrawState;
        public readonly DrawBlock DrawBlock;
        public readonly MetaBlock DrawableBlock;
        public readonly Node[] Nodes;
        public readonly HashSet<Tuple<MetaLocation, MetaDefinition, ReqStatement>> FailedReqStatements = new HashSet<Tuple<MetaLocation, MetaDefinition, ReqStatement>>();
        public readonly Dictionary<MetaLocation, bool> ReferencedMetaProperties = new Dictionary<MetaLocation, bool>();

        public Drawable(DrawBlock drawBlock, MetaBlock drawableBlock, Node[] nodes)
        {
            DrawBlock = drawBlock;
            DrawableBlock = drawableBlock;
            Nodes = nodes;

            // Do this last
            DrawState = new DrawState(this);
        }

        public override string ToString()
        {
            return DrawBlock + "~" + DrawableBlock;
        }

        int PathIndex
        {
            get
            {
                for (int i = 0; i < DrawBlock.Drawables.Length; i++)
                    if (DrawBlock.Drawables[i] == this)
                        return i;

                return -1;
            }
        }

        public struct Node
        {
            public Expression Object;
            public BlockBase Block;
            public int Top, Bottom;

            public Node(Expression obj, BlockBase block, int top, int bottom)
            {
                Object = obj;
                Block = block;
                Top = top;
                Bottom = bottom;
            }

            public Node(Expression obj, BlockBase block)
                : this(obj, block, 0, block.Members.Count - 1)
            {
            }

            public override string ToString()
            {
                var str = Block + " [" + Top + ".." + Bottom + "]";

                if (Object != null)
                    str += " (" + Object + ")";

                return str;
            }
        }
    }
}
