using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Domain.Graphics
{
    public abstract class Shader
    {
        public readonly DrawState State;
        public readonly ShaderFunction Entrypoint;
        public readonly List<ShaderFunction> Functions = new List<ShaderFunction>();
        public readonly Dictionary<string, Expression> Terminals = new Dictionary<string, Expression>();
        public readonly HashSet<int> ReferencedConstants = new HashSet<int>();
        public readonly HashSet<int> ReferencedUniforms = new HashSet<int>();
        public readonly HashSet<int> ReferencedStructs = new HashSet<int>();

        public Source Source => State.Path.DrawBlock.Source;
        public abstract ShaderType Type { get; }

        protected Shader(DrawState state, string entrypointName)
        {
            State = state;
            Entrypoint = new ShaderFunction(state.Path.Source, this, entrypointName, DataType.Void, new Parameter[0]);
        }

        public void Visit(Pass p)
        {
            if (!p.Begin(this))
                return;

            var ps = p.Shader;
            p.Shader = this;

            foreach (var f in Functions)
                f.Visit(p);

            Entrypoint.Visit(p);
            p.Shader = ps;
            p.End(this);
        }

        public void VisitTerminals(Statement parent, Pass p)
        {
            var keys = new string[Terminals.Count];
            Terminals.Keys.CopyTo(keys, 0);

            foreach (var key in keys)
            {
                var s = Terminals[key];
                p.Next(parent);
                p.Begin(ref s);
                s.Visit(p);
                p.End(ref s);
                Terminals[key] = s;
            }
        }
    }
}
