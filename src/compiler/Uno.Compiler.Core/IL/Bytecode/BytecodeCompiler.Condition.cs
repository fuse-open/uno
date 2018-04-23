using Uno.Compiler.API.Domain.Bytecode;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.IL.Bytecode
{
    public partial class BytecodeCompiler
    {
        public Condition CompileCondition(Expression condition, ConditionSequence sequence, Label trueLabel = null, Label falseLabel = null)
        {
            var cond = new Condition(sequence, trueLabel, falseLabel);

            CompileExpression(condition, false, false, cond);

            if (cond.TrueLabel == null) cond.TrueLabel = NewLabel();
            if (cond.FalseLabel == null) cond.FalseLabel = NewLabel();

            if (!cond.Handled)
            {
                if (cond.Sequence == ConditionSequence.TrueFollows)
                {
                    Branch(Opcodes.BrFalse, cond.FalseLabel);
                }
                else if (cond.Sequence == ConditionSequence.FalseFollows)
                {
                    Branch(Opcodes.BrTrue, cond.TrueLabel);
                }
                else
                {
                    Branch(Opcodes.BrTrue, cond.TrueLabel);
                    Branch(Opcodes.Br, cond.FalseLabel);
                }
            }

            return cond;
        }
    }
}
