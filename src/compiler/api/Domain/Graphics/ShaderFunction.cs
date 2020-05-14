using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.Graphics
{
    public class ShaderFunction : Function
    {
        public readonly Shader Shader;

        public override MemberType MemberType => MemberType.ShaderFunction;

        public ShaderFunction(
            Source src,
            Shader shader,
            string name,
            DataType returnType,
            Parameter[] parameterList)
            : base(src, shader.State.Path.DrawBlock.Method.DeclaringType, name, returnType, parameterList, new Scope(src), null, Modifiers.Static | Modifiers.Generated)
        {
            Shader = shader;
        }

        public override string ToString()
        {
            return NameAndParameterList;
        }
    }
}