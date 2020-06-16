using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.IO;

namespace Uno.Compiler.Core.IL.Optimizing
{
    class ILStripper : CompilerPass
    {
        int _fieldsStripped;
        int _functionsStripped;
        int _functionsSealed;
        int _functionsStrippedFromVTable;
        int _typesStripped;
        int _typesSealed;
        int _namespacesStripped;
        bool _hasFunctions;

        internal ILStripper(CompilerPass parent)
            : base(parent)
        {
        }

        internal new void Begin()
        {
            if (Environment.IsDefined("SIMULATOR"))
                KeepSimulatorEntities();

            VisitNullableScope(Data.StartupCode);
        }

        public void Keep(Member root)
        {
            switch (root.MemberType)
            {
                case MemberType.Field:
                    VisitField((Field)root);
                    break;
                case MemberType.Literal:
                    VisitType(root.DeclaringType);
                    root.Stats |= EntityStats.RefCount;
                    break;
                case MemberType.Cast:
                case MemberType.Constructor:
                case MemberType.Finalizer:
                case MemberType.Method:
                case MemberType.Operator:
                    VisitFunction((Function)root);
                    break;

                case MemberType.Event:
                {
                    var m = (Event)root;
                    if (m.ImplicitField != null)
                        VisitField(m.ImplicitField);
                    if (m.AddMethod != null)
                        VisitFunction(m.AddMethod);
                    if (m.RemoveMethod != null)
                        VisitFunction(m.RemoveMethod);
                    break;
                }
                case MemberType.Property:
                {
                    var m = (Property)root;
                    if (m.ImplicitField != null)
                        VisitField(m.ImplicitField);
                    if (m.GetMethod != null)
                        VisitFunction(m.GetMethod);
                    if (m.SetMethod != null)
                        VisitFunction(m.SetMethod);
                    break;
                }
            }
        }

        public bool Keep(IEntity root)
        {
            switch (root.EntityType)
            {
                case EntityType.Namescope:
                {
                    var dt = root as DataType;
                    if (dt != null)
                    {
                        VisitType(dt);
                        return true;
                    }
                    return false;
                }
                case EntityType.Member:
                {
                    Keep((Member)root);
                    return true;
                }
            }

            return false;
        }

        Method GetBaseMethod(Method m)
        {
            if (m.OverriddenMethod == null)
                return null;

            while (m.OverriddenMethod != null)
                m = m.OverriddenMethod;

            return m;
        }

        void VisitType(DataType dt)
        {
            if (dt.HasRefCount)
                return;

            dt.Stats |= EntityStats.RefCount;

            if (!dt.IsMasterDefinition)
                VisitType(dt.MasterDefinition);
            if (dt.IsArray)
                VisitType(dt.ElementType);
            if (dt.IsNestedType)
                VisitType(dt.ParentType);

            if (dt.IsFlattenedParameterization)
                foreach (var a in dt.FlattenedArguments)
                    VisitType(a);

            var delegateType = dt as DelegateType;
            if (delegateType != null)
            {
                VisitType(delegateType.ReturnType);

                foreach (var p in delegateType.Parameters)
                {
                    VisitType(p.Type);

                    if (dt.IsMasterDefinition)
                        if (!Backend.Has(TypeOptions.IgnoreAttributes))
                            for (int i = 0; i < p.Attributes.Length; i++)
                                 VisitAttribute(p.Attributes[i]);
                }
            }

            if (dt.IsMasterDefinition)
            {
                if (!Backend.Has(TypeOptions.IgnoreAttributes))
                    for (int i = 0; i < dt.Attributes.Length; i++)
                        VisitAttribute(dt.Attributes[i]);

                TypeExtension typeExt;
                if (Environment.TryGetExtension(dt, out typeExt))
                    VisitExtension(typeExt);

                if (!dt.HasAttribute(Essentials.NativeClassAttribute))
                {
                    var bt = dt;
                    while ((bt = bt.Base) != null)
                    {
                        // Quickly check inheritance chain for NativeClassAttribute
                        if (!bt.HasAttribute(Essentials.NativeClassAttribute))
                            continue;

                        foreach (var m in dt.Methods)
                        {
                            var baseMethod = GetBaseMethod(m);
                            if (baseMethod == null)
                                continue;

                            if (baseMethod.DeclaringType.HasAttribute(Essentials.NativeClassAttribute))
                            {
                                VisitFunction(m);
                                VisitFunction(baseMethod);
                            }
                        }

                        break;
                    }
                }
            }
        }

        void VisitFunction(Function f)
        {
            if (f.HasRefCount)
                return;

            f.Stats |= EntityStats.RefCount;

            VisitType(f.ReturnType);
            VisitType(f.DeclaringType);

            foreach (var p in f.Parameters)
            {
                VisitType(p.Type);

                if (f == f.MasterDefinition)
                    if (!Backend.Has(TypeOptions.IgnoreAttributes))
                        for (int i = 0; i < p.Attributes.Length; i++)
                            VisitAttribute(p.Attributes[i]);
            }

            var method = f as Method;
            if (method != null && method.IsGenericParameterization)
                foreach (var dt in method.GenericArguments)
                    VisitType(dt);

            if (!f.IsMasterDefinition)
                VisitFunction(f.MasterDefinition);
            else
            {
                _hasFunctions = true;

                if (!Backend.Has(TypeOptions.IgnoreAttributes))
                    for (int i = 0; i < f.Attributes.Length; i++)
                        VisitAttribute(f.Attributes[i]);

                FunctionExtension methodExt;
                if (Environment.TryGetExtension(f, out methodExt))
                    VisitExtension(methodExt);

                f.Visit(this);
            }

            if (f.IsConstructor && f.DeclaringType.IsGenericParameter)
            {
                var gt = f.DeclaringType.ParentType.MasterDefinition;
                var i = f.DeclaringType.GenericIndex;
                foreach (var p in gt.GenericParameterizations)
                    VisitFunction(p.FlattenedArguments[i].TryGetDefaultConstructor() ?? Function.Null);
            }
        }

        void VisitExtension(ExtensionEntity f)
        {
            if (f.Status.HasFlag(EntityStats.RefCount))
                return;

            f.Status |= EntityStats.RefCount;

            foreach (var e in f.RequiredEntities)
                if (!Keep(e))
                    Log.Warning(f.Source, ErrorCode.W0000, "Invalid reference to entity " + e.Quote());

            foreach (var e in f.RequiredTemplates)
                VisitExtension(e);

            foreach (var r in f.Requirements)
                foreach (var e in r.Value)
                    Environment.Expand(e.Source, e.String, false, null, e.Usings);

            foreach (var e in f.CopyFiles)
                VisitFile(e);
        }

        void VisitFile(CopyFile f)
        {
            if (f.Type == null || !f.Flags.HasFlag(CopyFileFlags.ProcessFile))
                return;

            var src = f.SourceName.String.IsValidPath() &&
                File.Exists(Path.Combine(Path.GetDirectoryName(f.SourceName.Source.FullPath), f.SourceName.String))
                    ? f.SourceName.String
                    : Environment.ExpandSingleLine(f.SourceName.Source, f.SourceName.String).NativeToUnix();

            if (Disk.GetFullPath(
                    f.SourceName.Source,
                    ref src,
                    PathFlags.AllowAbsolutePath))
                Environment.Expand(new Source(f.SourceName.Source.Package, src, 1, 1), f.Preprocess(File.ReadAllText(src).Replace("\r\n", "\n")));
        }

        void VisitField(Field f)
        {
            if (f.HasRefCount)
                return;

            f.Stats |= EntityStats.RefCount;

            VisitType(f.ReturnType);
            VisitType(f.DeclaringType);

            if (!f.IsMasterDefinition)
                VisitField(f.MasterDefinition);
            else if (!Backend.Has(TypeOptions.IgnoreAttributes))
                for (int i = 0; i < f.Attributes.Length; i++)
                    VisitAttribute(f.Attributes[i]);
        }

        void VisitAttribute(NewObject attr)
        {
            VisitFunction(attr.Constructor);
            attr.Visit(this);
        }

        void VisitNullableScope(Scope scope)
        {
            scope?.Visit(this);
        }

        public override void Begin(ref Statement s)
        {
            switch (s.StatementType)
            {
                case StatementType.VariableDeclaration:
                {
                    var v = s as VariableDeclaration;
                    VisitType(v.Variable.ValueType);
                    break;
                }
                case StatementType.FixedArrayDeclaration:
                {
                    var v = s as FixedArrayDeclaration;
                    VisitType(v.Variable.ValueType);
                    break;
                }
                case StatementType.TryCatchFinally:
                {
                    var v = s as TryCatchFinally;
                    foreach (var c in v.CatchBlocks)
                        if (c.Exception != null)
                            VisitType(c.Exception.ValueType);
                    break;
                }
                case StatementType.ExternScope:
                {
                    var e = s as ExternScope;
                    // Skip PInvoke methods since they use e.g. C++ features when targeting C#.
                    // Is there a cleaner way to do this?
                    if (!Function.IsPInvokable(Essentials, Log))
                        Environment.Expand(e.Source, e.String, false, Function, e.Scopes);
                    break;
                }
            }
        }

        public override void Begin(ref Expression s, ExpressionUsage u)
        {
            VisitType(s.ReturnType);

            switch (s.ExpressionType)
            {
                case ExpressionType.CallConstructor:
                    VisitFunction((s as CallConstructor).Constructor);
                    break;
                case ExpressionType.CallMethod:
                    VisitFunction((s as CallMethod).Method);
                    break;
                case ExpressionType.NewDelegate:
                    VisitFunction((s as NewDelegate).Method);
                    break;
                case ExpressionType.GetProperty:
                    VisitFunction((s as GetProperty).Property.GetMethod);
                    break;
                case ExpressionType.SetProperty:
                    VisitFunction((s as SetProperty).Property.SetMethod);
                    break;
                case ExpressionType.NewObject:
                    VisitFunction((s as NewObject).Constructor);
                    break;
                case ExpressionType.CallBinOp:
                    VisitFunction((s as CallBinOp).Operator);
                    break;
                case ExpressionType.CallUnOp:
                    VisitFunction((s as CallUnOp).Operator);
                    break;
                case ExpressionType.CallCast:
                    VisitFunction((s as CallCast).Cast);
                    break;
                case ExpressionType.LoadField:
                    VisitField((s as LoadField).Field);
                    break;
                case ExpressionType.StoreField:
                    VisitField((s as StoreField).Field);
                    break;
                case ExpressionType.AddListener:
                    VisitFunction((s as AddListener).Event.AddMethod);
                    break;
                case ExpressionType.RemoveListener:
                    VisitFunction((s as RemoveListener).Event.RemoveMethod);
                    break;
                case ExpressionType.IsOp:
                    VisitType((s as IsOp).TestType);
                    break;
                case ExpressionType.AsOp:
                    VisitType((s as AsOp).TestType);
                    break;
                case ExpressionType.TypeOf:
                    VisitType((s as TypeOf).Type);
                    break;

                case ExpressionType.ExternOp:
                {
                    var e = s as ExternOp;
                    Environment.Expand(e.Source, e.String, false, Function, e.Usings);
                    break;
                }
            }
        }

        void ConstrainBaseType(DataType dt)
        {
            if (dt.Stats.HasFlag(EntityStats.RefCountAsBase))
                return;

            dt.Stats |= EntityStats.RefCountAsBase;

            if (!dt.IsMasterDefinition)
                ConstrainBaseType(dt.MasterDefinition);
            if (dt.Base != null)
                ConstrainBaseType(dt.Base);
        }

        void ConstrainOverriddenMethod(Method m)
        {
            if (m.Stats.HasFlag(EntityStats.RefCountAsOverridden))
                return;

            m.Stats |= EntityStats.RefCountAsOverridden;

            if (m != m.MasterDefinition)
                ConstrainOverriddenMethod((Method)m.MasterDefinition);
            if (m.OverriddenMethod != null)
                ConstrainOverriddenMethod(m.OverriddenMethod);
        }

        void ConstrainOverridingMethod(Method m)
        {
            if (m.MasterDefinition.HasRefCount)
            {
                ConstrainOverriddenMethod(m.OverriddenMethod);
                return;
            }

            for (var om = m.OverriddenMethod; om != null; om = om.OverriddenMethod)
            {
                if (om.MasterDefinition.HasRefCount)
                {
                    VisitFunction(m);
                    ConstrainOverriddenMethod(m.OverriddenMethod);
                    return;
                }
            }
        }

        void ConstrainRefsRecursive(DataType dt)
        {
            if (!dt.HasRefCount)
            {
                // Visit generic parameterization used as base class
                if (!dt.IsMasterDefinition && dt.Stats.HasFlag(EntityStats.RefCountAsBase) && !CanStrip(dt))
                {
                    VisitType(dt);
                    ConstrainRefsRecursive(dt);
                    return;
                }

                // Visit all methods that implements referenced abstract methods
                foreach (var f in dt.EnumerateFunctions())
                {
                    var m = f as Method;
                    if (m?.OverriddenMethod != null && m.OverriddenMethod.IsAbstract && !CanStrip(m.OverriddenMethod))
                        VisitFunction(m);
                }

                return;
            }

            if (!dt.IsMasterDefinition)
            {
                foreach (var m in dt.EnumerateFields())
                    if (m.MasterDefinition.HasRefCount)
                        VisitField(m);
                foreach (var m in dt.EnumerateFunctions())
                    if (m.MasterDefinition.HasRefCount)
                        VisitFunction(m);
            }

            if (dt.IsGenericDefinition)
            {
                foreach (var p in dt.GenericParameterizations)
                    ConstrainRefsRecursive(p);

                ConstrainGenericParameters(dt);
            }

            for (int i = 0; i < dt.NestedTypes.Count; i++)
                ConstrainRefsRecursive(dt.NestedTypes[i]);

            if (dt.CanLink)
                foreach (var m in dt.EnumerateMembers())
                    Keep(m);

            if (dt.Base != null)
                ConstrainBaseType(dt.Base);

            if (dt.Initializer != null)
                VisitFunction(dt.Initializer);
            if (dt.Finalizer != null)
                VisitFunction(dt.Finalizer);

            if (dt.IsStruct)
                foreach (var f in dt.EnumerateFields())
                    VisitField(f);

            foreach (var m in dt.Methods)
            {
                if (m.OverriddenMethod != null)
                    ConstrainOverridingMethod(m);
                if (m.IsGenericDefinition)
                    ConstrainGenericParameters(m.GenericType);
            }

            foreach (var m in dt.Events)
            {
                if (m.HasRefCount ||
                    m.AddMethod != null && m.AddMethod.HasRefCount ||
                    m.RemoveMethod != null && m.RemoveMethod.HasRefCount ||
                    m.ImplicitField != null && m.ImplicitField.HasRefCount)
                {
                    if (m.AddMethod != null)
                        VisitFunction(m.AddMethod);
                    if (m.RemoveMethod != null)
                        VisitFunction(m.RemoveMethod);
                    if (m.ImplicitField != null)
                        VisitField(m.ImplicitField);
                }

                if (m.AddMethod?.OverriddenMethod != null)
                    ConstrainOverridingMethod(m.AddMethod);
                if (m.RemoveMethod?.OverriddenMethod != null)
                    ConstrainOverridingMethod(m.RemoveMethod);

                if (m.AddMethod != null && m.RemoveMethod != null &&
                    (m.AddMethod.Stats & EntityStats.RefCount) != (m.RemoveMethod.Stats & EntityStats.RefCount) &&
                    m.AddMethod.OverriddenMethod != null && m.RemoveMethod.OverriddenMethod != null &&
                    (m.AddMethod.OverriddenMethod.Stats & EntityStats.RefCount) != (m.RemoveMethod.OverriddenMethod.Stats & EntityStats.RefCount))
                {
                    VisitFunction(m.AddMethod.OverriddenMethod);
                    VisitFunction(m.RemoveMethod.OverriddenMethod);
                }
            }

            foreach (var m in dt.Properties)
            {
                if (m.HasRefCount ||
                    m.GetMethod != null && m.GetMethod.HasRefCount ||
                    m.SetMethod != null && m.SetMethod.HasRefCount ||
                    m.ImplicitField != null && m.ImplicitField.HasRefCount)
                {
                    if (m.GetMethod != null)
                        VisitFunction(m.GetMethod);
                    if (m.SetMethod != null)
                        VisitFunction(m.SetMethod);
                    if (m.ImplicitField != null)
                        VisitField(m.ImplicitField);
                }

                if (m.GetMethod?.OverriddenMethod != null)
                    ConstrainOverridingMethod(m.GetMethod);
                if (m.SetMethod?.OverriddenMethod != null)
                    ConstrainOverridingMethod(m.SetMethod);

                if (m.GetMethod != null && m.SetMethod != null &&
                    (m.GetMethod.Stats & EntityStats.RefCount) != (m.SetMethod.Stats & EntityStats.RefCount) &&
                    m.GetMethod.OverriddenMethod != null && m.SetMethod.OverriddenMethod != null &&
                    (m.GetMethod.OverriddenMethod.Stats & EntityStats.RefCount) != (m.SetMethod.OverriddenMethod.Stats & EntityStats.RefCount))
                {
                    VisitFunction(m.GetMethod.OverriddenMethod);
                    VisitFunction(m.SetMethod.OverriddenMethod);
                }
            }

            foreach (var m in dt.Operators)
            {
                switch (m.Type)
                {
                    case OperatorType.Equality:
                    case OperatorType.Inequality:
                        VisitFunction(m);
                        break;
                }
            }

            foreach (var e in dt.InterfaceMethods)
                if (!e.Value.IsPrototype &&
                    e.Value.Prototype.HasRefCount)
                    VisitFunction(e.Value);

            foreach (var it in dt.Interfaces)
            {
                foreach (var f in it.EnumerateFunctions())
                {
                    var decl = f as Method;

                    if (decl == null || !decl.MasterDefinition.HasRefCount)
                        continue;

                    for (var bt = dt; bt != null; bt = bt.Base)
                    {
                        Method impl;
                        if (bt.InterfaceMethods.TryGetValue(decl, out impl))
                        {
                            VisitFunction(impl);
                            break;
                        }
                    }
                }

                if (!it.HasRefCount && !CanStrip(it))
                    VisitType(it);
            }
        }

        void ConstrainGenericParameters(DataType dt)
        {
            for (int i = 0; i < dt.GenericParameters.Length; i++)
            {
                var gp = dt.GenericParameters[i];
                if (gp.Base != null)
                    ConstrainBaseType(gp.Base);

                if (gp.Constructors.Count != 0)
                    foreach (var pt in dt.GenericParameterizations)
                        VisitFunction(pt.GenericArguments[i].TryGetDefaultConstructor() ?? Function.Null);
            }
        }

        void ConstrainRefsRecursive(Namespace ns)
        {
            for (int i = 0; i < ns.Namespaces.Count; i++)
                ConstrainRefsRecursive(ns.Namespaces[i]);
            for (int i = 0; i < ns.Types.Count; i++)
                ConstrainRefsRecursive(ns.Types[i]);
        }

        void NormalizeRefsRecursive(DataType dt)
        {
            if (!dt.HasRefCount)
                return;

            if (dt.IsGenericDefinition)
                foreach (var p in dt.GenericParameterizations)
                    NormalizeRefsRecursive(p);

            for (int i = 0; i < dt.NestedTypes.Count; i++)
                NormalizeRefsRecursive(dt.NestedTypes[i]);

            foreach (var m in dt.Events)
            {
                if (m.AddMethod != null)
                    m.Stats |= m.AddMethod.Stats & (EntityStats.RefCount | EntityStats.RefCountAsOverridden);
                if (m.RemoveMethod != null)
                    m.Stats |= m.RemoveMethod.Stats & (EntityStats.RefCount | EntityStats.RefCountAsOverridden);

                if (m.AddMethod != null)
                    m.AddMethod.Stats |= m.Stats & (EntityStats.RefCount | EntityStats.RefCountAsOverridden);
                if (m.RemoveMethod != null)
                    m.RemoveMethod.Stats |= m.Stats & (EntityStats.RefCount | EntityStats.RefCountAsOverridden);
            }

            foreach (var m in dt.Properties)
            {
                if (m.GetMethod != null)
                    m.Stats |= m.GetMethod.Stats & (EntityStats.RefCount | EntityStats.RefCountAsOverridden);
                if (m.SetMethod != null)
                    m.Stats |= m.SetMethod.Stats & (EntityStats.RefCount | EntityStats.RefCountAsOverridden);

                if (m.GetMethod != null)
                    m.GetMethod.Stats |= m.Stats & (EntityStats.RefCount | EntityStats.RefCountAsOverridden);
                if (m.SetMethod != null)
                    m.SetMethod.Stats |= m.Stats & (EntityStats.RefCount | EntityStats.RefCountAsOverridden);
            }
        }

        void NormalizeRefsRecursive(Namespace ns)
        {
            for (int i = 0; i < ns.Namespaces.Count; i++)
                NormalizeRefsRecursive(ns.Namespaces[i]);
            for (int i = 0; i < ns.Types.Count; i++)
                NormalizeRefsRecursive(ns.Types[i]);
        }

        bool CanStrip(Namespace ns)
        {
            return Environment.Strip && ns.Blocks.Count == 0 && ns.Types.Count == 0 && ns.Namespaces.Count == 0;
        }

        bool CanStrip(DataType dt)
        {
            return Environment.Strip && !dt.MasterDefinition.HasRefCount && !dt.Prototype.HasRefCount ||
                !Backend.CanExportDontExports && !Environment.CanExport(dt);
        }

        bool CanStrip(Member m)
        {
            return Environment.Strip && !m.MasterDefinition.HasRefCount && !m.Prototype.HasRefCount ||
                !Backend.CanExportDontExports && !Environment.CanExport(m);
        }

        void IncrementCounters(Namespace ns)
        {
            _namespacesStripped++;
        }

        void IncrementCounters(DataType dt)
        {
            if (!dt.IsMasterDefinition || !Environment.CanExport(dt))
                return;

            _typesStripped++;
            _fieldsStripped += dt.EnumerateFields().Count();
            _functionsStripped += dt.EnumerateFunctions().Count();

            foreach (var it in dt.NestedTypes)
                IncrementCounters(it);
        }

        void IncrementCounters(Function f)
        {
            if (f != f.MasterDefinition || !Environment.CanExport(f))
                return;

            _functionsStripped++;
        }

        void IncrementCounters(Field f)
        {
            if (!f.IsMasterDefinition || !Environment.CanExport(f))
                return;

            _fieldsStripped++;
        }

        DataType GetStrippedBaseType(DataType dt)
        {
            return dt == null || !CanStrip(dt) ? dt : GetStrippedBaseType(dt.Base);
        }

        InterfaceType[] GetStrippedInterfaceTypes(InterfaceType[] interfaceTypes)
        {
            var result = new List<InterfaceType>();

            foreach (var it in interfaceTypes)
                if (!CanStrip(it))
                    result.Add(it);

            return result.ToArray();
        }

        Method GetStrippedOverriddenMethod(Method m)
        {
            return m == null || !CanStrip(m) ? m : GetStrippedOverriddenMethod(m.OverriddenMethod);
        }

        void StripOverriddenMethod(Method m)
        {
            m.SetOverriddenMethod(GetStrippedOverriddenMethod(m.OverriddenMethod));

            if (m.Modifiers.HasFlag(Modifiers.Override) && m.OverriddenMethod == null)
                m.Modifiers = (m.Modifiers & ~Modifiers.Override) | Modifiers.Virtual;

            if (Environment.Strip && !m.Stats.HasFlag(EntityStats.RefCountAsOverridden))
            {
                if (m.Modifiers.HasFlag(Modifiers.Virtual))
                {
                    m.Modifiers &= ~(Modifiers.Virtual | Modifiers.Sealed);

                    if (m == m.MasterDefinition)
                        _functionsStrippedFromVTable++;
                }
                else if (m.Modifiers.HasFlag(Modifiers.Override) && !m.Modifiers.HasFlag(Modifiers.Sealed))
                {
                    m.Modifiers |= Modifiers.Sealed;

                    if (m == m.MasterDefinition)
                        _functionsSealed++;
                }
            }
        }

        Property GetStrippedOverriddenProperty(Property m)
        {
            return m == null || !CanStrip(m) ? m : GetStrippedOverriddenProperty(m.OverriddenProperty);
        }

        void StripOverriddenProperty(Property m)
        {
            m.SetOverriddenProperty(GetStrippedOverriddenProperty(m.OverriddenProperty));

            if (m.Modifiers.HasFlag(Modifiers.Override) && m.OverriddenProperty == null)
                m.Modifiers = (m.Modifiers & ~Modifiers.Override) | Modifiers.Virtual;

            if (Environment.Strip && !m.Stats.HasFlag(EntityStats.RefCountAsOverridden))
            {
                if (m.Modifiers.HasFlag(Modifiers.Virtual))
                    m.Modifiers &= ~(Modifiers.Virtual | Modifiers.Sealed);
                else if (m.Modifiers.HasFlag(Modifiers.Override) && !m.Modifiers.HasFlag(Modifiers.Sealed))
                    m.Modifiers |= Modifiers.Sealed;
            }

            if (m.GetMethod != null)
                StripOverriddenMethod(m.GetMethod);
            if (m.SetMethod != null)
                StripOverriddenMethod(m.SetMethod);
        }

        Event GetStrippedOverriddenEvent(Event m)
        {
            return m == null || !CanStrip(m) ? m : GetStrippedOverriddenEvent(m.OverriddenEvent);
        }

        void StripOverriddenEvent(Event m)
        {
            m.SetOverriddenEvent(GetStrippedOverriddenEvent(m.OverriddenEvent));

            if (m.Modifiers.HasFlag(Modifiers.Override) && m.OverriddenEvent == null)
                m.Modifiers = (m.Modifiers & ~Modifiers.Override) | Modifiers.Virtual;

            if (Environment.Strip && !m.Stats.HasFlag(EntityStats.RefCountAsOverridden))
            {
                if (m.Modifiers.HasFlag(Modifiers.Virtual))
                    m.Modifiers &= ~(Modifiers.Virtual | Modifiers.Sealed);
                else if (m.Modifiers.HasFlag(Modifiers.Override) && !m.Modifiers.HasFlag(Modifiers.Sealed))
                    m.Modifiers |= Modifiers.Sealed;
            }

            if (m.AddMethod != null)
                StripOverriddenMethod(m.AddMethod);
            if (m.RemoveMethod != null)
                StripOverriddenMethod(m.RemoveMethod);
        }

        void OptimizeRefsRecursive(DataType dt)
        {
            if (dt.IsGenericDefinition)
            {
                foreach (var p in dt.GenericParameterizations)
                    OptimizeRefsRecursive(p);
                foreach (var gt in dt.FlattenedParameters)
                    OptimizeRefsRecursive(gt);
            }

            for (int i = dt.NestedTypes.Count - 1; i >= 0; i--)
            {
                if (CanStrip(dt.NestedTypes[i]))
                {
                    IncrementCounters(dt.NestedTypes[i]);
                    dt.StrippedTypes.Add(dt.NestedTypes[i]);
                    dt.NestedTypes.RemoveAt(i);
                }
                else
                    OptimizeRefsRecursive(dt.NestedTypes[i]);
            }

            dt.SetBase(GetStrippedBaseType(dt.Base));
            dt.SetInterfaces(GetStrippedInterfaceTypes(dt.Interfaces));

            if (Environment.Strip &&
                !dt.Stats.HasFlag(EntityStats.RefCountAsBase) &&
                dt.IsClass &&
                !dt.IsAbstract &&
                !dt.IsStatic &&
                !dt.IsSealed &&
                !dt.HasAttribute(Essentials.TargetSpecificTypeAttribute))
            {
                dt.Modifiers |= Modifiers.Sealed;

                if (dt.IsMasterDefinition)
                    _typesSealed++;
            }

            foreach (var m in dt.InterfaceMethods.Keys.ToArray())
                if (CanStrip(m))
                    dt.InterfaceMethods.Remove(m);

            for (int i = dt.Fields.Count - 1; i >= 0; i--)
            {
                if (CanStrip(dt.Fields[i]))
                {
                    IncrementCounters(dt.Fields[i]);
                    dt.StrippedMembers.Add(dt.Fields[i]);
                    dt.Fields.RemoveAt(i);
                }
            }

            for (int i = dt.Constructors.Count - 1; i >= 0; i--)
            {
                if (CanStrip(dt.Constructors[i]))
                {
                    IncrementCounters(dt.Constructors[i]);
                    dt.StrippedMembers.Add(dt.Constructors[i]);
                    dt.Constructors.RemoveAt(i);
                }
            }

            for (int i = dt.Methods.Count - 1; i >= 0; i--)
            {
                if (CanStrip(dt.Methods[i]))
                {
                    IncrementCounters(dt.Methods[i]);
                    dt.StrippedMembers.Add(dt.Methods[i]);
                    dt.Methods.RemoveAt(i);
                }
                else
                {
                    var m = dt.Methods[i];
                    if (m.IsGenericDefinition)
                    {
                        foreach (var p in m.GenericType.GenericParameterizations)
                            OptimizeRefsRecursive(p);
                        foreach (var gt in m.GenericType.FlattenedParameters)
                            OptimizeRefsRecursive(gt);
                        foreach (var p in m.GenericParameterizations)
                            StripOverriddenMethod(p);
                    }
                    StripOverriddenMethod(m);
                }
            }

            for (int i = dt.Operators.Count - 1; i >= 0; i--)
            {
                if (CanStrip(dt.Operators[i]))
                {
                    IncrementCounters(dt.Operators[i]);
                    dt.StrippedMembers.Add(dt.Operators[i]);
                    dt.Operators.RemoveAt(i);
                }
            }

            for (int i = dt.Casts.Count - 1; i >= 0; i--)
            {
                if (CanStrip(dt.Casts[i]))
                {
                    IncrementCounters(dt.Casts[i]);
                    dt.StrippedMembers.Add(dt.Casts[i]);
                    dt.Casts.RemoveAt(i);
                }
            }

            for (int i = dt.Properties.Count - 1; i >= 0; i--)
            {
                if (dt.Properties[i].GetMethod != null)
                {
                    if (CanStrip(dt.Properties[i].GetMethod))
                    {
                        IncrementCounters(dt.Properties[i].GetMethod);
                        dt.StrippedMembers.Add(dt.Properties[i].GetMethod);
                        dt.Properties[i].RemoveGetter();
                    }
                    else
                        StripOverriddenMethod(dt.Properties[i].GetMethod);
                }

                if (dt.Properties[i].SetMethod != null)
                {
                    if (CanStrip(dt.Properties[i].SetMethod))
                    {
                        IncrementCounters(dt.Properties[i].SetMethod);
                        dt.StrippedMembers.Add(dt.Properties[i].SetMethod);
                        dt.Properties[i].RemoveSetter();
                    }
                    else
                        StripOverriddenMethod(dt.Properties[i].SetMethod);
                }

                if (dt.Properties[i].GetMethod == null &&
                    dt.Properties[i].SetMethod == null)
                {
                    var m = dt.Properties[i];

                    if (m.ImplicitField != null)
                    {
                        if (CanStrip(m.ImplicitField))
                            IncrementCounters(m.ImplicitField);
                        else
                            dt.Fields.Add(m.ImplicitField);
                    }

                    dt.StrippedMembers.Add(dt.Properties[i]);
                    dt.Properties.RemoveAt(i);
                }
                else
                    StripOverriddenProperty(dt.Properties[i]);
            }

            for (int i = dt.Events.Count - 1; i >= 0; i--)
            {
                if (dt.Events[i].AddMethod != null)
                {
                    if (CanStrip(dt.Events[i].AddMethod))
                    {
                        IncrementCounters(dt.Events[i].AddMethod);
                        dt.StrippedMembers.Add(dt.Events[i].AddMethod);
                        dt.Events[i].RemoveAddMethod();
                    }
                    else
                        StripOverriddenMethod(dt.Events[i].AddMethod);
                }

                if (dt.Events[i].RemoveMethod != null)
                {
                    if (CanStrip(dt.Events[i].RemoveMethod))
                    {
                        IncrementCounters(dt.Events[i].RemoveMethod);
                        dt.StrippedMembers.Add(dt.Events[i].RemoveMethod);
                        dt.Events[i].RemoveRemoveMethod();
                    }
                    else
                        StripOverriddenMethod(dt.Events[i].RemoveMethod);
                }

                if (dt.Events[i].AddMethod == null &&
                    dt.Events[i].RemoveMethod == null)
                {
                    var m = dt.Events[i];

                    if (m.ImplicitField != null)
                    {
                        if (CanStrip(m.ImplicitField))
                            IncrementCounters(m.ImplicitField);
                        else
                            dt.Fields.Add(m.ImplicitField);
                    }

                    dt.StrippedMembers.Add(dt.Events[i]);
                    dt.Events.RemoveAt(i);
                }
                else
                    StripOverriddenEvent(dt.Events[i]);
            }

            if (Environment.Strip && !dt.IsEnum)
            {
                for (int i = dt.Literals.Count - 1; i >= 0; i--)
                {
                    var m = dt.Literals[i];

                    if (!m.Stats.HasFlag(EntityStats.RefCount))
                    {
                        dt.StrippedMembers.Add(m);
                        dt.Literals.RemoveAt(i);
                    }
                }
            }
        }

        void OptimizeRefsRecursive(Namespace ns)
        {
            if (!Backend.CanExportDontExports)
                ns.Blocks.Clear();

            for (int i = ns.Types.Count - 1; i >= 0; i--)
            {
                if (CanStrip(ns.Types[i]))
                {
                    IncrementCounters(ns.Types[i]);
                    ns.StrippedTypes.Add(ns.Types[i]);
                    ns.Types.RemoveAt(i);
                }
                else
                    OptimizeRefsRecursive(ns.Types[i]);
            }

            for (int i = ns.Namespaces.Count - 1; i >= 0; i--)
            {
                OptimizeRefsRecursive(ns.Namespaces[i]);

                if (CanStrip(ns.Namespaces[i]))
                {
                    IncrementCounters(ns.Namespaces[i]);
                    ns.StrippedNamespaces.Add(ns.Namespaces[i]);
                    ns.Namespaces.RemoveAt(i);
                }
            }
        }

        public new void End()
        {
            // Visit extensions last
            VisitExtension(Data.Extensions);

            while (_hasFunctions)
            {
                _hasFunctions = false;
                ConstrainRefsRecursive(Data.IL);
            }

            NormalizeRefsRecursive(Data.IL);
            OptimizeRefsRecursive(Data.IL);

            if (!Environment.Strip || Backend.BuildType == BuildType.Library || !Log.IsVerbose)
                return;

            PrintCounter(_namespacesStripped, "namespaces stripped");
            PrintCounter(_typesStripped, "types stripped");
            PrintCounter(_fieldsStripped, "fields stripped");
            PrintCounter(_functionsStripped, "functions stripped");
            PrintCounter(_typesSealed, "classes sealed");
            PrintCounter(_functionsSealed, "functions sealed");
            PrintCounter(_functionsStrippedFromVTable, "functions stripped from vtable");
        }

        void PrintCounter(int counter, string description)
        {
            Log.Message($"{counter,6} {description}");
        }

        void FindTypes(Namespace n, List<DataType> list)
        {
            list.AddRange(n.Types);
            foreach (var ns in n.Namespaces)
                FindTypes(ns, list);
        }

        void KeepSimulatorEntities()
        {
            var types = new List<DataType>();
            FindTypes(Data.IL, types);

            var neededTypes = types.Where(IsNeededBySimulator);

            foreach (var t in neededTypes)
            {
                Keep(t);

                foreach (var m in t.EnumerateMembersRecursive().Where(IsNeededBySimulator))
                    Keep(m);
            }
        }

        bool IsNeededBySimulator(Member m)
        {
            if (m.DeclaringType.FullName.Contains("Outracks.Simulator"))
                return true;

            var p = m as Property;

            if (p != null)
                return p.IsPublic;

            var e = m as Event;
            if (e != null)
                return e.IsPublic;

            var meth = m as Method;
            if (meth != null)
                return meth.IsPublic || meth.Prototype.IsConstructor;

            return false;
        }

        bool IsNeededBySimulator(DataType dt)
        {
            // Don't strip stuff needed by preview simulator
            if (dt.FullName.StartsWith("Outracks.Simulator.") ||
                dt.FullName.StartsWith("Fuse."))
                return true;

            return dt.IsClass && dt.IsPublic && !dt.IsAbstract && (
                    dt.TryGetDefaultConstructor() != null ||
                    dt.EnumerateMembersRecursive().OfType<Constructor>().Any(IsUXConstructor)
                );
        }

        bool IsUXConstructor(Constructor c)
        {
            return c.IsPublic && (
                    c.Parameters.Length == 0 ||
                    c.Attributes.Any(x => x.Constructor.DeclaringType.FullName == "Uno.UX.UXConstructorAttribute")
                );
        }
    }
}
