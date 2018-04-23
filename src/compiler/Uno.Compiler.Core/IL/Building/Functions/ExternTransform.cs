using System;
using System.Text;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Building.Functions
{
    class ExternTransform : CompilerPass
    {
        public ExternTransform(CompilerPass parent)
            : base(parent)
        {
        }

        public override bool Condition => !Backend.IsDefault;

        public override bool Begin(Function f)
        {
            return !f.HasAttribute(Essentials.ForeignAttribute);
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.ExternOp:
                {
                    var s = (ExternOp) e;
                    s.String = ExpandExtern(s.Source, s.String, s.Object, s.Arguments);
                    break;
                }
                case ExpressionType.ExternString:
                {
                    var s = (ExternString) e;
                    e = new Constant(s.Source, s.Type, Environment.ExpandSingleLine(s.Source, s.String));
                    break;
                }
            }
        }

        public override void Begin(ref Statement e)
        {
            switch (e.StatementType)
            {
                case StatementType.ExternScope:
                {
                    var s = (ExternScope) e;
                    s.String = ExpandExtern(s.Source, s.String, s.Object, s.Arguments);

                    if (!Function.ReturnType.IsVoid)
                        s.String = ExpandReturn(s.String);
                    break;
                }
            }
        }

        string ExpandReturn(string str)
        {
            var sb = new StringBuilder();
            int i = 0;

            while (true)
            {
                var di = str.IndexOf("return", i, StringComparison.Ordinal);

                if (di == -1)
                    break;

                var ei = str.IndexOf(';', di + 6);

                if (ei == -1)
                    break;

                var ri = di + 6;
                while (ri < ei && str[ri] == ' ')
                    ri++;

                // Verify that return statement is valid
                if (ei > ri &&
                    (di == 0 || !IsIdentifier(str[di - 1])) &&
                    !IsIdentifier(str[di + 6]))
                {
                    sb.Append(str, i, di - i);
                    sb.Append("@IL${");
                    sb.Append(str, ri, ei - ri);
                    sb.Append("@IL$};");
                }
                else
                    sb.Append(str, i, ei - i + 1);

                i = ei + 1;
            }

            sb.Append(str, i, str.Length - i);
            return sb.ToString();
        }

        string ExpandExtern(Source src, string str, Expression obj, Expression[] args)
        {
            var sb = new StringBuilder();
            int i = 0;

            while (true)
            {
                var di = str.IndexOf('$', i);

                if (di == -1 || di == str.Length - 1)
                    break;

                var nc = str[di + 1];

                if (nc == '$' || nc == '@' || char.IsDigit(nc))
                {
                    // Count backslashes
                    int bsCount = 0;
                    for (int bi = di - 1; bi >= 0 && str[bi] == '\\'; bi--)
                        bsCount++;

                    sb.Append(str, i, di - i - bsCount);
                    i = di;

                    // Escape backslashes
                    for (int j = 0; j < bsCount / 2; j++)
                        sb.Append('\\');

                    // Escape macro (when bsCount is odd)
                    if ((bsCount & 1) == 1)
                    {
                        sb.Append('$');
                        i++;
                        continue;
                    }

                    // Find end index
                    var ei = di + 1;
                    while (ei < str.Length - 1 && char.IsDigit(str[ei + 1]))
                        ei++;

                    // Check if the macro was found inside an identifier
                    if (di > 0 && IsIdentifier(str[di - 1]) ||
                        ei < str.Length - 1 && IsIdentifier(str[ei + 1]))
                    {
                        while (di > 0 && char.IsLetterOrDigit(str[di - 1]))
                            di--;
                        while (ei < str.Length - 1 && char.IsLetterOrDigit(str[ei + 1]))
                            ei++;

                        sb.Append(str, i, ei + 1 - i);
                    }
                    else
                    {
                        var isInsideMacro = di >= 2 && str[di - 1] == '{' && str[di - 2] == '@';

                        switch (nc)
                        {
                            case '$':
                            {
                                if (obj == null)
                                    Log.Error(src, ErrorCode.E9006, "'$$' is not allowed in static context");
                                else if (isInsideMacro)
                                    sb.Append(GetTypeName(obj.ReturnType) + ":Of(@IL$$)");
                                else
                                    sb.Append("@IL$$");
                                break;
                            }
                            case '@':
                            {
                                for (int argIndex = 0; argIndex < args.Length; argIndex++)
                                {
                                    sb.CommaWhen(argIndex > 0);
                                    sb.Append("@IL$" + argIndex);
                                }
                                break;
                            }
                            default:
                            {
                                int argIndex = int.Parse(str.Substring(di + 1, ei - di));

                                if (argIndex >= args.Length)
                                    Log.Error(src, ErrorCode.E9007, "'$" + argIndex + "' is out of range");
                                else if (isInsideMacro)
                                    sb.Append(GetTypeName(args[argIndex].ReturnType) + ":Of(@IL$" + argIndex + ")");
                                else
                                    sb.Append("@IL$" + argIndex);
                                break;
                            }
                        }
                    }

                    i = ei + 1;
                    continue;
                }

                sb.Append(str, i, di + 1 - i);
                i = di + 1;
            }

            sb.Append(str, i, str.Length - i);
            return sb.ToString();
        }

        string GetTypeName(DataType dt)
        {
            return dt.IsArray
                ? GetTypeName(dt.ElementType) + "[]"
                : (!dt.IsIntrinsic && !dt.IsVoid
                        ? "global::"
                        : null
                    ) + dt;
        }

        bool IsIdentifier(char c)
        {
            return c == '_' || char.IsLetterOrDigit(c);
        }
    }
}
