using System.Collections.Generic;
using System.IO;
using Uno.Compiler.API.Backends.Decompilers;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Backends.Shaders
{
    public abstract class ShaderWriter : SourceWriter
    {
        protected readonly string[] VectorFields = { "x", "y", "z", "w" };

        protected ShaderWriter(ShaderBackend backend, TextWriter w, bool minify)
            : base(backend, w, minify)
        {
            HasFloatSuffix = false;
        }

        public abstract void WriteVertexShader(VertexShader vs);
        public abstract void WritePixelShader(PixelShader ps);
        public abstract string GetTypeName(Source src, DataType dt);

        public void WriteShader(Shader shader)
        {
            switch (shader.Type)
            {
                case ShaderType.Vertex:
                    WriteVertexShader(shader as VertexShader);
                    break;
                case ShaderType.Pixel:
                    WritePixelShader(shader as PixelShader);
                    break;
            }
        }

        public override void WriteType(Source src, DataType dt)
        {
            Write(GetTypeName(src, dt));
        }

        public override void WriteDownCast(Source src, DataType dt, Expression s, ExpressionUsage u)
        {
            WriteType(src, dt);
            Write("(");
            WriteExpression(s);
            Write(")");
        }

        public override void WriteVariable(Variable var)
        {
            WriteType(var.Source, var.ValueType);
            Write(" " + var.Name);

            // TODO: Remove DefaultInit hack
            if (var.OptionalValue != null && !(var.OptionalValue is Default))
            {
                Write(Assign);
                WriteExpression(var.OptionalValue);
            }

            for (var = var.Next; var != null; var = var.Next)
            {
                if (!HasVariable(var.ValueType, var.OptionalValue))
                    continue;

                Write(Comma + var.Name);
                if (var.OptionalValue != null && !(var.OptionalValue is Default))
                {
                    Write(Assign);
                    WriteExpression(var.OptionalValue);
                }
            }
        }

        public override void WriteDefault(Source src, DataType dt, ExpressionUsage u = ExpressionUsage.Argument)
        {
            WriteType(src, dt);
            Write("(0.)");
        }

        public override void WriteNewObject(Source src, Constructor ctor, Expression[] args, ExpressionUsage u)
        {
            WriteStaticType(src, ctor.DeclaringType);
            WriteArguments(ctor, args, true);
        }

        public override void WriteCallCast(CallCast s, ExpressionUsage u)
        {
            WriteStaticType(s.Source, s.ReturnType);
            Write("(");
            WriteExpression(s.Operand);
            Write(")");
        }

        public override void WriteAddressOf(AddressOf s, ExpressionUsage u)
        {
            WriteExpression(s.ActualValue, u);
        }

        public override void WriteName(Field f)
        {
            switch (f.DeclaringType.BuiltinType)
            {
                case BuiltinType.Float:
                case BuiltinType.Float2:
                case BuiltinType.Float3:
                case BuiltinType.Float4:
                case BuiltinType.Int:
                case BuiltinType.Int2:
                case BuiltinType.Int3:
                case BuiltinType.Int4:
                    Write(VectorFields[f.Index]);
                    break;
                default:
                    base.WriteName(f);
                    break;
            }
        }

        public override void WriteLoadField(Source src, Field field, Expression obj, ExpressionUsage u)
        {
            switch (field.DeclaringType.BuiltinType)
            {
                case BuiltinType.Float4x4:
                {
                    WriteExpression(obj, ExpressionUsage.Object);
                    int fid = field.Index;
                    Write("[" + (fid / 4) + "]." + VectorFields[fid % 4]);
                    return;
                }
                case BuiltinType.Float3x3:
                {
                    WriteExpression(obj, ExpressionUsage.Object);
                    int fid = field.Index;
                    Write("[" + (fid / 3) + "]." + VectorFields[fid % 3]);
                    return;
                }
                case BuiltinType.Float2x2:
                {
                    WriteExpression(obj, ExpressionUsage.Object);
                    int fid = field.Index;
                    Write("[" + (fid / 2) + "]." + VectorFields[fid % 2]);
                    return;
                }
            }

            base.WriteLoadField(src, field, obj, u);
        }

        public override void WriteStoreField(Source src, Field field, Expression obj, Expression value, ExpressionUsage u)
        {
            Begin(u);
            WriteLoadField(src, field, obj, ExpressionUsage.Operand);
            Write(Assign);
            WriteExpression(value);
            End(u);
        }

        public override void WriteParameter(Source src, DataType dt, ParameterModifier m, string name)
        {
            var fat = dt as FixedArrayType;

            switch (m)
            {
                case ParameterModifier.Const:
                    WriteWhen(!EnableMinify, "in "); // Not actually necessary
                    break;
                case ParameterModifier.Ref:
                    Write("inout ");
                    break;
                case ParameterModifier.Out:
                    Write("out ");
                    break;
            }

            WriteType(src, fat != null ? fat.ElementType : dt);
            Write(" " + name);

            if (fat != null)
            {
                Write("[");

                if (fat.OptionalSize != null)
                    WriteExpression(fat.OptionalSize);
                else
                    Log.Error(src, ErrorCode.I0000, "Unknown length of 'fixed' array");

                Write("]");
            }
        }

        public void WriteFunction(ShaderFunction f)
        {
            BeginLine();
            WriteType(f.Source, f.ReturnType);
            Write(" " + f.Name);
            WriteParameters(f, true);
            WriteFunctionBody(f);
        }

        public void WriteStruct(StructType st)
        {
            BeginLine("struct " + st.Name);
            BeginScope();

            foreach (var f in st.Fields)
            {
                BeginLine();
                WriteType(st.Source, f.ReturnType);
                EndLine(" " + f.Name + ";");
            }

            EndScopeSemicolon();
        }

        public void WriteStructs(Shader shader)
        {
            foreach (var i in shader.ReferencedStructs)
                WriteStruct(shader.State.Structs[i]);
        }

        public List<ShaderField> GetUniforms(Shader s)
        {
            var result = new List<ShaderField>();

            foreach (var e in s.ReferencedUniforms)
            {
                var u = s.State.Uniforms[e];
                result.Add(u.IsArray
                    ? new ShaderField(GetTypeName(s.Source, u.ElementType), u.Name, u.ArraySize)
                    : new ShaderField(GetTypeName(s.Source, u.Type), u.Name));
            }

            return result;
        }

        public List<ShaderField> GetVaryings(Shader s)
        {
            var result = new List<ShaderField>();

            foreach (var u in s.State.Varyings)
                result.Add(new ShaderField(GetTypeName(s.Source, u.Type), u.Name));

            return result;
        }

        public List<ShaderField> GetVertexAttribs(Shader s)
        {
            var result = new List<ShaderField>();

            foreach (var u in s.State.VertexAttributes)
                result.Add(new ShaderField(GetTypeName(s.Source, u.Type), u.Name));

            return result;
        }

        public List<ShaderField> GetPixelSamplers(Shader s)
        {
            var result = new List<ShaderField>();

            foreach (var u in s.State.PixelSamplers)
                result.Add(new ShaderField(GetTypeName(s.Source, u.Type), u.Name));

            return result;
        }

        public void WriteFields(string prefix, IReadOnlyList<ShaderField> fields)
        {
            if (fields.Count > 0)
            {
                var map = new Dictionary<string, List<ShaderField>>();

                foreach (var e in fields)
                {
                    if (!map.ContainsKey(e.Type))
                        map.Add(e.Type, new List<ShaderField>());

                    map[e.Type].Add(e);
                }

                foreach (var e in map)
                {
                    BeginLine(prefix + " " + e.Key + " ");

                    for (int i = 0; i < e.Value.Count; i++)
                    {
                        if (i > 0)
                            Write(Comma);

                        var f = e.Value[i];
                        Write(f.Name);

                        if (f.ArraySize != null)
                        {
                            Write("[");
                            WriteExpression(f.ArraySize);
                            Write("]");
                        }
                    }

                    EndLine(";");
                }

                Skip();
            }
        }
    }
}
