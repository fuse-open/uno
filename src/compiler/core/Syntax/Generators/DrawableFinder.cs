using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Logging;

namespace Uno.Compiler.Core.Syntax.Generators
{
    static class DrawableFinder
    {
        static Expression ResolveApplyInstance(Pass parent, DrawBlock drawBlock, Expression applyObj, Expression parentInst)
        {
            if (applyObj != null)
            {
                applyObj = applyObj.CopyExpression(new CopyState(drawBlock.Method));
                new MetaObjectReplacer(parent, parentInst).VisitNullable(ref applyObj, ExpressionUsage.Object);
                return applyObj;
            }

            return null;
        }

        static void ExpandNode(Pass parent, DrawBlock drawBlock, Drawable.Node n, List<Drawable.Node> result)
        {
            for (int i = n.Bottom; i >= n.Top; i--)
            {
                var item = n.Block.Members[i];

                if (item is Node)
                {
                    result.Add(new Drawable.Node(n.Object, n.Block, i + 1, n.Bottom));
                    n.Bottom = i - 1;
                }
                else if (item is Apply)
                {
                    result.Add(new Drawable.Node(n.Object, n.Block, i + 1, n.Bottom));
                    n.Bottom = i - 1;

                    var apply = item as Apply;
                    ExpandNode(parent, drawBlock, new Drawable.Node(ResolveApplyInstance(parent, drawBlock, apply.Object, n.Object), apply.Block), result);
                    ClosePath(parent, drawBlock, result);
                }
            }

            result.Add(n);
        }

        static void ClosePath(Pass parent, DrawBlock drawBlock, List<Drawable.Node> result)
        {
            var n = result.Last();

            if (n.Top != 0)
                throw new FatalException(n.Block.Source, ErrorCode.I0075, "DrawableExposer: n.Top != 0");

            if (n.Block is Block && n.Block.IsNestedType && n.Block.ParentType.Block == n.Block)
            {
                var dt = n.Block.ParentType;

                if (dt.Base?.Block != null)
                {
                    ExpandNode(parent, drawBlock, new Drawable.Node(n.Object, dt.Base.Block), result);
                    ClosePath(parent, drawBlock, result);
                }
            }
        }

        static void FindDrawables(Compiler compiler, DrawBlock drawBlock, List<Drawable.Node> lower, List<Drawable.Node> upper, List<Drawable> result)
        {
            var n = lower.RemoveLast();

            // If node contains any information, add node source to list of sources
            if (!n.Block.Source.IsUnknown && n.Bottom >= n.Top)
                drawBlock.Method.DeclaringType.SourceFiles.Add(n.Block.Source.FullPath);

            for (int i = n.Bottom; i >= n.Top; i--)
            {
                var item = n.Block.Members[i];

                if (item is Node)
                {
                    var node = item as Node;

                    upper.Add(new Drawable.Node(n.Object, n.Block, n.Top, i - 1));
                    lower.Add(new Drawable.Node(n.Object, n.Block, i + 1, n.Bottom));
                    lower.Add(new Drawable.Node(n.Object, node.Block));

                    if (node.Block.MetaBlockType == MetaBlockType.Drawable)
                    {
                        var dp = new List<Drawable.Node>();

                        for (int j = 0; j < lower.Count; j++)
                            ExpandNode(compiler.Pass, drawBlock, lower[j], dp);

                        for (int j = upper.Count - 1; j >= 0; j--)
                        {
                            ExpandNode(compiler.Pass, drawBlock, upper[j], dp);
                            ClosePath(compiler.Pass, drawBlock, dp);
                        }

                        dp.Add(new Drawable.Node(null, compiler.BlockBuilder.TerminalProperties));
                        dp.Reverse();

                        result.Add(new Drawable(drawBlock, node.Block, dp.ToArray()));
                    }

                    FindDrawables(compiler, drawBlock, lower, upper, result);

                    lower.RemoveLast();
                    upper.RemoveLast();
                }
                else if (item is Apply)
                {
                    var apply = item as Apply;

                    upper.Add(new Drawable.Node(n.Object, n.Block, n.Top, i - 1));
                    lower.Add(new Drawable.Node(n.Object, n.Block, i + 1, n.Bottom));

                    lower.Add(new Drawable.Node(ResolveApplyInstance(compiler.Pass, drawBlock, apply.Object, n.Object), apply.Block));

                    FindDrawables(compiler, drawBlock, lower, upper, result);

                    lower.RemoveLast();
                    upper.RemoveLast();
                }
            }

            if (n.Block is Block && n.Block.IsNestedType && n.Block.ParentType.Block == n.Block)
            {
                var dt = n.Block.ParentType;

                if (dt.Base?.Block != null)
                {
                    lower.Add(n);
                    lower.Add(new Drawable.Node(n.Object, dt.Base.Block));

                    FindDrawables(compiler, drawBlock, lower, upper, result);

                    lower.RemoveLast();
                }
            }
        }

        public static bool VerifyCircularReferences(Compiler compiler, BlockBase block, List<Block> applyStack)
        {
            foreach (var item in block.Members)
            {
                if (item is Node)
                {
                    var node = item as Node;

                    if (!VerifyCircularReferences(compiler, node.Block, applyStack))
                        return false;
                }
                else if (item is Apply)
                {
                    var apply = item as Apply;

                    if (applyStack.Contains(apply.Block))
                    {
                        compiler.Log.Error(apply.Source, ErrorCode.E0105, "Circular block reference detected");
                        return false;
                    }

                    applyStack.Add(apply.Block);

                    if (!VerifyCircularReferences(compiler, apply.Block, applyStack))
                        return false;

                    applyStack.RemoveLast();

                    if (apply.Modifier == ApplyModifier.Virtual)
                    {
                        var dt = apply.Block.TryFindTypeParent();

                        if (dt != null)
                        {
                            HashSet<DataType> subs;
                            if (compiler.BlockBuilder.FlattenedTypes.TryGetValue(dt, out subs))
                            {
                                foreach (var st in subs)
                                {
                                    if (st.Block != null)
                                    {
                                        applyStack.Add(st.Block);

                                        if (!VerifyCircularReferences(compiler, st.Block, applyStack))
                                            return false;

                                        applyStack.RemoveLast();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (block.IsNestedType && block.ParentType.Block == block)
            {
                var dt = block.ParentType;

                if (dt.Base?.Block != null && !VerifyCircularReferences(compiler, dt.Base.Block, applyStack))
                    return false;
            }

            return true;
        }

        public static void FindDrawables(Compiler compiler, DrawBlock block, bool createImplicitDrawableBlockIfNoExplicitAreFound)
        {
            Expression obj = null;
            if (!block.Method.IsStatic)
                obj = new This(block.Source, compiler.TypeBuilder.Parameterize(block.Method.DeclaringType)).Address;

            var result = new List<Drawable>();
            var path = new List<Drawable.Node> {new Drawable.Node(obj, block)};
            FindDrawables(compiler, block, path, new List<Drawable.Node>(), result);

            if (result.Count == 0 && createImplicitDrawableBlockIfNoExplicitAreFound)
            {
                // Add an implicit drawable block when no explicit drawable blocks are found
                block.Members.Add(new Node(new MetaBlock(block.Source, block, ".implicit", MetaBlockType.Drawable)));

                path = new List<Drawable.Node> {new Drawable.Node(obj, block)};
                FindDrawables(compiler, block, path, new List<Drawable.Node>(), result);
            }

            result.Reverse();
            block.SetDrawables(result.ToArray());
        }
    }
}
