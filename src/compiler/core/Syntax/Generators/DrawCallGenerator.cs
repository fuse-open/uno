using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.Core.Syntax.Generators.Passes;

namespace Uno.Compiler.Core.Syntax.Generators
{
    static class DrawCallGenerator
    {
        public static void GenerateDrawCalls(Compiler compiler, DataType dt)
        {
            var initMethod = new Method(
                dt.Source, dt, null, Modifiers.Private | Modifiers.Generated, "init_DrawCalls",
                DataType.Void, ParameterList.Empty, new Scope(dt.Source));

            dt.Methods.Add(initMethod);

            foreach (var ctor in dt.Constructors)
                if (ctor.Body == null)
                    compiler.Log.Warning(ctor.Source, ErrorCode.I0000, "Can't call 'init_DrawCalls()' from bodyless constructor");
                else
                    ctor.Body.Statements.Add(new CallMethod(initMethod.Source, new This(initMethod.Source, ctor.DeclaringType), initMethod));

            var freeMethod = new Method(
                dt.Source, dt, null, Modifiers.Private | Modifiers.Generated, "free_DrawCalls",
                DataType.Void, ParameterList.Empty, new Scope(dt.Source));

            dt.Methods.Add(freeMethod);

            var drawScopes = new HashSet<Scope>();

            for (int m = 0, l = dt.Methods.Count; m < l; m++)
            {
                var drawMethod = dt.Methods[m];

                for (int i = 0; i < drawMethod.DrawBlocks.Count; i++)
                {
                    var drawBlock = drawMethod.DrawBlocks[i];

                    if (!DrawableFinder.VerifyCircularReferences(compiler, drawBlock, new List<Block>()))
                        continue;

                    int drawBlockIndex = i;
                    if (TrySplitVirtualAppliesInDrawBlockRecursive(compiler, drawBlock, ref drawBlockIndex, drawScopes))
                    {
                        var foundDrawables = false;

                        for (int j = i; j <= drawBlockIndex; j++)
                        {
                            var virtualBlock = drawMethod.DrawBlocks[j];
                            DrawableFinder.FindDrawables(compiler, virtualBlock, false);

                            if (!foundDrawables)
                                foundDrawables = virtualBlock.Drawables.Length > 0;
                        }

                        for (int j = i; j <= drawBlockIndex; j++)
                        {
                            var virtualBlock = drawMethod.DrawBlocks[j];

                            if (!foundDrawables)
                                DrawableFinder.FindDrawables(compiler, virtualBlock, true);

                            GenerateDrawCalls(compiler, initMethod, freeMethod, virtualBlock, drawScopes);
                        }

                        //compiler.Log.Message(drawBlock.Source, ErrorCode.M0000, "Created " + (drawBlockIndex + i + 1) + " draw blocks from virtual " + drawBlock.Quote());
                        i += drawBlockIndex - i;
                    }
                    else
                    {
                        DrawableFinder.FindDrawables(compiler, drawBlock, true);
                        GenerateDrawCalls(compiler, initMethod, freeMethod, drawBlock, drawScopes);
                    }
                }
            }

            FixedArrayProcessor.Process(compiler.Pass, dt, initMethod.Body, drawScopes);
            VariableInliner.Process(compiler.Pass, initMethod.Body);

            foreach (var s in drawScopes)
                VariableInliner.Process(compiler.Pass, s);

            foreach (var m in dt.Methods)
                if (m.DrawBlocks.Count > 0)
                    ScopeInliner.Process(compiler.Pass, m.Body, drawScopes);
        }

        static void GenerateDrawCalls(Compiler compiler, Method initMethod, Method freeMethod, DrawBlock drawBlock, HashSet<Scope> drawScopes)
        {
            foreach (var drawable in drawBlock.Drawables)
            {
                new ShaderGenerator(compiler, drawable, initMethod.Body, freeMethod.Body, drawBlock.DrawScope).Generate();
                FixedArrayProcessor.Process(compiler.Pass, drawable.DrawState.VertexShader);
                FixedArrayProcessor.Process(compiler.Pass, drawable.DrawState.PixelShader);
                VariableInliner.Process(compiler.Pass, drawable.DrawState.VertexShader);
                VariableInliner.Process(compiler.Pass, drawable.DrawState.PixelShader);
            }

            foreach (var drawable in drawBlock.Drawables)
            {
                drawBlock.DrawScope.Statements.Add(new Draw(drawable.Source, drawable.DrawState));
                drawScopes.Add(drawBlock.DrawScope);
            }
        }

        static DrawBlock CreateFlattenedVirtualApplyInDrawBlock(Source src, DrawBlock drawBlock, int ApplyIndex, Expression obj, Block instBlock)
        {
            var result = new DrawBlock(drawBlock);

            for (int i = 0, l = drawBlock.Members.Count; i < l; i++)
                result.Members.Add(i != ApplyIndex
                    ? drawBlock.Members[i]
                    : new Apply(src, ApplyModifier.Sealed, instBlock, obj));

            return result;
        }

        static bool TrySplitVirtualAppliesInDrawBlockRecursive(Compiler compiler, DrawBlock drawBlock, ref int drawBlockIndex, HashSet<Scope> drawScopes)
        {
            for (int i = 0, l = drawBlock.Members.Count; i < l; i++)
            {
                var apply = drawBlock.Members[i] as Apply;

                if (apply != null &&
                    apply.Modifier == ApplyModifier.Virtual &&
                    apply.Object != null)
                {
                    var dt = apply.Object.ReturnType;
                    int baseIndex = drawBlockIndex;

                    Statement root = null;
                    IfElse lastIf = null;

                    HashSet<DataType> subSet;
                    if (compiler.BlockBuilder.FlattenedTypes.TryGetValue(dt, out subSet))
                    {
                        var subArray = subSet.ToArray();

                        // Make sure array is sorted correctly.
                        // Reverse order since this is most similar to the original order making sorting ~O(n)

                        Array.Sort(subArray,
                            (a, b) =>
                            {
                                int al = 0, bl = 0;

                                for (var bt = a.Base; bt != null && bt != dt; bt = bt.Base)
                                    al++;
                                for (var bt = b.Base; bt != null && bt != dt; bt = bt.Base)
                                    bl++;

                                return al - bl;
                            });

                        foreach (var st in subArray.Reverse())
                        {
                            if (st.IsAbstract || st.Block == null || !st.IsAccessibleFrom(apply.Source))
                                continue;

                            var obj = new AsOp(apply.Object.Source, apply.Object, st);
                            var subBlock = CreateFlattenedVirtualApplyInDrawBlock(apply.Source, drawBlock, i, obj, st.Block);
                            var cond = new IsOp(apply.Object.Source, apply.Object, st, compiler.Essentials.Bool);
                            var subIf = new IfElse(subBlock.Source, cond, subBlock.DrawScope);

                            if (lastIf == null)
                            {
                                root = subIf;
                                lastIf = subIf;
                            }
                            else
                            {
                                lastIf.OptionalElseBody = subIf;
                                lastIf = subIf;
                            }

                            drawBlock.Method.DrawBlocks.Insert(++baseIndex, subBlock);
                            TrySplitVirtualAppliesInDrawBlockRecursive(compiler, subBlock, ref baseIndex, drawScopes);
                        }
                    }

                    if (!dt.IsAbstract)
                    {
                        var baseBlock = CreateFlattenedVirtualApplyInDrawBlock(apply.Source, drawBlock, i, apply.Object, apply.Block);

                        if (lastIf == null)
                            root = baseBlock.DrawScope;
                        else
                            lastIf.OptionalElseBody = baseBlock.DrawScope;

                        drawBlock.Method.DrawBlocks.Insert(++baseIndex, baseBlock);
                        TrySplitVirtualAppliesInDrawBlockRecursive(compiler, baseBlock, ref baseIndex, drawScopes);
                    }

                    if (root != null)
                        drawBlock.DrawScope.Statements.Add(root);

                    drawScopes.Add(drawBlock.Method.DrawBlocks[drawBlockIndex].DrawScope);
                    drawBlock.Method.DrawBlocks.RemoveAt(drawBlockIndex);
                    drawBlockIndex += baseIndex - drawBlockIndex - 1;
                    return true;
                }
            }

            return false;
        }
    }
}
