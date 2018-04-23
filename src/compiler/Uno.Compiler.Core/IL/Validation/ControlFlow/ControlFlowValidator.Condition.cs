using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.IL.Validation.ControlFlow
{
    public partial class ControlFlowValidator
    {
        public Condition CompileCondition(Expression condition, BasicBlock trueBlock = null, BasicBlock falseBlock = null)
        {
            var cond = new Condition(trueBlock, falseBlock);

            CompileExpression(condition);

            if (cond.TrueBlock == null)
                cond.TrueBlock = NewBlock(condition.Source);

            if (cond.FalseBlock == null)
                cond.FalseBlock = NewBlock(condition.Source);

            if (!cond.Handled)
            {
                if (condition is Constant)
                {
                    if ((bool)(condition as Constant).Value)
                        EndBlock(BlockEnding.Br, cond.TrueBlock);
                    else
                        EndBlock(BlockEnding.Br, cond.FalseBlock);
                }
                else
                {
                    EndBlock(BlockEnding.CondBr, cond.TrueBlock, cond.FalseBlock);
                }
            }

            return cond;
        }
    }
}
