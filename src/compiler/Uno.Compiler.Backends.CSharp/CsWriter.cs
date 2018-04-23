using System.Reflection;
using Uno.Compiler.API.Backends.Decompilers;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.IO;

namespace Uno.Compiler.Backends.CSharp
{
    public class CsWriter : SourceWriter
    {
        public readonly CsBackend Backend;

        public CsWriter(CsBackend backend, string filename)
            : base(backend, filename, NewLine.CrLf)
        {
            Backend = backend;
        }

        public void WriteModifiers(Modifiers tm, bool isVirtual = false)
        {
            if (isVirtual)
                switch (tm & Modifiers.ProtectionModifiers)
                {
                    case Modifiers.Public:
                        Write("public ");
                        break;
                    case Modifiers.Protected:
                        Write("protected ");
                        break;
                    case Modifiers.Private:
                        Write("private ");
                        break;
                    case Modifiers.Internal:
                        Write("internal ");
                        break;
                    case Modifiers.Protected | Modifiers.Internal:
                        Write("protected internal ");
                        break;
                }
            else
                switch (tm & Modifiers.ProtectionModifiers)
                {
                    case 0:
                        break;
                    default:
                        // Ugly workaround: Make everything public because InternalsVisibleTo doesn't currently work for prebuilt code (UnoCore).
                        Write("public ");
                        break;
                }

            if (tm.HasFlag(Modifiers.Static))
                Write("static ");
            if (tm.HasFlag(Modifiers.Explicit))
                Write("explicit ");
            if (tm.HasFlag(Modifiers.Implicit))
                Write("implicit ");
            if (tm.HasFlag(Modifiers.Virtual))
                Write("virtual ");
            if (tm.HasFlag(Modifiers.Override))
                Write("override ");
            if (tm.HasFlag(Modifiers.Abstract))
                Write("abstract ");
            if (tm.HasFlag(Modifiers.Sealed))
                Write("sealed ");
        }

        public void WriteFieldModifiers(FieldModifiers tm)
        {
            if (tm.HasFlag(FieldModifiers.Const))
                Write("const ");
            if (tm.HasFlag(FieldModifiers.ReadOnly))
                Write("readonly ");
            if (tm.HasFlag(FieldModifiers.Volatile))
                Write("volatile ");
        }

        public override void WriteStaticType(Source src, DataType dt)
        {
            Write(Backend.GetStaticTypeName(dt, Type));
        }

        public override void WriteType(Source src, DataType dt)
        {
            Write(Backend.GetStaticTypeName(dt, Type));
        }

        public override void WriteLoadField(Source src, Field field, Expression obj, ExpressionUsage u)
        {
            // Terrible hack (now slightly less terrible) :(
            if (field.Name == "Zero" && field.DeclaringType.HasAttribute(Essentials.TargetSpecificTypeAttribute))
            {
                Write("0");
                return;
            }

            base.WriteLoadField(src, field, obj, u);
        }

        public override void WriteConstant(Constant c, ExpressionUsage u)
        {
            if (c.ReturnType is EnumType)
            {
                foreach (var v in (c.ReturnType as EnumType).Literals)
                {
                    if (v.Value.Equals(c.Value))
                    {
                        WriteType(c.Source, c.ReturnType);
                        Write("." + v.UnoName);
                        return;
                    }
                }

                if (c.Value.Equals(0))
                {
                    Write("0");
                    return;
                }

                Write("(");
                WriteType(c.Source, c.ReturnType);
                Write(")");
            }

            base.WriteConstant(c, u);
        }

        public void WriteAttributes(NewObject[] attributes)
        {
            if (attributes != null)
            {
                foreach (var a in attributes)
                {
                    BeginLine("[");
                    Write(Backend.GetStaticTypeName(a.ReturnType, Data.IL));
                    WriteArguments(a.Constructor, a.Arguments, true);
                    EndLine("]");
                }
            }
        }

        public override void WriteDefault(Source src, DataType dt, ExpressionUsage u = ExpressionUsage.Argument)
        {
            var dotNetType = dt.TryGetAttributeString(Essentials.DotNetTypeAttribute);

            if (dotNetType == "System.Int32")
                Write("0");
            else
                base.WriteDefault(src, dt, u);
        }

        public void WriteGenericParameters(GenericParameterType[] t)
        {
            if (t != null && t.Length > 0)
            {
                Write("<");

                for (int i = 0; i < t.Length; i++)
                {
                    if (i > 0) Write(", ");
                    Write(t[i].UnoName);
                }

                Write(">");
            }
        }

        public void WriteType(DataType dt)
        {
            SetContext(dt);
            WriteAttributes(dt.Attributes);

            switch (dt.TypeType)
            {
                case TypeType.Class:
                    {
                        BeginLine();
                        WriteModifiers(dt.Modifiers);
                        Write("class " + dt.UnoName);
                        WriteGenericParameters(dt.GenericParameters);

                        var colon = false;

                        if (dt.Base != null && dt.Base != Essentials.Object)
                            Write(" : " + Backend.GetStaticTypeName(dt.Base, dt));
                        else
                            colon = true;

                        for (int i = 0; i < dt.Interfaces.Length; i++)
                        {
                            Write(i == 0 && colon ? " : " : ", ");
                            Write(Backend.GetStaticTypeName(dt.Interfaces[i], dt));
                        }

                        EndLine();
                        BeginScope();

                        foreach (var it in dt.NestedTypes)
                            if (!Backend.IgnoreType(it))
                                WriteType(it);

                        WriteMembers(dt);
                        EndScope();
                    }
                    break;

                case TypeType.Struct:
                    {
                        BeginLine();
                        WriteModifiers(dt.Modifiers);
                        Write("struct " + dt.UnoName);
                        WriteGenericParameters(dt.GenericParameters);

                        for (int i = 0; i < dt.Interfaces.Length; i++)
                        {
                            Write(i == 0 ? " : " : ", ");
                            Write(Backend.GetStaticTypeName(dt.Interfaces[i], dt));
                        }

                        EndLine();
                        BeginScope();

                        foreach (var it in dt.NestedTypes)
                            if (!Backend.IgnoreType(it))
                                WriteType(it);

                        WriteMembers(dt);
                        EndScope();
                    }
                    break;

                case TypeType.Interface:
                    {
                        BeginLine();
                        WriteModifiers(dt.Modifiers & Modifiers.ProtectionModifiers);
                        Write("interface " + dt.UnoName);
                        WriteGenericParameters(dt.GenericParameters);

                        for (int i = 0; i < dt.Interfaces.Length; i++)
                        {
                            Write(i == 0 ? " : " : ", ");
                            Write(Backend.GetStaticTypeName(dt.Interfaces[i], dt));
                        }

                        EndLine();
                        BeginScope();

                        foreach (var it in dt.NestedTypes)
                            if (!Backend.IgnoreType(it))
                                WriteType(it);

                        WriteMembers(dt);
                        EndScope();
                    }
                    break;

                case TypeType.Enum:
                    {
                        BeginLine();
                        WriteModifiers(dt.Modifiers);
                        EndLine("enum " + dt.UnoName);
                        BeginScope();

                        foreach (var l in dt.Literals)
                            WriteLine(l.UnoName + " = " + l.Value + ",");

                        EndScope();
                    }
                    break;

                case TypeType.Delegate:
                    {
                        var del = dt as DelegateType;
                        BeginLine();
                        WriteModifiers(del.Modifiers);
                        Write("delegate ");
                        WriteType(del.Source, del.ReturnType);
                        Write(" " + del.UnoName);
                        WriteGenericParameters(del.GenericParameters);
                        WriteParameters(del, true);
                        EndLine(";");
                    }
                    break;

                default:
                    throw new SourceException(dt.Source, "Unhandled data type in C# backend: " + dt);
            }
        }

        public void WriteMembers(DataType dt)
        {
            foreach (var f in dt.Fields)
            {
                WriteAttributes(f.Attributes);
                BeginLine();
                WriteModifiers(f.Modifiers);
                WriteFieldModifiers(f.FieldModifiers);
                WriteType(f.Source, f.ReturnType);
                EndLine(" " + f.UnoName + ";");
            }

            Skip();

            if (dt.Initializer != null)
            {
                WriteAttributes(dt.Initializer.Attributes);
                BeginLine();
                WriteModifiers(dt.Initializer.Modifiers);
                EndLine(dt.UnoName + "()");
                WriteFunctionBody(dt.Initializer);
            }

            foreach (var c in dt.Constructors)
            {
                WriteAttributes(c.Attributes);
                BeginLine();
                WriteModifiers(c.Modifiers);
                Write(c.DeclaringType.UnoName);
                WriteParameters(c, true);

                if (c.HasBody)
                {
                    // Look for base constructor call
                    for (int i = 0; i < c.Body.Statements.Count; i++)
                    {
                        var callCtor = c.Body.Statements[i] as CallConstructor;

                        if (callCtor != null)
                        {
                            if (callCtor.Arguments.Length > 0)
                            {
                                var declaringType = callCtor.Constructor.DeclaringType;
                                Write((declaringType.IsGenericParameterization ? declaringType.GenericDefinition : declaringType) == c.DeclaringType
                                    ? ": this"
                                    : ": base");
                                WriteArguments(callCtor.Constructor, callCtor.Arguments, true);
                            }

                            c.Body.Statements.RemoveAt(i);
                            break;
                        }
                    }
                }

                EndLine();
                WriteFunctionBody(c);
            }

            foreach (var c in dt.Methods)
            {
                if (c.HasAttribute(Essentials.DotNetOverrideAttribute))
                    continue;

                WriteAttributes(c.Attributes);
                BeginLine();

                if (!c.DeclaringType.IsInterface && c.ImplementedMethod == null)
                    WriteModifiers(c.Modifiers, c.IsVirtual);

                WriteType(c.Source, c.ReturnType);
                Write(" ");

                if (c.ImplementedMethod != null)
                {
                    WriteStaticType(c.Source, c.ImplementedMethod.DeclaringType);
                    Write("." + c.ImplementedMethod.UnoName);
                }
                else
                    Write(c.UnoName);

                if (c.IsGenericDefinition)
                    WriteGenericParameters(c.GenericParameters);

                WriteParameters(c, true);

                if (c.IsAbstract)
                {
                    Write(";");
                    EndLine();
                    Skip();
                    continue;
                }

                EndLine();
                WriteFunctionBody(c);
            }

            foreach (var c in dt.Properties)
            {
                WriteAttributes(c.Attributes);
                BeginLine();

                if (!c.DeclaringType.IsInterface && c.ImplementedProperty == null)
                    WriteModifiers(c.Modifiers, c.IsVirtual);

                WriteType(c.Source, c.ReturnType);
                Write(" ");

                if (c.Parameters.Length == 0)
                {
                    if (c.ImplementedProperty != null)
                    {
                        WriteStaticType(c.Source, c.ImplementedProperty.DeclaringType);
                        Write("." + c.ImplementedProperty.UnoName);
                    }
                    else
                        Write(c.UnoName);
                }
                else
                {
                    if (c.ImplementedProperty != null)
                    {
                        WriteStaticType(c.Source, c.ImplementedProperty.DeclaringType);
                        Write(".");
                    }

                    Write("this");
                    WriteParameters(c, true, '[', ']');
                }

                EndLine();
                BeginScope();

                if (c.GetMethod != null)
                    WriteAccessor("get", c.GetMethod, c.Modifiers, c.ImplicitField != null);
                if (c.SetMethod != null)
                    WriteAccessor("set", c.SetMethod, c.Modifiers, c.ImplicitField != null);

                EndScope();
            }

            foreach (var c in dt.Events)
            {
                WriteAttributes(c.Attributes);
                BeginLine();

                if (!c.DeclaringType.IsInterface && c.ImplementedEvent == null)
                    WriteModifiers(c.Modifiers, c.IsVirtual);

                Write("event ");
                WriteType(c.Source, c.ReturnType);
                Write(" ");

                if (c.ImplementedEvent != null)
                {
                    WriteStaticType(c.Source, c.ImplementedEvent.DeclaringType);
                    Write("." + c.ImplementedEvent.UnoName);
                }
                else
                    Write(c.UnoName);

                if (c.ImplicitField != null || c.DeclaringType.IsInterface)
                {
                    Write(";");
                    EndLine();
                }
                else
                {
                    EndLine();
                    BeginScope();

                    if (c.AddMethod != null)
                        WriteAccessor("add", c.AddMethod, c.Modifiers, false);

                    if (c.RemoveMethod != null)
                        WriteAccessor("remove", c.RemoveMethod, c.Modifiers, false);

                    EndScope();
                }
            }

            foreach (var o in dt.Operators)
            {
                WriteAttributes(o.Attributes);
                BeginLine();
                WriteModifiers(o.Modifiers);
                WriteType(o.Source, o.ReturnType);
                Write(" operator " + o.Symbol);
                WriteParameters(o, true);
                EndLine();
                WriteFunctionBody(o);
            }

            foreach (var o in dt.Casts)
            {
                WriteAttributes(o.Attributes);
                BeginLine();
                WriteModifiers(o.Modifiers);
                Write("operator ");
                WriteType(o.Source, o.ReturnType);
                WriteParameters(o, true);
                EndLine();
                WriteFunctionBody(o);
            }

            foreach (var m in dt.EnumerateInheritedDotNetInterfaceMembers(Essentials))
            {
                var pi = m as PropertyInfo;
                var mi = m as MethodInfo;
                var ei = m as EventInfo;

                if (pi != null)
                {
                    // TODO: Does not handle parameter list
                    WriteLine(Backend.GetDotNetTypeName(pi.PropertyType) + " " + Backend.GetDotNetTypeName(pi.DeclaringType) + "." + pi.Name);
                    BeginScope();

                    if (pi.GetGetMethod() != null)
                        WriteLine("get { throw new global::System.NotImplementedException(); }");

                    if (pi.GetSetMethod() != null)
                        WriteLine("set { throw new global::System.NotImplementedException(); }");

                    EndScope();
                }

                if (ei != null)
                {
                    WriteLine(Backend.GetDotNetTypeName(ei.EventHandlerType) + " " + Backend.GetDotNetTypeName(ei.DeclaringType) + "." + ei.Name);
                    BeginScope();
                    WriteLine("add { throw new global::System.NotImplementedException(); }");
                    WriteLine("remove { throw new global::System.NotImplementedException(); }");
                    EndScope();
                }

                if (mi != null)
                {
                    BeginLine(Backend.GetDotNetTypeName(mi.ReturnType) + " " + Backend.GetDotNetTypeName(mi.DeclaringType) + "." + mi.Name + "(");

                    for (int i = 0; i < mi.GetParameters().Length; i++)
                    {
                        var p = mi.GetParameters()[i];

                        if (i > 0)
                            Write(", ");

                        // TODO: Does not handle modifiers
                        Write(Backend.GetDotNetTypeName(p.ParameterType) + " " + p.Name);
                    }

                    EndLine(")");
                    BeginScope();
                    WriteLine("throw new global::System.NotImplementedException();");
                    EndScope();
                }
            }
        }

        public void WriteAccessor(string name, Method m, Modifiers ownerModifiers, bool omitBodies)
        {
            BeginLine();

            /*
            if ((m.Modifiers & Modifiers.ProtectionModifiers) != (ownerModifiers & Modifiers.ProtectionModifiers))
                WriteModifiers(m.Modifiers & Modifiers.ProtectionModifiers);
            */

            Write(name);

            if (m.IsAbstract || omitBodies)
            {
                Write(";");
                EndLine();
            }
            else if (m.HasBody && m.Body.Statements.Count == 1 && (m.Body.Statements[0] is Expression || m.Body.Statements[0] is Return))
            {
                Write(" { ");

                if (m.Body.Statements[0] is Return)
                {
                    var s = m.Body.Statements[0] as Return;
                    Write("return");

                    if (s.Value != null)
                    {
                        Write(" ");
                        WriteExpression(s.Value);
                    }
                }
                else
                {
                    WriteExpression(m.Body.Statements[0] as Expression, ExpressionUsage.Statement);
                }

                EndLine("; }");
            }
            else
            {
                EndLine();
                WriteFunctionBody(m);
                DisableSkip();
            }
        }
    }
}
