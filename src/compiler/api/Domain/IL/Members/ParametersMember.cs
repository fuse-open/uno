using System;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public abstract class ParametersMember : Member, IComparable<ParametersMember>, IParametersEntity
    {
        public Parameter[] Parameters
        {
            get;
            private set;
        }

        protected ParametersMember(Source src, string comment, Modifiers modifiers, string name, DataType owner, DataType returnType, Parameter[] parameters)
            : base(src, comment, modifiers, name, owner, returnType)
        {
            Parameters = parameters;
        }

        public void SetParameters(params Parameter[] parameters)
        {
            Parameters = parameters;
        }

        public int CompareTo(ParametersMember other)
        {
            var baseDiff = base.CompareTo(other);
            if (baseDiff != 0) return baseDiff;

            var minLen = Math.Min(Parameters.Length, other.Parameters.Length);
            for (int i = 0; i < minLen; i++)
            {
                var paramDiff = Parameters[i].Type.CompareTo(other.Parameters[i].Type);
                if (paramDiff != 0) return paramDiff;
            }

            return Parameters.Length - other.Parameters.Length;
        }
    }
}