abstract class SomethingBase {
    protected virtual void Foo() { }
}

class Something : SomethingBase {
    protected virtual void Foo() { } // $W 'Something.Foo()' hides inherited member 'SomethingBase.Foo()' -- use the 'new' modifier if hiding is intentional
}
