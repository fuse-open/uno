using System;
using System.IO;
using System.Text;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.IO;
using Uno.Logging;

namespace Uno.Compiler.API.Backends.Decompilers
{
    public partial class SourceWriter : TextFormatter
    {
        protected bool HasFloatSuffix = true;

        protected Log Log { get; }
        protected IBuildData Data { get; private set; }
        protected IEnvironment Environment { get; }
        protected IEssentials Essentials { get; }
        protected IILFactory ILFactory { get; private set; }

        public DataType Type { get; private set; }
        public Function Function { get; private set; }

        public readonly string Space = " ";
        public readonly string Assign = " = ";
        public readonly string NotEquals = " != ";
        public new readonly string Equals = " == ";
        public readonly string Comma = ", ";
        public readonly string And = " && ";
        public readonly string Or = " || ";
        public readonly string QuestionMark = " ? ";
        public readonly string Colon = " : ";
        public readonly string NullOp = " ?? ";

        protected SourceWriter(Backend backend, string filename, NewLine newline = NewLine.Lf)
            : this(backend, backend.Disk.CreateBufferedText(filename, newline))
        {
        }

        protected SourceWriter(Backend backend, StringBuilder sb, Function context)
            : this(backend, new StringWriter(sb))
        {
            SetContext(context ?? Function.Null);
        }

        protected SourceWriter(Backend backend, TextWriter w)
            : base(w)
        {
            Log = backend.Log;
            Data = backend.Data;
            Environment = backend.Environment;
            Essentials = backend.Essentials;
            ILFactory = backend.ILFactory;
        }

        protected SourceWriter(ShaderBackend backend, TextWriter w, bool minify)
            : this(w, minify)
        {
            Log = backend.Log;
            Data = backend.Data;
            Environment = backend.Environment;
            Essentials = backend.Essentials;
            ILFactory = backend.ILFactory;
        }

        SourceWriter(TextWriter w, bool minify)
            : base(w)
        {
            if (minify)
            {
                EnableMinify = true;
                Space = Space.Trim();
                Assign = Assign.Trim();
                NotEquals = NotEquals.Trim();
                Equals = Equals.Trim();
                Comma = Comma.Trim();
                And = And.Trim();
                Or = Or.Trim();
                QuestionMark = QuestionMark.Trim();
                Colon = Colon.Trim();
                NullOp = NullOp.Trim();
            }
        }

        bool _hasWarning;
        public void WriteOrigin(Source src)
        {
            if (!_hasWarning)
            {
                WriteLine(Environment.ExpandSingleLine(src, "// @(MSG_ORIGIN)"));
                WriteLine(Environment.ExpandSingleLine(src, "// @(MSG_EDIT_WARNING)"));
                _hasWarning = true;
            }
            else
            {
                WriteLine("// " + src.FullPath);
                WriteLine("// " + new string('-', src.FullPath.Length));
            }

            Skip();
        }

        public void WriteFunctionBody(Function func, bool curlyBraces = true, bool skip = true)
        {
            var oldType = Type;
            var oldFunction = Function;
            SetContext(func);

            if (!func.HasBody)
                Log.Error(func.Source, ErrorCode.I9003, "The function " + func.Quote() + " does not have an implementation");
            else
                WriteScope(func.Body, curlyBraces, skip);

            Function = oldFunction;
            Type = oldType;
        }

        public void SetContext(DataType dt)
        {
            Type = dt;
        }

        public void SetContext(Function func)
        {
            Function = func;

            if (func != null)
                Type = func.DeclaringType;
        }

        public void CommaWhen(bool cond)
        {
            if (cond)
                Write(Comma);
        }

        public void Begin(ExpressionUsage u)
        {
            if (u >= ExpressionUsage.Operand)
                Write("(");
        }

        public void End(ExpressionUsage u)
        {
            if (u >= ExpressionUsage.Operand)
                Write(")");
        }

        public void BeginWhen(bool cond, ExpressionUsage u)
        {
            if (cond && u >= ExpressionUsage.Operand)
                Write("(");
        }

        public void EndWhen(bool cond, ExpressionUsage u)
        {
            if (cond && u >= ExpressionUsage.Operand)
                Write(")");
        }

        public void BeginScope()
        {
            WriteLine("{");
            Indent();
            DisableSkip();
        }

        public void BeginScope(string str)
        {
            WriteLine(str);
            WriteLine("{");
            Indent();
            DisableSkip();
        }

        public void EndScope(bool skip = true)
        {
            DisableSkip();
            Unindent();
            WriteLine("}");
            Skip(skip);
        }

        public void EndScopeSemicolon(bool skip = true)
        {
            DisableSkip();
            Unindent();
            WriteLine("};");
            Skip(skip);
        }

        public virtual void WriteStaticBase(Source src, DataType dt, string member)
        {
            WriteStaticType(src, dt);
            Write("." + member);
        }

        public virtual void WriteMemberBase(Expression obj, DataType declType = null, string member = null)
        {
            WriteExpression(obj, ExpressionUsage.Object);
            Write("." + member);
        }

        public void WriteMemberBase(Source src, DataType declType, Expression obj = null, string member = null)
        {
            if (obj == null)
                WriteStaticBase(src, declType, member);
            else
                WriteMemberBase(obj, declType, member);
        }

        public virtual void WriteName(Field f)
        {
            Write(f.Name);
        }

        public virtual void WriteArgument(Parameter p, Expression s)
        {
            WriteExpression(s);
        }

        public void WriteGenericList(Source src, params DataType[] args)
        {
            Write("<");

            for (int i = 0; i < args.Length; i++)
            {
                CommaWhen(i > 0);
                WriteGenericType(src, args[i]);
            }

            Write(">");
        }

        public void WriteArguments(Parameter[] parameters, Expression[] args, bool p = false, char left = '(', char right = ')')
        {
            WriteWhen(p, left);

            for (int i = 0; i < parameters.Length; i++)
            {
                CommaWhen(i > 0);
                WriteArgument(parameters[i], args[i]);
            }

            WriteWhen(p, right);
        }

        public void WriteArguments(IParametersEntity entity, Expression[] args, bool p = false, char left = '(', char right = ')')
        {
            WriteArguments(entity.Parameters, args, p, left, right);
        }

        public void WriteParameters(Source src, Parameter[] parameters, bool p = false, char left = '(', char right = ')')
        {
            WriteWhen(p, left);

            for (int i = 0; i < parameters.Length; i++)
            {
                CommaWhen(i > 0);
                WriteParameter(src, parameters[i].Type, parameters[i].Modifier, parameters[i].Name);
            }

            WriteWhen(p, right);
        }

        public void WriteParameters(IParametersEntity entity, bool p = false, char left = '(', char right = ')')
        {
            WriteParameters(entity.Source, entity.Parameters, p, left, right);
        }

        public virtual void WriteParameter(Source src, DataType dt, ParameterModifier m, string name)
        {
            switch (m)
            {
                case ParameterModifier.Ref:
                case ParameterModifier.Out:
                case ParameterModifier.Const:
                case ParameterModifier.This:
                    Write(m.ToString().ToLower() + " ");
                    break;
            }

            WriteType(src, dt);
            Write(" " + name);
        }

        public virtual void WriteType(Source src, DataType dt)
        {
            Write(dt);
        }

        public virtual void WriteGenericType(Source src, DataType dt)
        {
            WriteType(src, dt);
        }

        public virtual void WriteStaticType(Source src, DataType dt)
        {
            WriteType(src, dt);
        }

        public virtual void WriteCaughtException(Source src, DataType dt, string name)
        {
            WriteParameter(src, dt, 0, name);
        }

        public virtual bool HasVariable(DataType dt, Expression optionalValue)
        {
            return true;
        }

        public virtual void WriteCast(Source src, DataType dt, Expression s, ExpressionUsage u = ExpressionUsage.Argument)
        {
            Begin(u.IsObject());
            Write("(");
            WriteType(src, dt);
            Write(")");
            WriteExpression(s, ExpressionUsage.Operand);
            End(u.IsObject());
        }

        public virtual void WriteDownCast(Source src, DataType dt, Expression s, ExpressionUsage u = ExpressionUsage.Argument)
        {
            WriteCast(src, dt, s, u);
        }

        public virtual void WriteUpCast(Source src, DataType dt, Expression s, ExpressionUsage u = ExpressionUsage.Argument)
        {
            WriteExpression(s, u);
        }

        public virtual void WriteBox(Source src, DataType dt, Expression s, bool stack = false, ExpressionUsage u = ExpressionUsage.Argument)
        {
            WriteCast(src, dt, s, u);
        }

        public void WriteBox(Expression operand)
        {
            WriteBox(operand.Source, Essentials.Object, operand, true);
        }

        public virtual void WriteUnbox(Source src, DataType dt, Expression s, ExpressionUsage u = ExpressionUsage.Argument)
        {
            WriteCast(src, dt, s, u);
        }

        public virtual void WriteConstant(Constant c, ExpressionUsage u = ExpressionUsage.Argument)
        {
            if (c.Value == null)
                WriteDefault(c.Source, c.ReturnType, u);
            else if (c.Value is float)
                Write(((float)c.Value).ToLiteral(HasFloatSuffix, EnableMinify));
            else
                Write(c.Value.ToLiteral());
        }

        public virtual void WriteNewObject(Source src, DataType dt, ExpressionUsage u = ExpressionUsage.Argument)
        {
            Write("new ");
            WriteStaticType(src, dt);
            Write("()");
        }

        public virtual void WriteTypeOf(Source src, DataType dt, ExpressionUsage u = ExpressionUsage.Argument)
        {
            Write("typeof(");
            WriteType(src, dt);
            Write(")");
        }

        public virtual void WriteDefault(Source src, DataType dt, ExpressionUsage u = ExpressionUsage.Argument)
        {
            switch (dt.TypeType)
            {
                case TypeType.Enum:
                    Write("0");
                    return;
                case TypeType.Class:
                case TypeType.Interface:
                case TypeType.Delegate:
                case TypeType.RefArray:
                    Write("null");
                    return;
                case TypeType.Struct:
                    switch (dt.BuiltinType)
                    {
                        case BuiltinType.Bool:
                            Write("false");
                            return;
                        case BuiltinType.Byte:
                        case BuiltinType.SByte:
                        case BuiltinType.UShort:
                        case BuiltinType.Short:
                        case BuiltinType.UInt:
                        case BuiltinType.Int:
                        case BuiltinType.ULong:
                        case BuiltinType.Long:
                            Write("0");
                            return;
                        case BuiltinType.Double:
                            Write("0.0");
                            return;
                        case BuiltinType.Float:
                            Write("0.0");
                            if (HasFloatSuffix)
                                Write("f");
                            return;
                    }
                    break;
            }

            Write("default(");
            WriteStaticType(src, dt);
            Write(")");
        }

        void WriteExternString(Source src, string str, Expression obj, Expression[] args, Namescope[] usings)
        {
            // Extern vars are validated and decorated by ExternTransform
            const string Prefix = "@IL$";
            const int PrefixLength = 4;

            str = Environment.Expand(src, str, false, Function, usings);
            int i = 0;

            while (true)
            {
                var di = str.IndexOf(Prefix, i, StringComparison.Ordinal);

                if (di == -1 || di == str.Length - PrefixLength)
                    break;

                // Count backslashes
                int bsCount = 0;
                for (int bi = di - 1; bi >= 0 && str[bi] == '\\'; bi--)
                    bsCount++;

                WriteBlock(str.Substring(i, di - i - bsCount));
                i = di;

                // Add half of the backslashes because of escaping
                for (int j = 0; j < bsCount / 2; j++)
                    Write('\\');

                // Escape macro when bsCount is odd
                if ((bsCount & 1) == 1)
                {
                    Write(Prefix);
                    i += PrefixLength;
                    continue;
                }

                // Find end index
                var ei = di + PrefixLength;
                while (ei < str.Length - 1 && char.IsDigit(str[ei + 1]))
                    ei++;

                var u = ExpressionUsage.Argument;
                var ui = ei + 1;

                // Skip whitespace
                while (ui < str.Length && char.IsWhiteSpace(str[ui]))
                    ui++;

                // See if member operator is used
                if (ui < str.Length && str[ui] == '.' ||                            // .
                    ui + 2 < str.Length && str[ui] == '-' && str[ui + 1] == '>')    // ->
                    u = ExpressionUsage.Object;

                switch (str[di + PrefixLength])
                {
                    case '{':
                    {
                        ei = di + PrefixLength;
                        BeginReturn();
                        break;
                    }
                    case '}':
                    {
                        ei = di + PrefixLength;
                        EndReturn();
                        break;
                    }
                    case '$':
                    {
                        WriteExpression(obj, u);
                        break;
                    }
                    default:
                    {
                        var startIndex = di + PrefixLength;
                        var length = ei + 1 - startIndex;
                        var arg = str.Substring(startIndex, length);

                        try
                        {
                            WriteExpression(args[int.Parse(arg)], u);
                        }
                        catch (Exception e)
                        {
                            Log.Error(src, ErrorCode.I0000, "'$" + arg + "': " + e.Message);
                        }
                        break;
                    }
                }

                i = ei + 1;
            }

            WriteBlock(str.Substring(i));
        }
    }
}
