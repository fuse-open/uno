using Uno;
using Uno.Collections;
using Uno.Diagnostics;
using Uno.Text;

public class StringBuilderBenchmark : Uno.Application
{
    static char[] GetRandomCharArray(int length)
    {
        var chars = new char[length];

        for (int i = 0; i < length; ++i)
            chars[i] = 'A';

        return chars;
    }

    static string GetRandomString(int length)
    {
        return new string(GetRandomCharArray(length));
    }

    public static void LongStrings(int count)
    {
        var random = new Random(1337);
        var stringsToConcat = new string[count];
        for (int i = 0; i < count; ++i)
        {
            var length = random.Next(100, 150);
            stringsToConcat[i] = GetRandomString(length);
        }

        var startTick = Clock.GetSeconds();

        var sb = new StringBuilder();
        for (int i = 0; i < count; ++i)
            sb.Append(stringsToConcat[i]);
        var str = sb.ToString();

        var endTick = Clock.GetSeconds();
        Debug.Log("length: " + str.Length);
        Debug.Log("time: " + (endTick - startTick) * 1000 + " ms");
    }

    public static void ShortStrings(int count)
    {
        var random = new Random(1337);
        var stringsToConcat = new string[count];
        for (int i = 0; i < count; ++i)
        {
            var length = random.NextInt(3, 5);
            stringsToConcat[i] = GetRandomString(length);
        }

        var startTick = Clock.GetSeconds();

        var sb = new StringBuilder();
        for (int i = 0; i < count; ++i)
            sb.Append(stringsToConcat[i]);
        var str = sb.ToString();

        var endTick = Clock.GetSeconds();
        Debug.Log("length: " + str.Length);
        Debug.Log("time: " + (endTick - startTick) * 1000 + " ms");
    }

    public static void Chars(int count)
    {
        var random = new Random(1337);
        var charsToConcat = new char[count];
        for (int i = 0; i < count; ++i)
            charsToConcat[i] = 'A';

        var startTick = Clock.GetSeconds();

        var sb = new StringBuilder();
        for (int i = 0; i < count; ++i)
            sb.Append(charsToConcat[i]);
        var str = sb.ToString();

        var endTick = Clock.GetSeconds();
        Debug.Log("length: " + str.Length);
        Debug.Log("time: " + (endTick - startTick) * 1000 + " ms");
    }

    public static void CharArrays(int count)
    {
        var random = new Random(1337);
        var charsArraysToConcat = new char[count][];
        for (int i = 0; i < count; ++i)
            charsArraysToConcat[i] = GetRandomCharArray(50);

        var startTick = Clock.GetSeconds();

        var sb = new StringBuilder();
        for (int i = 0; i < count; ++i)
            sb.Append(charsArraysToConcat[i]);
        var str = sb.ToString();

        var endTick = Clock.GetSeconds();
        Debug.Log("length: " + str.Length);
        Debug.Log("time: " + (endTick - startTick) * 1000 + " ms");
    }

    public StringBuilderBenchmark()
    {
        Debug.Log("LongStrings:");
        LongStrings(10000);
        Debug.Log("");

        Debug.Log("ShortStrings:");
        ShortStrings(10000);
        Debug.Log("");

        Debug.Log("Chars:");
        Chars(10000);
        Debug.Log("");

        Debug.Log("CharArrays:");
        CharArrays(10000);
        Debug.Log("");
    }
}
