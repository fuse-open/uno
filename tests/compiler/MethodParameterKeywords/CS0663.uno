class Main
{
    public void Method(ref int i) {} // $E [Ignore] Cannot define overloaded method 'Method' because it differs from another method only on ref and out
    public void Method(out int i) {} // $E4121 $E4513 [Ignore]

    public void Method2(int i) {}
    public void Method2(out int i) {} // $E4513 [Ignore]
}
