namespace Uno.Compiler.ExportTargetInterop
{
    public sealed class RequireAttribute : Attribute
    {
        public readonly string Key;
        public readonly string Value;

        public RequireAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public sealed class SetAttribute : Attribute
    {
        public readonly string Key;
        public readonly string Value;

        public SetAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public sealed class ProcessFileAttribute : Attribute
    {
        public ProcessFileAttribute(string type, string name, string targetName = null)
        {
        }
    }
}
