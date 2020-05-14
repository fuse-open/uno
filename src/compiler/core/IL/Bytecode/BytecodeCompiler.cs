using System.Collections.Generic;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.Bytecode;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Bytecode
{
    public partial class BytecodeCompiler : IBytecodeCompiler
    {
        readonly Stack<Label> _breakLabels = new Stack<Label>();
        readonly Stack<Label> _continueLabels = new Stack<Label>();
        readonly List<TryCatchFinally> _tryCatchStack = new List<TryCatchFinally>();
        readonly List<Instruction> _leaveCode = new List<Instruction>();
        int _tempVariableCounter;

        public Function Function { get; }
        public List<Instruction> Code { get; } = new List<Instruction>();
        public List<Label> Labels { get; } = new List<Label>();
        public List<Variable> Locals { get; } = new List<Variable>();
        TryCatchFinally CurrentTryCatch => _tryCatchStack.Count > 0 ? _tryCatchStack.Last() : null;

        public BytecodeCompiler(Function func)
        {
            Function = func;
        }

        public void Compile()
        {
            if (Function.IsAbstract || !Function.HasBody)
                return;

            CompileStatement(Function.Body, !Function.IsGenerated);

            if (Function.Stats.HasFlag(EntityStats.ImplicitReturn))
                Emit(Opcodes.Ret);

            if (_leaveCode.Count > 0)
            {
                foreach (var c in _leaveCode)
                {
                    if (c.Opcode == Opcodes.MarkLabel)
                        (c.Argument as Label).Offset = Code.Count;

                    Code.Add(c);
                }
            }


            // Remove redundant branches out of the code
            // Must be done, even if the branches are unreachable, else .NET complaints

            // Step 1 - remove redundant MarkLabels at the end of the program
            for (int i = Code.Count - 1; i >= 0; i--)
            {
                if (Code[i].Opcode == Opcodes.MarkLabel)
                {
                    Labels.Remove(Code[i].Argument as Label);
                    Code.RemoveAt(i);
                }
                else
                    break;
            }

            // Step 2 - Remove branches out of code
            // This is safe - assuming that control flow has already been validated
            for (int i = 0; i < Code.Count; i++)
            {
                if (Code[i].Opcode.IsBranch())
                {
                    var lbl = Code[i].Argument as Label;

                    if (lbl.Offset >= Code.Count)
                    {
                        foreach (var l in Labels)
                            if (l.Offset >= i)
                                l.Offset--;

                        Code.RemoveAt(i--);
                    }
                }
            }

        }
        /*
        bool EndReachable(int pos, bool[] visited)
        {
            if (pos == -1)
                return false;

            while (true)
            {
                if (pos >= visited.Length)
                    return true;

                if (visited[pos])
                    return false;

                visited[pos] = true;

                switch (Code[pos].Opcode)
                {
                    case Opcodes.Leave:
                    case Opcodes.Br:
                        return EndReachable((Code[pos].Argument as Label).Offset, visited);

                    // These are equivalent with branches to the end of the TryCatch <-- not true!
                    case Opcodes.BeginCatchBlock:
                    case Opcodes.BeginExceptionBlock:
                        pos++;
                        for (int xpos = pos; xpos < Code.Count; xpos++)
                        {
                            if (Code[xpos].Opcode == Opcodes.EndExceptionBlock)
                            {
                                if (EndReachable(xpos, visited)) return true;
                                break;
                            }
                        }
                        break;


                    case Opcodes.BrEq:
                    case Opcodes.BrNeq:
                    case Opcodes.BrGt:
                    case Opcodes.BrGte:
                    case Opcodes.BrLt:
                    case Opcodes.BrLte:
                    case Opcodes.BrGt_Unsigned:
                    case Opcodes.BrGte_Unsigned:
                    case Opcodes.BrLt_Unsigned:
                    case Opcodes.BrLte_Unsigned:
                    case Opcodes.BrTrue:
                    case Opcodes.BrFalse:
                    case Opcodes.BrNull:
                    case Opcodes.BrNotNull:
                        if (EndReachable((Code[pos].Argument as Label).Offset, visited)) return true;
                        pos++;
                        continue;

                    case Opcodes.Throw:
                        return false;

                    case Opcodes.Ret:
                        return false;

                    default:
                        pos++;
                        continue;
                }
            }
        }*/
    }
}
