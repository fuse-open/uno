public class TestClass1
{
    public void WriteSomething(string s) {}
}

public class TestClass2
{
    public void DisplaySomething(string s) {}
}

public class Main
{
    public Main()
    {
        var tc1 = new TestClass1();
        var tc2 = new TestClass2();
        // The following call fails because TestClass1 does not have
        // a method called DisplaySomething.
        tc1.DisplaySomething("Hello");      // $E3104

        // To correct the error, change the method call to either
        // tc1.WriteSomething or tc2.DisplaySomething.
        tc1.WriteSomething("Hello from TestClass1");
        tc2.DisplaySomething("Hello from TestClass2");
    }
}