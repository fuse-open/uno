using Uno;
using Uno.Testing;
using Uno.Graphics;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

public extern(android) class Actions
{
    [Test]
    public void Test()
    {
        Assert.IsTrue(ForeignCallActionOfInt(ActionInt0));
        Assert.IsTrue(AnotherActionOfInt(ActionInt0));

        Assert.IsTrue(ActionTest(ActionIntInt0));

        Assert.IsTrue(ConflictTest0(ActionObj0));
        Assert.IsTrue(ActionsExt.ConflictTest0(ActionObj0));
    }

    //------------------------------------------------------------

    void ActionInt0(int x)
    {
        assert(x==1);
    }

    [Foreign(Language.Java)]
    public bool ForeignCallActionOfInt(Action<int> y)
    @{
        y.run(1);
        return true;
    @}

    [Foreign(Language.Java)]
    public bool AnotherActionOfInt(Action<int> y)
    @{
        // making sure duplicate signatures dont fail
        return true;
    @}

    //------------------------------------------------------------

    static void ActionIntInt0(int a, int b)
    {
        assert(a==1 && b==2);
    }

    [Foreign(Language.Java)]
    static bool ActionTest(Action<int,int> jeff)
    @{
        jeff.run(1,2);
        return true;
    @}

    //------------------------------------------------------------

    // ConflictTest0 here, and in ActionsExt are to test whether actions
    // of types that resolve to java type Object cause conflicts. ActionObj0
    // of object & Java.Object both will do this.

    [Foreign(Language.Java)]
    public bool ConflictTest0(Action<object> y)
    @{
        y.run(null);
        return true;
    @}

    void ActionObj0(object x)
    {
    }
    void ActionObj0(Java.Object x)
    {
    }
}

// See note above Actions.ConflictTest0 for context for this class
static public extern(android) class ActionsExt
{
    [Foreign(Language.Java)]
    static public bool ConflictTest0(Action<Java.Object> y)
    @{
        y.run(null);
        return true;
    @}
}
