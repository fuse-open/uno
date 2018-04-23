using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL
{
    public class CopyState
    {
        Dictionary<Variable, Variable> _vars;
        Dictionary<Lambda, Lambda> _lambdas;
        readonly CopyProvider _typeProvider;

        public Function Function { get; private set; }

        public CopyState(Function func, CopyProvider typeProvider = null)
        {
            Function = func;
            _typeProvider = typeProvider;
        }

        public void SetFunction(Function func)
        {
            _vars?.Clear();
            Function = func;
        }

        public void AddVariable(Variable find, Variable replace)
        {
            if (_vars == null)
                _vars = new Dictionary<Variable, Variable>();

            _vars.Add(find, replace);
        }

        public Variable GetVariable(Variable var, bool allowNotFound = false)
        {
            Variable result;
            if (_vars != null && _vars.TryGetValue(var, out result))
                return result;

            if (allowNotFound)
                return var;

            throw new CopyException("The variable '" + var.ValueType + " " + var.Name + "' was not found");
        }

        public void AddLambda(Lambda find, Lambda replace)
        {
            if (_lambdas == null)
                _lambdas = new Dictionary<Lambda, Lambda>();

            _lambdas.Add(find, replace);
        }

        public Lambda GetLambda(Lambda lam, bool allowNotFound = false)
        {
            Lambda result;
            if (_lambdas != null && _lambdas.TryGetValue(lam, out result))
                return result;

            if (allowNotFound)
                return lam;

            throw new CopyException("The lambda '" + lam + "' was not found");
        }

        public T GetType<T>(T dt) where T : DataType
        {
            if (_typeProvider != null)
                return (T)_typeProvider.TryGetType(dt) ?? dt;

            return dt;
        }

        public T GetMember<T>(T member) where T : Member
        {
            if (_typeProvider != null)
                return (T)_typeProvider.TryGetMember(member) ?? member;

            return member;
        }
    }
}
