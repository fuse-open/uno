using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.API.Backends.Shaders
{
    public struct ShaderField
    {
        public string Type;
        public string Name;
        public Expression ArraySize;

        public ShaderField(string type, string name, Expression arraySize = null)
        {
            Type = type;
            Name = name;
            ArraySize = arraySize;
        }
    }
}