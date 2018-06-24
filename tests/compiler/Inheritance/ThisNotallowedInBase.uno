public class A
{
    public A(A a){}
}

public class B : A
{
    public B() : base(this) {} // $E4111
}