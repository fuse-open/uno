using System.Linq;
using System.Text;
using Uno.Compiler;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Disasm.Disassemblers;

namespace Uno.Disasm
{
    public abstract class Disassembler
    {
        public static readonly Disassembler[] Items =
        {
            new UnoDisassembler(),
            new BytecodeDisassembler(),
        };

        readonly StringBuilder _sb;
        protected abstract void AppendBody(Function function);

        protected Disassembler(StringBuilder sb = null)
        {
            _sb = sb ?? new StringBuilder();
        }

        public void Clear()
        {
            _sb.Clear();
        }

        public string GetText()
        {
            return _sb.ToString();
        }

        public void Append(object value)
        {
            _sb.Append(value);
        }

        public void Append(string value)
        {
            _sb.Append(value);
        }

        public void Append(char value)
        {
            _sb.Append(value);
        }

        public void Append(char value, int count)
        {
            _sb.Append(value, count);
        }

        public void AppendLine()
        {
            _sb.AppendLine();
        }

        public void AppendLine(string value)
        {
            _sb.AppendLine(value);
        }

        public void BeginHeader(string value)
        {
            value = value ?? "(null)";
            AppendLine("///");
            AppendLine("/// " + value);
            AppendLine("/// " + new string('-', value.Length));
        }

        public void BeginHeader(object value)
        {
            BeginHeader((value ?? "(null)").ToString());
        }

        public void EndHeader()
        {
            AppendLine("///");
            AppendLine();
        }

        public void AppendHeader(object obj)
        {
            BeginHeader(obj);
            EndHeader();
        }

        public void AppendHeader(SourcePackage upk)
        {
            BeginHeader(upk);
            AppendLine("/// Source:             " + upk.Source);
            AppendLine("/// CacheDirectory:     " + upk.CacheDirectory);
            AppendLine("/// SourceDirectory:    " + upk.SourceDirectory);
            if (!string.IsNullOrEmpty(upk.BuildCondition))
                AppendLine("/// BuildCondition:     " + upk.BuildCondition);
            if (upk.InternalsVisibleTo.Count > 0)
                AppendLine("/// InternalsVisibleTo: " + string.Join(", ", upk.InternalsVisibleTo));
            if (upk.Flags != 0)
                AppendLine("/// Flags:              " + upk.Flags);
            EndHeader();
        }

        public void AppendHeader(DataType dt)
        {
            BeginHeader(dt);
            Append("/// SourceFile(s):  ");

            bool firstSource = true;
            foreach (var f in dt.SourceFiles)
            {
                if (!firstSource)
                    Append("\n///                 ");
                else
                    firstSource = false;

                Append(f);
            }

            AppendLine();

            if (dt.Prototype != dt)
                AppendLine("/// Prototype:      " + dt.Prototype);
            if (dt.Stats != 0)
                AppendLine("/// Flags:          " + dt.Stats);

            if (dt.IsFlattenedParameterization)
            {
                Append("/// TypeArguments:  ");

                for (int i = 0; i < dt.FlattenedArguments.Length; i++)
                {
                    if (i > 0) Append("\n///                 ");
                    Append(dt.MasterDefinition.FlattenedParameters[i].Name + " = " + dt.FlattenedArguments[i].FullName);
                }

                AppendLine();
            }

            if (dt.InterfaceMethods.Count > 0)
            {
                Append("/// Interfaces:     ");
                var first = true;

                foreach (var impl in dt.InterfaceMethods)
                {
                    if (!first)
                        Append("\n///                 ");
                    else
                        first = false;

                    Append(impl.Key + " = " + impl.Value);
                }

                AppendLine();
            }

            EndHeader();
        }

        public void AppendHeader(Function func)
        {
            BeginHeader(func);
                AppendLine("/// SourceFile:     " + func.Source.FullPath);
            if (func.Prototype != func)
                AppendLine("/// Prototype:      " + func.Prototype);
            if (func.Stats != 0)
                AppendLine("/// Flags:          " + func.Stats);
            if (func is Method && (func as Method).OverriddenMethod != null)
                AppendLine("/// Overrides:      " + (func as Method).OverriddenMethod);
            if (func is Method && (func as Method).ImplementedMethod != null)
                AppendLine("/// Implements:     " + (func as Method).ImplementedMethod);
            if (func is Method && (func as Method).IsGenericParameterization)
            {
                Append("/// TypeArguments:  ");

                for (int i = 0; i < (func as Method).GenericArguments.Length; i++)
                {
                    if (i > 0) Append("\n///                 ");
                    var t = (func as Method).GenericArguments[i];

                    if (t is GenericParameterType)
                        Append(t.Parent + "." + t.Name);
                    else
                        Append(t);
                }

                AppendLine();
            }

            EndHeader();
        }

        public void AppendHeader(Member member)
        {
            BeginHeader(member);
                AppendLine("/// SourceFile:     " + member.Source.FullPath);
            if (member.Prototype != member)
                AppendLine("/// Prototype:      " + member.Prototype);
            if (member.Stats != 0)
                AppendLine("/// Flags:          " + member.Stats);
            EndHeader();
        }

        public void BeginNamespace(Namespace ns)
        {
            AppendLine("namespace " + ns);
            AppendLine("{");
        }

        public void EndNamespace()
        {
            AppendLine("}");
        }

        public void AppendType(DataType dt, bool publicOnly)
        {
            AppendDocComment(dt.DocComment);

            foreach (var attr in dt.Attributes)
                AppendAttribute(attr, 1);

            if (dt is EnumType)
                AppendEnum(dt as EnumType);
            else if (dt is DelegateType)
                AppendDelegate(dt as DelegateType);
            else
                AppendClass(dt, publicOnly);
        }

        void Skip(char c = '\0')
        {
            var last = _sb.Length - 1;
            while (char.IsWhiteSpace(_sb[last]))
                last--;
            if (_sb[last] != '{' && _sb[last] != ']' && _sb[last] != '/' && _sb[last] != c)
                AppendLine();
        }

        void AppendEnum(EnumType dt)
        {
            Skip();
            Append("    ");
            AppendModifiers(dt.Modifiers);
            AppendLine("enum " + dt.GetNestedName() + " : " + dt.Base);
            AppendLine("    {");

            foreach (var v in dt.Literals)
                AppendLine("        " + v.Name + " = " + v.Value.ToLiteral() + ",");

            AppendLine("    }");
        }

        void AppendDelegate(DelegateType dt)
        {
            Skip(';');
            Append("    ");
            AppendModifiers(dt.Modifiers);
            Append("delegate " + dt.ReturnType + " " + dt.GetNestedName());

            if (dt.IsGenericDefinition)
            {
                Append("<");
                for (int i = 0; i < dt.GenericParameters.Length; i++)
                {
                    if (i > 0) Append(",");
                    Append(dt.GenericParameters[i].Name);
                }
                Append(">");
            }

            Append("(");

            bool first = true;
            foreach (var p in dt.Parameters)
            {
                if (!first)
                    Append(", ");
                else
                    first = false;

                Append(p.Modifier.ToLiteral(true) + p.Type + " " + p.Name);
            }

            AppendLine(");");
        }

        void AppendClass(DataType dt, bool publicOnly)
        {
            Skip();
            var prefix = "    " + GetPrefix(dt);
            int alignLen = prefix.Length + 1;
            prefix += " " + dt.GetNestedName();

            if (dt.Base != null)
            {
                alignLen = prefix.Length + 3;
                prefix += " : " + dt.Base;

                if (dt.Interfaces.Length > 0)
                    prefix += ",";
            }

            AppendLine(prefix);

            for (int i = 0; i < dt.Interfaces.Length; i++)
            {
                Append(new string(' ', alignLen));
                Append(dt.Interfaces[i]);

                if (i < dt.Interfaces.Length - 1)
                    AppendLine(",");
                else
                    AppendLine();
            }

            if (dt.IsGenericDefinition)
                AppendConstraints(dt.GenericParameters);
            else if (dt.IsGenericParameterization)
                AppendConstraints(dt.GenericDefinition.GenericParameters);

            AppendLine("    {");

            var literals = dt.Literals.Where(x => !publicOnly || x.IsPublic && !x.IsGenerated).ToArray();
            var fields = dt.Fields.Where(x => !publicOnly || x.IsPublic && !x.IsGenerated).ToArray();
            var events = dt.Events.Where(x => !publicOnly || x.IsPublic && !x.IsGenerated).ToArray();
            var properties = dt.Properties.Where(x => !publicOnly || x.IsPublic && !x.IsGenerated).ToArray();
            var constructors = dt.Constructors.Where(x => !publicOnly || x.IsPublic && !x.IsGenerated).ToArray();
            var methods = dt.Methods.Where(x => !publicOnly || x.IsPublic && !x.IsGenerated).ToArray();
            var operators = dt.Operators.Where(x => !publicOnly || x.IsPublic && !x.IsGenerated).ToArray();
            var casts = dt.Casts.Where(x => !publicOnly || x.IsPublic && !x.IsGenerated).ToArray();

            if (literals.Length > 0)
            {
                AppendComment("Constants");
                foreach (var literal in literals)
                    AppendLiteral(literal, true);
            }

            if (fields.Length > 0)
            {
                AppendComment("Fields");
                foreach (var field in fields)
                    AppendField(field, true);
            }

            if (events.Length > 0)
            {
                AppendComment("Events");
                foreach (var ev in events)
                    AppendEvent(ev, true);
            }

            if (properties.Length > 0)
            {
                AppendComment("Properties");
                foreach (var prop in properties)
                    AppendProperty(prop, true);
            }

            if (constructors.Length > 0)
            {
                AppendComment("Constructors");
                foreach (var ctor in constructors)
                    AppendSignature(ctor, true);
            }

            if (methods.Length > 0)
            {
                AppendComment("Methods");
                foreach (var method in methods)
                    AppendSignature(method, true);
            }

            if (operators.Length > 0)
            {
                AppendComment("Operators");
                foreach (var method in operators)
                    AppendSignature(method, true);
            }

            if (casts.Length > 0)
            {
                AppendComment("Casts");
                foreach (var method in casts)
                    AppendSignature(method, true);
            }

            AppendLine("    }");
        }

        void AppendAttribute(NewObject attr, int indent)
        {
            Skip();
            Append(new string(' ', indent * 4));
            AppendLine(("[" + attr.ToString().Substring(4) + "]").Replace("()", "").Replace("Attribute(", "(").Replace("Attribute]", "]"));
        }

        void AppendAttributes(NewObject[] attrs, bool isClass)
        {
            foreach (var attr in attrs)
                AppendAttribute(attr, isClass ? 2 : 1);
            Append(isClass ? "        " : "    ");
        }

        void AppendDocComment(string comment, bool isClass = false)
        {
            if (string.IsNullOrEmpty(comment))
                return;

            Skip();
            Append(isClass ? "        " : "    ");
            AppendLine(comment.Replace("\n", "\n" + (isClass ? "        " : "    ")));
        }

        void AppendComment(string comment, int indent = 2)
        {
            Skip();
            Append(new string(' ', indent * 4));
            AppendLine("// " + comment);
        }

        void AppendModifiers(Modifiers m, DataType parent = null)
        {
            if (parent != null && parent.IsInterface)
                m &= ~(Modifiers.Public | Modifiers.Abstract);
            Append(m.ToLiteral(true));
        }

        static string GetPrefix(DataType dt)
        {
            var m = dt.Modifiers;
            if (dt.IsInterface)
                m &= ~Modifiers.Abstract;
            return m.ToLiteral(true) + dt.TypeType.ToLiteral();
        }

        void AppendConstraints(GenericParameterType[] paramTypes)
        {
            foreach (var gt in paramTypes)
            {
                var hasBaseTypeConstraint = gt.Base != null && gt.Base.BuiltinType != BuiltinType.Object;
                var hasInterfaceTypeConstraint = gt.Interfaces.Length > 0;
                var hasDefCtorConstraint = gt.Constructors.Count > 0;

                if (hasBaseTypeConstraint ||
                    hasInterfaceTypeConstraint ||
                    hasDefCtorConstraint ||
                    gt.ConstraintType != 0)
                {
                    Append("        where " + gt.Name + " : ");

                    if (gt.ConstraintType != 0)
                        Append(gt.ConstraintType.ToString().ToLower());

                    if (hasDefCtorConstraint)
                        AppendLeadingComma("new()");
                    if (hasBaseTypeConstraint)
                        AppendLeadingComma(gt.Base);
                    foreach (var it in gt.Interfaces)
                        AppendLeadingComma(it);

                    AppendLine();
                }
            }
        }

        void AppendLeadingComma(object obj)
        {
            if (_sb[_sb.Length - 1] != ' ')
                Append(", ");

            Append(obj);
        }

        void AppendLeadingComma(int i, object obj)
        {
            if (i > 0)
                Append(", ");

            Append(obj);
        }

        void Qualify(DataType declType, bool isClass = false)
        {
            if (!isClass && declType != null)
                Append(declType.GetNestedName() + ".");
        }

        void BeginMember(Member member, bool isClass, string modifier = null)
        {
            if (!isClass)
                BeginNamespace(member.DeclaringType.FirstNamespace);

            AppendDocComment(member.DocComment, isClass);
            AppendAttributes(member.Attributes, isClass);
            AppendModifiers(member.Modifiers, member.DeclaringType);
            Append(modifier + member.ReturnType + " ");
            Qualify(member.DeclaringType, isClass);
            Append(member.Name);
        }

        void EndMember(bool isClass)
        {
            if (!isClass)
                EndNamespace();
        }

        public void AppendField(Field field, bool isClass = false)
        {
            BeginMember(field, isClass, field.FieldModifiers.ToLiteral(true));
            AppendLine(";");
            EndMember(isClass);
        }

        void AppendSignature(Function f, bool isClass = false)
        {
            AppendDocComment(f.DocComment, isClass);
            AppendAttributes(f.Attributes, isClass);

            var startLength = _sb.Length;
            AppendModifiers(f.Modifiers, f.DeclaringType);

            if (!f.IsConstructor)
                Append(f.ReturnType + " ");

            Qualify(f.DeclaringType, isClass);
            Append(f.IsConstructor ? f.DeclaringType.Name : f.Name);

            if (f is Method)
            {
                var m = f as Method;

                if (m.IsGenericDefinition)
                {
                    Append("<");
                    for (int i = 0; i < m.GenericParameters.Length; i++)
                        AppendLeadingComma(i, m.GenericParameters[i].Name);
                    Append(">");
                }
                else if (m.IsGenericParameterization)
                {
                    Append("<");
                    for (int i = 0; i < m.GenericArguments.Length; i++)
                        AppendLeadingComma(i, m.GenericArguments[i]);
                    Append(">");
                }
            }

            Append("(");

            int declLength = _sb.Length - startLength + 4;
            bool first = true;
            foreach (var p in f.Parameters)
            {
                if (isClass && !first)
                    Append(", ");
                else if (!first)
                    Append(",\n" + new string(' ', declLength));
                else
                    first = false;

                Append(p.Modifier.ToLiteral(true) + p.Type + " " + p.Name);

                if (p.OptionalDefault != null)
                    Append(" = " + p.OptionalDefault);
            }

            Append(")");

            if (isClass)
            {
                if (f.HasBody && f.Body.Statements.Count == 1)
                {
                    var s = f.Body.Statements[0];
                    if (s is Expression)
                        AppendLine(" => " + s + ";");
                    else if (s is Return)
                        AppendLine(" => " + (s as Return).Value + ";");
                    else
                        AppendLine(";");
                }
                else
                    AppendLine(";");
            }
        }

        public void AppendFunction(Function func)
        {
            BeginNamespace(func.DeclaringType.FirstNamespace);

            if (func is ShaderFunction && (func as ShaderFunction).Shader.Entrypoint == func)
            {
                var sf = func as ShaderFunction;

                if (sf.Shader.ReferencedConstants.Count > 0)
                {
                    AppendLine("    // Constants");

                    foreach (var i in sf.Shader.ReferencedConstants)
                    {
                        var v = sf.Shader.State.RuntimeConstants[i];
                        AppendLine("    const " + v.Type + " " + v.Name + ";");
                    }

                    AppendLine();
                }

                if (sf.Shader.ReferencedUniforms.Count > 0)
                {
                    AppendLine("    // Uniforms");

                    foreach (var i in sf.Shader.ReferencedUniforms)
                    {
                        var v = sf.Shader.State.Uniforms[i];
                        AppendLine("    uniform " + v.Type + " " + v.Name + ";");
                    }

                    AppendLine();
                }

                if (sf.Shader is VertexShader && sf.Shader.State.VertexAttributes.Count > 0)
                {
                    AppendLine("    // Vertex Attributes");

                    foreach (var v in sf.Shader.State.VertexAttributes)
                        AppendLine("    vertex_attrib " + v.Type + " " + v.Name + ";");

                    AppendLine();
                }

                if (sf.Shader is PixelShader && sf.Shader.State.PixelSamplers.Count > 0)
                {
                    AppendLine("    // Pixel Samplers");

                    foreach (var v in sf.Shader.State.PixelSamplers)
                        AppendLine("    pixel_sampler " + v.Type + " " + v.Name + ";");

                    AppendLine();
                }

                if (sf.Shader.State.Varyings.Count > 0)
                {
                    AppendLine("    // Varyings");

                    foreach (var v in sf.Shader.State.Varyings)
                        AppendLine("    interpolate " + v.Type + " " + v.Name + ";");

                    AppendLine();
                }

                if (sf.Shader.ReferencedStructs.Count > 0)
                {
                    AppendLine("    // Structs");

                    foreach (var r in sf.Shader.ReferencedStructs)
                    {
                        var s = sf.Shader.State.Structs[r];

                        AppendLine("    struct " + s.Name);
                        AppendLine("    {");

                        foreach (var f in s.Fields)
                            AppendLine("        " + f.ReturnType + " " + f.Name + ";");

                        AppendLine("    }\n");
                    }
                }

                if (sf.Shader.Functions.Count > 0)
                {
                    AppendLine("    // Functions");

                    foreach (var f in sf.Shader.Functions)
                    {
                        AppendSignature(f);

                        if (f.HasBody)
                        {
                            AppendLine("\n    {");
                            AppendBody(f);
                            AppendLine("    }\n");
                        }
                        else
                            AppendLine(";\n");
                    }
                }

                AppendLine("    // Entrypoint");
            }

            AppendSignature(func);

            if (func is Method)
            {
                var m = func as Method;

                if (m.IsGenericDefinition)
                {
                    AppendLine();
                    AppendConstraints(m.GenericParameters);
                    _sb.Remove(_sb.Length - 1, 1);
                }
                else if (m.IsGenericParameterization)
                {
                    AppendLine();
                    AppendConstraints(m.GenericDefinition.GenericParameters);
                    _sb.Remove(_sb.Length - 1, 1);
                }
            }

            if (func.HasBody)
            {
                AppendLine("\n    {");
                AppendBody(func);

                if (func is ShaderFunction && (func as ShaderFunction).Shader.Entrypoint == func)
                {
                    if ((func as ShaderFunction).Shader is VertexShader)
                        foreach (var t in (func as ShaderFunction).Shader.State.Varyings)
                            AppendLine("        interpolate " + t.Name + " = " + t.Value + ";");

                    foreach (var t in (func as ShaderFunction).Shader.Terminals)
                        AppendLine("        terminal " + t.Key + " = " + t.Value + ";");
                }

                AppendLine("    }");
            }
            else
                AppendLine(";");

            EndNamespace();
        }

        public void AppendMetaProperty(MetaProperty mp)
        {
            AppendLine(mp.Visibility.ToLiteral(true) + mp.ReturnType + " " + mp.Parent + "." + mp.Name + ":");

            bool first = true;
            foreach (var d in mp.Definitions)
            {
                if (!first)
                    AppendLine(",\n");
                else
                    first = false;

                var indent = "    ";

                if (d.Requirements.Count > 0 || d.Tags.Count > 0)
                {
                    foreach (var tag in d.Tags)
                        AppendLine(indent + "tag(" + tag.ToLiteral() + ")");

                    foreach (var req in d.Requirements)
                        AppendLine(indent + req);

                    indent += indent;
                }

                if (d.Value is Expression)
                    Append(indent + (d.Value as Expression));
                else
                    new UnoDisassembler(_sb).AppendStatement(d.Value, indent);
            }

            AppendLine(mp.Definitions.Length == 0
                ? "    undefined;"
                : ";");
        }

        public void AppendLiteral(Literal literal, bool isClass = false)
        {
            BeginMember(literal, isClass, "const ");
            AppendLine(" = " + literal.Value.ToLiteral() + ";");
            EndMember(isClass);
        }

        public void AppendEvent(Event @event, bool isClass = false)
        {
            BeginMember(@event, isClass);
            AppendLine(";");
            EndMember(isClass);
        }

        public void AppendProperty(Property prop, bool isClass = false)
        {
            BeginMember(prop, isClass);
            Append(" {");

            if (prop.GetMethod != null)
            {
                Append(" ");
                var m = prop.GetMethod.Modifiers & ~prop.Modifiers;
                Append(m.ToLiteral(true) + "get;");
            }

            if (prop.SetMethod != null)
            {
                Append(" ");
                var m = prop.SetMethod.Modifiers & ~prop.Modifiers;
                Append(m.ToLiteral(true) + "set;");
            }

            Append(" }");

            if (prop.GetMethod != null &&
                prop.GetMethod.HasBody &&
                prop.GetMethod.Body.Statements.Count == 1)
            {
                var s = prop.GetMethod.Body.Statements[0];
                if (s is Expression)
                    AppendLine(" => " + s + ";");
                else if (s is Return)
                    AppendLine(" => " + (s as Return).Value + ";");
                else
                    AppendLine();
            }
            else
                AppendLine();

            EndMember(isClass);
        }
    }
}
