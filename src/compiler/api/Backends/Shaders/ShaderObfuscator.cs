using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Utilities;

namespace Uno.Compiler.API.Backends.Shaders
{
    public class ShaderObfuscator : Pass
    {
        readonly IdentifierGenerator _names;

        int _localCount;
        int _globalCount;

        public ShaderObfuscator(ShaderBackend backend)
            : base(backend)
        {
            _names = new IdentifierGenerator(backend);
        }

        public override void Begin(ref Statement s)
        {
            switch (s.StatementType)
            {
                case StatementType.FixedArrayDeclaration:
                    Minify(((FixedArrayDeclaration) s).Variable);
                    break;
                case StatementType.VariableDeclaration:
                    Minify(((VariableDeclaration) s).Variable);
                    break;
            }
        }

        public void Minify(DrawState d)
        {
            _globalCount = 0;

            foreach (var st in d.Structs)
                st.SetName(_names.Get(_globalCount++));

            foreach (var v in d.VertexAttributes)
                v.Name = _names.Get(_globalCount++);
            foreach (var v in d.RuntimeConstants)
                v.Name = _names.Get(_globalCount++);
            foreach (var v in d.Uniforms)
                v.Name = _names.Get(_globalCount++);
            foreach (var v in d.Varyings)
                v.Name = _names.Get(_globalCount++);
            foreach (var v in d.PixelSamplers)
                v.Name = _names.Get(_globalCount++);

            foreach (var f in d.VertexShader.Functions)
                f.SetName(_names.Get(_globalCount++));
            foreach (var f in d.PixelShader.Functions)
                f.SetName(_names.Get(_globalCount++));

            foreach (var st in d.Structs)
            {
                int fieldCount = _globalCount;
                foreach (var f in st.Fields)
                    f.SetName(_names.Get(fieldCount++));
            }

            Minify(d.VertexShader.Entrypoint);
            foreach (var f in d.VertexShader.Functions)
                Minify(f);

            Minify(d.PixelShader.Entrypoint);
            foreach (var f in d.PixelShader.Functions)
                Minify(f);
        }

        void Minify(Function f)
        {
            _localCount = _globalCount;

            for (int i = 0; i < f.Parameters.Length; i++)
                f.Parameters[i].SetName(_names.Get(_localCount++));

            f.Body.Visit(this);
        }

        public void Minify(Variable v)
        {
            for (; v != null; v = v.Next)
                v.SetName(_names.Get(_localCount++));
        }
    }
}
