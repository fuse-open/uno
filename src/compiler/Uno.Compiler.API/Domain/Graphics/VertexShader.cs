namespace Uno.Compiler.API.Domain.Graphics
{
    public sealed class VertexShader : Shader
    {
        public override ShaderType Type => ShaderType.Vertex;

        public VertexShader(DrawState ds)
            : base(ds, "vs_Main")
        {
        }
    }
}