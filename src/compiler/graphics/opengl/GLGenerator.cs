using System.Collections.Generic;
using Uno.Collections;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends.Shaders;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Graphics.OpenGL
{
    class GLGenerator : ShaderPass
    {
        readonly GLBackend _backend;
        readonly IBundle _bundle;
        readonly ShaderObfuscator _obfuscator;
        readonly RefArrayType _stringArray;
        readonly ListDictionary<string, Expression> _programs = new ListDictionary<string, Expression>();
        readonly Dictionary<string, Field> _fields = new Dictionary<string, Field>();
        readonly bool _dumpShaders;

        public GLGenerator(GLBackend backend, IBundle bundle)
            : base(backend)
        {
            _backend = backend;
            _bundle = bundle;
            _obfuscator = new ShaderObfuscator(backend);
            _stringArray = (RefArrayType)ILFactory.GetType("string[]");
            _dumpShaders = Environment.IsDefined("DUMP_SHADERS");
        }

        protected override Statement TransformDraw(Draw draw)
        {
            var src = draw.Source;
            var scope = new Scope(src);
            var field = GetField(draw);
            var obj = new LoadField(src, new This(src, field.DeclaringType).Address, field).Address;
            var index = 0;

            foreach (var t in TerminalFields)
                if (t.Value != draw.State.Terminals[t.Key].ToString())
                    scope.Statements.Add(ILFactory.SetProperty(src, obj, t.Key, draw.State.Terminals[t.Key]));

            foreach (var v in draw.State.RuntimeConstants)
                scope.Statements.Add(CallConst(src, obj, v, index++));

            scope.Statements.Add(ILFactory.CallMethod(src, obj, "Use"));

            foreach (var v in draw.State.VertexAttributes)
                scope.Statements.Add(CallAttrib(src, obj, v, index++));
            foreach (var v in draw.State.Uniforms)
                scope.Statements.Add(CallUniform(src, obj, v, index++));
            foreach (var v in draw.State.PixelSamplers)
                scope.Statements.Add(CallSampler(src, obj, v, index++));

            scope.Statements.Add(draw.State.OptionalIndices == null
                ? ILFactory.CallMethod(src, obj,
                    "DrawArrays",
                    draw.State.Terminals["VertexCount"])
                : ILFactory.CallMethod(src, obj,
                    "Draw",
                    draw.State.Terminals["VertexCount"],
                    draw.State.OptionalIndices.IndexType,
                    draw.State.OptionalIndices.Buffer));

            return draw.State.Terminals.ContainsKey("CullDrawable")
                ? (Statement)new IfElse(src, ILFactory.CallOperator(src, Essentials.Bool, "!", draw.State.Terminals["CullDrawable"]), scope) 
                : scope;
        }

        Field GetField(Draw draw)
        {
            var prog = GetProgram(draw);
            var key = Type + ":" + prog;

            Field result;
            if (!_fields.TryGetValue(key, out result))
            {
                var src = draw.Source;
                var initMethod = Type.TryGetMethod("init_DrawCalls", false);

                if (initMethod == null)
                    throw new SourceException(src, "No 'init_DrawCalls()' method was found in " + Type.Quote());

                var dc = ILFactory.NewObject(src, "Uno.Graphics.OpenGL.GLDrawCall", prog);
                result = new Field(src, Type, "_draw_" + draw.State.Path.Suffix, null, Modifiers.Private | Modifiers.Generated, 0, dc.ReturnType);
                initMethod.Body.Statements.Add(new StoreField(src, new This(src, Type), result, dc));
                Type.Fields.Add(result);
                _fields.Add(key, result);
            }

            return result;
        }

        Expression GetProgram(Draw draw)
        {
            _obfuscator.Minify(draw.State);

            var src = draw.Source;
            var vsSource = GlslWriter.BuildString(_backend, draw.State.VertexShader, !Environment.Debug);
            var fsSource = GlslWriter.BuildString(_backend, draw.State.PixelShader, !Environment.Debug);
            var key = vsSource + ":" + fsSource;

            foreach (var e in _programs.GetList(key))
                if (e.Source.Package.IsAccessibleFrom(draw.Source.Package))
                    return e;

            int constCount = draw.State.RuntimeConstants.Count;
            int attribCount = draw.State.VertexAttributes.Count;
            int uniformCount = draw.State.Uniforms.Count;
            int samplerCount = draw.State.PixelSamplers.Count;
            var array = new Expression[constCount + attribCount + uniformCount + samplerCount];
            var index = 0;

            foreach (var v in draw.State.RuntimeConstants)
                array[index++] = new Constant(src, Essentials.String, v.Name);
            foreach (var v in draw.State.VertexAttributes)
                array[index++] = new Constant(src, Essentials.String, v.Name);
            foreach (var v in draw.State.Uniforms)
                array[index++] = new Constant(src, Essentials.String, v.Name);
            foreach (var v in draw.State.PixelSamplers)
                array[index++] = new Constant(src, Essentials.String, v.Name);

            var prog = ILFactory.CallMethod(src,
                "Uno.Graphics.OpenGL.GLProgram", "Create",
                new Constant(src, Essentials.String, vsSource),
                new Constant(src, Essentials.String, fsSource),
                new Constant(src, Essentials.Int, constCount),
                new Constant(src, Essentials.Int, attribCount),
                new NewArray(src, _stringArray, array));

            var result = _bundle.AddProgram(draw.State.Path.DrawBlock, prog);
            _programs.Add(key, result);
                
            if (_dumpShaders)
            {
                var prefix = Environment.Combine("shaders",
                    Type.QualifiedName + "." + key.GetHashCode().ToString("x8"));
                Disk.WriteAllText(prefix + ".vert", vsSource);
                Disk.WriteAllText(prefix + ".frag", fsSource);
            }

            return result;
        }

        Expression CallAttrib(Source src, Expression obj, VertexAttribute attr, int index)
        {
            return ILFactory.CallMethod(src, obj, "Attrib", new Constant(src, Essentials.Int, index), attr.AttribType, attr.Buffer, attr.Stride, attr.Offset);
        }

        Expression CallConst(Source src, Expression obj, ShaderVariable var, int index)
        {
            return ILFactory.CallMethod(src, obj, "Const", new Constant(src, Essentials.Int, index), var.Value);
        }

        Expression CallUniform(Source src, Expression obj, ShaderVariable var, int index)
        {
            return ILFactory.CallMethod(src, obj, "Uniform", new Constant(src, Essentials.Int, index), var.Value);
        }

        Expression CallSampler(Source src, Expression obj, PixelSampler sampler, int index)
        {
            return sampler.OptionalState == null
                ? ILFactory.CallMethod(src, obj, "Sampler", new Constant(src, Essentials.Int, index), sampler.Texture)
                : ILFactory.CallMethod(src, obj, "Sampler", new Constant(src, Essentials.Int, index), sampler.Texture, sampler.OptionalState);
        }

        static readonly Dictionary<string, string> TerminalFields = new Dictionary<string, string>()
        {
            { "BlendEnabled", "false" },
            { "DepthTestEnabled", "true" },
            { "WriteRed", "true" },
            { "WriteGreen", "true" },
            { "WriteBlue", "true" },
            { "WriteAlpha", "true" },
            { "WriteDepth", "true" },

            { "BaseVertex", "0" },
            { "LineWidth", "1.0f" },

            { "CullFace", "Uno.Graphics.PolygonFace.Back" },
            { "DepthFunc", "Uno.Graphics.CompareFunc.LessOrEqual" },
            { "PolygonWinding", "Uno.Graphics.PolygonWinding.CounterClockwise" },
            { "PrimitiveType", "Uno.Graphics.PrimitiveType.Triangles" },

            { "BlendSrcRgb", "Uno.Graphics.BlendOperand.One" },
            { "BlendSrcAlpha", "Uno.Graphics.BlendOperand.One" },
            { "BlendDstRgb", "Uno.Graphics.BlendOperand.One" },
            { "BlendDstAlpha", "Uno.Graphics.BlendOperand.One" },
            { "BlendEquationRgb", "Uno.Graphics.BlendEquation.Add" },
            { "BlendEquationAlpha", "Uno.Graphics.BlendEquation.Add" },
        };
    }
}
