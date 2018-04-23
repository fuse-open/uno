namespace Mono.gtest_472
{
    class C<T>
    {
        public virtual void CopyUnsafe(T[] value, params long[] fromIdx){}
        public virtual bool CopyUnsafe(T[] value, long fromIdx) { return true; }
    
        public virtual void CopyUnsafe(T[] value)
        {
            bool b = CopyUnsafe(value, 0);
        }
    }
    
    class A
    {
        [Uno.Testing.Test] public static void gtest_472() { Main(); }
        public static void Main()
        {
        }
    }
}
