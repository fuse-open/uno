using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Building.Types
{
    class MemberTransform : CompilerPass
    {
        public override bool Condition => !Backend.IsDefault;

        public MemberTransform(CompilerPass parent)
            : base(parent)
        {
        }

        public override bool Begin()
        {
            Traverse(FlattenMembers);
            return true;
        }

        void FlattenMembers(DataType dt)
        {
            if (dt.HasAttribute(Essentials.DontExportAttribute))
                return;

            if (Backend.Has(TypeOptions.FlattenConstructors))
            {
                foreach (var f in dt.Constructors)
                {
                    var src = f.Source;
                    Method init = null
                        , factory = null;

                    if (f.DeclaringType.BuiltinType != BuiltinType.Object)
                    {
                        init = new Method(src, dt, f.DocComment, 
                            (f.Modifiers & ~Modifiers.ProtectionModifiers) | Modifiers.Protected,
                            ".ctor", DataType.Void, f.Parameters, f.Body);
                        if (!f.IsMasterDefinition)
                            init.SetMasterDefinition(((ConstructorPair) f.MasterDefinition.Tag).Initializer);
                        init.SetPrototype(f);
                        dt.Methods.Add(init);
                    }

                    if (!dt.IsAbstract)
                    {
                        var pt = TypeBuilder.Parameterize(dt.MasterDefinition);

                        if (f.IsMasterDefinition)
                            factory = new Method(src, dt, f.DocComment, 
                                (f.Modifiers & Modifiers.ProtectionModifiers) | Modifiers.Static | Modifiers.Generated,
                                "New", pt, f.Parameters, new Scope(src));
                        else
                        {
                            var master = ((ConstructorPair) f.MasterDefinition.Tag).Factory;
                            factory = new Method(src, dt, f.DocComment, 
                                (f.Modifiers & Modifiers.ProtectionModifiers) | Modifiers.Static | Modifiers.Generated,
                                master.UnoName, dt, f.Parameters, master.Body);
                            factory.SetMasterDefinition(master);
                        }

                        factory.SetPrototype(f);
                        dt.Methods.Add(factory);

                        if (pt == dt)
                        {
                            var var = new Variable(src, factory, dt.MasterDefinition.GetUniqueIdentifier("obj"), dt);

                            if (dt.IsClass)
                                var.OptionalValue = new AllocObject(src, dt);

                            factory.Body.Statements.Add(new VariableDeclaration(var));

                            if (init != null)
                            {
                                var args = new Expression[f.Parameters.Length];

                                for (int i = 0; i < args.Length; i++)
                                {
                                    args[i] = new LoadArgument(src, factory, i);

                                    switch (f.Parameters[i].Modifier)
                                    {
                                        case ParameterModifier.Out:
                                            args[i] = new AddressOf(args[i], AddressType.Out);
                                            break;
                                        case ParameterModifier.Ref:
                                            args[i] = new AddressOf(args[i], AddressType.Ref);
                                            break;
                                        case ParameterModifier.Const:
                                            args[i] = new AddressOf(args[i], AddressType.Const);
                                            break;
                                    }
                                }

                                factory.Body.Statements.Add(new CallMethod(src, new LoadLocal(src, var).Address, init, args));
                            }

                            factory.Body.Statements.Add(new Return(src, new LoadLocal(src, var)));
                        }
                    }

                    f.Tag = new ConstructorPair
                    {
                        Initializer = init,
                        Factory = factory
                    };
                }

                dt.Constructors.Clear();
            }
            if (Backend.Has(TypeOptions.FlattenProperties))
            {
                foreach (var f in dt.Properties)
                {
                    if (f.GetMethod != null)
                        dt.Methods.Add(f.GetMethod);
                    if (f.SetMethod != null)
                        dt.Methods.Add(f.SetMethod);
                    if (f.ImplicitField != null)
                        dt.Fields.Add(f.ImplicitField);
                }

                dt.Properties.Clear();
            }
            if (Backend.Has(TypeOptions.FlattenEvents))
            {
                foreach (var f in dt.Events)
                {
                    if (f.AddMethod != null)
                        dt.Methods.Add(f.AddMethod);
                    if (f.RemoveMethod != null)
                        dt.Methods.Add(f.RemoveMethod);
                    if (f.ImplicitField != null)
                        dt.Fields.Add(f.ImplicitField);
                }

                dt.Events.Clear();
            }
            if (Backend.Has(TypeOptions.FlattenOperators))
            {
                foreach (var f in dt.Operators)
                {
                    if (f.IsIntrinsic)
                        continue;

                    var m = new Method(f.Source, dt, f.DocComment, f.Modifiers, f.UnoName, f.ReturnType, f.Parameters, f.Body);
                    m.SetMasterDefinition(f.MasterDefinition.Tag as Method);
                    m.SetPrototype(f);
                    dt.Methods.Add(m);
                    f.Tag = m;
                }

                dt.Operators.Clear();
            }
            if (Backend.Has(TypeOptions.FlattenCasts))
            {
                foreach (var f in dt.Casts)
                {
                    if (f.IsIntrinsic)
                        continue;

                    var m = new Method(f.Source, dt, f.DocComment, f.Modifiers & ~Modifiers.CastModifiers, f.UnoName, f.ReturnType, f.Parameters, f.Body);
                    m.SetMasterDefinition(f.MasterDefinition.Tag as Method);
                    m.SetPrototype(f);
                    dt.Methods.Add(m);
                    f.Tag = m;
                }

                dt.Casts.Clear();
            }
        }

        public override void Begin(ref Expression e, ExpressionUsage u)
        {
            switch (e.ExpressionType)
            {
                case ExpressionType.NewObject:
                {
                    var s = (NewObject) e;
                    var pair = s.Constructor.Tag as ConstructorPair;
                    if (pair != null)
                        e = pair.Factory != null
                            ? new CallMethod(s.Source, null, pair.Factory, s.Arguments)
                            : (Expression)new AllocObject(s.Source, s.ReturnType);
                    break;
                }
                case ExpressionType.CallConstructor:
                {
                    var s = (CallConstructor) e;
                    var pair = s.Constructor.Tag as ConstructorPair;
                    if (pair != null)
                    {
                        var pt = TypeBuilder.Parameterize(Type);
                        e = pair.Initializer != null
                            ? new CallMethod(s.Source,
                                    pair.Initializer.DeclaringType.MasterDefinition == pt.MasterDefinition
                                        ? (Expression)new This(s.Source, pt)
                                        : new Base(s.Source, pt.Base),
                                    pair.Initializer, s.Arguments)
                            : (Expression)new NoOp(s.Source);
                    }
                    break;
                }
                case ExpressionType.CallBinOp:
                {
                    var s = (CallBinOp) e;
                    var m = s.Operator.Tag as Method;
                    if (m != null)
                        e = new CallMethod(s.Source, null, m, s.Left, s.Right);
                    break;
                }
                case ExpressionType.CallUnOp:
                {
                    var s = (CallUnOp) e;
                    var m = s.Operator.Tag as Method;
                    if (m != null)
                        e = new CallMethod(s.Source, null, m, s.Operand);
                    break;
                }
                case ExpressionType.CallCast:
                {
                    var s = (CallCast) e;
                    var m = s.Cast.Tag as Method;
                    if (m != null)
                        e = new CallMethod(s.Source, null, m, s.Operand);
                    break;
                }
                case ExpressionType.SetProperty:
                {
                    var s = (SetProperty) e;
                    if (Backend.Has(TypeOptions.FlattenProperties))
                        e = new CallMethod(s.Source, s.Object, s.Property.SetMethod, s.Arguments.Concat(s.Value));
                    break;
                }
                case ExpressionType.GetProperty:
                {
                    var s = (GetProperty) e;
                    if (Backend.Has(TypeOptions.FlattenProperties))
                        e = new CallMethod(s.Source, s.Object, s.Property.GetMethod, s.Arguments);
                    break;
                }
                case ExpressionType.AddListener:
                {
                    var s = (AddListener) e;
                    if (Backend.Has(TypeOptions.FlattenEvents))
                        e = new CallMethod(s.Source, s.Object, s.Event.AddMethod, s.Listener);
                    break;
                }
                case ExpressionType.RemoveListener:
                {
                    var s = (RemoveListener) e;
                    if (Backend.Has(TypeOptions.FlattenEvents))
                        e = new CallMethod(s.Source, s.Object, s.Event.RemoveMethod, s.Listener);
                    break;
                }
            }
        }
    }
}
