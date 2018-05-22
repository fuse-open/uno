using Uno.Testing;

namespace Uno.Runtime.InteropServices.Test
{
	public class GCHandleTest
	{
		class MyObject
		{
			public int Field;
		}

		struct MyStruct
		{
			public int Field;
		}

		readonly object[] _targets = new object[]
		{
			new object(),
			123,
			null,
			new MyObject(),
			new MyStruct(),
			new float4(1.0f, 2.0f, 3.0f, 4.0f),
		};

		[Test]
		public void AllocFree()
		{
			foreach (var target in _targets)
			{
				var handle = GCHandle.Alloc(target);
				Assert.AreEqual(target, handle.Target);
				handle.Free();
			}
		}

		IntPtr CreateHandlePtr()
		{
			if defined(CPLUSPLUS)
				extern "::uAutoReleasePool __pool";
			var target = new MyObject();
			target.Field = 123;
			var handle = GCHandle.Alloc(target);
			return GCHandle.ToIntPtr(handle);
		}

		[Test]
		public void AllocRetains()
		{
			var ptr = CreateHandlePtr();
			var handle = GCHandle.FromIntPtr(ptr);
			Assert.IsTrue(handle.Target is MyObject);
			Assert.AreEqual(123, ((MyObject)handle.Target).Field);
			handle.Free();
		}

		[Test]
		public void FromAndToIntPtr()
		{
			foreach (var target in _targets)
			{
				var handle = GCHandle.Alloc(target);
				var ptr = (IntPtr)handle;
				var handle2 = (GCHandle)ptr;
				Assert.AreEqual(handle.Target, handle2.Target);
				Assert.AreEqual(handle, handle2);
				handle.Free();
			}
		}
	}
}
