using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class BlockBuilder
    {
        public void PopulateBlock(AstBlockBase ast, BlockBase result)
        {
            // TODO
            /*
            if (result is Block)
                ((Block) result).SetAttributes(_compiler.CompileAttributes(result.Parent, ast.Attributes));
            */
            if (ast is AstBlock)
            {
                var blockDecl = ast as AstBlock;
                var usingBlocks = new Block[blockDecl.UsingBlocks?.Count ?? 0];

                for (int i = 0; i < usingBlocks.Length; i++)
                    usingBlocks[i] = _resolver.GetBlock(result.Parent, blockDecl.UsingBlocks[i]);

                ((Block)result).SetUsingBlocks(usingBlocks);
            }

            var dt = result.ParentType;
            dt?.PopulateMembers();

            if (result is Block)
                foreach (var block in (result as Block).UsingBlocks)
                    block.Populate();

            foreach (var item in ast.Members)
            {
                switch (item.MemberType)
                {
                    case AstMemberType.ApplyStatement:
                        CompileApply((AstApply)item, result);
                        continue;
                    case AstMemberType.Block:
                        CreateBlock((AstBlock)item, result);
                        continue;
                    case AstMemberType.MetaProperty:
                        result.Members.Add(CreateMetaProperty((AstMetaProperty)item, result));
                        continue;
                    case AstMemberType.NodeBlock:
                        result.Members.Add(new Node(CreateNodeBlock((AstNode) item, result)));
                        continue;

                    case AstMemberType.Field:
                    case AstMemberType.Property:
                        if (dt == null)
                            break;

                        var f = (AstNamedMember) item;
                        var fd = f as AstField;
                        var pd = f as AstProperty;

                        if (f.Modifiers.HasFlag(Modifiers.Static) ||
                            !_compiler.Environment.Test(f.Name.Source, f.OptionalCondition) ||
                            fd != null && fd.FieldModifiers.HasFlag(FieldModifiers.Const) ||
                            pd != null && (pd.Get == null || pd.OptionalInterfaceType != null))
                            continue;

                        var pt = _compiler.TypeBuilder.Parameterize(dt);
                        var fc = new FunctionCompiler(_compiler, pt);
                        var pe = fc.TryResolveTypeMember(pt, f.Name, null, null, new GetMetaObject(f.Name.Source, pt));

                        if (pe == null)
                        {
                            if (!dt.CanLink)
                                Log.Warning(f.Name.Source, ErrorCode.IW3205, f.Name.Symbol.Quote() + " was not found in class scope");
                            continue;
                        }

                        var mp = new MetaProperty(f.Name.Source, result, _resolver.GetType(pt, f.ReturnType), f.Name.Symbol, f.Modifiers.HasFlag(Modifiers.Public) ? MetaVisibility.Public : 0);
                        mp.SetDefinitions(new MetaDefinition(fc.CompilePartial(pe), new string[0], new ReqObject(f.Name.Source, pt)));
                        result.Members.Add(mp);
                        break;
                }

                if (dt == null)
                    Log.Error(((item as AstNamedMember)?.Name ?? ast.Name).Source, ErrorCode.E3207, "<" + item.MemberType + "> is not allowed in this scope");
            }
        }
    }
}
