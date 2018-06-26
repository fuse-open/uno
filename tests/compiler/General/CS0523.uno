struct RecursiveLayoutStruct1
{
    public RecursiveLayoutStruct2 field; // $E4129
}

struct RecursiveLayoutStruct2
{
    public RecursiveLayoutStruct1 field; // $E4129
}