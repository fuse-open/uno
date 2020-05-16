using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.AST.Members;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.Core.Syntax.Binding;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class BlockBuilder
    {
        void CompileApply(AstApply ast, BlockBase parent)
        {
            var e = ast.Block;
            var src = e.Source;
            var fc = new FunctionCompiler(_compiler, parent);
            var p = fc.ResolveExpression(e, null);
            if (p.IsInvalid) return;

            if (p is PartialType)
            {
                var dt = (p as PartialType).Type;

                if (dt.Block != null)
                {
                    dt.PopulateMembers();
                    dt.Block.Populate();
                    parent.Members.Add(new Apply(src, ast.Modifier, dt.Block, null));
                    return;
                }
            }
            else if (p is PartialBlock)
            {
                var b = (p as PartialBlock).Block;
                b.Populate();
                parent.Members.Add(new Apply(src, ast.Modifier, b, null));
                return;
            }

            var sym = fc.CompilePartial(p);
            if (sym.IsInvalid)
                return;

            if (sym.ReturnType.Block != null)
            {
                sym.ReturnType.PopulateMembers();
                sym.ReturnType.Block.Populate();
                parent.Members.Add(new Apply(src, ast.Modifier, sym.ReturnType.Block, sym));
                return;
            }

            Log.Error(src, ErrorCode.E3213, "Only 'block', 'class', 'struct' or 'interface' types can be applied");
        }
    }
}
