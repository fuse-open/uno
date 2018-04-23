using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Disasm.ILView.Namespaces;

namespace Uno.Disasm.ILView.Members
{
    public class FunctionItem : ILItem
    {
        public readonly Function Function;

        public override string DisplayName => Function.NameAndParameterList;
        public override object Object => Function;

        public override ILIcon Icon
        {
            get
            {
                if (Function.IsCast)
                    return ILIcon.Cast;
                if (Function.IsOperator)
                    return ILIcon.Operator;

                if (Function.IsConstructor)
                    return Function.IsStatic ?
                        Function.IsPublic ? ILIcon.ConstructorStatic : ILIcon.ConstructorStaticNonPublic :
                        Function.IsPublic ? ILIcon.Constructor : ILIcon.ConstructorNonPublic;
                
                return Function.IsStatic ?
                    Function.IsPublic ? ILIcon.MethodStatic : ILIcon.MethodStaticNonPublic :
                    Function.IsPublic ? ILIcon.Method : ILIcon.MethodNonPublic;
            }
        }

        public FunctionItem(Function function)
        {
            Function = function;

            if (!function.IsConstructor && !function.ReturnType.IsVoid)
                Suffix = function.ReturnType.ToString();

            if (function is Method && (function as Method).IsGenericDefinition)
                AddChild(new ParameterizationCollection((function as Method).GenericParameterizations));

            if (function is Method && (function as Method).DrawBlocks.Count > 0)
            {
                foreach (var d in (function as Method).DrawBlocks)
                {
                    var di = new BlockItem(d);
                    AddChild(di);

                    if (d.Drawables != null)
                        foreach (var p in d.Drawables)
                            if (p != null) di.AddChild(new DrawItem(p));
                }
            }
        }

        protected override void Disassemble(Disassembler disasm)
        {
            disasm.AppendHeader(Function);
            disasm.AppendFunction(Function);
        }
    }
}
