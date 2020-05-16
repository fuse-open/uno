using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Validation.ControlFlow
{
    public partial class ControlFlowValidator
    {
        readonly Stack<TryCatchFinally> _tryStack = new Stack<TryCatchFinally>();
        readonly List<BasicBlock> _blocks = new List<BasicBlock>();
        BasicBlock _current;

        void EmitValidateNodeIsInitialized(Source s, MemoryNode node)
        {
            _current.Instructions.Add(new Instruction(s, Opcodes.ValidateNodeIsInitialized, node));
        }

        void EmitWriteNode(Source s, MemoryNode node)
        {
            _current.Instructions.Add(new Instruction(s, Opcodes.WriteNode, node));
        }

        void EmitReadNode(Source s, MemoryNode node)
        {
            _current.Instructions.Add(new Instruction(s, Opcodes.ReadNode, node));
        }

        BasicBlock NewBlock(Source s)
        {
            var b = new BasicBlock(s, _tryStack.Count > 0 ? _tryStack.Peek() : null, _tryStack.Count);
            _blocks.Add(b);
            return b;
        }

        void UnreachableCodeDetected(Source src)
        {
            Log.Warning(src, ErrorCode.E0000, "Unreachable code detected");
        }

        void EndBlock(BlockEnding ending, params BasicBlock[] successors)
        {
            if (_current != null)
            {
                _current.Terminate(ending, successors);
                _current = null;
            }
            else
                UnreachableCodeDetected(_blocks.Last().Source);
        }
    }
}
