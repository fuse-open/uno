public delegate void MyEvent(object sender);

// Ditched the interface part from the C# example and tested operators on delegate fields instead. This should work
class Main
{
    private MyEvent field;

    event MyEvent Clicked
    {
        add
        {
            field += value;
        }
        remove
        {
            field -= value;
        }
    }
}
