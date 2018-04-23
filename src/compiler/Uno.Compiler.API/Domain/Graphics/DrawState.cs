using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.Graphics
{
    public class DrawState
    {
        public readonly Dictionary<string, Expression> Terminals = new Dictionary<string, Expression>();
        public readonly List<VertexAttribute> VertexAttributes = new List<VertexAttribute>();
        public readonly List<PixelSampler> PixelSamplers = new List<PixelSampler>();
        public readonly List<ShaderVariable> RuntimeConstants = new List<ShaderVariable>();
        public readonly List<ShaderVariable> Uniforms = new List<ShaderVariable>();
        public readonly List<ShaderVariable> Varyings = new List<ShaderVariable>();
        public readonly List<StructType> Structs = new List<StructType>();
        public readonly VertexShader VertexShader;
        public readonly PixelShader PixelShader;
        public readonly Drawable Path;
        public IndexBinding OptionalIndices;

        public DrawState(Drawable path)
        {
            Path = path;
            VertexShader = new VertexShader(this);
            PixelShader = new PixelShader(this);
        }

        public void VisitVaryings(Statement parent, Pass p)
        {
            for (int i = 0; i < Varyings.Count; i++)
            {
                var u = Varyings[i];
                var s = u.Value;

                p.Next(parent);
                p.Begin(ref s);
                s.Visit(p);
                p.End(ref s);

                u.Value = s;
                Varyings[i] = u;
            }
        }

        public void Visit(Statement parent, Pass p)
        {
            foreach (var key in Terminals.Keys.ToArray())
            {
                var s = Terminals[key];

                p.Next(parent);
                p.Begin(ref s);
                s.Visit(p);
                p.End(ref s);

                Terminals[key] = s;
            }

            if (OptionalIndices != null)
            {
                p.Next(parent);
                p.Begin(ref OptionalIndices.Buffer);
                OptionalIndices.Buffer.Visit(p);
                p.End(ref OptionalIndices.Buffer);

                p.Next(parent);
                p.Begin(ref OptionalIndices.IndexType);
                OptionalIndices.IndexType.Visit(p);
                p.End(ref OptionalIndices.IndexType);
            }

            for (int i = 0; i < VertexAttributes.Count; i++)
            {
                var e = VertexAttributes[i];

                p.Next(parent);
                p.Begin(ref e.AttribType);
                e.AttribType.Visit(p);
                p.End(ref e.AttribType);

                p.Next(parent);
                p.Begin(ref e.Buffer);
                e.Buffer.Visit(p);
                p.End(ref e.Buffer);

                p.Next(parent);
                p.Begin(ref e.Offset);
                e.Offset.Visit(p);
                p.End(ref e.Offset);

                p.Next(parent);
                p.Begin(ref e.Stride);
                e.Stride.Visit(p);
                p.End(ref e.Stride);
            }

            for (int i = 0; i < RuntimeConstants.Count; i++)
            {
                var e = RuntimeConstants[i];

                p.Next(parent);
                p.Begin(ref e.Value);
                e.Value.Visit(p);
                p.End(ref e.Value);
            }

            for (int i = 0; i < Uniforms.Count; i++)
            {
                var e = Uniforms[i];

                p.Next(parent);
                p.Begin(ref e.Value);
                e.Value.Visit(p);
                p.End(ref e.Value);
            }

            for (int i = 0; i < PixelSamplers.Count; i++)
            {
                var e = PixelSamplers[i];

                p.Next(parent);
                p.Begin(ref e.Texture);
                e.Texture.Visit(p);
                p.End(ref e.Texture);

                if (e.OptionalState != null)
                {
                    p.Next(parent);
                    p.Begin(ref e.OptionalState);
                    e.OptionalState.Visit(p);
                    p.End(ref e.OptionalState);
                }
            }
        }
    }
}
