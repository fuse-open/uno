using Uno.Compiler.ExportTargetInterop;

namespace HTML5
{
    [TargetSpecificType]
    public extern(JAVASCRIPT) struct Float32Array
    {
        public static Float32Array Create(float2x2 x)
        @{
            return new Float32Array([
                @{$0.M11}, @{$0.M12},
                @{$0.M21}, @{$0.M22}]);
        @}

        public static Float32Array Create(float3x3 x)
        @{
            return new Float32Array([
                @{$0.M11}, @{$0.M12}, @{$0.M13},
                @{$0.M21}, @{$0.M22}, @{$0.M23},
                @{$0.M31}, @{$0.M32}, @{$0.M33}]);
        @}

        public static Float32Array Create(float4x4 x)
        @{
            return new Float32Array([
                @{$0.M11}, @{$0.M12}, @{$0.M13}, @{$0.M14},
                @{$0.M21}, @{$0.M22}, @{$0.M23}, @{$0.M24},
                @{$0.M31}, @{$0.M32}, @{$0.M33}, @{$0.M34},
                @{$0.M41}, @{$0.M42}, @{$0.M43}, @{$0.M44}]);
        @}

        public static Float32Array Create(float[] x)
        @{
            var l = $0.length,
                r = new Float32Array(l);

            for (var i = 0; i < l; i++)
                r[i] = $0[i];

            return r;
        @}

        public static Float32Array Create(float2[] x)
        @{
            var p = 0,
                l = $0.length,
                r = new Float32Array(l * 2);

            for (var i = 0; i < l; i++) {
                r[p++] = @{$0:Get(i).X};
                r[p++] = @{$0:Get(i).Y};
            }

            return r;
        @}

        public static Float32Array Create(float3[] x)
        @{
            var p = 0,
                l = $0.length,
                r = new Float32Array(l * 3);

            for (var i = 0; i < l; i++) {
                r[p++] = @{$0:Get(i).X};
                r[p++] = @{$0:Get(i).Y};
                r[p++] = @{$0:Get(i).Z};
            }

            return r;
        @}

        public static Float32Array Create(float4[] x)
        @{
            var p = 0,
                l = $0.length,
                r = new Float32Array(l * 4);

            for (var i = 0; i < l; i++) {
                r[p++] = @{$0:Get(i).X};
                r[p++] = @{$0:Get(i).Y};
                r[p++] = @{$0:Get(i).Z};
                r[p++] = @{$0:Get(i).W};
            }

            return r;
        @}

        public static Float32Array Create(float2x2[] x)
        @{
            var p = 0,
                l = $0.length,
                r = new Float32Array(l * 4);

            for (var i = 0; i < l; i++) {
                r[p++] = @{$0:Get(i).M11};
                r[p++] = @{$0:Get(i).M12};
                r[p++] = @{$0:Get(i).M21};
                r[p++] = @{$0:Get(i).M22};
            }

            return r;
        @}

        public static Float32Array Create(float3x3[] x)
        @{
            var p = 0,
                l = $0.length,
                r = new Float32Array(l * 9);

            for (var i = 0; i < l; i++) {
                r[p++] = @{$0:Get(i).M11};
                r[p++] = @{$0:Get(i).M12};
                r[p++] = @{$0:Get(i).M13};
                r[p++] = @{$0:Get(i).M21};
                r[p++] = @{$0:Get(i).M22};
                r[p++] = @{$0:Get(i).M23};
                r[p++] = @{$0:Get(i).M31};
                r[p++] = @{$0:Get(i).M32};
                r[p++] = @{$0:Get(i).M33};
            }

            return r;
        @}

        public static Float32Array Create(float4x4[] x)
        @{
            var p = 0,
                l = $0.length,
                r = new Float32Array(l * 16);

            for (var i = 0; i < l; i++) {
                r[p++] = @{$0:Get(i).M11};
                r[p++] = @{$0:Get(i).M12};
                r[p++] = @{$0:Get(i).M13};
                r[p++] = @{$0:Get(i).M14};
                r[p++] = @{$0:Get(i).M21};
                r[p++] = @{$0:Get(i).M22};
                r[p++] = @{$0:Get(i).M23};
                r[p++] = @{$0:Get(i).M24};
                r[p++] = @{$0:Get(i).M31};
                r[p++] = @{$0:Get(i).M32};
                r[p++] = @{$0:Get(i).M33};
                r[p++] = @{$0:Get(i).M34};
                r[p++] = @{$0:Get(i).M41};
                r[p++] = @{$0:Get(i).M42};
                r[p++] = @{$0:Get(i).M43};
                r[p++] = @{$0:Get(i).M44};
            }

            return r;
        @}
    }
}
