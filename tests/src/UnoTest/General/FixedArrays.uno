using Uno;
using Uno.Graphics;
using Uno.Testing;

namespace UnoTest.General
{
    public class FixedArrays
    {
        struct StructWithFixedField
        {
            int Foo;
            //public fixed float FixedField[FixedArrayLength]; // Error
        }

        const int FixedArrayLength = 10;

        public fixed float FixedField[FixedArrayLength];
        //fixed float FixedField2[]; // Error

        fixed float FixedMetaProperty[]: fixed float[] { 0, 1, 2, };
        fixed float FixedMetaProperty[]: fixed[] { (float)0, 1, 2, };
        fixed float FixedMetaProperty[]: fixed float[FixedArrayLength];
        /*
        fixed float FixedMetaScope[]: // Error
        {
            fixed float floats[]; // Error
            return floats;
        };
        */

        float MetaScope:
        {
            fixed float floats[FixedArrayLength];

            for (int i = 0; i < floats.Length; i++)
                floats[i] = (float)i;

            return Sum(floats);
        };

        fixed float FixedMetaProperty[]: fixed[] { readonly MetaScope };
        FixedMetaProperty: fixed[] { readonly MetaScope };

        void Draw(const fixed float2 fixedArgument[2])
        {
            //fixedArgument[1] = float2(0); // Error

            fixed float fixedLocal[] = { 0, 1, 2 };

            draw this, DefaultShading, Quad
            {
                ClipPosition:
                    prev + float4(FixedMetaProperty[0]);

                PixelColor:
                    {
                        // sample will force this to pixel stage
                        var p = sample(new texture2D(int2(256), Format.RGBA8888, true), TexCoord);

                        for (int i = 0; i < FixedField.Length; i++)
                            p.X += FixedField[i];

                        for (int i = 0; i < fixedArgument.Length; i++)
                            p.Y += fixedArgument[i].X;

                        for (int i = 0; i < fixedArgument.Length; i++)
                            p.Z += fixedArgument[i].Y;

                        for (int i = 0; i < fixedLocal.Length; i++)
                            p.W += fixedLocal[i];

                        return p;
                    };
            };
        }

        static float Sum(const fixed float arg[FixedArrayLength])
        {
            float sum = 0;

            for (int i = 0; i < arg.Length; i++)
                sum += arg[i];

            return sum;
        }

        static float2 Sum(const fixed float2 arg[FixedArrayLength])
        {
            float2 sum = float2(0);

            for (int i = 0; i < arg.Length; i++)
                sum += arg[i];

            return sum;
        }

        static void Double(ref fixed float2 arg[FixedArrayLength])
        {
            for (int i = 0; i < arg.Length; i++)
                arg[i] *= 2;
        }

        [Test]
        public void Run()
        {
            fixed float floats[] = {0, 1, 2,};
            fixed float2 float2s1[1] = {float2(0)};
            fixed float2 float2s2[1] = fixed float2[] {float2(0)};
            fixed float2 float2s5[FixedArrayLength];

            for (int i = 0; i < float2s5.Length; i++)
                float2s5[i] = float2(0);

            Double(ref float2s5);
            Assert.AreEqual(float2(0), Sum(float2s5));
        }
    }
}
