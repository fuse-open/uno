using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL.Types
{
    public sealed class DelegateType : GenericType, IParametersEntity
    {
        public new DataType ReturnType { get; private set; }
        public new Parameter[] Parameters { get; private set; }

        public DelegateType(Source src, Namescope parent, string comment, Modifiers modifiers, string name)
            : base(src, parent, comment, modifiers, name)
        {
        }

        public void SetReturnType(DataType returnType)
        {
            ReturnType = returnType;
            _func = null;
        }

        public void SetParameters(params Parameter[] parameters)
        {
            Parameters = parameters;
            _func = null;
        }

        public override TypeType TypeType => TypeType.Delegate;

        protected override GenericType CreateParameterizable()
        {
            return new DelegateType(Source, Parent, DocComment, Modifiers, UnoName);
        }

        Function _func;
        internal Function Function => _func ?? (_func = new Method(Source, this, DocComment, Modifiers.Generated, ".func", ReturnType, Parameters));
    }
}
