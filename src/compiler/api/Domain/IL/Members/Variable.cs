using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public class Variable : Entity
    {
        public DataType ValueType;
        public Expression OptionalValue;
        public readonly Function Function;
        public readonly VariableType VariableType;
        public Variable Next;

        public override EntityType EntityType => EntityType.Variable;
        public bool IsConstant => VariableType == VariableType.Constant;
        public bool IsException => VariableType == VariableType.Exception;
        public bool IsExtern => VariableType == VariableType.Extern;
        public bool IsIndirection => VariableType == VariableType.Indirection;
        public bool IsIterator => VariableType == VariableType.Iterator;

        public Variable(Source src, Function func, string name, DataType dt, VariableType type = 0, Expression optionalValue = null)
            : base(src, name)
        {
            ValueType = dt;
            Function = func;
            VariableType = type;
            OptionalValue = optionalValue;
        }

        public Variable Copy(CopyState state)
        {
            var result = new Variable(Source, Function, Name, state.GetType(ValueType), VariableType, OptionalValue.CopyNullable(state));
            state.AddVariable(this, result);
            if (Next != null)
                result.Next = Next.Copy(state);
            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}