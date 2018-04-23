using System;
using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.IL.Types
{
    public abstract class GenericType : DataType
    {
        List<DataType> _genericParameterizations;
        GenericType _genericDefinition;
        GenericParameterType[] _typeParams;
        DataType[] _typeArgs;

        protected GenericType(Source src, Namescope parent, string comment, Modifiers modifiers, string name)
            : base(src, parent, comment, modifiers, name)
        {
        }

        protected abstract GenericType CreateParameterizable();

        public override DataType GenericDefinition => _genericDefinition;

        public override GenericParameterType[] GenericParameters => _typeParams;

        public override DataType[] GenericArguments => _typeArgs;

        public override bool IsGenericDefinition => _typeParams != null;

        public override bool IsGenericParameterization => _typeArgs != null;

        public override List<DataType> GenericParameterizations => _genericParameterizations;

        public override DataType CreateParameterization(params DataType[] args)
        {
            var result = CreateParameterizable();
            _genericParameterizations.Add(result);

            result._genericDefinition = this;
            result._typeArgs = new DataType[args.Length];
            result.Stats |= Stats;

            for (int i = 0; i < args.Length; i++)
                result._typeArgs[i] = args[i];

            return result;
        }

        public override void MakeGenericDefinition(params GenericParameterType[] typeParams)
        {
            _genericParameterizations = new List<DataType>();
            _typeParams = typeParams;

            if (_flattenedParameters != null)
            {
                int start = _flattenedParameters.Length;
                Array.Resize(ref _flattenedParameters, start + _typeParams.Length);
                for (int i = 0; i < _typeParams.Length; i++)
                    _flattenedParameters[start + i] = _typeParams[i];
            }
            else
                _flattenedParameters = _typeParams;
        }
    }
}
