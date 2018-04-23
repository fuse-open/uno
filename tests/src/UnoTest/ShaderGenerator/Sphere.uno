using Uno;
using Uno.Collections;
using Uno.Graphics;

namespace UnoTest
{
    public block Sphere
    {
        apply DefaultPrimitivesBlock;

        public float Radius: prev, 10.0f;
        public float3 Scale: float3(Radius);

        int Slices: 32;
        int Stacks: 32;

    }
}
