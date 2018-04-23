using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Extensions
{
    public class Element : IDisambiguable
    {
        public readonly Source Source;
        public readonly string String;
        public readonly Disambiguation Disambiguation;
        public readonly Namescope[] Usings;

        Source IDisambiguable.Source => Source;
        Disambiguation IDisambiguable.Disambiguation => Disambiguation;

        public Element(Source src, string value, Disambiguation disamg = 0, params Namescope[] usings)
        {
            Source = src;
            String = value;
            Disambiguation = disamg;
            Usings = usings;
        }

        public override string ToString()
        {
            return String;
        }
    }
}
