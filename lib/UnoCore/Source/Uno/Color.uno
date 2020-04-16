using Uno.Math;
using Uno.Vector;

namespace Uno
{
    /*
        Standard color model assumed to be RGB in float3 -- thus name often omitted
        HSV has H normalized between 0..1
        Clamping is avoided where possible to allow for HDR/Bloom/Glow/oversaturation/etc.
        LHC is Luma-Hue-Chroma a variant of LUV models
    */
    static public class Color
    {
        static int3 ToInt3( float3 rgb )
        {
            return Clamp((int3)(rgb * 255 + 0.5f), 0, 255);
        }

        static int4 ToInt4( float4 rgba )
        {
            return Clamp((int4)(rgba * 255 + 0.5f), 0, 255);
        }

        static uint ToArgb( int4 rgba )
        {
            return (uint)((rgba.W << 24) | (rgba.X << 16) | (rgba.Y << 8) | rgba.Z);
        }

        static public uint ToArgb( float4 rgba )
        {
            return ToArgb(ToInt4(rgba));
        }

        static public float3 FromRgb( uint rgb )
        {
            return float3(
                ((rgb >> 16) & 0xFF) / 255f,
                ((rgb >> 8) & 0xFF) / 255f,
                ((rgb >> 0) & 0xFF) / 255f );
        }

        static float4 FromInt4( int4 rgba )
        {
            return (float4)rgba / 255f;
        }

        static public float4 FromRgba( uint rgba )
        {
            return float4(
                ((rgba >> 24) & 0xFF) / 255f,
                ((rgba >> 16) & 0xFF) / 255f,
                ((rgba >> 8) & 0xFF) / 255f,
                ((rgba >> 0) & 0xFF) / 255f );
        }

        static public float4 FromArgb( uint argb )
        {
            return float4(
                ((argb >> 16) & 0xFF) / 255f,
                ((argb >> 8) & 0xFF) / 255f,
                ((argb >> 0) & 0xFF) / 255f,
                ((argb >> 24) & 0xFF) / 255f);
        }

        static public float3 ToHsv( float3 rgb )
        {
            var r = rgb.X;
            var g = rgb.Y;
            var b = rgb.Z;

            float min = Min(r, Min(g, b));
            float max = Max(r, Max(g, b));
            float h = 0.0f;
            float s = 0.0f;
            float v = max;
            if (min != max)
            {
                float delta = max - min;
                s = delta / max;
                h = r != max ? (g != max ? 4.0f + (r - g) / delta : 2.0f + (b - r) / delta) : (g - b) / delta;
                if (h < 0.0f)
                    h += 6.0f;
                h /= 6.0f;
            }
            h = Fract( h );
            return float3(h, s, v);
        }

        static public float4 ToHsv( float4 rgba )
        {
            return float4( ToHsv( rgba.XYZ ), rgba.W );
        }

        static public float3 FromHsv( float3 hsv )
        {
            float h = hsv.X * 6;
            float c = hsv.Z * hsv.Y;
            float x = c * ( 1 - Abs( Mod( h, 2 ) - 1 ) );

            float3 rgb;
            //no switch since we can't compile those to shader code yet
            int hi = (int)h;
            if( hi == 0 )
                rgb = float3(c,x,0);
            else if( hi == 1 )
                rgb = float3(x,c,0);
            else if( hi == 2 )
                rgb = float3(0,c,x);
            else if( hi == 3 )
                rgb = float3(0,x,c);
            else if( hi == 4 )
                rgb = float3(x,0,c);
            else if( hi == 5 )
                rgb = float3(c,0,x);
            else
                rgb = float3(0,0,0);

            float m = hsv.Z - c;
            return rgb + m;
        }

        static public float4 FromHsv( float4 hsva )
        {
            return float4( FromHsv( hsva.XYZ ), hsva.W );
        }

        static bool TryParseHexDigit(char ch, out int result)
        {
            if (ch >= '0' && ch <= '9')
            {
                result = (int)ch - (int)'0';
                return true;
            }
            else if (ch >= 'a' && ch <= 'f' )
            {
                result = 10 + ((int)ch - (int)'a');
                return true;
            }
            else if (ch >= 'A' && ch <= 'F' )
            {
                result = 10 + ((int)ch - (int)'A');
                return true;
            }

            result = default(int);
            return false;
        }

        static bool TryParseHexNibble(char ch, out int result)
        {
            int v;
            if (!TryParseHexDigit(ch, out v))
            {
                result = default(int);
                return false;
            }

            result = (v << 4) | v;
            return true;
        }

        static bool TryParseHexByte(char ch1, char ch2, out int result)
        {
            int v1, v2 = 0; // BUG: seems uno thinks v2 can be undefined in the non-error path...
            if (!TryParseHexDigit(ch1, out v1) ||
                !TryParseHexDigit(ch2, out v2))
            {
                result = default(int);
                return false;
            }

            result = (v1 << 4) | v2;
            return true;
        }

        static bool TryParseHexString(string hex, out int4 result)
        {
            if (hex.Length == 3)
            {
                int r, g = 0, b = 0; // BUG: seems uno thinks g or b can be undefined in the non-error path...
                if (!TryParseHexNibble(hex[0], out r) ||
                    !TryParseHexNibble(hex[1], out g) ||
                    !TryParseHexNibble(hex[2], out b))
                {
                    result = default(int4);
                    return false;
                }
                result = int4(r, g, b, 255);
                return true;
            }
            else if (hex.Length == 4)
            {
                int r, g = 0, b = 0, a = 0; // BUG: seems uno thinks g or b can be undefined in the non-error path...
                if (!TryParseHexNibble(hex[0], out r) ||
                    !TryParseHexNibble(hex[1], out g) ||
                    !TryParseHexNibble(hex[2], out b) ||
                    !TryParseHexNibble(hex[3], out a))
                {
                    result = default(int4);
                    return false;
                }
                result = int4(r, g, b, a);
                return true;
            }
            else if (hex.Length == 6)
            {
                int r, g = 0, b = 0; // BUG: seems uno thinks g or b can be undefined in the non-error path...
                if (!TryParseHexByte(hex[0], hex[1], out r) ||
                    !TryParseHexByte(hex[2], hex[3], out g) ||
                    !TryParseHexByte(hex[4], hex[5], out b))
                {
                    result = default(int4);
                    return false;
                }
                result = int4(r, g, b, 255);
                return true;
            }
            else if (hex.Length == 8)
            {
                int r, g = 0, b = 0, a = 0; // BUG: seems uno thinks g or b can be undefined in the non-error path...
                if (!TryParseHexByte(hex[0], hex[1], out r) ||
                    !TryParseHexByte(hex[2], hex[3], out g) ||
                    !TryParseHexByte(hex[4], hex[5], out b) ||
                    !TryParseHexByte(hex[6], hex[7], out a))
                {
                    result = default(int4);
                    return false;
                }
                result = int4(r, g, b, a);
                return true;
            }

            result = default(int4);
            return false;
        }

        [Obsolete("Use Color.Parse(string) instead")]
        static public float4 FromHex( string hex )
        {
            if (hex.Length > 0 && hex[0] == '#')
                hex = hex.Substring(1);

            int4 result;
            if (!TryParseHexString(hex, out result))
                throw new ArgumentException(nameof(hex));

            return FromInt4( result );
        }

        static public float4 Parse(string str)
        {
            if (str.Length > 0 && str[0] == '#')
            {
                var hex = str.Substring(1);

                int4 result;
                if (!TryParseHexString(hex, out result))
                    throw new FormatException("failed to parse hex-string");

                return FromInt4(result);
            }

            throw new FormatException("Unrecognized format");
        }

        static public bool TryParse(string str, out float4 color)
        {
            if (str.Length > 0 && str[0] == '#')
            {
                var hex = str.Substring(1);

                int4 result;
                if (!TryParseHexString(hex, out result))
                {
                    color = default(float4);
                    return false;
                }

                color = FromInt4(result);
                return true;
            }

            color = default(float4);
            return false;
        }

        static string FormatHex( int3 rgb )
        {
            return String.Format( "{0:X2}{1:X2}{2:X2}", rgb.X, rgb.Y, rgb.Z );
        }

        static string FormatHex( int4 rgba )
        {
            return String.Format( "{0:X2}{1:X2}{2:X2}{3:X2}", rgba.X, rgba.Y, rgba.Z, rgba.W );
        }

        static public String ToHex( float3 rgb )
        {
            return FormatHex( ToInt3(rgb) );
        }

        static public String ToHex( float4 rgb )
        {
            return FormatHex( ToInt4(rgb) );
        }

        //https://en.wikipedia.org/wiki/YCbCr
        static public float3x3 ToYCbCrMat = float3x3( 0.299f, -0.168736f, 0.5f,
                0.587f, -0.331264f, -0.418688f,
                0.114f, 0.5f, -0.081312f );
        static public float3x3 ToYCbCrMatInv = float3x3( 1f, 1f, 1f,
                0f, -0.344136f, 1.772f,
                1.4020f, -0.714136f, 0f );

        static public float3 ToYCbCr( float3 rgb )
        {
            var ToYCbCrMat = float3x3( 0.299f, -0.168736f, 0.5f,
                0.587f, -0.331264f, -0.418688f,
                0.114f, 0.5f, -0.081312f );
            return Transform( rgb, ToYCbCrMat );
        }

        static public float3 FromYCbCr( float3 ycbcr )
        {
            var ToYCbCrMatInv = float3x3( 1f, 1f, 1f,
                0f, -0.344136f, 1.772f,
                1.4020f, -0.714136f, 0f );
            return Transform( ycbcr, ToYCbCrMatInv );
        }

        static public float4 ToYCbCr( float4 rgba )
        {
            return float4( ToYCbCr( rgba.XYZ ), rgba.W );
        }

        static public float4 FromYCbCr( float4 ycbcra )
        {
            return float4( FromYCbCr( ycbcra.XYZ ), ycbcra.W );
        }

        static public float3 LhcFromYuv(  float3 yuv )
        {
            return float3(
                yuv.X,
                Atan2( yuv.Z, yuv.Y ),
                Length( yuv.YX ) );
        }

        static public float3 YuvFromLhc( float3 lhc )
        {
            return float3(
                lhc.X,
                lhc.Z * Cos( lhc.Y ),
                lhc.Z * Sin( lhc.Z ) );
        }

        static public float4 Overlay( float4 dst, float4 color )
        {
            return float4( color.XYZ * color.W + dst.XYZ * dst.W *
                (1 - color.W ), color.W + dst.W * (1 - color.W) );
        }
    }
}
