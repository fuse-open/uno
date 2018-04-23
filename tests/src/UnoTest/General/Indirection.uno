using Uno;

namespace UnoTest.General
{
    public struct StructWithString
    {
        public string foo;
    }

    public struct StructWithStruct
    {
        public StructWithString s;
    }

    public class App
    {
        public StructWithStruct sws;

        public void foo()
        {
            sws = new StructWithStruct{s = get()};
        }

        public StructWithString get()
        {
            return new StructWithString();
        }
    }

}
