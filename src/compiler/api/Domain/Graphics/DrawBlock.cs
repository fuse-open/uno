using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.Graphics
{
    public sealed class DrawBlock : BlockBase
    {
        public readonly Method Method;
        public readonly Scope DrawScope;
        public readonly Dictionary<string, Variable> CapturedLocals;

        public override BlockType BlockType => BlockType.DrawBlock;
        public Drawable[] Drawables { get; private set; }

        public DrawBlock(DrawBlock sourceBlock)
            : this(sourceBlock.Source, (ClassType)sourceBlock.Parent.Parent, sourceBlock.Method, sourceBlock.CapturedLocals, sourceBlock)
        {
        }

        public DrawBlock(Source src, ClassType parent, Method method, Dictionary<string, Variable> capturedLocals, DrawBlock optionalSourceBlock = null)
            : base(src, parent.Block, (optionalSourceBlock != null ? optionalSourceBlock.Name + "_" : ".draw") + method.DrawBlocks.Count)
        {
            Method = method;
            CapturedLocals = capturedLocals;
            DrawScope = new Scope(src);
        }

        public override string ToString()
        {
            return Method + "." + Name;
        }

        public void SetDrawables(params Drawable[] drawables)
        {
            Drawables = drawables;
        }
    }
}