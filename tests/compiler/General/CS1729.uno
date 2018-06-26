class Main
{
    static void Test()
    {
        // Class Main has only a default constructor, which takes no arguments.
        var test1 = new Main(2); // $E2009 [Ignore]

        var test2 = new Main();

        // Class Parent has only one constructor, which takes two int parameters.
        var exampleParent1 = new Parent(10); // $E2009 [Ignore]
        // The following line resolves the error.
        var exampleParent2 = new Parent(10, 1);
    }
}

public class Parent
{
    public Parent(int i, int j) { }
}

// The following declaration causes a compiler error because class Parent
// does not have a constructor that takes no arguments. The declaration of
// class Child2 shows how to resolve this error.
public class Child : Parent {} // $E3001

public class Child2 : Parent
{
    public Child2(int k) : base(k, 0) {}
}
