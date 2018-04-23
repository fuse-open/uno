using System;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;

namespace Uno.Compiler.Core.IL.Building.Functions
{
    class ExtensionTransform : CompilerPass
    {
        public static Expression CreateObject(Source src, Function f, DataType dt)
        {
            return f.IsStatic ? null : new This(src, dt).Address;
        }

        public static Expression[] CreateArgumentList(Source src, Function f)
        {
            var result = new Expression[f.Parameters.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new LoadArgument(src, f, i);

                switch (f.Parameters[i].Modifier)
                {
                    case ParameterModifier.Const:
                        result[i] = new AddressOf(result[i], AddressType.Const);
                        break;
                    case ParameterModifier.Ref:
                        result[i] = new AddressOf(result[i], AddressType.Ref);
                        break;
                    case ParameterModifier.Out:
                        result[i] = new AddressOf(result[i], AddressType.Out);
                        break;
                }
            }

            return result;
        }

        public ExtensionTransform(CompilerPass parent)
            : base(parent)
        {
        }

        public override bool Condition => !Backend.IsDefault;

        public override bool Begin(DataType dt)
        {
            foreach (var e in dt.EnumerateAttribute(Essentials.ProcessFileAttribute, 3))
            {
                var type = e.Arguments[0].ConstantString;
                var name = e.Arguments[1].ConstantString;
                var targetName = e.Arguments[2].ConstantString;
                Environment.ProcessFile(dt, type, e.Source, name, targetName);
            }

            foreach (var e in dt.EnumerateAttribute(Essentials.RequireAttribute, 2))
            {
                var key = e.Arguments[0].ConstantString;
                var value = e.Arguments[1].ConstantString;
                Environment.Require(dt, key, e.Source, value);
            }

            foreach (var e in dt.EnumerateAttribute(Essentials.SetAttribute, 2))
            {
                var key = e.Arguments[0].ConstantString;
                var value = e.Arguments[1].ConstantString;
                Environment.Set(dt, key, e.Source, value);
            }

            return true;
        }

        public override bool Begin(Function f)
        {
            foreach (var e in f.EnumerateAttribute(Essentials.ProcessFileAttribute, 3))
            {
                var type = e.Arguments[0].ConstantString;
                var name = e.Arguments[1].ConstantString;
                var targetName = e.Arguments[2].ConstantString;
                Environment.ProcessFile(f, type, e.Source, name, targetName);
            }

            foreach (var e in f.EnumerateAttribute(Essentials.RequireAttribute, 2))
            {
                var key = e.Arguments[0].ConstantString;
                var value = e.Arguments[1].ConstantString;
                Environment.Require(f, key, e.Source, value);
            }

            foreach (var e in f.EnumerateAttribute(Essentials.SetAttribute, 2))
            {
                var key = e.Arguments[0].ConstantString;
                var value = e.Arguments[1].ConstantString;
                Environment.Set(f, key, e.Source, value);
            }

            if (!f.CanLink && Backend.CanLink(f))
                f.Stats |= EntityStats.CanLink;

            FunctionExtension ext;
            if (!f.CanLink &&
                Environment.TryGetExtension(f, out ext) && ext.HasImplementation)
                f.SetBody(new Scope(ext.ImplementationSource, CreateStatement(f, ext)));

            return false;
        }

        Statement CreateStatement(Function f, FunctionExtension ext)
        {
            var obj = CreateObject(ext.ImplementationSource, f, f.DeclaringType);
            var args = CreateArgumentList(ext.ImplementationSource, f);

            switch (ext.ImplementationType)
            {
                case ImplementationType.EmptyBody:
                    return new NoOp(ext.ImplementationSource);
                case ImplementationType.Body:
                    return new ExternScope(ext.ImplementationSource, AttributeList.Empty, ext.ImplementationString, obj, args, ext.Scopes);
                case ImplementationType.Expression:
                    return f.ReturnType.IsVoid
                        ? (Statement) new ExternOp(ext.ImplementationSource, AttributeList.Empty, f.ReturnType, ext.ImplementationString, obj, args, ext.Scopes)
                        :             new Return(ext.ImplementationSource, new ExternOp(ext.ImplementationSource, AttributeList.Empty, f.ReturnType, ext.ImplementationString, obj, args, ext.Scopes));
            }

            throw new InvalidOperationException();
        }
    }
}
