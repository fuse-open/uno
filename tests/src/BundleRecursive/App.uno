using Uno.IO;
using Uno.Testing;

public class BundleRecursive
{
    [Test]
    public static void Test()
    {
        int count = 0;
        foreach (var f in Bundle.Get("BundleRecursive").Files)
            count++;
        Assert.AreEqual(4, count);
    }
}
