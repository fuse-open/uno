using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Domain.Graphics
{
    public class PixelSampler
    {
        public DataType Type;
        public string Name;
        public Expression Texture;
        public Expression OptionalState;

        public PixelSampler(DataType dt, string name, Expression texture)
        {
            Type = dt;
            Name = name;
            Texture = texture;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}