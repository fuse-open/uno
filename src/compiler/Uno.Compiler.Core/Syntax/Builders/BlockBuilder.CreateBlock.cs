using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class BlockBuilder
    {
        public Block CreateBlock(AstBlock ast, Namescope parent)
        {
            Block result;
            if (_processedBlocks.TryGetValue(ast, out result))
                return result;

            var src = ast.Name.Source;
            result = new Block(src, parent, ast.DocComment, TypeBuilder.GetTypeModifiers(parent, ast.Modifiers), ast.Name.Symbol);

            if (parent is Namespace)
                (parent as Namespace).Blocks.Add(result);
            else if (parent is Block)
                (parent as Block).NestedBlocks.Add(result);
            else if ((parent as DataType)?.Block != null)
                (parent as DataType).Block.NestedBlocks.Add(result);
            else if (parent is DrawBlock && ast.Modifiers.HasFlag(Modifiers.Generated))
                ApplyLambdaBlock(ast, parent as DrawBlock, result);
            else
                Log.Error(src, ErrorCode.E3214, "Block declaration is not allowed in this scope");

            _processedBlocks.Add(ast, result);
            EnqueueBlock(result, x => PopulateBlock(ast, x));

            return result;
        }

        void ApplyLambdaBlock(AstBlock ast, DrawBlock parent, Block result)
        {
            if (ast.UsingBlocks == null)
            {
                var usingBlocks = new List<Block>();

                for (int j = 0, l = result.Members.Count; j < l; j++)
                {
                    var apply = result.Members[j] as Apply;

                    if (apply != null)
                        usingBlocks.Add(apply.Block);
                }

                result.SetUsingBlocks(usingBlocks.ToArray());
            }

            Expression obj = null;
            if (!parent.Method.IsStatic)
                obj = new This(parent.Source, _compiler.TypeBuilder.Parameterize(parent.Method.DeclaringType)).Address;

            parent.Members.Add(new Apply(ast.Name.Source, 0, result, obj));
        }

        MetaBlock CreateNodeBlock(AstNode ast, BlockBase parent)
        {
            MetaBlock result;

            switch (ast.NodeType)
            {
                case AstNodeType.Drawable:
                    result = new MetaBlock(ast.Name.Source, parent, ast.Name.Symbol, MetaBlockType.Drawable);
                    break;
                case AstNodeType.Node:
                    result = new MetaBlock(ast.Name.Source, parent, ast.Name.Symbol, MetaBlockType.Scope);
                    break;
                default:
                    Log.Error(ast.Name.Source, ErrorCode.E3216, "<" + ast.MemberType + "> is not allowed in this scope");
                    return MetaBlock.Invalid;
            }

            EnqueueBlock(result, x => PopulateBlock(ast, x));
            return result;
        }
    }
}
