namespace Uno.Compiler.API.Domain.Graphics
{
    public sealed class PixelShader : Shader
    {
        public override ShaderType Type => ShaderType.Pixel;

        public PixelShader(DrawState ds)
            : base(ds, "ps_Main")
        {
        }
    }
}