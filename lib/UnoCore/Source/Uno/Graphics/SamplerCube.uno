using Uno.Compiler.ExportTargetInterop;
using Uno.Compiler.ShaderGenerator;

namespace Uno.Graphics
{
    [DontExport]
    public sealed intrinsic class SamplerCube
    {
        [RequireShaderStage(ShaderStage.Pixel)]
        public extern float4 Sample(float3 texCoord);
    }
}
