using Uno;
using Uno.Testing;

namespace Uno.Test
{
    public class ColorTest
    {
        [Test]
        public void FromRgb()
        {
            Assert.AreEqual(float3(0, 0, 0), Color.FromRgb(0x000000));
            Assert.AreEqual(float3(1, 0, 0), Color.FromRgb(0xFF0000));
            Assert.AreEqual(float3(0, 1, 0), Color.FromRgb(0x00FF00));
            Assert.AreEqual(float3(0, 0, 1), Color.FromRgb(0x0000FF));
            Assert.AreEqual(float3(0.5f, 0.5f, 0.5f), Color.FromRgb(0x808080), 0.51f / 255);
        }

        [Test]
        public void FromRgba()
        {
            Assert.AreEqual(float4(0, 0, 0, 0), Color.FromRgba(0x00000000));
            Assert.AreEqual(float4(1, 0, 0, 0), Color.FromRgba(0xFF000000));
            Assert.AreEqual(float4(0, 1, 0, 0), Color.FromRgba(0x00FF0000));
            Assert.AreEqual(float4(0, 0, 1, 0), Color.FromRgba(0x0000FF00));
            Assert.AreEqual(float4(0, 0, 0, 1), Color.FromRgba(0x000000FF));
            Assert.AreEqual(float4(0.5f, 0.5f, 0.5f, 0.5f), Color.FromRgba(0x80808080), 0.51f / 255);
        }

        [Test]
        public void FromArgb()
        {
            Assert.AreEqual(float4(0, 0, 0, 0), Color.FromArgb(0x00000000));
            Assert.AreEqual(float4(1, 0, 0, 0), Color.FromArgb(0x00FF0000));
            Assert.AreEqual(float4(0, 1, 0, 0), Color.FromArgb(0x0000FF00));
            Assert.AreEqual(float4(0, 0, 1, 0), Color.FromArgb(0x000000FF));
            Assert.AreEqual(float4(0, 0, 0, 1), Color.FromArgb(0xFF000000));
            Assert.AreEqual(float4(0.5f, 0.5f, 0.5f, 0.5f), Color.FromArgb(0x80808080), 0.51f / 255);
        }

        [Test]
        public void ToArgb()
        {
            Assert.AreEqual(0x00000000, Color.ToArgb(float4(0, 0, 0, 0)));
            Assert.AreEqual(0x00FF0000, Color.ToArgb(float4(1, 0, 0, 0)));
            Assert.AreEqual(0x0000FF00, Color.ToArgb(float4(0, 1, 0, 0)));
            Assert.AreEqual(0x000000FF, Color.ToArgb(float4(0, 0, 1, 0)));
            Assert.AreEqual(0xFF000000, Color.ToArgb(float4(0, 0, 0, 1)));
            Assert.AreEqual(0x80808080, Color.ToArgb(float4(0.5f, 0.5f, 0.5f, 0.5f) + 0.01f / 255));
            Assert.AreEqual(0x7F7F7F7F, Color.ToArgb(float4(0.5f, 0.5f, 0.5f, 0.5f) - 0.01f / 255));
        }

        [Test]
        public void ToHsv()
        {
            //according to http://codebeautify.org/rgb-to-hsv-converter
            _ToHsv( int3(0,0,0), float3(0,0,0) );
            _ToHsv( int3(255,128,0), float3(30.1f/360f,1,1) );
            _ToHsv( int3(128,128,255), float3(240/360f,0.498f,1) );
            _ToHsv( int3(71, 143, 73), float3(121.7f/360f, 0.5035f, 0.5608f ) );

            _ToHsv( int4(71, 137, 143,0), float4(185/360f, 0.5035f, 0.5608f, 0 ) );
            _ToHsv( int4(155,130,173,128), float4(274.9f/360f, 0.2486f, 0.6784f, 0.5019608f ) );
            _ToHsv( int4(196, 179, 181,255), float4(352.9f/360f, 0.0867f, 0.7686f, 1f ) );
        }

        void _ToHsv( int3 rgb, float3 hsv )
        {
            var f = (float3)rgb / 255f;
            var v = Color.ToHsv( f );
            //unfortunately our reference has low precision
            Assert.AreEqual( hsv.X, v.X, 0.1f );
            Assert.AreEqual( hsv.Y, v.Y, 0.0001f );
            Assert.AreEqual( hsv.Z, v.Z, 0.0001f );
        }

        void _ToHsv( int4 rgb, float4 hsv )
        {
            var f = (float4)rgb / 255f;
            var v = Color.ToHsv( f );
            //unfortunately our reference has low precision
            Assert.AreEqual( hsv.X, v.X, 0.1f );
            Assert.AreEqual( hsv.Y, v.Y, 0.0001f );
            Assert.AreEqual( hsv.Z, v.Z, 0.0001f );
            Assert.AreEqual( hsv.W, v.W, 0.0001f );
        }

        [Test]
        public void FromHsv()
        {
            //table from http://www.rapidtables.com/convert/color/hsv-to-rgb.htm
            _FromHsv( float3(0,0,0), int3(0,0,0));
            _FromHsv( float3(0,0,1), int3(255,255,255));
            _FromHsv( float3(120/360f,1,1), int3(0,255,0));
            _FromHsv( float3(60/360f,1,1), int3(255,255,0));
            _FromHsv( float3(180/360f,1,1), int3(0,255,255));
            _FromHsv( float3(240/360f,1,1), int3(0,0,255));

            _FromHsv( float4(0/360f,0,0.7529f,0), int4(192,192,192,0));
            _FromHsv( float4(0/360f,1,0.5f,0.5f), int4(128,0,0,128));
            _FromHsv( float4(300/360f,1,0.5f,1), int4(128,0,128,255));
        }

        void _FromHsv( float3 hsv, int3 rgb )
        {
            var v = Color.FromHsv( hsv );
            Assert.AreEqual((float3)rgb / 255f, v, 1f/255 );
        }

        void _FromHsv( float4 hsv, int4 rgb )
        {
            var h = Color.FromHsv( hsv );
            Assert.AreEqual((float4)rgb / 255f, h, 1f/255 );
        }

        [Test]
        [Obsolete]
        public void FromHex()
        {
            Assert.AreEqual(Color.FromRgba(0xAABBCCFF), Color.FromHex("ABC"));
            Assert.AreEqual(Color.FromRgba(0x112233FF), Color.FromHex("123"));

            Assert.AreEqual(Color.FromRgba(0xAABBCCDD), Color.FromHex("ABCD"));
            Assert.AreEqual(Color.FromRgba(0x11223344), Color.FromHex("1234"));

            Assert.AreEqual(Color.FromRgba(0x001080FF), Color.FromHex("001080"));
            Assert.AreEqual(Color.FromRgba(0xFFCCAAFF), Color.FromHex("FFCCAA"));

            Assert.AreEqual(Color.FromRgba(0x789abc80), Color.FromHex("789abc80"));
            Assert.AreEqual(Color.FromRgba(0xFFFFFF00), Color.FromHex("FFfffF00"));

            Assert.Throws<ArgumentException>(_FromHexWithIllegal);
            Assert.Throws<ArgumentException>(_FromHexWithSign);

            Assert.AreEqual(Color.FromHex("ABC"), Color.FromHex("#ABC"));
            Assert.AreEqual((float4)int4(0xAA,0xBB,0xCC,0xFF) / 255f, Color.FromHex("ABC"));
        }

        [Obsolete]
        void _FromHexWithIllegal()
        {
            Color.FromHex("EFG");
        }

        [Obsolete]
        void _FromHexWithSign()
        {
            Color.FromHex("---");
        }

        [Test]
        public void Parse()
        {
            Assert.AreEqual(Color.FromRgba(0xAABBCCFF), Color.Parse("#ABC"));
            Assert.AreEqual(Color.FromRgba(0x112233FF), Color.Parse("#123"));

            Assert.AreEqual(Color.FromRgba(0xAABBCCDD), Color.Parse("#ABCD"));
            Assert.AreEqual(Color.FromRgba(0x11223344), Color.Parse("#1234"));

            Assert.AreEqual(Color.FromRgba(0x001080FF), Color.Parse("#001080"));
            Assert.AreEqual(Color.FromRgba(0xFFCCAAFF), Color.Parse("#FFCCAA"));

            Assert.AreEqual(Color.FromRgba(0x789abc80), Color.Parse("#789abc80"));
            Assert.AreEqual(Color.FromRgba(0xFFFFFF00), Color.Parse("#FFfffF00"));

            Assert.Throws<FormatException>(_ParseWithoutHashPrefix);
            Assert.Throws<FormatException>(_ParseWithIllegal);
            Assert.Throws<FormatException>(_ParseWithSign);
        }

        void _ParseWithoutHashPrefix()
        {
            Color.Parse("deadbeef");
        }

        void _ParseWithIllegal()
        {
            Color.Parse("#EFG");
        }

        void _ParseWithSign()
        {
            Color.Parse("#---");
        }

        [Test]
        public void TryParse()
        {
            float4 color;

            Assert.AreEqual(true, Color.TryParse("#ABC", out color));
            Assert.AreEqual(Color.FromRgba(0xAABBCCFF), color);
            Assert.AreEqual(true, Color.TryParse("#123", out color));
            Assert.AreEqual(Color.FromRgba(0x112233FF), color);

            Assert.AreEqual(true, Color.TryParse("#ABCD", out color));
            Assert.AreEqual(Color.FromRgba(0xAABBCCDD), color);
            Assert.AreEqual(true, Color.TryParse("#1234", out color));
            Assert.AreEqual(Color.FromRgba(0x11223344), color);

            Assert.AreEqual(true, Color.TryParse("#001080", out color));
            Assert.AreEqual(Color.FromRgba(0x001080FF), color);
            Assert.AreEqual(true, Color.TryParse("#FFCCAA", out color));
            Assert.AreEqual(Color.FromRgba(0xFFCCAAFF), color);

            Assert.AreEqual(true, Color.TryParse("#789abc80", out color));
            Assert.AreEqual(Color.FromRgba(0x789abc80), color);
            Assert.AreEqual(true, Color.TryParse("#FFfffF00", out color));
            Assert.AreEqual(Color.FromRgba(0xFFFFFF00), color);

            Assert.AreEqual(false, Color.TryParse("deadbeef", out color));
            Assert.AreEqual(false, Color.TryParse("#EFG", out color));
            Assert.AreEqual(false, Color.TryParse("#---", out color));
        }
    }
}
