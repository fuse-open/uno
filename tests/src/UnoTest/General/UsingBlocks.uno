using Uno;
using Uno.Graphics;
using Uno.Testing;

namespace UnoTest.General
{
    public class UsingBlocks
    {
        [Test]
        public void Run()
        {
            using (var tex = new Texture2D(int2(128, 128), Format.RGBA8888, true))
                Assert.AreEqual(int2(128, 128), tex.Size);

            using (var tex = new Texture2D(int2(128, 128), Format.RGBA8888, true))
                ;

            using (texture2D tex1 = new Texture2D(int2(128, 128), Format.RGBA8888, true),
                             tex2 = new Texture2D(int2(128, 128), Format.RGBA8888, true))
            {
                Assert.AreEqual(int2(128, 128), tex1.Size);
                Assert.AreEqual(int2(128, 128), tex2.Size);
            }

            var tex3 = new Texture2D(int2(128, 128), Format.RGBA8888, true);
            using (tex3) Assert.AreEqual(int2(128, 128), tex3.Size);
        }

        int _disposals;

        public class MyDisposable: IDisposable
        {
            UsingBlocks _parent;

            public MyDisposable(UsingBlocks parent)
            {
                _parent = parent;
            }

            public void Dispose()
            {
                ++_parent._disposals;
            }
        }

        [Test]
        public void UsingDisposes()
        {
            _disposals = 0;
            using (var md = new MyDisposable(this))
            {
            }
            Assert.AreEqual(1, _disposals);
        }

        void UsingWithReturn()
        {
            using (var md = new MyDisposable(this))
            {
                return;
            }
        }

        [Test]
        public void UsingWithReturnDisposes()
        {
            _disposals = 0;
            UsingWithReturn();
            Assert.AreEqual(1, _disposals);
        }

        [Test]
        public void UsingWithBreakDisposes()
        {
            _disposals = 0;
            for (int i = 0; i < 10; ++i)
            {
                using (var md = new MyDisposable(this))
                {
                    break;
                }
            }
            Assert.AreEqual(1, _disposals);
        }

        [Test]
        public void UsingWithContinueDisposes()
        {
            _disposals = 0;
            for (int i = 0; i < 1; ++i)
            {
                using (var md = new MyDisposable(this))
                {
                    continue;
                }
            }
            Assert.AreEqual(1, _disposals);
        }
    }
}
