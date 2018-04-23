using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.API.Backends.Decompilers
{
    public abstract partial class SourceWriter
    {
        protected void UnsupportedStatement(Statement e)
        {
            Log.Error(e.Source, ErrorCode.I0000, "<" + e.StatementType+ "> is not supported by this backend");
            WriteLine("<error>;");
        }

        public void WriteStatement(Statement s)
        {
            switch (s.StatementType)
            {
                case StatementType.Expression: WriteExpressionStatement(s as Expression); break;
                case StatementType.VariableDeclaration: WriteVariableDeclaration(s as VariableDeclaration); break;
                case StatementType.FixedArrayDeclaration: WriteFixedArrayDeclaration(s as FixedArrayDeclaration); break;
                case StatementType.ExternScope: WriteExternScope(s as ExternScope); break;
                case StatementType.TryCatchFinally: WriteTryCatchFinally(s as TryCatchFinally); break;
                case StatementType.Throw: WriteThrow(s as Throw); break;
                case StatementType.Switch: WriteSwitch(s as Switch); break;
                case StatementType.While: WriteWhile(s as While); break;
                case StatementType.For: WriteFor(s as For); break;
                case StatementType.IfElse: WriteIfElse(s as IfElse); break;
                case StatementType.Return: WriteReturn(s as Return); break;
                case StatementType.Break: WriteBreak(s as Break); break;
                case StatementType.Continue: WriteContinue(s as Continue); break;
                case StatementType.Draw: WriteDrawCall(s as Draw); break;
                case StatementType.Scope: WriteScope((Scope)s, false, false); break;
                default: UnsupportedStatement(s); break;
            }
        }

        public void WriteExpressionStatement(Expression s)
        {
            if (s is SequenceOp)
            {
                var o = s as SequenceOp;
                WriteExpressionStatement(o.Left);
                WriteExpressionStatement(o.Right);
                return;
            }

            BeginLine();
            WriteExpression(s, ExpressionUsage.Statement);
            EndLine(";");
        }

        public virtual void WriteVariableDeclaration(VariableDeclaration s)
        {
            if (!HasVariable(s.Variable.ValueType, s.Variable.OptionalValue))
                return;

            BeginLine();
            WriteVariable(s.Variable);
            EndLine(";");
        }

        public virtual void WriteVariable(Variable var)
        {
            WriteType(var.Source, var.ValueType);
            Write(" " + var.Name);

            if (var.OptionalValue != null)
            {
                Write(Assign);
                WriteExpression(var.OptionalValue);
            }

            for (var = var.Next; var != null; var = var.Next)
            {
                if (!HasVariable(var.ValueType, var.OptionalValue))
                    continue;

                Write(Comma + var.Name);
                if (var.OptionalValue != null)
                {
                    Write(Assign);
                    WriteExpression(var.OptionalValue);
                }
            }
        }

        public virtual void WriteFixedArrayDeclaration(FixedArrayDeclaration s)
        {
            UnsupportedStatement(s);

            /*
            BeginLine();

            var fat = (FixedArrayType)s.Variable.DataType;

            WriteDataType(s.Source, fat.ElementType);
            Write(" " + s.Variable.Name + "[");

            if (fat.OptionalSize != null)
                WriteExpression(fat.OptionalSize);

            Write("]");

            if (s.OptionalInitializer != null)
            {
                EndLine(" =");
                PushScope();

                foreach (var init in s.OptionalInitializer)
                {
                    BeginLine();
                    WriteExpression(init);
                    EndLine(",");
                }

                PopScopeSemicolon();
            }
            else
            {
                EndLine(";");
            }
            */
        }

        public void WriteDrawCall(Draw s)
        {
            UnsupportedStatement(s);
        }

        public virtual void WriteTryCatchFinally(TryCatchFinally s)
        {
            Skip();
            WriteLine("try");
            WriteScope(s.TryBody, true, false);

            foreach (var c in s.CatchBlocks)
            {
                BeginLine("catch (");
                WriteCaughtException(c.Exception.Source, c.Exception.ValueType, c.Exception.Name);
                EndLine(")");
                WriteScope(c.Body, true, false);
            }

            if (s.OptionalFinallyBody != null)
            {
                WriteLine("finally");
                WriteScope(s.OptionalFinallyBody, true, false);
            }

            Skip();
        }

        public virtual void WriteThrow(Throw s)
        {
            BeginLine("throw ");
            WriteExpression(s.Exception);
            EndLine(";");
        }

        public virtual void BeginReturn(Expression value = null)
        {
            Write("return");
            WriteWhen(!Function.ReturnType.IsVoid, " ");
        }

        public virtual void EndReturn(Expression value = null)
        {
        }

        public void WriteReturn(Return s)
        {
            BeginLine();
            BeginReturn(s.Value);

            if (s.Value != null)
                WriteExpression(s.Value);

            EndReturn(s.Value);
            EndLine(";");
        }

        public void WriteFor(For s)
        {
            Skip();
            BeginLine("for" + Space + "(");

            if (s.OptionalInitializer != null)
            {
                switch (s.OptionalInitializer.StatementType)
                {
                    case StatementType.Expression:
                    {
                        WriteExpression((Expression) s.OptionalInitializer, ExpressionUsage.Statement);
                        break;
                    }
                    case StatementType.VariableDeclaration:
                    {
                        var vd = (VariableDeclaration) s.OptionalInitializer;
                        WriteVariable(vd.Variable);
                        break;
                    }
                    default:
                    {
                        Log.Error(s.Source, ErrorCode.I0048, "Unsupported 'for' initializer");
                        break;
                    }
                }
            }

            Write(";" + Space);

            if (s.OptionalCondition != null)
                WriteExpression(s.OptionalCondition);

            Write(";" + Space);

            if (s.OptionalIncrement != null)
                WriteExpression(s.OptionalIncrement, ExpressionUsage.Statement);

            EndLine(")");
            WriteShortScope(s.OptionalBody);
        }

        public void WriteWhile(While s)
        {
            Skip();

            if (s.DoWhile)
                WriteLine("do");
            else
            {
                BeginLine("while" + Space + "(");
                WriteExpression(s.Condition);
                EndLine(")");
            }

            WriteShortScope(s.OptionalBody);

            if (s.DoWhile)
            {
                DisableSkip();
                BeginLine("while" + Space + "(");
                WriteExpression(s.Condition);
                EndLine(");");
                Skip();
            }
        }

        public void WriteSwitch(Switch s)
        {
            Skip();
            BeginLine("switch" + Space + "(");
            WriteExpression(s.ControlVariable);
            Write(")");
            EndLine();
            BeginScope();

            foreach (var c in s.Cases)
            {
                foreach (var v in c.Values)
                {
                    BeginLine();
                    Write("case ");
                    WriteExpression(v);
                    Write(":");
                    EndLine();
                }

                if (c.HasDefault)
                    WriteLine("default:");

                WriteShortScope(c.Scope);
                DisableSkip();
            }

            EndScope();
        }

        public void WriteBreak(Break s)
        {
            WriteLine("break;");
        }

        public void WriteContinue(Continue s)
        {
            WriteLine("continue;");
        }

        public void WriteExternScope(ExternScope s)
        {
            BeginLine();
            WriteExternString(s.Source, s.String, s.Object, s.Arguments, s.Scopes);
            EndLine();
        }

        public void WriteIfElse(IfElse s, bool chain = false)
        {
            if (!chain)
            {
                Skip();
                BeginLine();
            }

            Write("if" + Space + "(");
            WriteExpression(s.Condition);
            EndLine(")");
            WriteShortScope(s.OptionalIfBody, true);

            if (s.OptionalElseBody != null)
            {
                DisableSkip();
                BeginLine("else");
                var elseIf = s.OptionalElseBody as IfElse;

                if (elseIf != null)
                {
                    Write(" ");
                    WriteIfElse(elseIf, true);
                }
                else
                {
                    // Avoid missing space in a minified shader, e.g.: `... elsereturn ...`
                    if (EnableMinify && s.OptionalElseBody.StatementType != StatementType.Scope)
                        Write(" ");
                    else
                        EndLine();

                    WriteShortScope(s.OptionalElseBody, true);
                }
            }
            else
                Skip();
        }

        void WriteShortScope(Scope scope, bool parentIsIfElse = false)
        {
            switch (scope.Statements.Count)
            {
                case 0:
                    Indent();
                    WriteLine(";");
                    Unindent();
                    Skip();
                    break;
                case 1:
                    if (scope.Statements[0] is Scope)
                        WriteShortScope((Scope)scope.Statements[0], parentIsIfElse);
                    else if (EmitBraces(scope.Statements[0], parentIsIfElse))
                        WriteScope(scope);
                    else
                    {
                        Indent();

                        if (scope.Statements[0] is IfElse)
                        {
                            BeginLine();
                            WriteIfElse((IfElse)scope.Statements[0], true);
                        }
                        else
                            WriteStatement(scope.Statements[0]);

                        Unindent();
                        Skip();
                    }
                    break;
                default:
                    WriteScope(scope);
                    break;
            }
        }

        void WriteShortScope(Statement s, bool parentIsIfElse = false)
        {
            var scope = s as Scope;
            if (scope != null)
                WriteShortScope(scope, parentIsIfElse);
            else if (s == null)
            {
                Indent();
                WriteLine(";");
                Unindent();
                Skip();
            }
            else
            {
                var braces = EmitBraces(s, parentIsIfElse);
                var ie = s as IfElse;

                if (braces)
                    BeginScope();
                else
                    Indent();

                if (ie != null)
                {
                    BeginLine();
                    WriteIfElse(ie, true);
                }
                else
                    WriteStatement(s);

                if (braces)
                    EndScope();
                else
                    Unindent();

                Skip();
            }
        }

        bool EmitBraces(Statement s, bool parentIsIfElse)
        {
            return s is VariableDeclaration ||
                   s is SequenceOp ||
                   s is ExternScope ||
                   parentIsIfElse && (
                       s is IfElse ||
                       s is TryCatchFinally
                       );
        }

        public void WriteScope(Scope s, bool curlyBraces = true, bool skip = true)
        {
            if (curlyBraces)
                BeginScope();

            for (var i = 0; i < s.Statements.Count; i++)
                WriteStatement(s.Statements[i]);

            if (curlyBraces)
                EndScope(skip);
        }
    }
}
