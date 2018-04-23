namespace Mono.test_named_08
{
    using Uno;
    
    
    class C
    {
        static int Foo (string packageId, int version)
        {
            return Foo (packageId, version, ignoreDependencies: false, allowPrereleaseVersions: false);
        }
    
        static int Foo (string packageId, int version, bool ignoreDependencies, bool allowPrereleaseVersions)
        {
            return 1;
        }
    
        static int Foo (double package, bool ignoreDependencies, bool allowPrereleaseVersions, bool ignoreWalkInfo)
        {
            return 2;
        }
    
        [Uno.Testing.Test] public static void test_named_08() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            if (Foo ("", 1) != 1)
                return 1;
    
            return 0;
        }
    }
}
