using Uno;

class C<T> : Application
{
    T t = new T(); // $E 'T' has no default constructor

    public void ExampleMethod()
    {
        T t = new T();
    }
}
