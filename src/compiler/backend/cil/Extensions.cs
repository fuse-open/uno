using System;
using IKVM.Reflection;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Logging;
using ParameterModifier = Uno.Compiler.API.Domain.ParameterModifier;
using Type = IKVM.Reflection.Type;

namespace Uno.Compiler.Backends.CIL
{
    public static class Extensions
    {
        private static readonly System.Type _IKVM_Reflection_GenericTypeInstance =
            typeof(Type).Assembly.GetType("IKVM.Reflection.GenericTypeInstance");

        public static bool IsTypeBuilder(this Type t)
        {
            return t.GetType() == _IKVM_Reflection_GenericTypeInstance;
        }

        public static string GetReflectedName(this MethodInfo mi)
        {
            var rt = mi.ReflectedType;
            return rt.Namespace + "." + rt.Name + "." + mi.Name;
        }

        public static string CilTypeName(this DataType dt)
        {
            var result = dt.UnoName;
            var parent = dt.ParentNamespace;

            if (parent != null)
            {
                while (!parent.IsRoot)
                {
                    result = parent.UnoName + "." + result;
                    parent = parent.ParentNamespace;
                }
            }

            if (dt.IsGenericDefinition)
                result += "`" + dt.GenericParameters.Length;

            return result;
        }

        public static TypeAttributes CilTypeAttributes(this DataType dt, bool nestedType)
        {
            var m = dt.Modifiers;
            TypeAttributes result = 0;

            if (nestedType)
            {
                switch (m & Modifiers.ProtectionModifiers)
                {
                    case Modifiers.Public:
                        result |= TypeAttributes.NestedPublic;
                        break;
                    case Modifiers.Protected:
                        result |= TypeAttributes.NestedFamily;
                        break;
                    case Modifiers.Private:
                        result |= TypeAttributes.NestedPrivate;
                        break;
                    case Modifiers.Internal:
                        result |= TypeAttributes.NestedAssembly;
                        break;
                    case Modifiers.Protected | Modifiers.Internal:
                        result |= TypeAttributes.NestedFamORAssem;
                        break;
                }
            }
            else
                result |= m.HasFlag(Modifiers.Public) ?
                    TypeAttributes.Public :
                    TypeAttributes.NotPublic;

            if (m.HasFlag(Modifiers.Static))
                result |= TypeAttributes.Abstract | TypeAttributes.Sealed;
            if (m.HasFlag(Modifiers.Abstract))
                result |= TypeAttributes.Abstract;
            if (m.HasFlag(Modifiers.Sealed))
                result |= TypeAttributes.Sealed;

            if (!dt.HasInitializer)
                result |= TypeAttributes.BeforeFieldInit;

            return result;
        }

        public static MethodAttributes CilMethodAttributes(this Function f)
        {
            var m = f.Modifiers;
            var result = MethodAttributes.HideBySig;

            switch (m & Modifiers.ProtectionModifiers)
            {
                case Modifiers.Public:
                    result |= MethodAttributes.Public;
                    break;
                case Modifiers.Protected:
                    result |= MethodAttributes.Family;
                    break;
                case Modifiers.Private:
                    result |= MethodAttributes.Private;
                    break;
                case Modifiers.Internal:
                    result |= MethodAttributes.Assembly;
                    break;
                case Modifiers.Protected | Modifiers.Internal:
                    result |= MethodAttributes.FamORAssem;
                    break;
            }

            if (m.HasFlag(Modifiers.Static))
                result |= MethodAttributes.Static;
            if (m.HasFlag(Modifiers.Virtual))
                result |= MethodAttributes.Virtual;
            if (m.HasFlag(Modifiers.Abstract))
                result |= MethodAttributes.Abstract | MethodAttributes.Virtual;

            if (m.HasFlag(Modifiers.Override))
                result |= MethodAttributes.Virtual;
            else
                result |= MethodAttributes.NewSlot;

            if (m.HasFlag(Modifiers.Sealed))
                result |= MethodAttributes.Final;

            // Make non-virtual instance methods sealed virtual by default, it could be used to implement interface in derived class
            if (!result.HasFlag(MethodAttributes.Static) && !result.HasFlag(MethodAttributes.Virtual) && f.IsMethod && !f.DeclaringType.IsInterface)
                result |= MethodAttributes.Virtual | MethodAttributes.Final;
            else if (!result.HasFlag(MethodAttributes.Virtual))
                result &= ~MethodAttributes.NewSlot;

            return result;
        }

        public static FieldAttributes CilFieldAttributes(this Field f)
        {
            var m = f.Modifiers;
            var t = f.FieldModifiers;
            FieldAttributes result = 0;

            switch (m & Modifiers.ProtectionModifiers)
            {
                case Modifiers.Public:
                    result |= FieldAttributes.Public;
                    break;
                case Modifiers.Protected:
                    result |= FieldAttributes.Family;
                    break;
                case Modifiers.Private:
                    result |= FieldAttributes.Private;
                    break;
                case Modifiers.Internal:
                    result |= FieldAttributes.Assembly;
                    break;
                case Modifiers.Protected | Modifiers.Internal:
                    result |= FieldAttributes.FamORAssem;
                    break;
            }

            if (m.HasFlag(Modifiers.Static))
                result |= FieldAttributes.Static;
            if (t.HasFlag(FieldModifiers.ReadOnly))
                result |= FieldAttributes.InitOnly;

            return result;
        }

        public static ParameterAttributes CilParameterAttributes(this Parameter p)
        {
            switch (p.Modifier)
            {
                case ParameterModifier.Out:
                    return ParameterAttributes.Out;
                default:
                    return p.OptionalDefault is Constant 
                        ? ParameterAttributes.Optional 
                        : 0;
            }
        }

        public static Version ParseVersion(this SourcePackage package, Log log)
        {
            var str = package.Version;

            if (string.IsNullOrEmpty(str))
                return new Version();

            // Remove suffix
            var i = str.IndexOf('-');
            if (i != -1)
            {
                str = str.Substring(0, i);
                if (string.IsNullOrEmpty(str))
                    return new Version();
            }

            // Check that version string only contains numbers or periods (X.X.X)
            foreach (var c in str)
                if (!char.IsNumber(c) && c != '.')
                    return new Version();

            try
            {
                return new Version(str);
            }
            catch
            {
                log.Warning(package.Source, null, "Failed to parse version string " + str.Quote());
                return new Version();
            }
        }
    }
}
