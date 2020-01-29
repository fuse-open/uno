using Uno.Compiler.ExportTargetInterop;
using Uno.Compiler.ShaderGenerator;

namespace Uno
{
    [extern(CPLUSPLUS) Require("Source.Include", "cmath")]
    public static class Math
    {
        public const double PI = 3.14159265358979323846;
        public const float PIf = 3.14159274f;

        public const double E = 2.7182818284590452354;
        public const float Ef = 2.71828175f;


        // Angle and trigonometry functions

        public static double DegreesToRadians(double degrees) { return degrees * (PI / 180.0); }
        [GlslIntrinsic("radians")] public static float DegreesToRadians(float degrees) { return degrees * (PIf / 180.0f); }
        [GlslIntrinsic("radians")] public static float2 DegreesToRadians(float2 degrees) { return degrees * (PIf / 180.0f); }
        [GlslIntrinsic("radians")] public static float3 DegreesToRadians(float3 degrees) { return degrees * (PIf / 180.0f); }
        [GlslIntrinsic("radians")] public static float4 DegreesToRadians(float4 degrees) { return degrees * (PIf / 180.0f); }

        public static double RadiansToDegrees(double radians) { return radians * (180.0 / PI); }
        [GlslIntrinsic("degrees")] public static float RadiansToDegrees(float radians) { return radians * (180.0f / PIf); }
        [GlslIntrinsic("degrees")] public static float2 RadiansToDegrees(float2 radians) { return radians * (180.0f / PIf); }
        [GlslIntrinsic("degrees")] public static float3 RadiansToDegrees(float3 radians) { return radians * (180.0f / PIf); }
        [GlslIntrinsic("degrees")] public static float4 RadiansToDegrees(float4 radians) { return radians * (180.0f / PIf); }

        public static double Sin(double radians) {
            if defined(DOTNET) return System.Math.Sin(radians);
            else if defined(CPLUSPLUS) return extern<double> "sin($@)";
            else build_error;
        }
        [GlslIntrinsic("sin")]
        public static float Sin(float radians) {
            if defined(DOTNET) return (float)System.Math.Sin(radians);
            else if defined(CPLUSPLUS) return extern<float> "sinf($@)";
            else build_error;
        }
        [GlslIntrinsic("sin")] public static float2 Sin(float2 radians) { return float2(Sin(radians.X), Sin(radians.Y)); }
        [GlslIntrinsic("sin")] public static float3 Sin(float3 radians) { return float3(Sin(radians.X), Sin(radians.Y), Sin(radians.Z)); }
        [GlslIntrinsic("sin")] public static float4 Sin(float4 radians) { return float4(Sin(radians.X), Sin(radians.Y), Sin(radians.Z), Sin(radians.W)); }

        public static double Cos(double radians) {
            if defined(DOTNET) return System.Math.Cos(radians);
            else if defined(CPLUSPLUS) return extern<double> "cos($@)";
            else build_error;
        }
        [GlslIntrinsic("cos")]
        public static float Cos(float radians) {
            if defined(DOTNET) return (float)System.Math.Cos(radians);
            else if defined(CPLUSPLUS) return extern<float> "cosf($@)";
            else build_error;
        }
        [GlslIntrinsic("cos")] public static float2 Cos(float2 radians) { return float2(Cos(radians.X), Cos(radians.Y)); }
        [GlslIntrinsic("cos")] public static float3 Cos(float3 radians) { return float3(Cos(radians.X), Cos(radians.Y), Cos(radians.Z)); }
        [GlslIntrinsic("cos")] public static float4 Cos(float4 radians) { return float4(Cos(radians.X), Cos(radians.Y), Cos(radians.Z), Cos(radians.W)); }

        public static double Tan(double radians) {
            if defined(DOTNET) return System.Math.Tan(radians);
            else if defined(CPLUSPLUS) return extern<double> "tan($@)";
            else build_error;
        }
        [GlslIntrinsic("tan")]
        public static float Tan(float radians) {
            if defined(DOTNET) return (float)System.Math.Tan(radians);
            else if defined(CPLUSPLUS) return extern<float> "tanf($@)";
            else build_error;
        }
        [GlslIntrinsic("tan")] public static float2 Tan(float2 radians) { return float2(Tan(radians.X), Tan(radians.Y)); }
        [GlslIntrinsic("tan")] public static float3 Tan(float3 radians) { return float3(Tan(radians.X), Tan(radians.Y), Tan(radians.Z)); }
        [GlslIntrinsic("tan")] public static float4 Tan(float4 radians) { return float4(Tan(radians.X), Tan(radians.Y), Tan(radians.Z), Tan(radians.W)); }

        public static double Asin(double radians) {
            if defined(DOTNET) return System.Math.Asin(radians);
            else if defined(CPLUSPLUS) return extern<double> "asin($@)";
            else build_error;
        }
        [GlslIntrinsic("asin")]
        public static float Asin(float radians) {
            if defined(DOTNET) return (float)System.Math.Asin(radians);
            else if defined(CPLUSPLUS) return extern<float> "asinf($@)";
            else build_error;
        }
        [GlslIntrinsic("asin")] public static float2 Asin(float2 radians) { return float2(Asin(radians.X), Asin(radians.Y)); }
        [GlslIntrinsic("asin")] public static float3 Asin(float3 radians) { return float3(Asin(radians.X), Asin(radians.Y), Asin(radians.Z)); }
        [GlslIntrinsic("asin")] public static float4 Asin(float4 radians) { return float4(Asin(radians.X), Asin(radians.Y), Asin(radians.Z), Asin(radians.W)); }

        public static double Acos(double radians) {
            if defined(DOTNET) return System.Math.Acos(radians);
            else if defined(CPLUSPLUS) return extern<double> "acos($@)";
            else build_error;
        }
        [GlslIntrinsic("acos")]
        public static float Acos(float radians) {
            if defined(DOTNET) return (float)System.Math.Acos(radians);
            else if defined(CPLUSPLUS) return extern<float> "acosf($@)";
            else build_error;
        }
        [GlslIntrinsic("acos")] public static float2 Acos(float2 radians) { return float2(Acos(radians.X), Acos(radians.Y)); }
        [GlslIntrinsic("acos")] public static float3 Acos(float3 radians) { return float3(Acos(radians.X), Acos(radians.Y), Acos(radians.Z)); }
        [GlslIntrinsic("acos")] public static float4 Acos(float4 radians) { return float4(Acos(radians.X), Acos(radians.Y), Acos(radians.Z), Acos(radians.W)); }

        public static double Atan(double radians) {
            if defined(DOTNET) return System.Math.Atan(radians);
            else if defined(CPLUSPLUS) return extern<double> "atan($@)";
            else build_error;
        }
        [GlslIntrinsic("atan")]
        public static float Atan(float radians) {
            if defined(DOTNET) return (float)System.Math.Atan(radians);
            else if defined(CPLUSPLUS) return extern<float> "atanf($@)";
            else build_error;
        }
        [GlslIntrinsic("atan")] public static float2 Atan(float2 radians) { return float2(Atan(radians.X), Atan(radians.Y)); }
        [GlslIntrinsic("atan")] public static float3 Atan(float3 radians) { return float3(Atan(radians.X), Atan(radians.Y), Atan(radians.Z)); }
        [GlslIntrinsic("atan")] public static float4 Atan(float4 radians) { return float4(Atan(radians.X), Atan(radians.Y), Atan(radians.Z), Atan(radians.W)); }

        public static double Atan2(double y, double x) {
            if defined(DOTNET) return System.Math.Atan2(y, x);
            else if defined(CPLUSPLUS) return extern<double> "atan2($@)";
            else build_error;
        }
        [GlslIntrinsic("atan")]
        public static float Atan2(float y, float x) {
            if defined(DOTNET) return (float)System.Math.Atan2(y, x);
            else if defined(CPLUSPLUS) return extern<float> "atan2f($@)";
            else build_error;
        }
        [GlslIntrinsic("atan")] public static float2 Atan2(float2 y, float2 x) { return float2(Atan2(y.X, x.X), Atan2(y.Y, x.Y)); }
        [GlslIntrinsic("atan")] public static float3 Atan2(float3 y, float3 x) { return float3(Atan2(y.X, x.X), Atan2(y.Y, x.Y), Atan2(y.Z, x.Z)); }
        [GlslIntrinsic("atan")] public static float4 Atan2(float4 y, float4 x) { return float4(Atan2(y.X, x.X), Atan2(y.Y, x.Y), Atan2(y.Z, x.Z), Atan2(y.W, x.W)); }


        // Exponential functions

        public static double Pow(double x, double y) {
            if defined(DOTNET) return System.Math.Pow(x, y);
            else if defined(CPLUSPLUS) return extern<double> "pow($@)";
            else build_error;
        }
        [GlslIntrinsic("pow")]
        public static float Pow(float x, float y) {
            if defined(DOTNET) return (float)System.Math.Pow(x, y);
            else if defined(CPLUSPLUS) return extern<float> "powf($@)";
            else build_error;
        }
        [GlslIntrinsic("pow")] public static float2 Pow(float2 x, float2 y) { return float2(Pow(x.X, y.X), Pow(x.Y, y.Y)); }
        [GlslIntrinsic("pow")] public static float3 Pow(float3 x, float3 y) { return float3(Pow(x.X, y.X), Pow(x.Y, y.Y), Pow(x.Z, y.Z)); }
        [GlslIntrinsic("pow")] public static float4 Pow(float4 x, float4 y) { return float4(Pow(x.X, y.X), Pow(x.Y, y.Y), Pow(x.Z, y.Z), Pow(x.W, y.W)); }

        public static double Exp(double x) { return Pow(E, x); }
        [GlslIntrinsic("exp")] public static float Exp(float x) { return Pow(Ef, x); }
        [GlslIntrinsic("exp")] public static float2 Exp(float2 x) { return float2(Exp(x.X), Exp(x.Y)); }
        [GlslIntrinsic("exp")] public static float3 Exp(float3 x) { return float3(Exp(x.X), Exp(x.Y), Exp(x.Z)); }
        [GlslIntrinsic("exp")] public static float4 Exp(float4 x) { return float4(Exp(x.X), Exp(x.Y), Exp(x.Z), Exp(x.W)); }

        public static double Log(double x) {
            if defined(DOTNET) return System.Math.Log(x);
            else if defined(CPLUSPLUS) return extern<double> "log($@)";
            else build_error;
        }
        [GlslIntrinsic("log")]
        public static float Log(float x) {
            if defined(DOTNET) return (float)System.Math.Log(x);
            else if defined(CPLUSPLUS) return extern<float> "logf($@)";
            else build_error;
        }
        [GlslIntrinsic("log")] public static float2 Log(float2 x) { return float2(Log(x.X), Log(x.Y)); }
        [GlslIntrinsic("log")] public static float3 Log(float3 x) { return float3(Log(x.X), Log(x.Y), Log(x.Z)); }
        [GlslIntrinsic("log")] public static float4 Log(float4 x) { return float4(Log(x.X), Log(x.Y), Log(x.Z), Log(x.W)); }

        public static double Log10(double x)
        {
            if defined(DOTNET)
                return System.Math.Log10(x);
            else if defined(CPLUSPLUS)
                return extern<double> "log10($@)";
            else build_error;
        }

        public static double Exp2(double x) { return Pow(2.0, x); }
        [GlslIntrinsic("exp2")] public static float Exp2(float x) { return Pow(2.0f, x); }
        [GlslIntrinsic("exp2")] public static float2 Exp2(float2 x) { return float2(Exp2(x.X), Exp2(x.Y)); }
        [GlslIntrinsic("exp2")] public static float3 Exp2(float3 x) { return float3(Exp2(x.X), Exp2(x.Y), Exp2(x.Z)); }
        [GlslIntrinsic("exp2")] public static float4 Exp2(float4 x) { return float4(Exp2(x.X), Exp2(x.Y), Exp2(x.Z), Exp2(x.W)); }

        public static double Log2(double x) { return Log(x) / Log(2.0); }
        [GlslIntrinsic("log2")] public static float Log2(float x) { return Log(x) / Log(2.0f); }
        [GlslIntrinsic("log2")] public static float2 Log2(float2 x) { return float2(Log2(x.X), Log2(x.Y)); }
        [GlslIntrinsic("log2")] public static float3 Log2(float3 x) { return float3(Log2(x.X), Log2(x.Y), Log2(x.Z)); }
        [GlslIntrinsic("log2")] public static float4 Log2(float4 x) { return float4(Log2(x.X), Log2(x.Y), Log2(x.Z), Log2(x.W)); }

        public static double Sqrt(double x) {
            if defined(DOTNET) return System.Math.Sqrt(x);
            else if defined(CPLUSPLUS) return extern<double> "sqrt($@)";
            else build_error;
        }
        [GlslIntrinsic("sqrt")]
        public static float Sqrt(float x) {
            if defined(DOTNET) return (float)System.Math.Sqrt(x);
            else if defined(CPLUSPLUS) return extern<float> "sqrtf($@)";
            else build_error;
        }
        [GlslIntrinsic("sqrt")] public static float2 Sqrt(float2 x) { return float2(Sqrt(x.X), Sqrt(x.Y)); }
        [GlslIntrinsic("sqrt")] public static float3 Sqrt(float3 x) { return float3(Sqrt(x.X), Sqrt(x.Y), Sqrt(x.Z)); }
        [GlslIntrinsic("sqrt")] public static float4 Sqrt(float4 x) { return float4(Sqrt(x.X), Sqrt(x.Y), Sqrt(x.Z), Sqrt(x.W)); }

        public static double InverseSqrt(double x) { return 1.0 / Sqrt(x); }
        [GlslIntrinsic("inversesqrt")] public static float InverseSqrt(float x) { return 1.0f / Sqrt(x); }
        [GlslIntrinsic("inversesqrt")] public static float2 InverseSqrt(float2 x) { return float2(InverseSqrt(x.X), InverseSqrt(x.Y)); }
        [GlslIntrinsic("inversesqrt")] public static float3 InverseSqrt(float3 x) { return float3(InverseSqrt(x.X), InverseSqrt(x.Y), InverseSqrt(x.Z)); }
        [GlslIntrinsic("inversesqrt")] public static float4 InverseSqrt(float4 x) { return float4(InverseSqrt(x.X), InverseSqrt(x.Y), InverseSqrt(x.Z), InverseSqrt(x.W)); }


        // Common functions

        public static double Abs(double x) { return x >= 0.0 ? x : -x; }
        [GlslIntrinsic("abs")] public static float Abs(float x) { return x >= 0.0f ? x : -x; }
        [GlslIntrinsic("abs")] public static float2 Abs(float2 a) { return float2(Abs(a.X), Abs(a.Y)); }
        [GlslIntrinsic("abs")] public static float3 Abs(float3 a) { return float3(Abs(a.X), Abs(a.Y), Abs(a.Z)); }
        [GlslIntrinsic("abs")] public static float4 Abs(float4 a) { return float4(Abs(a.X), Abs(a.Y), Abs(a.Z), Abs(a.W)); }

        public static sbyte Abs(sbyte x)
        {
            if (x < 0)
            {
                if (x == sbyte.MinValue)
                    throw new OverflowException("Negating the minimum value of a twos complement number is invalid.");

                return (sbyte)-x;
            } else
                return x;
        }

        public static short Abs(short x)
        {
            if (x < 0)
            {
                if (x == short.MinValue)
                    throw new OverflowException("Negating the minimum value of a twos complement number is invalid.");

                return (short)-x;
            } else
                return x;
        }

        public static int Abs(int x)
        {
            if (x < 0)
            {
                if (x == int.MinValue)
                    throw new OverflowException("Negating the minimum value of a twos complement number is invalid.");

                return -x;
            } else
                return x;
        }
        public static int2 Abs(int2 a) { return int2(Abs(a.X), Abs(a.Y)); }
        public static int3 Abs(int3 a) { return int3(Abs(a.X), Abs(a.Y), Abs(a.Z)); }
        public static int4 Abs(int4 a) { return int4(Abs(a.X), Abs(a.Y), Abs(a.Z), Abs(a.W)); }

        public static long Abs(long x)
        {
            if (x < 0)
            {
                if (x == long.MinValue)
                    throw new OverflowException("Negating the minimum value of a twos complement number is invalid.");

                return -x;
            } else
                return x;
        }

        public static double Sign(double x) { return x < 0.0 ? -1.0 : x > 0.0 ? 1.0 : 0.0; }
        [GlslIntrinsic("sign")] public static float Sign(float x) { return x < 0.0f ? -1.0f : x > 0.0f ? 1.0f : 0.0f; }
        [GlslIntrinsic("sign")] public static float2 Sign(float2 a) { return float2(Sign(a.X), Sign(a.Y)); }
        [GlslIntrinsic("sign")] public static float3 Sign(float3 a) { return float3(Sign(a.X), Sign(a.Y), Sign(a.Z)); }
        [GlslIntrinsic("sign")] public static float4 Sign(float4 a) { return float4(Sign(a.X), Sign(a.Y), Sign(a.Z), Sign(a.W)); }
        public static int Sign(int x) { return x < 0 ? -1 : x > 0 ? 1 : 0; }
        public static int2 Sign(int2 a) { return int2(Sign(a.X), Sign(a.Y)); }
        public static int3 Sign(int3 a) { return int3(Sign(a.X), Sign(a.Y), Sign(a.Z)); }
        public static int4 Sign(int4 a) { return int4(Sign(a.X), Sign(a.Y), Sign(a.Z), Sign(a.W)); }

        public static double Floor(double x) {
            if defined(DOTNET) return System.Math.Floor(x);
            else if defined(CPLUSPLUS) return extern<double> "floor($@)";
            else build_error;
        }
        [GlslIntrinsic("floor")]
        public static float Floor(float x) {
            if defined(DOTNET) return (float)System.Math.Floor(x);
            else if defined(CPLUSPLUS) return extern<float> "floorf($@)";
            else build_error;
        }
        [GlslIntrinsic("floor")] public static float2 Floor(float2 v) { return float2(Floor(v.X), Floor(v.Y)); }
        [GlslIntrinsic("floor")] public static float3 Floor(float3 v) { return float3(Floor(v.X), Floor(v.Y), Floor(v.Z)); }
        [GlslIntrinsic("floor")] public static float4 Floor(float4 v) { return float4(Floor(v.X), Floor(v.Y), Floor(v.Z), Floor(v.W)); }

        public static double Ceil(double x) {
            if defined(DOTNET) return System.Math.Ceiling(x);
            else if defined(CPLUSPLUS) return extern<double> "ceil($@)";
            else build_error;
        }
        [GlslIntrinsic("ceil")]
        public static float Ceil(float x) {
            if defined(DOTNET) return (float)System.Math.Ceiling(x);
            else if defined(CPLUSPLUS) return extern<float> "ceilf($@)";
            else build_error;
        }
        [GlslIntrinsic("ceil")] public static float2 Ceil(float2 v) { return float2(Ceil(v.X), Ceil(v.Y)); }
        [GlslIntrinsic("ceil")] public static float3 Ceil(float3 v) { return float3(Ceil(v.X), Ceil(v.Y), Ceil(v.Z)); }
        [GlslIntrinsic("ceil")] public static float4 Ceil(float4 v) { return float4(Ceil(v.X), Ceil(v.Y), Ceil(v.Z), Ceil(v.W)); }

        public static double Fract(double x) { return x - Floor(x); }
        [GlslIntrinsic("fract")] public static float Fract(float x) { return x - Floor(x); }
        [GlslIntrinsic("fract")] public static float2 Fract(float2 x) { return x - Floor(x); }
        [GlslIntrinsic("fract")] public static float3 Fract(float3 x) { return x - Floor(x); }
        [GlslIntrinsic("fract")] public static float4 Fract(float4 x) { return x - Floor(x); }

        public static double Mod(double x, double y) { return x - y * Floor(x / y); }
        [GlslIntrinsic("mod")] public static float Mod(float x, float y) { return x - y * Floor(x / y); }
        [GlslIntrinsic("mod")] public static float2 Mod(float2 x, float y) { return x - y * Floor(x / y); }
        [GlslIntrinsic("mod")] public static float2 Mod(float2 x, float2 y) { return x - y * Floor(x / y); }
        [GlslIntrinsic("mod")] public static float3 Mod(float3 x, float y) { return x - y * Floor(x / y); }
        [GlslIntrinsic("mod")] public static float3 Mod(float3 x, float3 y) { return x - y * Floor(x / y); }
        [GlslIntrinsic("mod")] public static float4 Mod(float4 x, float y) { return x - y * Floor(x / y); }
        [GlslIntrinsic("mod")] public static float4 Mod(float4 x, float4 y) { return x - y * Floor(x / y); }

        public static double Max(double a, double b) { return a > b ? a : b; }
        [GlslIntrinsic("max")] public static float Max(float a, float b) { return a > b ? a : b; }
        [GlslIntrinsic("max")] public static float2 Max(float2 a, float b) { return float2(Max(a.X, b), Max(a.Y, b)); }
        [GlslIntrinsic("max")] public static float2 Max(float2 a, float2 b) { return float2(Max(a.X, b.X), Max(a.Y, b.Y)); }
        [GlslIntrinsic("max")] public static float3 Max(float3 a, float b) { return float3(Max(a.X, b), Max(a.Y, b), Max(a.Z, b)); }
        [GlslIntrinsic("max")] public static float3 Max(float3 a, float3 b) { return float3(Max(a.X, b.X), Max(a.Y, b.Y), Max(a.Z, b.Z)); }
        [GlslIntrinsic("max")] public static float4 Max(float4 a, float b) { return float4(Max(a.X, b), Max(a.Y, b), Max(a.Z, b), Max(a.W, b)); }
        [GlslIntrinsic("max")] public static float4 Max(float4 a, float4 b) { return float4(Max(a.X, b.X), Max(a.Y, b.Y), Max(a.Z, b.Z), Max(a.W, b.W)); }
        public static int Max(int a, int b) { return a > b ? a : b; }
        public static int2 Max(int2 a, int b) { return int2(Max(a.X, b), Max(a.Y, b)); }
        public static int2 Max(int2 a, int2 b) { return int2(Max(a.X, b.X), Max(a.Y, b.Y)); }
        public static int3 Max(int3 a, int b) { return int3(Max(a.X, b), Max(a.Y, b), Max(a.Z, b)); }
        public static int3 Max(int3 a, int3 b) { return int3(Max(a.X, b.X), Max(a.Y, b.Y), Max(a.Z, b.Z)); }
        public static int4 Max(int4 a, int b) { return int4(Max(a.X, b), Max(a.Y, b), Max(a.Z, b), Max(a.W, b)); }
        public static int4 Max(int4 a, int4 b) { return int4(Max(a.X, b.X), Max(a.Y, b.Y), Max(a.Z, b.Z), Max(a.W, b.W)); }

        public static double Min(double a, double b) { return a < b ? a : b; }
        [GlslIntrinsic("min")] public static float Min(float a, float b) { return a < b ? a : b; }
        [GlslIntrinsic("min")] public static float2 Min(float2 a, float b) { return float2(Min(a.X, b), Min(a.Y, b)); }
        [GlslIntrinsic("min")] public static float2 Min(float2 a, float2 b) { return float2(Min(a.X, b.X), Min(a.Y, b.Y)); }
        [GlslIntrinsic("min")] public static float3 Min(float3 a, float b) { return float3(Min(a.X, b), Min(a.Y, b), Min(a.Z, b)); }
        [GlslIntrinsic("min")] public static float3 Min(float3 a, float3 b) { return float3(Min(a.X, b.X), Min(a.Y, b.Y), Min(a.Z, b.Z)); }
        [GlslIntrinsic("min")] public static float4 Min(float4 a, float b) { return float4(Min(a.X, b), Min(a.Y, b), Min(a.Z, b), Min(a.W, b)); }
        [GlslIntrinsic("min")] public static float4 Min(float4 a, float4 b) { return float4(Min(a.X, b.X), Min(a.Y, b.Y), Min(a.Z, b.Z), Min(a.W, b.W)); }
        public static int Min(int a, int b) { return a < b ? a : b; }
        public static int2 Min(int2 a, int b) { return int2(Min(a.X, b), Min(a.Y, b)); }
        public static int2 Min(int2 a, int2 b) { return int2(Min(a.X, b.X), Min(a.Y, b.Y)); }
        public static int3 Min(int3 a, int b) { return int3(Min(a.X, b), Min(a.Y, b), Min(a.Z, b)); }
        public static int3 Min(int3 a, int3 b) { return int3(Min(a.X, b.X), Min(a.Y, b.Y), Min(a.Z, b.Z)); }
        public static int4 Min(int4 a, int b) { return int4(Min(a.X, b), Min(a.Y, b), Min(a.Z, b), Min(a.W, b)); }
        public static int4 Min(int4 a, int4 b) { return int4(Min(a.X, b.X), Min(a.Y, b.Y), Min(a.Z, b.Z), Min(a.W, b.W)); }

        public static double Clamp(double x, double minimum, double maximum) { return Max(Min(x, maximum), minimum); }
        [GlslIntrinsic("clamp")] public static float Clamp(float x, float minimum, float maximum) { return Max(Min(x, maximum), minimum); }
        [GlslIntrinsic("clamp")] public static float2 Clamp(float2 x, float minimum, float maximum) { return Max(Min(x, maximum), minimum); }
        [GlslIntrinsic("clamp")] public static float2 Clamp(float2 x, float2 minimum, float2 maximum) { return Max(Min(x, maximum), minimum); }
        [GlslIntrinsic("clamp")] public static float3 Clamp(float3 x, float minimum, float maximum) { return Max(Min(x, maximum), minimum); }
        [GlslIntrinsic("clamp")] public static float3 Clamp(float3 x, float3 minimum, float3 maximum) { return Max(Min(x, maximum), minimum); }
        [GlslIntrinsic("clamp")] public static float4 Clamp(float4 x, float minimum, float maximum) { return Max(Min(x, maximum), minimum); }
        [GlslIntrinsic("clamp")] public static float4 Clamp(float4 x, float4 minimum, float4 maximum) { return Max(Min(x, maximum), minimum); }
        public static int Clamp(int x, int minimum, int maximum) { return Max(Min(x, maximum), minimum); }
        public static int2 Clamp(int2 x, int minimum, int maximum) { return Max(Min(x, maximum), minimum); }
        public static int2 Clamp(int2 x, int2 minimum, int2 maximum) { return Max(Min(x, maximum), minimum); }
        public static int3 Clamp(int3 x, int minimum, int maximum) { return Max(Min(x, maximum), minimum); }
        public static int3 Clamp(int3 x, int3 minimum, int3 maximum) { return Max(Min(x, maximum), minimum); }
        public static int4 Clamp(int4 x, int minimum, int maximum) { return Max(Min(x, maximum), minimum); }
        public static int4 Clamp(int4 x, int4 minimum, int4 maximum) { return Max(Min(x, maximum), minimum); }

        public static double Lerp(double a, double b, double t) { return a + (b - a) * t; }
        [GlslIntrinsic("mix")] public static float Lerp(float a, float b, float t) { return a + (b - a) * t; }
        [GlslIntrinsic("mix")] public static float2 Lerp(float2 a, float2 b, float t) { return a + (b - a) * t; }
        [GlslIntrinsic("mix")] public static float2 Lerp(float2 a, float2 b, float2 t) { return a + (b - a) * t; }
        [GlslIntrinsic("mix")] public static float3 Lerp(float3 a, float3 b, float t) { return a + (b - a) * t; }
        [GlslIntrinsic("mix")] public static float3 Lerp(float3 a, float3 b, float3 t) { return a + (b - a) * t; }
        [GlslIntrinsic("mix")] public static float4 Lerp(float4 a, float4 b, float t) { return a + (b - a) * t; }
        [GlslIntrinsic("mix")] public static float4 Lerp(float4 a, float4 b, float4 t) { return a + (b - a) * t; }

        public static double Step(double edge, double x) { return x < edge ? 0.0 : 1.0; }
        [GlslIntrinsic("step")] public static float Step(float edge, float x) { return x < edge ? 0.0f : 1.0f; }
        [GlslIntrinsic("step")] public static float2 Step(float edge, float2 x) { return float2(Step(edge, x.X), Step(edge, x.Y)); }
        [GlslIntrinsic("step")] public static float2 Step(float2 edge, float2 x) { return float2(Step(edge.X, x.X), Step(edge.Y, x.Y)); }
        [GlslIntrinsic("step")] public static float3 Step(float edge, float3 x) { return float3(Step(edge, x.X), Step(edge, x.Y), Step(edge, x.Z)); }
        [GlslIntrinsic("step")] public static float3 Step(float3 edge, float3 x) { return float3(Step(edge.X, x.X), Step(edge.Y, x.Y), Step(edge.Z, x.Z)); }
        [GlslIntrinsic("step")] public static float4 Step(float edge, float4 x) { return float4(Step(edge, x.X), Step(edge, x.Y), Step(edge, x.Z), Step(edge, x.W)); }
        [GlslIntrinsic("step")] public static float4 Step(float4 edge, float4 x) { return float4(Step(edge.X, x.X), Step(edge.Y, x.Y), Step(edge.Z, x.Z), Step(edge.W, x.W)); }

        public static double SmoothStep(double edge0, double edge1, double x)
        {
            double t = Clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
            return t * t * (3.0 - 2.0 * t);
        }

        [GlslIntrinsic("smoothstep")]
        public static float SmoothStep(float edge0, float edge1, float x)
        {
            float t = Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
            return t * t * (3.0f - 2.0f * t);
        }

        [GlslIntrinsic("smoothstep")] public static float2 SmoothStep(float edge0, float edge1, float2 x) { return float2(SmoothStep(edge0, edge1, x.X), SmoothStep(edge0, edge1, x.Y)); }
        [GlslIntrinsic("smoothstep")] public static float2 SmoothStep(float2 edge0, float2 edge1, float2 x) { return float2(SmoothStep(edge0.X, edge1.X, x.X), SmoothStep(edge0.Y, edge1.Y, x.Y)); }
        [GlslIntrinsic("smoothstep")] public static float3 SmoothStep(float edge0, float edge1, float3 x) { return float3(SmoothStep(edge0, edge1, x.X), SmoothStep(edge0, edge1, x.Y), SmoothStep(edge0, edge1, x.Z)); }
        [GlslIntrinsic("smoothstep")] public static float3 SmoothStep(float3 edge0, float3 edge1, float3 x) { return float3(SmoothStep(edge0.X, edge1.X, x.X), SmoothStep(edge0.Y, edge1.Y, x.Y), SmoothStep(edge0.Z, edge1.Z, x.Z)); }
        [GlslIntrinsic("smoothstep")] public static float4 SmoothStep(float edge0, float edge1, float4 x) { return float4(SmoothStep(edge0, edge1, x.X), SmoothStep(edge0, edge1, x.Y), SmoothStep(edge0, edge1, x.Z), SmoothStep(edge0, edge1, x.W)); }
        [GlslIntrinsic("smoothstep")] public static float4 SmoothStep(float4 edge0, float4 edge1, float4 x) { return float4(SmoothStep(edge0.X, edge1.X, x.X), SmoothStep(edge0.Y, edge1.Y, x.Y), SmoothStep(edge0.Z, edge1.Z, x.Z), SmoothStep(edge0.W, edge1.W, x.W)); }


        // Pixel shader partial derivative functions

        [GlslIntrinsic("dFdx"), RequireShaderStage(ShaderStage.Pixel), DontExport] public static extern float Ddx(float x);
        [GlslIntrinsic("dFdx"), RequireShaderStage(ShaderStage.Pixel), DontExport] public static float2 Ddx(float2 x) { return float2(Ddx(x.X), Ddx(x.Y)); }
        [GlslIntrinsic("dFdx"), RequireShaderStage(ShaderStage.Pixel), DontExport] public static float3 Ddx(float3 x) { return float3(Ddx(x.X), Ddx(x.Y), Ddx(x.Z)); }
        [GlslIntrinsic("dFdx"), RequireShaderStage(ShaderStage.Pixel), DontExport] public static float4 Ddx(float4 x) { return float4(Ddx(x.X), Ddx(x.Y), Ddx(x.Z), Ddx(x.W)); }

        [GlslIntrinsic("dFdy"), RequireShaderStage(ShaderStage.Pixel), DontExport] public static extern float Ddy(float x);
        [GlslIntrinsic("dFdy"), RequireShaderStage(ShaderStage.Pixel), DontExport] public static float2 Ddy(float2 x) { return float2(Ddy(x.X), Ddy(x.Y)); }
        [GlslIntrinsic("dFdy"), RequireShaderStage(ShaderStage.Pixel), DontExport] public static float3 Ddy(float3 x) { return float3(Ddy(x.X), Ddy(x.Y), Ddy(x.Z)); }
        [GlslIntrinsic("dFdy"), RequireShaderStage(ShaderStage.Pixel), DontExport] public static float4 Ddy(float4 x) { return float4(Ddy(x.X), Ddy(x.Y), Ddy(x.Z), Ddy(x.W)); }

        [GlslIntrinsic("fwidth"), RequireShaderStage(ShaderStage.Pixel), DontExport] public static float Fwidth(float x) { return Abs(Ddx(x)) + Abs(Ddy(x)); }
        [GlslIntrinsic("fwidth"), RequireShaderStage(ShaderStage.Pixel), DontExport] public static float2 Fwidth(float2 x) { return Abs(Ddx(x)) + Abs(Ddy(x)); }
        [GlslIntrinsic("fwidth"), RequireShaderStage(ShaderStage.Pixel), DontExport] public static float3 Fwidth(float3 x) { return Abs(Ddx(x)) + Abs(Ddy(x)); }
        [GlslIntrinsic("fwidth"), RequireShaderStage(ShaderStage.Pixel), DontExport] public static float4 Fwidth(float4 x) { return Abs(Ddx(x)) + Abs(Ddy(x)); }


        // Non-GLSL functions

        public static double Saturate(double x) { return Clamp(x, 0.0, 1.0); }
        public static float Saturate(float x) { return Clamp(x, 0.0f, 1.0f); }
        public static float2 Saturate(float2 x) { return Clamp(x, 0.0f, 1.0f); }
        public static float3 Saturate(float3 x) { return Clamp(x, 0.0f, 1.0f); }
        public static float4 Saturate(float4 x) { return Clamp(x, 0.0f, 1.0f); }

        public static double Round(double x) { return Floor(x + 0.5); }

        [Obsolete("Use Floor(x + 0.5f) instead")]
        public static float Round(float x) { return Floor(x + 0.5f); }

        [Obsolete("Use Floor(x + 0.5f) instead")]
        public static float2 Round(float2 x) { return Floor(x + 0.5f); }

        [Obsolete("Use Floor(x + 0.5f) instead")]
        public static float3 Round(float3 x) { return Floor(x + 0.5f); }

        [Obsolete("Use Floor(x + 0.5f) instead")]
        public static float4 Round(float4 x) { return Floor(x + 0.5f); }

        [Obsolete("Use (float)Floor((double)d, decimals) instead")]
        public static float Round(float d, int decimals)
        {
            return (float)Round((double)d, decimals);
        }

        static double[] positivePowersOfTen = new double[]
        {
            1e0, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8,
            1e9, 1e10, 1e11, 1e12, 1e13, 1e14, 1e15
        };

        static double[] negativePowersOfTen = new double[]
        {
            1e0, 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6, 1e-7, 1e-8,
            1e-9, 1e-10, 1e-11, 1e-12, 1e-13, 1e-14, 1e-15
        };

        public static double Round(double d, int digits)
        {
            if (digits < 0 || digits > 15)
                throw new ArgumentOutOfRangeException(nameof(digits));

            if (Abs(d) < 1e16)
                return Round(d * positivePowersOfTen[digits]) * negativePowersOfTen[digits];
            else
                return d;
        }

        [Obsolete("Use float2((float)Floor((double)x.X, decimals), (float)Floor((double)x.Y, decimals)) instead")]
        public static float2 Round(float2 x, int decimals)
        {
            return float2(Round(x.X, decimals), Round(x.Y, decimals));
        }

        [Obsolete("Use float3((float)Floor((double)x.X, decimals), (float)Floor((double)x.Y, decimals), (float)Floor((double)x.Z, decimals)) instead")]
        public static float3 Round(float3 x, int decimals)
        {
            return float3(Round(x.X, decimals), Round(x.Y, decimals), Round(x.Z, decimals));
        }

        [Obsolete("Use float4((float)Floor((double)x.X, decimals), (float)Floor((double)x.Y, decimals), (float)Floor((double)x.Z, decimals), (float)Floor((double)x.W, decimals)) instead")]
        public static float4 Round(float4 x, int decimals)
        {
            return float4(Round(x.X, decimals), Round(x.Y, decimals), Round(x.Z, decimals), Round(x.W, decimals));
        }


        // Component functions

        public static float ComponentMax(float2 x) { return Max(x.X, x.Y); }
        public static float ComponentMax(float3 x) { return Max(Max(x.X, x.Y), x.Z); }
        public static float ComponentMax(float4 x) { return Max(Max(Max(x.X, x.Y), x.Z), x.W); }
        public static int ComponentMax(int2 x) { return Max(x.X, x.Y); }
        public static int ComponentMax(int3 x) { return Max(Max(x.X, x.Y), x.Z); }
        public static int ComponentMax(int4 x) { return Max(Max(Max(x.X, x.Y), x.Z), x.W); }

        public static float ComponentMin(float2 x) { return Min(x.X, x.Y); }
        public static float ComponentMin(float3 x) { return Min(Min(x.X, x.Y), x.Z); }
        public static float ComponentMin(float4 x) { return Min(Min(Min(x.X, x.Y), x.Z), x.W); }
        public static int ComponentMin(int2 x) { return Min(x.X, x.Y); }
        public static int ComponentMin(int3 x) { return Min(Min(x.X, x.Y), x.Z); }
        public static int ComponentMin(int4 x) { return Min(Min(Min(x.X, x.Y), x.Z), x.W); }

        public static float ComponentSum(float2 x) { return x.X + x.Y; }
        public static float ComponentSum(float3 x) { return x.X + x.Y + x.Z; }
        public static float ComponentSum(float4 x) { return x.X + x.Y + x.Z + x.W; }
        public static int ComponentSum(int2 x) { return x.X + x.Y; }
        public static int ComponentSum(int3 x) { return x.X + x.Y + x.Z; }
        public static int ComponentSum(int4 x) { return x.X + x.Y + x.Z + x.W; }


        // Integer functions

        public static int NextPow2(int x)
        {
            int y = x - 1;
            y = y | (y >> 1);
            y = y | (y >> 2);
            y = y | (y >> 4);
            y = y | (y >> 8);
            y = y | (y >> 16);
            return y + 1;
        }

        public static bool IsPow2(int x)
        {
            return (x == (x & -x));
        }
    }
}
