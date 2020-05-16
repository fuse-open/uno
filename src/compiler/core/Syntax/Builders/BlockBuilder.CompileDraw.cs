using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Statements;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class BlockBuilder
    {
        public Statement CompileDraw(Function func, List<VariableScope> vscopeStack, AstDraw draw)
        {
            var method = func as Method;

            if (method == null || method.IsStatic ||
                !method.DeclaringType.Methods.Contains(method))
            {
                Log.Error(draw.Source, ErrorCode.E3222, "'draw' is not allowed in this scope because " + func.Quote() + " is not a non-static method");
                return Expression.Invalid;
            }

            var parent = method.DeclaringType as ClassType;

            if (parent == null || parent.IsFlattenedDefinition)
            {
                Log.Error(draw.Source, ErrorCode.E3223, "'draw' is not allowed in this scope because " + method.DeclaringType.Quote() + " is not a non-generic class");
                return Expression.Invalid;
            }

            var result = new DrawBlock(draw.Source, parent, method, FlattenVariableScopes(vscopeStack));
            method.DrawBlocks.Add(result);

            EnqueueBlock(result, x => PopulateBlock(draw.Block, x));
            _enqueuedDrawClasses.Add(method.DeclaringType);

            return result.DrawScope;
        }

        static Dictionary<string, Variable> FlattenVariableScopes(List<VariableScope> vscopeStack)
        {
            var result = new Dictionary<string, Variable>();

            for (int i = 0; i < vscopeStack.Count; i++)
                foreach (var v in vscopeStack[i].Variables)
                    result[v.Key] = v.Value;

            return result;
        }
    }
}
