using Uno;

class C<T>
{
    T t = new T(); // $E 'T' has no default constructor

    public void ExampleMethod()
    {
        T t = new T();
    }
}
