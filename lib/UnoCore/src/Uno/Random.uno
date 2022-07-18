
namespace Uno
{
    public class Random
    {
        int z, w;

        public Random(int seed)
        {
            SetSeedInternal(seed);
        }

        private void SetSeedInternal(int seed)
        {
            z = seed;
            w = seed + 1;
        }

        [Obsolete("Construct a new Random object instead")]
        public void SetSeed(int seed)
        {
            SetSeedInternal(seed);
        }

        public int Next()
        {
            z = (36969 * (z & 0xFFFF) + (z >> 16)) & 0x7FFFFFF;
            w = (18000 * (w & 0xFFFF) + (w >> 16)) & 0x7FFFFFF;
            return ((z << 16) + w) & 0x7FFFFFF;
        }

        [Obsolete("Use Next() instead")]
        public int NextInt()
        {
            return Next();
        }


        public int Next(int high)
        {
            if (high < 0)
                throw new ArgumentOutOfRangeException(nameof(high), "high cannot be negative");

            return (int)(NextDouble() * high);
        }

        [Obsolete("Use Next(int) instead")]
        public int NextInt(int high)
        {
            return Next(high);
        }

        public int Next(int low, int high)
        {
            if (low > high)
                throw new ArgumentOutOfRangeException("low cannot be greater than high");

            return low + (int)(NextDouble() * (high - low));
        }

        [Obsolete("Use Next(int, int) instead")]
        public int NextInt(int low, int high)
        {
            return Next(low, high);
        }

        public double NextDouble()
        {
            return (double)Next() / 0x7FFFFFF;
        }

        [Obsolete("Use NextDouble() instead")]
        public float NextFloat()
        {
            return (float)NextDouble();
        }

        [Obsolete("Use `min + NextDouble() * (max - min)` instead ")]
        public float NextFloat(float minv, float maxv)
        {
            if (minv == maxv)
                return minv;
            if (minv > maxv)
                throw new ArgumentOutOfRangeException("minv cannot be greater than maxv");
            return NextFloat() * (maxv - minv) + minv;
        }

        [Obsolete("Use `float2((float)NextDouble(), (float)NextDouble())` instead")]
        public float2 NextFloat2()
        {
            return float2((float)NextDouble(), (float)NextDouble());
        }

        [Obsolete("Use `float3((float)NextDouble(), (float)NextDouble(), (float)NextDouble())` instead")]
        public float3 NextFloat3()
        {
            return float3((float)NextDouble(), (float)NextDouble(), (float)NextDouble());
        }

        [Obsolete("Use `float4((float)NextDouble(), (float)NextDouble(), (float)NextDouble(), (float)NextDouble())` instead")]
        public float4 NextFloat4()
        {
            return float4((float)NextDouble(), (float)NextDouble(), (float)NextDouble(), (float)NextDouble());
        }
    }
}
