class A
{
    public virtual void F() {}
    public virtual void G() {}
    public void H() {}
}
class B : A
{
    sealed override public void F() {}
    override public void G() {}
    override public void H() {} // $E4023
}
class C : B
{
    override public void F() {} // $E4093
    override public void G() {}
}
