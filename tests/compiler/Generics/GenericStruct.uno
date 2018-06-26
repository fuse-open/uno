struct GStruct1<T, U>
{
    public int dummyField;

    public GStruct1(T t, U u) {} // $E4512 [Ignore]
    public GStruct1(T t) {} // $E4512 [Ignore]
}
