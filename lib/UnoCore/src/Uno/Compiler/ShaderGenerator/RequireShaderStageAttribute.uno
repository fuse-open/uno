namespace Uno.Compiler.ShaderGenerator
{
    public enum ShaderStage
    {
        Vertex = 4,
        Pixel = 5,
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RequireShaderStageAttribute : Attribute
    {
        public RequireShaderStageAttribute(ShaderStage stage)
        {
        }
    }
}
