using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

[ForeignInclude(Language.Java, "lol")]
public class Share
{
    [Test]
    public void ShouldCompile() { }
}
