using Uno.Compiler.ExportTargetInterop;

namespace System
{
    [extern(DOTNET) DotNetType]
    public static extern(DOTNET) class Math
    {
        public static extern double Sin(double a);
        public static extern double Cos(double a);
        public static extern double Tan(double a);
        public static extern double Asin(double a);
        public static extern double Acos(double a);
        public static extern double Atan(double a);
        public static extern double Atan2(double a, double b);
        public static extern double Pow(double a, double b);
        public static extern double Sqrt(double a);
        public static extern double Floor(double a);
        public static extern double Ceiling(double a);
        public static extern double Log(double a);
        public static extern double Log10(double a);
    }
}
