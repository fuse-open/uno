namespace Uno.Compiler.Core.IL.Validation.ControlFlow
{
    public class Condition
    {
        public BasicBlock TrueBlock, FalseBlock;
        public bool Handled;

        public Condition(BasicBlock trueBlock, BasicBlock falseBlock)
        {
            TrueBlock = trueBlock;
            FalseBlock = falseBlock;
            Handled = false;
        }
    }
}