struct S
{
    public int AIProp { get; set; }
    public S(int i) {} // $E4512 [Ignore]
}
