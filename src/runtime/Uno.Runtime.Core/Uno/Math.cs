// This file was generated based on Library/Core/UnoCore/Source/Uno/Math.uno.
// WARNING: Changes might be lost if you edit this file directly.

namespace Uno
{
    [global::Uno.Compiler.ExportTargetInterop.DotNetTypeAttribute(null)]
    public static class Math
    {
        public static double[] positivePowersOfTen;
        public static double[] negativePowersOfTen;

        static Math()
        {
            Math.positivePowersOfTen = new double[] { 1.0, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, 10000000.0, 100000000.0, 1000000000.0, 10000000000.0, 100000000000.0, 1000000000000.0, 10000000000000.0, 100000000000000.0, 1e+15};
            Math.negativePowersOfTen = new double[] { 1.0, 0.1, 0.01, 0.001, 0.0001, 1e-05, 1e-06, 1e-07, 1e-08, 1e-09, 1e-10, 1e-11, 1e-12, 1e-13, 1e-14, 1e-15};
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * 0.017453292519943295;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("radians")]
        public static float DegreesToRadians(float degrees)
        {
            return degrees * 0.0174532924f;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("radians")]
        public static Float2 DegreesToRadians(Float2 degrees)
        {
            return degrees * 0.0174532924f;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("radians")]
        public static Float3 DegreesToRadians(Float3 degrees)
        {
            return degrees * 0.0174532924f;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("radians")]
        public static Float4 DegreesToRadians(Float4 degrees)
        {
            return degrees * 0.0174532924f;
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * 57.295779513082323;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("degrees")]
        public static float RadiansToDegrees(float radians)
        {
            return radians * 57.2957764f;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("degrees")]
        public static Float2 RadiansToDegrees(Float2 radians)
        {
            return radians * 57.2957764f;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("degrees")]
        public static Float3 RadiansToDegrees(Float3 radians)
        {
            return radians * 57.2957764f;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("degrees")]
        public static Float4 RadiansToDegrees(Float4 radians)
        {
            return radians * 57.2957764f;
        }

        public static double Sin(double radians)
        {
            return global::System.Math.Sin(radians);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("sin")]
        public static float Sin(float radians)
        {
            return (float)global::System.Math.Sin((double)radians);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("sin")]
        public static Float2 Sin(Float2 radians)
        {
            return new Float2(Math.Sin(radians.X), Math.Sin(radians.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("sin")]
        public static Float3 Sin(Float3 radians)
        {
            return new Float3(Math.Sin(radians.X), Math.Sin(radians.Y), Math.Sin(radians.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("sin")]
        public static Float4 Sin(Float4 radians)
        {
            return new Float4(Math.Sin(radians.X), Math.Sin(radians.Y), Math.Sin(radians.Z), Math.Sin(radians.W));
        }

        public static double Cos(double radians)
        {
            return global::System.Math.Cos(radians);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("cos")]
        public static float Cos(float radians)
        {
            return (float)global::System.Math.Cos((double)radians);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("cos")]
        public static Float2 Cos(Float2 radians)
        {
            return new Float2(Math.Cos(radians.X), Math.Cos(radians.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("cos")]
        public static Float3 Cos(Float3 radians)
        {
            return new Float3(Math.Cos(radians.X), Math.Cos(radians.Y), Math.Cos(radians.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("cos")]
        public static Float4 Cos(Float4 radians)
        {
            return new Float4(Math.Cos(radians.X), Math.Cos(radians.Y), Math.Cos(radians.Z), Math.Cos(radians.W));
        }

        public static double Tan(double radians)
        {
            return global::System.Math.Tan(radians);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("tan")]
        public static float Tan(float radians)
        {
            return (float)global::System.Math.Tan((double)radians);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("tan")]
        public static Float2 Tan(Float2 radians)
        {
            return new Float2(Math.Tan(radians.X), Math.Tan(radians.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("tan")]
        public static Float3 Tan(Float3 radians)
        {
            return new Float3(Math.Tan(radians.X), Math.Tan(radians.Y), Math.Tan(radians.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("tan")]
        public static Float4 Tan(Float4 radians)
        {
            return new Float4(Math.Tan(radians.X), Math.Tan(radians.Y), Math.Tan(radians.Z), Math.Tan(radians.W));
        }

        public static double Asin(double radians)
        {
            return global::System.Math.Asin(radians);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("asin")]
        public static float Asin(float radians)
        {
            return (float)global::System.Math.Asin((double)radians);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("asin")]
        public static Float2 Asin(Float2 radians)
        {
            return new Float2(Math.Asin(radians.X), Math.Asin(radians.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("asin")]
        public static Float3 Asin(Float3 radians)
        {
            return new Float3(Math.Asin(radians.X), Math.Asin(radians.Y), Math.Asin(radians.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("asin")]
        public static Float4 Asin(Float4 radians)
        {
            return new Float4(Math.Asin(radians.X), Math.Asin(radians.Y), Math.Asin(radians.Z), Math.Asin(radians.W));
        }

        public static double Acos(double radians)
        {
            return global::System.Math.Acos(radians);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("acos")]
        public static float Acos(float radians)
        {
            return (float)global::System.Math.Acos((double)radians);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("acos")]
        public static Float2 Acos(Float2 radians)
        {
            return new Float2(Math.Acos(radians.X), Math.Acos(radians.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("acos")]
        public static Float3 Acos(Float3 radians)
        {
            return new Float3(Math.Acos(radians.X), Math.Acos(radians.Y), Math.Acos(radians.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("acos")]
        public static Float4 Acos(Float4 radians)
        {
            return new Float4(Math.Acos(radians.X), Math.Acos(radians.Y), Math.Acos(radians.Z), Math.Acos(radians.W));
        }

        public static double Atan(double radians)
        {
            return global::System.Math.Atan(radians);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("atan")]
        public static float Atan(float radians)
        {
            return (float)global::System.Math.Atan((double)radians);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("atan")]
        public static Float2 Atan(Float2 radians)
        {
            return new Float2(Math.Atan(radians.X), Math.Atan(radians.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("atan")]
        public static Float3 Atan(Float3 radians)
        {
            return new Float3(Math.Atan(radians.X), Math.Atan(radians.Y), Math.Atan(radians.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("atan")]
        public static Float4 Atan(Float4 radians)
        {
            return new Float4(Math.Atan(radians.X), Math.Atan(radians.Y), Math.Atan(radians.Z), Math.Atan(radians.W));
        }

        public static double Atan2(double y, double x)
        {
            return global::System.Math.Atan2(y, x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("atan")]
        public static float Atan2(float y, float x)
        {
            return (float)global::System.Math.Atan2((double)y, (double)x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("atan")]
        public static Float2 Atan2(Float2 y, Float2 x)
        {
            return new Float2(Math.Atan2(y.X, x.X), Math.Atan2(y.Y, x.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("atan")]
        public static Float3 Atan2(Float3 y, Float3 x)
        {
            return new Float3(Math.Atan2(y.X, x.X), Math.Atan2(y.Y, x.Y), Math.Atan2(y.Z, x.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("atan")]
        public static Float4 Atan2(Float4 y, Float4 x)
        {
            return new Float4(Math.Atan2(y.X, x.X), Math.Atan2(y.Y, x.Y), Math.Atan2(y.Z, x.Z), Math.Atan2(y.W, x.W));
        }

        public static double Pow(double x, double y)
        {
            return global::System.Math.Pow(x, y);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("pow")]
        public static float Pow(float x, float y)
        {
            return (float)global::System.Math.Pow((double)x, (double)y);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("pow")]
        public static Float2 Pow(Float2 x, Float2 y)
        {
            return new Float2(Math.Pow(x.X, y.X), Math.Pow(x.Y, y.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("pow")]
        public static Float3 Pow(Float3 x, Float3 y)
        {
            return new Float3(Math.Pow(x.X, y.X), Math.Pow(x.Y, y.Y), Math.Pow(x.Z, y.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("pow")]
        public static Float4 Pow(Float4 x, Float4 y)
        {
            return new Float4(Math.Pow(x.X, y.X), Math.Pow(x.Y, y.Y), Math.Pow(x.Z, y.Z), Math.Pow(x.W, y.W));
        }

        public static double Exp(double x)
        {
            return Math.Pow(2.7182818284590451, x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("exp")]
        public static float Exp(float x)
        {
            return Math.Pow(2.71828175f, x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("exp")]
        public static Float2 Exp(Float2 x)
        {
            return new Float2(Math.Exp(x.X), Math.Exp(x.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("exp")]
        public static Float3 Exp(Float3 x)
        {
            return new Float3(Math.Exp(x.X), Math.Exp(x.Y), Math.Exp(x.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("exp")]
        public static Float4 Exp(Float4 x)
        {
            return new Float4(Math.Exp(x.X), Math.Exp(x.Y), Math.Exp(x.Z), Math.Exp(x.W));
        }

        public static double Log(double x)
        {
            return global::System.Math.Log(x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("log")]
        public static float Log(float x)
        {
            return (float)global::System.Math.Log((double)x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("log")]
        public static Float2 Log(Float2 x)
        {
            return new Float2(Math.Log(x.X), Math.Log(x.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("log")]
        public static Float3 Log(Float3 x)
        {
            return new Float3(Math.Log(x.X), Math.Log(x.Y), Math.Log(x.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("log")]
        public static Float4 Log(Float4 x)
        {
            return new Float4(Math.Log(x.X), Math.Log(x.Y), Math.Log(x.Z), Math.Log(x.W));
        }

        public static double Log10(double x)
        {
            return global::System.Math.Log10(x);
        }

        public static double Exp2(double x)
        {
            return Math.Pow(2.0, x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("exp2")]
        public static float Exp2(float x)
        {
            return Math.Pow(2.0f, x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("exp2")]
        public static Float2 Exp2(Float2 x)
        {
            return new Float2(Math.Exp2(x.X), Math.Exp2(x.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("exp2")]
        public static Float3 Exp2(Float3 x)
        {
            return new Float3(Math.Exp2(x.X), Math.Exp2(x.Y), Math.Exp2(x.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("exp2")]
        public static Float4 Exp2(Float4 x)
        {
            return new Float4(Math.Exp2(x.X), Math.Exp2(x.Y), Math.Exp2(x.Z), Math.Exp2(x.W));
        }

        public static double Log2(double x)
        {
            return Math.Log(x) / Math.Log(2.0);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("log2")]
        public static float Log2(float x)
        {
            return Math.Log(x) / Math.Log(2.0f);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("log2")]
        public static Float2 Log2(Float2 x)
        {
            return new Float2(Math.Log2(x.X), Math.Log2(x.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("log2")]
        public static Float3 Log2(Float3 x)
        {
            return new Float3(Math.Log2(x.X), Math.Log2(x.Y), Math.Log2(x.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("log2")]
        public static Float4 Log2(Float4 x)
        {
            return new Float4(Math.Log2(x.X), Math.Log2(x.Y), Math.Log2(x.Z), Math.Log2(x.W));
        }

        public static double Sqrt(double x)
        {
            return global::System.Math.Sqrt(x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("sqrt")]
        public static float Sqrt(float x)
        {
            return (float)global::System.Math.Sqrt((double)x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("sqrt")]
        public static Float2 Sqrt(Float2 x)
        {
            return new Float2(Math.Sqrt(x.X), Math.Sqrt(x.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("sqrt")]
        public static Float3 Sqrt(Float3 x)
        {
            return new Float3(Math.Sqrt(x.X), Math.Sqrt(x.Y), Math.Sqrt(x.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("sqrt")]
        public static Float4 Sqrt(Float4 x)
        {
            return new Float4(Math.Sqrt(x.X), Math.Sqrt(x.Y), Math.Sqrt(x.Z), Math.Sqrt(x.W));
        }

        public static double InverseSqrt(double x)
        {
            return 1.0 / Math.Sqrt(x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("inversesqrt")]
        public static float InverseSqrt(float x)
        {
            return 1.0f / Math.Sqrt(x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("inversesqrt")]
        public static Float2 InverseSqrt(Float2 x)
        {
            return new Float2(Math.InverseSqrt(x.X), Math.InverseSqrt(x.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("inversesqrt")]
        public static Float3 InverseSqrt(Float3 x)
        {
            return new Float3(Math.InverseSqrt(x.X), Math.InverseSqrt(x.Y), Math.InverseSqrt(x.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("inversesqrt")]
        public static Float4 InverseSqrt(Float4 x)
        {
            return new Float4(Math.InverseSqrt(x.X), Math.InverseSqrt(x.Y), Math.InverseSqrt(x.Z), Math.InverseSqrt(x.W));
        }

        public static double Abs(double x)
        {
            return (x >= 0.0) ? x : -x;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("abs")]
        public static float Abs(float x)
        {
            return (x >= 0.0f) ? x : -x;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("abs")]
        public static Float2 Abs(Float2 a)
        {
            return new Float2(Math.Abs(a.X), Math.Abs(a.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("abs")]
        public static Float3 Abs(Float3 a)
        {
            return new Float3(Math.Abs(a.X), Math.Abs(a.Y), Math.Abs(a.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("abs")]
        public static Float4 Abs(Float4 a)
        {
            return new Float4(Math.Abs(a.X), Math.Abs(a.Y), Math.Abs(a.Z), Math.Abs(a.W));
        }

        public static sbyte Abs(sbyte x)
        {
            if (x < 0)
            {
                if (x == -128)
                    throw new global::System.OverflowException("Negating the minimum value of a twos complement number is invalid.");

                return (sbyte)-x;
            }
            else
                return x;
        }

        public static short Abs(short x)
        {
            if (x < 0)
            {
                if (x == -32768)
                    throw new global::System.OverflowException("Negating the minimum value of a twos complement number is invalid.");

                return (short)-x;
            }
            else
                return x;
        }

        public static int Abs(int x)
        {
            if (x < 0)
            {
                if (x == -2147483648)
                    throw new global::System.OverflowException("Negating the minimum value of a twos complement number is invalid.");

                return -x;
            }
            else
                return x;
        }

        public static Int2 Abs(Int2 a)
        {
            return new Int2(Math.Abs(a.X), Math.Abs(a.Y));
        }

        public static Int3 Abs(Int3 a)
        {
            return new Int3(Math.Abs(a.X), Math.Abs(a.Y), Math.Abs(a.Z));
        }

        public static Int4 Abs(Int4 a)
        {
            return new Int4(Math.Abs(a.X), Math.Abs(a.Y), Math.Abs(a.Z), Math.Abs(a.W));
        }

        public static long Abs(long x)
        {
            if (x < 0)
            {
                if (x == -9223372036854775808)
                    throw new global::System.OverflowException("Negating the minimum value of a twos complement number is invalid.");

                return -x;
            }
            else
                return x;
        }

        public static double Sign(double x)
        {
            return (x < 0.0) ? -1.0 : ((x > 0.0) ? 1.0 : 0.0);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("sign")]
        public static float Sign(float x)
        {
            return (x < 0.0f) ? -1.0f : ((x > 0.0f) ? 1.0f : 0.0f);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("sign")]
        public static Float2 Sign(Float2 a)
        {
            return new Float2(Math.Sign(a.X), Math.Sign(a.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("sign")]
        public static Float3 Sign(Float3 a)
        {
            return new Float3(Math.Sign(a.X), Math.Sign(a.Y), Math.Sign(a.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("sign")]
        public static Float4 Sign(Float4 a)
        {
            return new Float4(Math.Sign(a.X), Math.Sign(a.Y), Math.Sign(a.Z), Math.Sign(a.W));
        }

        public static int Sign(int x)
        {
            return (x < 0) ? -1 : ((x > 0) ? 1 : 0);
        }

        public static Int2 Sign(Int2 a)
        {
            return new Int2(Math.Sign(a.X), Math.Sign(a.Y));
        }

        public static Int3 Sign(Int3 a)
        {
            return new Int3(Math.Sign(a.X), Math.Sign(a.Y), Math.Sign(a.Z));
        }

        public static Int4 Sign(Int4 a)
        {
            return new Int4(Math.Sign(a.X), Math.Sign(a.Y), Math.Sign(a.Z), Math.Sign(a.W));
        }

        public static double Floor(double x)
        {
            return global::System.Math.Floor(x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("floor")]
        public static float Floor(float x)
        {
            return (float)global::System.Math.Floor((double)x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("floor")]
        public static Float2 Floor(Float2 v)
        {
            return new Float2(Math.Floor(v.X), Math.Floor(v.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("floor")]
        public static Float3 Floor(Float3 v)
        {
            return new Float3(Math.Floor(v.X), Math.Floor(v.Y), Math.Floor(v.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("floor")]
        public static Float4 Floor(Float4 v)
        {
            return new Float4(Math.Floor(v.X), Math.Floor(v.Y), Math.Floor(v.Z), Math.Floor(v.W));
        }

        public static double Ceil(double x)
        {
            return global::System.Math.Ceiling(x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("ceil")]
        public static float Ceil(float x)
        {
            return (float)global::System.Math.Ceiling((double)x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("ceil")]
        public static Float2 Ceil(Float2 v)
        {
            return new Float2(Math.Ceil(v.X), Math.Ceil(v.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("ceil")]
        public static Float3 Ceil(Float3 v)
        {
            return new Float3(Math.Ceil(v.X), Math.Ceil(v.Y), Math.Ceil(v.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("ceil")]
        public static Float4 Ceil(Float4 v)
        {
            return new Float4(Math.Ceil(v.X), Math.Ceil(v.Y), Math.Ceil(v.Z), Math.Ceil(v.W));
        }

        public static double Fract(double x)
        {
            return x - Math.Floor(x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("fract")]
        public static float Fract(float x)
        {
            return x - Math.Floor(x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("fract")]
        public static Float2 Fract(Float2 x)
        {
            return x - Math.Floor(x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("fract")]
        public static Float3 Fract(Float3 x)
        {
            return x - Math.Floor(x);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("fract")]
        public static Float4 Fract(Float4 x)
        {
            return x - Math.Floor(x);
        }

        public static double Mod(double x, double y)
        {
            return x - (y * Math.Floor(x / y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mod")]
        public static float Mod(float x, float y)
        {
            return x - (y * Math.Floor(x / y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mod")]
        public static Float2 Mod(Float2 x, float y)
        {
            return x - (y * Math.Floor(x / y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mod")]
        public static Float2 Mod(Float2 x, Float2 y)
        {
            return x - (y * Math.Floor(x / y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mod")]
        public static Float3 Mod(Float3 x, float y)
        {
            return x - (y * Math.Floor(x / y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mod")]
        public static Float3 Mod(Float3 x, Float3 y)
        {
            return x - (y * Math.Floor(x / y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mod")]
        public static Float4 Mod(Float4 x, float y)
        {
            return x - (y * Math.Floor(x / y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mod")]
        public static Float4 Mod(Float4 x, Float4 y)
        {
            return x - (y * Math.Floor(x / y));
        }

        public static double Max(double a, double b)
        {
            return (a > b) ? a : b;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("max")]
        public static float Max(float a, float b)
        {
            return (a > b) ? a : b;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("max")]
        public static Float2 Max(Float2 a, float b)
        {
            return new Float2(Math.Max(a.X, b), Math.Max(a.Y, b));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("max")]
        public static Float2 Max(Float2 a, Float2 b)
        {
            return new Float2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("max")]
        public static Float3 Max(Float3 a, float b)
        {
            return new Float3(Math.Max(a.X, b), Math.Max(a.Y, b), Math.Max(a.Z, b));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("max")]
        public static Float3 Max(Float3 a, Float3 b)
        {
            return new Float3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("max")]
        public static Float4 Max(Float4 a, float b)
        {
            return new Float4(Math.Max(a.X, b), Math.Max(a.Y, b), Math.Max(a.Z, b), Math.Max(a.W, b));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("max")]
        public static Float4 Max(Float4 a, Float4 b)
        {
            return new Float4(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z), Math.Max(a.W, b.W));
        }

        public static int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        public static Int2 Max(Int2 a, int b)
        {
            return new Int2(Math.Max(a.X, b), Math.Max(a.Y, b));
        }

        public static Int2 Max(Int2 a, Int2 b)
        {
            return new Int2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }

        public static Int3 Max(Int3 a, int b)
        {
            return new Int3(Math.Max(a.X, b), Math.Max(a.Y, b), Math.Max(a.Z, b));
        }

        public static Int3 Max(Int3 a, Int3 b)
        {
            return new Int3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        }

        public static Int4 Max(Int4 a, int b)
        {
            return new Int4(Math.Max(a.X, b), Math.Max(a.Y, b), Math.Max(a.Z, b), Math.Max(a.W, b));
        }

        public static Int4 Max(Int4 a, Int4 b)
        {
            return new Int4(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z), Math.Max(a.W, b.W));
        }

        public static double Min(double a, double b)
        {
            return (a < b) ? a : b;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("min")]
        public static float Min(float a, float b)
        {
            return (a < b) ? a : b;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("min")]
        public static Float2 Min(Float2 a, float b)
        {
            return new Float2(Math.Min(a.X, b), Math.Min(a.Y, b));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("min")]
        public static Float2 Min(Float2 a, Float2 b)
        {
            return new Float2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("min")]
        public static Float3 Min(Float3 a, float b)
        {
            return new Float3(Math.Min(a.X, b), Math.Min(a.Y, b), Math.Min(a.Z, b));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("min")]
        public static Float3 Min(Float3 a, Float3 b)
        {
            return new Float3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("min")]
        public static Float4 Min(Float4 a, float b)
        {
            return new Float4(Math.Min(a.X, b), Math.Min(a.Y, b), Math.Min(a.Z, b), Math.Min(a.W, b));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("min")]
        public static Float4 Min(Float4 a, Float4 b)
        {
            return new Float4(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z), Math.Min(a.W, b.W));
        }

        public static int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        public static Int2 Min(Int2 a, int b)
        {
            return new Int2(Math.Min(a.X, b), Math.Min(a.Y, b));
        }

        public static Int2 Min(Int2 a, Int2 b)
        {
            return new Int2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }

        public static Int3 Min(Int3 a, int b)
        {
            return new Int3(Math.Min(a.X, b), Math.Min(a.Y, b), Math.Min(a.Z, b));
        }

        public static Int3 Min(Int3 a, Int3 b)
        {
            return new Int3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        }

        public static Int4 Min(Int4 a, int b)
        {
            return new Int4(Math.Min(a.X, b), Math.Min(a.Y, b), Math.Min(a.Z, b), Math.Min(a.W, b));
        }

        public static Int4 Min(Int4 a, Int4 b)
        {
            return new Int4(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z), Math.Min(a.W, b.W));
        }

        public static double Clamp(double x, double minimum, double maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("clamp")]
        public static float Clamp(float x, float minimum, float maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("clamp")]
        public static Float2 Clamp(Float2 x, float minimum, float maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("clamp")]
        public static Float2 Clamp(Float2 x, Float2 minimum, Float2 maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("clamp")]
        public static Float3 Clamp(Float3 x, float minimum, float maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("clamp")]
        public static Float3 Clamp(Float3 x, Float3 minimum, Float3 maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("clamp")]
        public static Float4 Clamp(Float4 x, float minimum, float maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("clamp")]
        public static Float4 Clamp(Float4 x, Float4 minimum, Float4 maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        public static int Clamp(int x, int minimum, int maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        public static Int2 Clamp(Int2 x, int minimum, int maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        public static Int2 Clamp(Int2 x, Int2 minimum, Int2 maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        public static Int3 Clamp(Int3 x, int minimum, int maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        public static Int3 Clamp(Int3 x, Int3 minimum, Int3 maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        public static Int4 Clamp(Int4 x, int minimum, int maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        public static Int4 Clamp(Int4 x, Int4 minimum, Int4 maximum)
        {
            return Math.Max(Math.Min(x, maximum), minimum);
        }

        public static double Lerp(double a, double b, double t)
        {
            return a + ((b - a) * t);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mix")]
        public static float Lerp(float a, float b, float t)
        {
            return a + ((b - a) * t);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mix")]
        public static Float2 Lerp(Float2 a, Float2 b, float t)
        {
            return a + ((b - a) * t);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mix")]
        public static Float2 Lerp(Float2 a, Float2 b, Float2 t)
        {
            return a + ((b - a) * t);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mix")]
        public static Float3 Lerp(Float3 a, Float3 b, float t)
        {
            return a + ((b - a) * t);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mix")]
        public static Float3 Lerp(Float3 a, Float3 b, Float3 t)
        {
            return a + ((b - a) * t);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mix")]
        public static Float4 Lerp(Float4 a, Float4 b, float t)
        {
            return a + ((b - a) * t);
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("mix")]
        public static Float4 Lerp(Float4 a, Float4 b, Float4 t)
        {
            return a + ((b - a) * t);
        }

        public static double Step(double edge, double x)
        {
            return (x < edge) ? 0.0 : 1.0;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("step")]
        public static float Step(float edge, float x)
        {
            return (x < edge) ? 0.0f : 1.0f;
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("step")]
        public static Float2 Step(float edge, Float2 x)
        {
            return new Float2(Math.Step(edge, x.X), Math.Step(edge, x.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("step")]
        public static Float2 Step(Float2 edge, Float2 x)
        {
            return new Float2(Math.Step(edge.X, x.X), Math.Step(edge.Y, x.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("step")]
        public static Float3 Step(float edge, Float3 x)
        {
            return new Float3(Math.Step(edge, x.X), Math.Step(edge, x.Y), Math.Step(edge, x.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("step")]
        public static Float3 Step(Float3 edge, Float3 x)
        {
            return new Float3(Math.Step(edge.X, x.X), Math.Step(edge.Y, x.Y), Math.Step(edge.Z, x.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("step")]
        public static Float4 Step(float edge, Float4 x)
        {
            return new Float4(Math.Step(edge, x.X), Math.Step(edge, x.Y), Math.Step(edge, x.Z), Math.Step(edge, x.W));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("step")]
        public static Float4 Step(Float4 edge, Float4 x)
        {
            return new Float4(Math.Step(edge.X, x.X), Math.Step(edge.Y, x.Y), Math.Step(edge.Z, x.Z), Math.Step(edge.W, x.W));
        }

        public static double SmoothStep(double edge0, double edge1, double x)
        {
            double t = Math.Clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
            return (t * t) * (3.0 - (2.0 * t));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("smoothstep")]
        public static float SmoothStep(float edge0, float edge1, float x)
        {
            float t = Math.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
            return (t * t) * (3.0f - (2.0f * t));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("smoothstep")]
        public static Float2 SmoothStep(float edge0, float edge1, Float2 x)
        {
            return new Float2(Math.SmoothStep(edge0, edge1, x.X), Math.SmoothStep(edge0, edge1, x.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("smoothstep")]
        public static Float2 SmoothStep(Float2 edge0, Float2 edge1, Float2 x)
        {
            return new Float2(Math.SmoothStep(edge0.X, edge1.X, x.X), Math.SmoothStep(edge0.Y, edge1.Y, x.Y));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("smoothstep")]
        public static Float3 SmoothStep(float edge0, float edge1, Float3 x)
        {
            return new Float3(Math.SmoothStep(edge0, edge1, x.X), Math.SmoothStep(edge0, edge1, x.Y), Math.SmoothStep(edge0, edge1, x.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("smoothstep")]
        public static Float3 SmoothStep(Float3 edge0, Float3 edge1, Float3 x)
        {
            return new Float3(Math.SmoothStep(edge0.X, edge1.X, x.X), Math.SmoothStep(edge0.Y, edge1.Y, x.Y), Math.SmoothStep(edge0.Z, edge1.Z, x.Z));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("smoothstep")]
        public static Float4 SmoothStep(float edge0, float edge1, Float4 x)
        {
            return new Float4(Math.SmoothStep(edge0, edge1, x.X), Math.SmoothStep(edge0, edge1, x.Y), Math.SmoothStep(edge0, edge1, x.Z), Math.SmoothStep(edge0, edge1, x.W));
        }

        [global::Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute("smoothstep")]
        public static Float4 SmoothStep(Float4 edge0, Float4 edge1, Float4 x)
        {
            return new Float4(Math.SmoothStep(edge0.X, edge1.X, x.X), Math.SmoothStep(edge0.Y, edge1.Y, x.Y), Math.SmoothStep(edge0.Z, edge1.Z, x.Z), Math.SmoothStep(edge0.W, edge1.W, x.W));
        }

        public static double Saturate(double x)
        {
            return Math.Clamp(x, 0.0, 1.0);
        }

        public static float Saturate(float x)
        {
            return Math.Clamp(x, 0.0f, 1.0f);
        }

        public static Float2 Saturate(Float2 x)
        {
            return Math.Clamp(x, 0.0f, 1.0f);
        }

        public static Float3 Saturate(Float3 x)
        {
            return Math.Clamp(x, 0.0f, 1.0f);
        }

        public static Float4 Saturate(Float4 x)
        {
            return Math.Clamp(x, 0.0f, 1.0f);
        }

        public static double Round(double x)
        {
            return Math.Floor(x + 0.5);
        }

        [global::System.ObsoleteAttribute("Use Floor(x + 0.5f) instead")]
        public static float Round(float x)
        {
            return Math.Floor(x + 0.5f);
        }

        [global::System.ObsoleteAttribute("Use Floor(x + 0.5f) instead")]
        public static Float2 Round(Float2 x)
        {
            return Math.Floor(x + 0.5f);
        }

        [global::System.ObsoleteAttribute("Use Floor(x + 0.5f) instead")]
        public static Float3 Round(Float3 x)
        {
            return Math.Floor(x + 0.5f);
        }

        [global::System.ObsoleteAttribute("Use Floor(x + 0.5f) instead")]
        public static Float4 Round(Float4 x)
        {
            return Math.Floor(x + 0.5f);
        }

        [global::System.ObsoleteAttribute("Use (float)Floor((double)d, decimals) instead")]
        public static float Round(float d, int decimals)
        {
            return (float)Math.Round((double)d, decimals);
        }

        public static double Round(double d, int digits)
        {
            if ((digits < 0) || (digits > 15))
                throw new global::System.ArgumentOutOfRangeException("digits");

            if (Math.Abs(d) < 1e+16)
                return Math.Round(d * Math.positivePowersOfTen[digits]) * Math.negativePowersOfTen[digits];
            else
                return d;
        }

        [global::System.ObsoleteAttribute("Use float2((float)Floor((double)x.X, decimals), (float)Floor((double)x.Y, decimals)) instead")]
        public static Float2 Round(Float2 x, int decimals)
        {
            return new Float2(Math.Round(x.X, decimals), Math.Round(x.Y, decimals));
        }

        [global::System.ObsoleteAttribute("Use float3((float)Floor((double)x.X, decimals), (float)Floor((double)x.Y, decimals), (float)Floor((double)x.Z, decimals)) instead")]
        public static Float3 Round(Float3 x, int decimals)
        {
            return new Float3(Math.Round(x.X, decimals), Math.Round(x.Y, decimals), Math.Round(x.Z, decimals));
        }

        [global::System.ObsoleteAttribute("Use float4((float)Floor((double)x.X, decimals), (float)Floor((double)x.Y, decimals), (float)Floor((double)x.Z, decimals), (float)Floor((double)x.W, decimals)) instead")]
        public static Float4 Round(Float4 x, int decimals)
        {
            return new Float4(Math.Round(x.X, decimals), Math.Round(x.Y, decimals), Math.Round(x.Z, decimals), Math.Round(x.W, decimals));
        }

        public static float ComponentMax(Float2 x)
        {
            return Math.Max(x.X, x.Y);
        }

        public static float ComponentMax(Float3 x)
        {
            return Math.Max(Math.Max(x.X, x.Y), x.Z);
        }

        public static float ComponentMax(Float4 x)
        {
            return Math.Max(Math.Max(Math.Max(x.X, x.Y), x.Z), x.W);
        }

        public static int ComponentMax(Int2 x)
        {
            return Math.Max(x.X, x.Y);
        }

        public static int ComponentMax(Int3 x)
        {
            return Math.Max(Math.Max(x.X, x.Y), x.Z);
        }

        public static int ComponentMax(Int4 x)
        {
            return Math.Max(Math.Max(Math.Max(x.X, x.Y), x.Z), x.W);
        }

        public static float ComponentMin(Float2 x)
        {
            return Math.Min(x.X, x.Y);
        }

        public static float ComponentMin(Float3 x)
        {
            return Math.Min(Math.Min(x.X, x.Y), x.Z);
        }

        public static float ComponentMin(Float4 x)
        {
            return Math.Min(Math.Min(Math.Min(x.X, x.Y), x.Z), x.W);
        }

        public static int ComponentMin(Int2 x)
        {
            return Math.Min(x.X, x.Y);
        }

        public static int ComponentMin(Int3 x)
        {
            return Math.Min(Math.Min(x.X, x.Y), x.Z);
        }

        public static int ComponentMin(Int4 x)
        {
            return Math.Min(Math.Min(Math.Min(x.X, x.Y), x.Z), x.W);
        }

        public static float ComponentSum(Float2 x)
        {
            return x.X + x.Y;
        }

        public static float ComponentSum(Float3 x)
        {
            return (x.X + x.Y) + x.Z;
        }

        public static float ComponentSum(Float4 x)
        {
            return ((x.X + x.Y) + x.Z) + x.W;
        }

        public static int ComponentSum(Int2 x)
        {
            return x.X + x.Y;
        }

        public static int ComponentSum(Int3 x)
        {
            return (x.X + x.Y) + x.Z;
        }

        public static int ComponentSum(Int4 x)
        {
            return ((x.X + x.Y) + x.Z) + x.W;
        }

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
            return x == (x & -x);
        }
    }
}
