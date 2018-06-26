public class
{// $E Expected identifier following 'class' -- found '{' (LeftCurlyBrace)
    public int Num { get; set; }
    void MethodA() {}
}

public class Main
{
    enum Colors /* : int */ // Uno does not support this atm
    {
        'a', // $E Expected identifier following '{' -- found ''a'' (CharLiteral)
        'b'
    }
}
