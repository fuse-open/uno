using System.IO;
using Uno.Compiler.API.Backends.Shaders;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Graphics.OpenGL
{
    enum ErrorCode
    {
        I9105,
        I0000,
        E9101,
        I9103,
        I9104
    }

    public class GlslWriter : ShaderWriter
    {
        readonly GLBackend Backend;

        public GlslWriter(GLBackend backend, TextWriter w, bool minify)
            : base(backend, w, minify)
        {
            Backend = backend;
        }

        public static string BuildString(GLBackend backend, Shader shader, bool minify)
        {
            using (var w = new StringWriter())
            {
                new GlslWriter(backend, w, minify).WriteShader(shader);
                return w.ToString();
            }
        }

        public override string GetTypeName(Source src, DataType dt)
        {
            string type;
            if (Backend.Types.TryGetValue(dt, out type))
                return type;

            if (dt is StructType)
                return (dt as StructType).Name;

            // Should not happen
            Log.Error(src, ErrorCode.I9105, dt.Quote() + " is not supported in GLSL");
            return "<invalid>";
        }

        public override void WriteFixedArrayDeclaration(FixedArrayDeclaration s)
        {
            BeginLine();

            var fat = (FixedArrayType)s.Variable.ValueType;

            WriteType(s.Source, fat.ElementType);
            Write(" " + s.Variable.Name + "[");

            if (fat.OptionalSize != null)
                WriteExpression(fat.OptionalSize);
            else if (s.OptionalInitializer != null)
                Write(s.OptionalInitializer.Length);
            else
                Log.Error(s.Source, ErrorCode.I0000, "Unknown length of 'fixed' array");

            EndLine("];");

            if (s.OptionalInitializer != null)
            {
                for (int i = 0; i < s.OptionalInitializer.Length; i++)
                {
                    BeginLine(s.Variable.Name + "[" + i + "] = ");
                    WriteExpression(s.OptionalInitializer[i]);
                    EndLine(";");
                }
            }
        }

        public override void WriteCallMethod(Source src, Method method, Expression obj, Expression[] args, Variable ret, ExpressionUsage u)
        {
            var intrinsic = method.TryGetAttributeString(Backend.GlslIntrinsicAttribute);

            if (intrinsic != null)
            {
                Write(intrinsic);
                WriteArguments(method, args, true);
                return;
            }

            GLBackend.CallWriter write;
            if (Backend.Functions.TryGetValue(method, out write) && write != null)
            {
                write(this, obj, args, u);
                return;
            }

            Log.Error(src, ErrorCode.E9101, method.Quote() + " is unsupported in GLSL");
        }

        public override void WritePixelShader(PixelShader shader)
        {
            WriteStructs(shader);
            WriteFields("uniform", GetUniforms(shader));
            WriteFields("uniform", GetPixelSamplers(shader));
            WriteFields("varying", GetVaryings(shader));

            foreach (var func in shader.Functions)
                WriteFunction(func);

            WriteLine("void main()");
            BeginScope();
            WriteFunctionBody(shader.Entrypoint, false);

            foreach (var t in shader.Terminals)
            {
                switch (t.Key)
                {
                    case "PixelColor":
                        BeginLine("gl_FragColor" + Space + "=" + Space);
                        WriteExpression(t.Value);
                        EndLine(";");
                        continue;

                    case "DiscardPixel":
                        BeginLine("if (");
                        WriteExpression(t.Value);
                        EndLine(") discard;");
                        continue;
                }

                Log.Error(t.Value.Source, ErrorCode.I9103, "Unsupported terminal in GLSL fragment shader: " + t.Key);
            }

            EndScope();
        }

        public override void WriteVertexShader(VertexShader shader)
        {
            WriteStructs(shader);
            WriteFields("uniform", GetUniforms(shader));
            WriteFields("attribute", GetVertexAttribs(shader));
            WriteFields("varying", GetVaryings(shader));

            foreach (var func in shader.Functions)
                WriteFunction(func);

            WriteLine("void main()");
            BeginScope();
            WriteFunctionBody(shader.Entrypoint, false);

            foreach (var v in shader.State.Varyings)
            {
                BeginLine(v.Name + Assign);
                WriteExpression(v.Value);
                EndLine(";");
            }

            foreach (var t in shader.Terminals)
            {
                switch (t.Key)
                {
                    case "ClipPosition":
                        BeginLine("gl_Position" + Assign);
                        WriteExpression(t.Value);
                        EndLine(";");
                        continue;

                    case "PointSize":
                        BeginLine("gl_PointSize" + Assign);
                        WriteExpression(t.Value);
                        EndLine(";");
                        continue;
                }

                Log.Error(t.Value.Source, ErrorCode.I9104, "Unsupported terminal in GLSL vertex shader: " + t.Key);
            }

            EndScope();
        }
    }
}
