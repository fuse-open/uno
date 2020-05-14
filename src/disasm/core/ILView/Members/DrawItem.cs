using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Disasm.ILView.Members
{
    public class DrawItem : ILItem
    {
        public readonly Drawable Draw;

        public override string DisplayName => Draw.DrawableBlock.ToString();
        public override ILIcon Icon => ILIcon.Component;
        public override object Object => Draw.DrawableBlock;

        public DrawItem(Drawable draw)
        {
            Draw = draw;

            if (draw.DrawState != null)
            {
                AddChild(new FunctionItem(draw.DrawState.VertexShader.Entrypoint));
                AddChild(new FunctionItem(draw.DrawState.PixelShader.Entrypoint));
            }
        }

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.Append("/*\n    Draw '" + Draw + "'\n*/\n\n");

            for (int i = 0; i < Draw.Nodes.Length; i++)
            {
                var n = Draw.Nodes[i];
                disasm.Append("//// [" + i.ToString("000") + "]: " + n + "\n");

                for (int j = n.Top; j <= n.Bottom; j++)
                {
                    var mp = n.Block.Members[j] as MetaProperty;

                    if (mp != null)
                    {
                        bool status;
                        if (Draw.ReferencedMetaProperties.TryGetValue(new MetaLocation(i, j), out status) && !status)
                            disasm.Append("FAIL ");
                        else
                            disasm.Append("     ");
                    }
                    else
                        disasm.Append("     ");

                    disasm.Append("[" + j.ToString("000") + "]: ");

                    if (mp != null)
                    {
                        disasm.Append(mp.Name + " as ");
                        disasm.Append(mp.ReturnType);

                        foreach (var f in Draw.FailedReqStatements)
                            if (f.Item1.NodeIndex == i && f.Item1.BlockIndex == j)
                                disasm.Append("\nFLBK        " + f.Item3);

                        if (!Draw.ReferencedMetaProperties.ContainsKey(new MetaLocation(i, j)))
                            disasm.Append(" /* Not Referenced */");

                    }
                    else
                    {
                        disasm.Append("<invalid>");
                    }

                    disasm.Append("\n");
                }

                disasm.Append("\n");
            }
        }
    }
}
