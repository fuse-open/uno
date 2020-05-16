using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Disasm.ILView.Members;

namespace Uno.Disasm.ILView.Namespaces
{
    public class DefinitionCollection : ILItem
    {
        public DefinitionCollection(DataType p)
        {
            AddChild(new TypeItem(p));
        }

        public DefinitionCollection(Method p)
        {
            AddChild(new FunctionItem(p));
        }

        public override string DisplayName => "Definition";
        public override ILIcon Icon => ILIcon.GenericParameterizations;
    }
}