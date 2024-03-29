namespace Mono.gtest_083
{
    public class CollectionValueBase<T>
    {
        public virtual T[] ToArray()
        {
            return null;
        }
    }

    public class CollectionBase<T>: CollectionValueBase<T>
    {
    }

    public class SequencedBase<T>: CollectionBase<T>
    {
    }

    public class ArrayBase<T>: SequencedBase<T>
    {
        public override T[] ToArray()
        {
            return null;
        }
    }

    class X
    {
        [Uno.Testing.Test] public static void gtest_083() { Main(); }
        public static void Main()
        { }
    }
}
