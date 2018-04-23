namespace Uno.Compiler.API.Domain.Graphics
{
    public class TerminalProperty
    {
        public readonly string Name;
        public readonly MetaStage Stage;
        public readonly string TypeString;
        public readonly string DefaultString;
        public readonly bool Required;

        public TerminalProperty(string name, MetaStage stage, string typeString, string defaultString = null, bool required = true)
        {
            Name = name;
            Stage = stage;
            TypeString = typeString;
            DefaultString = defaultString;
            Required = required;
        }
    }
}
