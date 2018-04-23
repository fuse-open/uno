using System.Text;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Disasm.Disassemblers
{
    public class UnoDisassembler : Disassembler
    {
        public UnoDisassembler(StringBuilder sb = null)
            : base(sb)
        {
        }

        public override string ToString()
        {
            return "High-Level IL";
        }

        protected override void AppendBody(Function function)
        {
            foreach (var s in function.Body.Statements)
                AppendStatement(s, "        ");
        }

        void AppendScope(Statement s, string indent)
        {
            if (s is Scope)
                AppendStatement(s, indent);
            else if (s != null)
                AppendStatement(s, indent + "    ");
            else
                AppendLine(indent + "    ;");
        }

        void AppendIfElse(IfElse s, string indent, bool parentWasElse = false)
        {
            Append(parentWasElse ? " " : indent);
            AppendLine("if (" + s.Condition + ")");
            AppendScope(s.OptionalIfBody, indent);

            if (s.OptionalElseBody != null)
            {
                Append(indent + "else");

                if (s.OptionalElseBody is IfElse)
                {
                    AppendIfElse(s.OptionalElseBody as IfElse, indent, true);
                }
                else
                {
                    AppendLine();
                    AppendScope(s.OptionalElseBody, indent);
                }
            }
        }

        public void AppendStatement(Statement x, string indent = "")
        {
            if (x == null)
            {
                AppendLine(indent + "/* null */;");
                return;
            }

            switch (x.StatementType)
            {
                case StatementType.Expression:
                    AppendLine(indent + x + ";");
                    break;

                case StatementType.Scope:
                    {
                        AppendLine(indent + "{");

                        foreach (var c in (x as Scope).Statements)
                            AppendStatement(c, indent + "    ");

                        AppendLine(indent + "}");
                    }

                    break;

                case StatementType.While:
                    {
                        var s = x as While;

                        if (s.DoWhile)
                        {
                            AppendLine(indent + "do");
                            AppendScope(s.OptionalBody, indent);
                            AppendLine(indent + "while (" + s.Condition + ")");
                        }
                        else
                        {
                            AppendLine(indent + "while (" + s.Condition + ")");
                            AppendScope(s.OptionalBody, indent);
                        }
                    }

                    break;

                case StatementType.For:
                    {
                        var s = x as For;

                        Append(indent + "for (");

                        if (s.OptionalInitializer is VariableDeclaration)
                        {
                            var var = (s.OptionalInitializer as VariableDeclaration).Variable;
                            Append(var.ValueType + " " + var.Name);

                            if (var.OptionalValue != null)
                                Append(" = " + var.OptionalValue);

                            for (var = var.Next;
                                var != null;
                                var = var.Next)
                            {
                                Append(", " + var.Name);
                                if (var.OptionalValue != null)
                                    Append(" = " + var.OptionalValue);
                            }
                        }
                        else if (s.OptionalInitializer != null)
                            Append(s.OptionalInitializer);

                        Append("; ");

                        if (s.OptionalCondition != null)
                            Append(s.OptionalCondition);

                        Append("; ");

                        if (s.OptionalIncrement != null)
                            Append(s.OptionalIncrement);

                        AppendLine(")");
                        AppendScope(s.OptionalBody, indent);
                    }

                    break;

                case StatementType.Switch:
                    {
                        var s = x as Switch;

                        AppendLine(indent + "switch (" + s.ControlVariable + ")");
                        AppendLine(indent + "{");

                        foreach (var c in s.Cases)
                        {
                            foreach (var v in c.Values)
                                AppendLine(indent + "    case " + v + ":");

                            if (c.HasDefault)
                                AppendLine(indent + "    default:");

                            AppendScope(c.Scope, indent + "    ");
                        }

                        AppendLine(indent + "}");
                    }

                    break;

                case StatementType.IfElse:
                    AppendIfElse(x as IfElse, indent);
                    break;

                case StatementType.Return:
                    {
                        var s = x as Return;

                        if (s.Value != null)
                            AppendLine(indent + "return " + s.Value + ";");
                        else
                            AppendLine(indent + "return;");
                    }

                    break;

                case StatementType.Break:
                    AppendLine(indent + "break;");
                    break;

                case StatementType.Continue:
                    AppendLine(indent + "continue;");
                    break;

                case StatementType.Throw:
                    AppendLine(indent + "throw " + (x as Throw).Exception + ";" + ((x as Throw).IsRethrow ? " // Rethrow" : ""));
                    break;

                case StatementType.TryCatchFinally:
                    {
                        var s = x as TryCatchFinally;

                        AppendLine(indent + "try");
                        AppendScope(s.TryBody, indent);

                        foreach (var c in s.CatchBlocks)
                        {
                            AppendLine(indent + "catch (" + c.Exception.ValueType + " " + c.Exception.Name + ")");
                            AppendScope(c.Body, indent);
                        }

                        if (s.OptionalFinallyBody != null)
                        {
                            AppendLine(indent + "finally");
                            AppendScope(s.OptionalFinallyBody, indent);
                        }
                    }

                    break;

                case StatementType.VariableDeclaration:
                    {
                        var s = x as VariableDeclaration;
                        var var = s.Variable;
                        Append(indent + 
                            var.VariableType.ToLiteral(true) +
                            var.ValueType + " " + var.Name);

                        if (var.OptionalValue != null)
                            Append(" = " + var.OptionalValue);

                        for (var = var.Next;
                             var != null;
                             var = var.Next)
                        {
                            Append(", " + var.Name);
                            if (var.OptionalValue != null)
                                Append(" = " + var.OptionalValue);
                        }

                        AppendLine(";");
                    }

                    break;

                case StatementType.FixedArrayDeclaration:
                    {
                        var s = x as FixedArrayDeclaration;
                        var fat = (FixedArrayType) s.Variable.ValueType;

                        Append(indent + "fixed " + fat.ElementType + " " + s.Variable.Name + "[" + fat.OptionalSize + "]");

                        if (s.OptionalInitializer != null)
                        {
                            AppendLine(" =");
                            AppendLine(indent + "{");

                            foreach (var v in s.OptionalInitializer)
                                AppendLine(indent + "    " + v + ",");

                            Append(indent + "}");
                        }

                        AppendLine(";");
                    }

                    break;

                case StatementType.Draw:
                    {
                        var s = x as Draw;

                        AppendLine(indent + "draw " + s.State.Path.DrawableBlock.ToString().ToLiteral());
                        AppendLine(indent + "{");

                        if (s.State.OptionalIndices != null)
                        {
                            AppendLine(indent + "    \"IndexBuffer\":");
                            AppendLine(indent + "    {");
                            AppendLine(indent + "        \"Type\": " + s.State.OptionalIndices.IndexType + ",");
                            AppendLine(indent + "        \"Buffer\": " + s.State.OptionalIndices.Buffer);
                            AppendLine(indent + "    },");
                        }

                        if (s.State.VertexAttributes.Count > 0)
                        {
                            AppendLine(indent + "    \"VertexAttributes\":");
                            AppendLine(indent + "    {");

                            foreach (var t in s.State.VertexAttributes)
                            {
                                AppendLine(indent + "        \"" + t.Name + "\":");
                                AppendLine(indent + "        {");
                                AppendLine(indent + "            \"Type\": " + t.Type + ",");
                                AppendLine(indent + "            \"Buffer\": " + t.Buffer + ",");
                                AppendLine(indent + "            \"Stride\": " + t.Stride + ",");
                                AppendLine(indent + "            \"Offset\": " + t.Offset);
                                AppendLine(indent + "        },");
                            }

                            AppendLine(indent + "    },");
                        }

                        if (s.State.PixelSamplers.Count > 0)
                        {
                            AppendLine(indent + "    \"PixelSamplers\":");
                            AppendLine(indent + "    {");

                            foreach (var t in s.State.PixelSamplers)
                                AppendLine(indent + "        \"" + t.Name + "\": " + t.Texture + ",");

                            AppendLine(indent + "    },");
                        }

                        if (s.State.RuntimeConstants.Count > 0)
                        {
                            AppendLine(indent + "    \"ShaderConstants\":");
                            AppendLine(indent + "    {");

                            foreach (var t in s.State.RuntimeConstants)
                                AppendLine(indent + "        \"" + t.Name + "\": " + t.Value + ",");

                            AppendLine(indent + "    },");
                        }

                        if (s.State.Uniforms.Count > 0)
                        {
                            AppendLine(indent + "    \"ShaderUniforms\":");
                            AppendLine(indent + "    {");

                            foreach (var t in s.State.Uniforms)
                                AppendLine(indent + "        \"" + t.Name + "\": " + t.Value + ",");

                            AppendLine(indent + "    },");
                        }

                        if (s.State.Terminals.Count > 0)
                        {
                            AppendLine(indent + "    \"RenderState\":");
                            AppendLine(indent + "    {");

                            foreach (var t in s.State.Terminals)
                                AppendLine(indent + "        \"" + t.Key + "\": " + t.Value + ",");

                            AppendLine(indent + "    },");
                        }

                        AppendLine(indent + "};");
                    }

                    break;

                case StatementType.DrawDispose:
                    AppendLine(indent + "draw_dispose;");
                    break;

                case StatementType.ExternScope:
                {
                    var s = (ExternScope) x;

                    if (s.Object != null)
                        Append(indent + s.Object + ".");
                    else
                        Append(indent);

                    AppendLine("extern" + s.Arguments.Disassemble());
                    AppendLine(indent + "@{");
                    AppendLine(indent + "    " + s.String.Replace("\n", "\n" + indent + "    "));
                    AppendLine(indent + "@}");
                    break;
                }

                default:
                    AppendLine(indent + "<" + x.StatementType + ">;");
                    break;
            }
        }
    }
}
