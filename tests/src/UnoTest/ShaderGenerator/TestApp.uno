using Uno;
using Uno.Collections;

namespace UnoTest.ShaderGenerator
{
	// Taken from https://github.com/fusetools/uno/issues/1202
	class TestApp
	{
		public void Draw()
		{
			float2[] someArray = new[]
			{
				float2(0, 0), float2(1, 0), float2(1, 1),
				float2(1, 1), float2(0, 1), float2(0, 0)
			};

			//var someOtherArray = someArray.ToArray(); // <- Interestingly, uncommenting this makes it work...

			draw
			{
				float2 Vertices: vertex_attrib(someArray.ToArray()); // <- ...but ToArray here causes the error

				ClipPosition: float4(Vertices, 0, 1);

				PixelColor: float4(1, 0, 1, 1);
			};
		}
	}
}
