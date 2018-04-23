using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Disasm.ILView.Members;

namespace Uno.Disasm.ILView.Namespaces
{
    public class ParameterizationCollection : ILItem
    {
        public ParameterizationCollection(IEnumerable<DataType> ps)
        {
            foreach (var p in ps)
                AddChild(new TypeItem(p));
        }

        public ParameterizationCollection(IEnumerable<Method> ps)
        {
            foreach (var p in ps)
                AddChild(new FunctionItem(p));
        }

        public override string DisplayName => "Parameterizations";
        public override ILIcon Icon => ILIcon.GenericParameterizations;
    }
}
