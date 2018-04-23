using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Bytecode;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;

namespace Uno.Compiler.Core.IL.Bytecode
{
    public partial class BytecodeCompiler
    {
        Label NewLabel()
        {
            var lab = new Label(_tryCatchStack.Count > 0 ? _tryCatchStack.Last() : null);
            Labels.Add(lab);

            return lab;
        }

        public void Emit(Opcodes op, object arg = null)
        {
            Code.Add(new Instruction(op, arg));
        }

        public void Pop()
        {
            Emit(Opcodes.Pop);
        }

        void Call(Expression obj, Function func)
        {
            if (obj == null || obj is Base ||
                    func.DeclaringType.IsValueType)
                Emit(Opcodes.Call, func);
            else
            {
                if (obj.ReturnType.IsValueType ||
                        func.IsVirtualBase &&
                        func.DeclaringType.BuiltinType == BuiltinType.Object &&
                        obj.ReturnType.IsGenericParameter)
                    Emit(Opcodes.Constrained, obj.ReturnType);

                Emit(Opcodes.CallVirtual, func);
            }
        }

        public void MarkLabel(Label label)
        {
            label.Offset = Code.Count;
            Emit(Opcodes.MarkLabel, label);
        }

        public void Return(DataType dt)
        {
            if (CurrentTryCatch != null)
            {
                var temp = StoreTemp(dt, dt == null);
                var leaveLabel = new Label(null);
                Labels.Add(leaveLabel);
                _leaveCode.Add(new Instruction(Opcodes.MarkLabel, leaveLabel));
                if (temp != null) _leaveCode.Add(new Instruction(Opcodes.LoadLocal, temp));
                _leaveCode.Add(new Instruction(Opcodes.Ret));
                Emit(Opcodes.Leave, leaveLabel);
            }
            else
                Emit(Opcodes.Ret);
        }

        public void Branch(Opcodes branchOp, Label dst)
        {
            if (Code.Count == 0)
                return;

            // if last instruction sends control flow elsewhere, ignore the branch
            switch (Code[Code.Count - 1].Opcode)
            {
                case Opcodes.Ret: return;
                case Opcodes.Br: return;
                case Opcodes.Throw: return;
            }

            // Handle branching out of try-blocks gracefully with the 'leave'-instruction
            if (dst.TryCatchBlock != CurrentTryCatch)
            {
                if (branchOp == Opcodes.Br)
                {
                    Emit(Opcodes.Leave, dst);
                }
                else
                {
                    var tempLabel = NewLabel();
                    Emit(InvertOp(branchOp), tempLabel);
                    Emit(Opcodes.Leave, dst);
                    MarkLabel(tempLabel);
                }
            }
            else
            {
                Emit(branchOp, dst);
            }
        }
    }
}
