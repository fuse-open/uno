using Uno.Testing;

namespace UnoTest.Issues
{
    public abstract class Transform { }
    public class Rotation : Transform { }

    public abstract class TransformAnimator<TransformType> where TransformType : Transform, new()
    {
        TransformType transform;

        TransformType GetTransform()
        {
            transform = new TransformType();
            return transform;
        }

        public bool Seek()
        {
            return Update(GetTransform());
        }

        protected abstract bool Update(TransformType transform);
    }

    public sealed class Rotate: TransformAnimator<Rotation>
    {
        protected override bool Update(Rotation t)
        {
            return t is Rotation;
        }
    }

    public class Issue283
    {
        [Test]
        public void Run()
        {
            var b = new Rotate();
            Assert.IsTrue( b.Seek() );
        }
    }
}
