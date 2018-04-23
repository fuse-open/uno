using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Disasm.ILView.Members;

namespace Uno.Disasm.ILView.Namespaces
{
    public class BlockItem : ILItem
    {
        public readonly BlockBase Block;

        public override object Object => Block;
        public override string DisplayName => Block.Name;
        public override ILIcon Icon => Block is Block && (Block as Block).IsPublic
            ? ILIcon.Block
            : ILIcon.BlockNonPublic;

        void FindReferencedMetaProperties(BlockBase block, HashSet<string> defined, HashSet<string> referenced, List<string> drawables, string prefix = "")
        {
            foreach (var e in block.Members)
            {
                switch (e.Type)
                {
                    case BlockMemberType.MetaProperty:
                    {
                        var mp = e as MetaProperty;

                        if (mp.Definitions.Length > 0 &&
                            !(mp.Definitions.Length == 1 &&
                                mp.Definitions[0].Value is GetMetaProperty &&
                                (mp.Definitions[0].Value as GetMetaProperty).Name == mp.Name))
                            defined.Add(mp.Name);

                        foreach (var def in mp.Definitions)
                        {
                            foreach (var req in def.Requirements)
                            {
                                if (req is ReqProperty)
                                {
                                    var reqMp = req as ReqProperty;
                                    referenced.Add(reqMp.PropertyName);
                                }
                            }
                        }
                        break;
                    }
                    case BlockMemberType.Node:
                    {
                        var node = e as Node;

                        if (node.Block.MetaBlockType == MetaBlockType.Drawable)
                            drawables.Add(prefix + node.Block.Name);

                        FindReferencedMetaProperties(node.Block, defined, referenced, drawables, prefix + node.Block.Name + ".");
                        break;
                    }
                }
            }
        }

        protected override void Disassemble(Disassembler sb)
        {
            var definedSet = new HashSet<string>();
            var referencedSet = new HashSet<string>();
            var drawablesList = new List<string>();
            FindReferencedMetaProperties(Block, definedSet, referencedSet, drawablesList);

            foreach (var p in definedSet)
                referencedSet.Remove(p);

            var defined = definedSet.ToArray();
            var referenced = referencedSet.ToArray();

            Array.Sort(defined);
            Array.Sort(referenced);

            drawablesList.Sort();

            sb.AppendLine();
            sb.AppendLine("// Defined");
            sb.AppendLine();

            foreach (var p in defined)
                sb.AppendLine(p);

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("// Referenced");
            sb.AppendLine();

            foreach (var p in referenced)
                sb.AppendLine(p);

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("// Drawables");
            sb.AppendLine();

            foreach (var p in drawablesList)
                sb.AppendLine(p);
        }

        public BlockItem(BlockBase block)
        {
            Block = block;

            foreach (var e in block.Members)
                if (e is MetaProperty)
                    AddChild(new MetaPropertyItem(e as MetaProperty));
                else if (e is Node)
                    AddChild(new BlockItem((e as Node).Block));
        }
    }
}
