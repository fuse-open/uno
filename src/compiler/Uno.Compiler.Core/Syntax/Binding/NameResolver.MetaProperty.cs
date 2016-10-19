using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax.Binding
{
    public partial class NameResolver
    {
        void FindMetaPropertiesRecursive(BlockBase block, string name, HashSet<BlockBase> visited, List<MetaProperty> result)
        {
            if (block == null ||
                visited.Contains(block))
                return;

            visited.Add(block);

            for (int i = block.Members.Count - 1; i >= 0; i--)
            {
                var m = block.Members[i];

                if (m is MetaProperty)
                {
                    var mp = m as MetaProperty;

                    if (mp.Name == name)
                        result.Add(mp);
                }
                else if (m is Apply)
                {
                    var apply = m as Apply;
                    FindMetaPropertiesRecursive(apply.Block, name, visited, result);

                    if (apply.Modifier == ApplyModifier.Virtual)
                    {
                        var dt = apply.Block.TryFindTypeParent();
                        if (dt == null)
                            continue;

                        HashSet<DataType> subs;
                        if (_compiler.BlockBuilder.FlattenedTypes.TryGetValue(dt, out subs))
                            foreach (var st in subs)
                                if (st?.Block != null && st.IsAccessibleFrom(block.Source))
                                    FindMetaPropertiesRecursive(st.Block, name, visited, result);
                    }
                }
            }

            switch (block.BlockType)
            {
                case BlockType.Block:
                {
                    foreach (var bb in ((Block)block).UsingBlocks)
                        FindMetaPropertiesRecursive(bb, name, visited, result);

                    var dt = block.ParentType;

                    if (dt != null && dt.Block == block && dt.Base?.Block != null)
                        FindMetaPropertiesRecursive(dt.Base.Block, name, visited, result);
                    else
                        FindMetaPropertiesRecursive(_compiler.BlockBuilder.TerminalProperties, name, visited, result);
                    break;
                }
                case BlockType.MetaBlock:
                {
                    FindMetaPropertiesRecursive(
                        block.IsNestedBlock 
                            ? block.ParentBlock 
                            : _compiler.BlockBuilder.TerminalProperties, 
                        name,
                        visited,
                        result);
                    break;
                }
                case BlockType.DrawBlock:
                {
                    // TODO:
                    // Commented out code is not correct, becuase we don't want a PartialMetaProperty node when 'this' is not applied in draw statement.
                    // Also theres issues with the way apply is compiled, don't want PartialMetaPropertys there either, but risk getting them currently.
                    // But we want to do this case of error, to be able to give error message that makes sense (in case user need, but forgot, to 'apply this')
                    // Clean up these things later.

                    /*
                    var m = (block as DrawBlock).Method;
                    var parentBlock = m.DeclaringType.Block;

                    if (parentBlock != null)
                        FindMetaPropertiesRecursive(Compiler.BlockBuilder.TerminalProperties, parentBlock, name, visited, result);
                    else
                    */
                    FindMetaPropertiesRecursive(_compiler.BlockBuilder.TerminalProperties, name, visited, result);
                    break;
                }
            }
        }

        public MetaProperty TryGetMetaProperty(Source src, Namescope scope, BlockBase block, string name, bool reportErrorWhenAmbiguous)
        {
            // TODO: Figure out a way to cache this

            var result = new List<MetaProperty>();
            FindMetaPropertiesRecursive(block, name, new HashSet<BlockBase>(), result);

            for (int i = result.Count - 1; i > 0; i--)
            {
                var mp = result[i];

                if (!mp.IsAccessibleFrom(scope))
                {
                    result.RemoveAt(i);
                    continue;
                }

                for (int j = i - 1; j >= 0; j--)
                {
                    if (mp.ReturnType.Equals(result[j].ReturnType))
                    {
                        result.RemoveAt(i);
                        break;
                    }
                }
            }

            switch (result.Count)
            {
                case 1:
                    return result[0];
                case 0:
                    return null;
                default:
                    if (reportErrorWhenAmbiguous)
                    {
                        Log.Error(src, ErrorCode.E0000, "Meta property " + name.Quote() + " is ambiguous. Use 'req(" + name + " as TYPE)' to disambiguate");
                        return new MetaProperty(src, _compiler.BlockBuilder.TerminalProperties, DataType.Invalid, "<ambiguous>", MetaVisibility.Public);
                    }
                    return null;
            }
        }
    }
}
