using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Validation.ControlFlow
{
    public class BasicBlock
    {
        public Source Source;
        public TryCatchFinally TryBlock;
        public int TryDepth;
        public readonly List<Instruction> Instructions = new List<Instruction>();

        public BasicBlock(Source src, TryCatchFinally tryBlock, int tryDepth)
        {
            TryBlock = tryBlock;
            TryDepth = tryDepth;
            Source = src;
            Ending = BlockEnding.Open;
        }

        public BlockEnding Ending
        {
            get;
            set;
        }

        public List<BasicBlock> Successors
        {
            get;
            private set;
        }

        public void Terminate(BlockEnding ending, params BasicBlock[] successors)
        {
            if (Ending != BlockEnding.Open)
                throw new InvalidOperationException("Basic block terminated more than once");

            Ending = ending;
            Successors = new List<BasicBlock>(successors);
        }
    }
}
