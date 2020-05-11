using System.Collections.Generic;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Graphics.OpenGL
{
    public class GLBackend : ShaderBackend
    {
        public override string Name => "OpenGL";

        internal delegate void CallWriter(GlslWriter w, Expression obj, Expression[] args, ExpressionUsage u);

        internal readonly Dictionary<DataType, string> Types = new Dictionary<DataType, string>();
        internal readonly Dictionary<Function, CallWriter> Functions = new Dictionary<Function, CallWriter>();

        internal DataType GlslIntrinsicAttribute { get; private set; }

        public override void Initialize(ICompiler compiler, IBundle bundle)
        {
            base.Initialize(compiler, bundle);
            Scheduler.AddGenerator(new GLGenerator(this, bundle));
            GlslIntrinsicAttribute = ILFactory.GetType("Uno.Compiler.ExportTargetInterop.GlslIntrinsicAttribute");

            // GLSL types
            Types.Add(DataType.Void, "void");
            Types.Add(Essentials.Bool, "bool");
            Types.Add(Essentials.Byte, "float"); // No byte in GLES
            Types.Add(Essentials.Double, "float"); // No double in GLES
            Types.Add(Essentials.Int, "int");
            Types.Add(Essentials.Int2, "ivec2");
            Types.Add(Essentials.Int3, "ivec3");
            Types.Add(Essentials.Int4, "ivec4");
            Types.Add(Essentials.UInt, "uint");
            Types.Add(Essentials.Float, "float");
            Types.Add(Essentials.Float2, "vec2");
            Types.Add(Essentials.Float3, "vec3");
            Types.Add(Essentials.Float4, "vec4");
            Types.Add(Essentials.Float2x2, "mat2");
            Types.Add(Essentials.Float3x3, "mat3");
            Types.Add(Essentials.Float4x4, "mat4");
            Types.Add(Essentials.Sampler2D, "sampler2D");
            Types.Add(Essentials.SamplerCube, "samplerCube");
            Types.Add(Essentials.VideoSampler, 
                Environment.IsDefined("ANDROID") 
                    ? "samplerExternalOES" 
                    : "sampler2D");

            // GLSL functions
            AddFunction("float4x4.get_Item(int)");
            AddFunction("float4x4.set_Item(int,float4)");
            AddFunction("float3x3.get_Item(int)");
            AddFunction("float3x3.set_Item(int,float3)");
            AddFunction("float2x2.get_Item(int)");
            AddFunction("float2x2.set_Item(int,float2)");

            AddFunction("Uno.Matrix.Mul(float2x2,float2x2)",
                (w, obj, args, p) =>
                {
                    if (p >= ExpressionUsage.Operand)
                        w.Write("(");

                    w.WriteExpression(args[1], ExpressionUsage.Operand);
                    w.Write(w.Space + "*" + w.Space);
                    w.WriteExpression(args[0], ExpressionUsage.Operand);

                    if (p >= ExpressionUsage.Operand)
                        w.Write(")");
                });

            AddFunction("Uno.Matrix.Mul(float3x3,float3x3)",
                (w, obj, args, p) =>
                {
                    if (p >= ExpressionUsage.Operand)
                        w.Write("(");

                    w.WriteExpression(args[1], ExpressionUsage.Operand);
                    w.Write(w.Space + "*" + w.Space);
                    w.WriteExpression(args[0], ExpressionUsage.Operand);

                    if (p >= ExpressionUsage.Operand)
                        w.Write(")");
                });

            AddFunction("Uno.Matrix.Mul(float4x4,float4x4)",
                (w, obj, args, p) =>
                {
                    if (p >= ExpressionUsage.Operand)
                        w.Write("(");

                    w.WriteExpression(args[1], ExpressionUsage.Operand);
                    w.Write(w.Space + "*" + w.Space);
                    w.WriteExpression(args[0], ExpressionUsage.Operand);

                    if (p >= ExpressionUsage.Operand)
                        w.Write(")");
                });

            AddFunction("Uno.Vector.TransformNormal(float3,float4x4)",
                (w, obj, args, p) =>
                {
                    w.Write("(");
                    w.WriteExpression(args[1], ExpressionUsage.Operand);
                    w.Write(w.Space + "*" + w.Space);
                    w.Write("vec4(");
                    w.WriteExpression(args[0]);
                    w.Write("," + w.Space + "0.)).xyz");
                });

            AddFunction("Uno.Vector.Transform(float4,float4x4)",
                (w, obj, args, p) =>
                {
                    if (p >= ExpressionUsage.Operand)
                        w.Write("(");

                    w.WriteExpression(args[1], ExpressionUsage.Operand);
                    w.Write(w.Space + "*" + w.Space);
                    w.WriteExpression(args[0], ExpressionUsage.Operand);

                    if (p >= ExpressionUsage.Operand)
                        w.Write(")");
                });

            AddFunction("Uno.Vector.Transform(float3,float4x4)",
                (w, obj, args, p) =>
                {
                    if (p >= ExpressionUsage.Operand)
                        w.Write("(");

                    w.WriteExpression(args[1], ExpressionUsage.Operand);
                    w.Write(w.Space + "*" + w.Space);
                    w.Write("vec4(");
                    w.WriteExpression(args[0]);
                    w.Write("," + w.Space + "1.)");

                    if (p >= ExpressionUsage.Operand)
                        w.Write(")");
                });

            AddFunction("Uno.Vector.Transform(float2,float4x4)",
                (w, obj, args, p) =>
                {
                    if (p >= ExpressionUsage.Operand)
                        w.Write("(");

                    w.WriteExpression(args[1], ExpressionUsage.Operand);
                    w.Write(w.Space + "*" + w.Space);
                    w.Write("vec4(");
                    w.WriteExpression(args[0]);
                    w.Write("," + w.Space + "0.");
                    w.Write("," + w.Space + "1.)");

                    if (p >= ExpressionUsage.Operand)
                        w.Write(")");
                });

            AddFunction("Uno.Vector.TransformAffine(float3,float4x4)",
                (w, obj, args, p) =>
                {
                    w.Write("(");
                    w.WriteExpression(args[1], ExpressionUsage.Operand);
                    w.Write(w.Space + "*" + w.Space);
                    w.Write("vec4(");
                    w.WriteExpression(args[0]);
                    w.Write("," + w.Space + "1.)).xyz");
                });

            AddFunction("Uno.Vector.Transform(float3,float3x3)",
                (w, obj, args, p) =>
                {
                    if (p >= ExpressionUsage.Operand)
                        w.Write("(");

                    w.WriteExpression(args[1], ExpressionUsage.Operand);
                    w.Write(w.Space + "*" + w.Space);
                    w.WriteExpression(args[0], ExpressionUsage.Operand);

                    if (p >= ExpressionUsage.Operand)
                        w.Write(")");
                });

            AddFunction("Uno.Vector.Transform(float2,float2x2)",
                (w, obj, args, p) =>
                {
                    if (p >= ExpressionUsage.Operand)
                        w.Write("(");

                    w.WriteExpression(args[1], ExpressionUsage.Operand);
                    w.Write(w.Space + "*" + w.Space);
                    w.WriteExpression(args[0], ExpressionUsage.Operand);

                    if (p >= ExpressionUsage.Operand)
                        w.Write(")");
                });

            AddFunction("Uno.Graphics.Sampler2D.Sample(float2)",
                (w, obj, args, p) =>
                {
                    w.Write("texture2D(");
                    w.WriteExpression(obj);
                    w.Write(w.Comma);
                    w.WriteExpression(args[0]);
                    w.Write(")");
                });

            AddFunction("Uno.Graphics.SamplerCube.Sample(float3)",
                (w, obj, args, p) =>
                {
                    w.Write("textureCube(");
                    w.WriteExpression(obj);
                    w.Write(w.Comma);
                    w.WriteExpression(args[0]);
                    w.Write(")");
                });

            AddFunction("Uno.Graphics.VideoSampler.Sample(float2)",
                (w, obj, args, p) =>
                {
                    w.Write("texture2D(");
                    w.WriteExpression(obj);
                    w.Write(w.Comma);
                    w.WriteExpression(args[0]);
                    w.Write(")");
                });
        }

        public override bool IsIntrinsic(Method f)
        {
            return Functions.ContainsKey(f) || f.HasAttribute(GlslIntrinsicAttribute);
        }

        void AddFunction(string signature, CallWriter writer = null)
        {
            Functions.Add((Function)ILFactory.GetEntity(signature), writer);
        }

        public override bool IsReserved(string id)
        {
            return Keywords.Contains(id);
        }

        public static readonly HashSet<string> Keywords = new HashSet<string>
        {
            "attribute", "const", "uniform", "varying", "centroid", "break", "continue", "do",
            "for", "while", "if", "else", "in", "out", "inout", "float", "int", "void", "bool",
            "true", "false", "invariant", "discard", "return", "mat2", "mat3", "mat4", "mat2x2",
            "mat2x3", "mat2x4", "mat3x2", "mat3x3", "mat3x4", "mat4x2", "mat4x3", "mat4x4", "vec2",
            "vec3", "vec4", "ivec2", "ivec3", "ivec4", "bvec2", "bvec3", "bvec4", "sampler1D",
            "sampler2D", "sampler3D", "samplerCube", "sampler1DShadow", "sampler2DShadow", "struct",
            "asm", "class", "union", "enum", "typedef", "template", "this", "packed", "goto",
            "switch", "default", "inline", "noinline", "volatile", "public", "static", "extern",
            "external", "interface", "long", "short", "double", "half", "fixed", "unsigned", "lowp",
            "mediump", "highp", "precision", "input", "output", "hvec2", "hvec3", "hvec4", "dvec2",
            "dvec3", "dvec4", "fvec2", "fvec3", "fvec4", "sampler2DRect", "sampler3DRect",
            "sampler2DRectShadow", "sizeof", "cast", "namespace", "using",

            // intrinsics
            "radians", "degrees", "sin", "cos", "tan", "asin", "acos", "atan", "pow", "exp", "log",
            "exp2", "log2", "sqrt", "inversesqrt", "abs", "sign", "floor", "ceil", "fract", "mod",
            "min", "max", "clamp", "min", "mix", "step", "smoothstep", "length", "distance", "dot",
            "cross", "normalize", "faceforward", "reflect", "refract", "matrixCompMult", "lessThan",
            "lessThanEqual", "greaterThan", "greaterThanEqual", "equal", "notEqual", "any", "all",
            "not", "texture2DLod", "texture2DProjLod", "textureCubeLod", "texture2D",
            "texture2DProj", "textureCube"
        };
    }
}
