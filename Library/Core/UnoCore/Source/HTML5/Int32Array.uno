using Uno.Compiler.ExportTargetInterop;

namespace HTML5
{
    [TargetSpecificType]
    public extern(JAVASCRIPT) struct Int32Array
    {
        public static Int32Array Create(int[] x)
        @{
            var l = $0.length,
                r = new Int32Array(l);

            for (var i = 0; i < l; i++)
                r[i] = $0[i];

            return r;
        @}

        public static Int32Array Create(int2[] x)
        @{
            var p = 0,
                l = $0.length,
                r = new Int32Array(l * 2);

            for (var i = 0; i < l; i++) {
                r[p++] = @{$0:Get(i).X};
                r[p++] = @{$0:Get(i).Y};
            }

            return r;
        @}

        public static Int32Array Create(int3[] x)
        @{
            var p = 0,
                l = $0.length,
                r = new Int32Array(l * 3);

            for (var i = 0; i < l; i++) {
                r[p++] = @{$0:Get(i).X};
                r[p++] = @{$0:Get(i).Y};
                r[p++] = @{$0:Get(i).Z};
            }

            return r;
        @}

        public static Int32Array Create(int4[] x)
        @{
            var p = 0,
                l = $0.length,
                r = new Int32Array(l * 4);

            for (var i = 0; i < l; i++) {
                r[p++] = @{$0:Get(i).X};
                r[p++] = @{$0:Get(i).Y};
                r[p++] = @{$0:Get(i).Z};
                r[p++] = @{$0:Get(i).W};
            }

            return r;
        @}
    }
}
