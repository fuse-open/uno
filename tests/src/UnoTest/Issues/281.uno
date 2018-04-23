using Uno;
using Uno.Testing;
using Uno.Collections;

namespace UnoTest.Issues
{
    static class BlenderMap
    {
        static Dictionary<Type,object> _blenders = new Dictionary<Type,object>();
        static public Blender<T> Get<T>()
        {
            object blender;
            if (!_blenders.TryGetValue(typeof(T), out blender))
            {
                if (typeof(T) == typeof(float))
                    blender = new FloatBlender();
                else if (typeof(T) == typeof(float4))
                    blender = new Float4Blender();
                else
                    throw new Exception( "Unsupported blender type" );

                _blenders.Add(typeof(T),blender);
            }

            return (Blender<T>)blender;
        }
    }

    interface Blender<T>
    {
        T Weight( T v, float w );
        T Add( T a, T b );
    }

    class FloatBlender : Blender<float>
    {
        public float Weight( float v, float w ) { return v * w; }
        public float Add( float a, float b ) { return a + b; }
    }

    class Float4Blender : Blender<float4>
    {
        public float4 Weight( float4 v, float w ) { return v * w; }
        public float4 Add( float4 a, float4 b ) { return a + b; }
    }

    public class Issue281
    {
        [Test]
        public void Run()
        {
            Assert.AreEqual( typeof(FloatBlender), BlenderMap.Get<float>().GetType() );
            Assert.AreEqual( typeof(Float4Blender), BlenderMap.Get<float4>().GetType() );
        }
    }
}
