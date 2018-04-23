using Uno.Testing;

namespace UnoTest.Issues
{
    class MasterBase<T>
    {
    }

    abstract class MixerBase
    {
        abstract public MasterBase<T> CreateMaster<T>();
    }

    class AverageMixer : MixerBase
    {
        public override MasterBase<T> CreateMaster<T>()
        {
            return new MasterBase<T>();
        }
    }

    public class Issue285
    {
        [Test]
        public void Run()
        {
            var a = new AverageMixer();
            Assert.AreEqual( typeof(MasterBase<float>), a.CreateMaster<float>().GetType() );
            Assert.AreEqual( typeof(MasterBase<float4>), a.CreateMaster<float4>().GetType() );
        }
    }
}
