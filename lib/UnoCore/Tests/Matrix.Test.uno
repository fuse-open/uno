using Uno;
using Uno.Testing;

namespace Uno.Test
{

    public class MatrixTest
    {
		[Test]
		public void Invert2()
		{	
			var i = Matrix.Invert( float2x2(1,2,3,4) );
			Assert.AreEqual( float2(-2,1), i[0] );
			Assert.AreEqual( float2(1.5f,-0.5f), i[1] );
			
			//expect Identity in these deprecated functions
			i = Matrix.Invert( float2x2(0,0,0,0) );
			Assert.AreEqual( float2(1,0), i[0] );
			Assert.AreEqual( float2(0,1), i[1] );
			
			float2x2 m;
			Assert.IsFalse( Matrix.TryInvert(float2x2(0,0,0,0), out m) );
		}
		
		[Test]
		public void Invert3()
		{	
			var i = Matrix.Invert( float3x3(1,2,0,3,4,0,5,6,1) );
			Assert.AreEqual( float3(-2,1,0), i[0] );
			Assert.AreEqual( float3(1.5f,-0.5f,0), i[1] );
			Assert.AreEqual( float3(1, -2, 1), i[2] );
			
			//expect Identity in these deprecated functions
			i = Matrix.Invert( float3x3(0,0,0,0,0,0,0,0,0) );
			Assert.AreEqual( float3(1,0,0), i[0] );
			Assert.AreEqual( float3(0,1,0), i[1] );
			Assert.AreEqual( float3(0,0,1), i[2] );
			
			float3x3 m;
			Assert.IsFalse( Matrix.TryInvert(float3x3(0,0,0,0,0,0,0,0,0), out m) );
		}
		
		[Test]
		public void Invert4()
		{	
			var i = Matrix.Invert( float4x4(1,2,0,0,
				3,4,0,0,
				0,5,6,0,
				0,0,0,1) );
			Assert.AreEqual( float4(-2,1,0,0), i[0] );
			Assert.AreEqual( float4(1.5f,-0.5f,0,0), i[1] );
			Assert.AreEqual( float4(-1.25f, 5.0f/12, 1.0f/6,0), i[2] );
			Assert.AreEqual( float4(0, 0, 0, 1), i[3] );
			
			//expect Identity in these deprecated functions
			i = Matrix.Invert( float4x4(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0) );
			Assert.AreEqual( float4(1,0,0,0), i[0] );
			Assert.AreEqual( float4(0,1,0,0), i[1] );
			Assert.AreEqual( float4(0,0,1,0), i[2] );
			Assert.AreEqual( float4(0,0,0,1), i[3] );
			
			float4x4 m;
			Assert.IsFalse( Matrix.TryInvert(float4x4(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0), out m) );
		}
    }

}