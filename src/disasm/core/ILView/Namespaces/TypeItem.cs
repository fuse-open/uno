using System.Linq;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Disasm.ILView.Members;

namespace Uno.Disasm.ILView.Namespaces
{
    public class TypeItem : ILItem
    {
        public readonly DataType Type;
        public readonly OverloadCollection Overloads = new OverloadCollection();

        public override object Object => Type;
        public override string DisplayName => Type.GetNestedName();

        public override ILIcon Icon
        {
            get
            {
                switch (Type.TypeType)
                {
                    case TypeType.Struct:
                        return Type.IsPublic ? ILIcon.Struct : ILIcon.StructNonPublic;
                    case TypeType.Enum:
                        return Type.IsPublic ? ILIcon.Enum : ILIcon.EnumNonPublic;
                    case TypeType.Delegate:
                        return Type.IsPublic ? ILIcon.Delegate : ILIcon.DelegateNonPublic;
                    case TypeType.Interface:
                        return Type.IsPublic ? ILIcon.Interface : ILIcon.InterfaceNonPublic;
                    default:
                        return Type.IsPublic ? ILIcon.Class : ILIcon.ClassNonPublic;
                }
            }
        }

        public TypeItem(DataType type)
        {
            Type = type;
            AddChild(Overloads);

            if (Type.Base != null &&
                Type.Base.BuiltinType != BuiltinType.Object &&
                Type.Base.BuiltinType != BuiltinType.Int)
                Suffix = Type.Base.ToString();

            if (type.Initializer != null)
                AddChild(new FunctionItem(type.Initializer));
            if (type.Finalizer != null)
                AddChild(new FunctionItem(type.Finalizer));
            foreach (var l in type.Literals)
                AddChild(new LiteralItem(l));
            foreach (var e in type.Events)
                AddChild(new EventItem(e));
            foreach (var f in type.Fields)
                AddChild(new FieldItem(f));
            foreach (var p in type.Properties)
                AddChild(new PropertyItem(p));
            foreach (var m in type.Methods)
                AddChild(CreateMethodItem(m));
            foreach (var m in type.Constructors)
                AddChild(new FunctionItem(m));
            foreach (var m in type.Operators)
                AddChild(new FunctionItem(m));
            foreach (var m in type.Casts)
                AddChild(new FunctionItem(m));
        }

        static FunctionItem CreateMethodItem(Method m)
        {
            var result = new FunctionItem(m);

            if (m.IsGenericDefinition && m.GenericParameterizations.Count() > 0)
            {
                var ptItems = new ParameterizationCollection(m.GenericParameterizations);

                for (int i = 0; i < ptItems.Children.Count; i++)
                {
                    var ptRoot = ptItems.Children[i] as FunctionItem;

                    if (ptRoot != null)
                    {
                        var method = (Method)ptRoot.Object;
                        var param = method.GenericArguments.FirstOrDefault();

                        if (param is GenericParameterType && param.Parent == m.GenericType)
                        {
                            ptItems.Children.RemoveAt(i);
                            ptRoot.AddChild(new DefinitionCollection(m));
                            ptRoot.AddChild(ptItems);
                            result = ptRoot;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        protected override void Disassemble(Disassembler disasm, bool publicOnly)
        {
            disasm.AppendHeader(Type);
            disasm.BeginNamespace(Type.FirstNamespace);
            disasm.AppendType(Type, publicOnly);

            foreach (var c in Overloads.Children)
            {
                var t = c as TypeItem;
                if (t != null)
                    disasm.AppendType(t.Type, publicOnly);
            }

            disasm.EndNamespace();
        }
    }
}
