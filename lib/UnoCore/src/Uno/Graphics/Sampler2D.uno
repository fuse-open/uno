using Uno.Compiler.ExportTargetInterop;
using Uno.Compiler.ShaderGenerator;

namespace Uno.Graphics
{
    [DontExport]
    public sealed intrinsic class Sampler2D
    {
        [RequireShaderStage(ShaderStage.Pixel)]
        public extern float4 Sample(float2 texCoord);
    }
}
