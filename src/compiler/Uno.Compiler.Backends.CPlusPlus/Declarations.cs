namespace Uno.Compiler.Backends.CPlusPlus
{
    public class Declarations
    {
        public string[] Header;
        public string[] Source;
        public string[] Inline;

        public static readonly Declarations Empty = new Declarations
        {
            Header = new string[0],
            Inline = new string[0],
            Source = new string[0]
        };
    }
}