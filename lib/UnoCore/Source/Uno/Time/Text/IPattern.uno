namespace Uno.Time.Text
{
    public interface IPattern<T>
    {
        ParseResult<T> Parse(string text);

        string Format(T value);
    }
}
