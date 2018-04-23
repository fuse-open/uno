namespace Uno.Compiler.API.Domain.Bytecode
{
    public static class Extensions
    {
        public static bool IsBranch(this Opcodes e)
        {
            switch (e)
            {
                case Opcodes.Leave:
                case Opcodes.Br:
                case Opcodes.BrEq:
                case Opcodes.BrNeq:
                case Opcodes.BrTrue:
                case Opcodes.BrFalse:
                case Opcodes.BrLt:
                case Opcodes.BrLte:
                case Opcodes.BrGt:
                case Opcodes.BrGte:
                case Opcodes.BrLt_Unsigned:
                case Opcodes.BrLte_Unsigned:
                case Opcodes.BrGt_Unsigned:
                case Opcodes.BrGte_Unsigned:
                case Opcodes.BrNull:
                case Opcodes.BrNotNull:
                    return true;
                default:
                    return false;
            }
        }
    }
}
