public class Main : Uno.Application
{
    public int myProp { get; } // $E3041

    public int myProp2 { get; private set; }
}