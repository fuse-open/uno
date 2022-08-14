using Uno.Compiler.ExportTargetInterop;
using Uno;

namespace OpenGL
{
    public extern(OPENGL) struct GLShaderPrecisionFormat
    {
        public int RangeMin;
        public int RangeMax;
        public int Precision;

        public GLShaderPrecisionFormat(int rangeMin, int rangeMax, int precision)
        {
            RangeMin = rangeMin;
            RangeMax = rangeMax;
            Precision = precision;
        }
    }
}
