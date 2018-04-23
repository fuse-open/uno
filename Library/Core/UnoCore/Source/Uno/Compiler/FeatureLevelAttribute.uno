namespace Uno.Compiler
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class FeatureLevelAttribute : Attribute
    {
        public int Value { get; private set; }

        public FeatureLevelAttribute(int value)
        {
            Value = value;
        }
    }
}
