using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class TypeBuilder
    {
        public void CreateDelegate(AstDelegate ast, Namescope parent)
        {
            var src = ast.Name.Source;
            var result = new DelegateType(src, parent, ast.DocComment, GetTypeModifiers(parent, ast.Modifiers), ast.Name.Symbol);

            if (ast.OptionalGenericSignature != null)
                CreateGenericSignature(result, ast.OptionalGenericSignature, false);

            if (parent is DataType)
                (parent as DataType).NestedTypes.Add(result);
            else if (parent is Namespace)
                (parent as Namespace).Types.Add(result);
            else
                Log.Error(result.Source, ErrorCode.I3047, "'delegate' is not allowed in this context");

            if (ast.Members != null)
                Log.Error(src, ErrorCode.I3019, "'delegate' cannot contain members");

            if (ast.Attributes.Count > 0)
                EnqueueAttributes(result,
                    x => result.SetAttributes(_compiler.CompileAttributes(result.Parent, ast.Attributes)));

            EnqueueType(result,
                assignBaseType: x =>
                {
                    var deferredActions = new List<Action>();
                    var parameterizedType = Parameterize(result);

                    result.SetBase(_ilf.Essentials.Delegate);
                    result.SetReturnType(_resolver.GetType(result, ast.ReturnType));
                    result.SetParameters(_compiler.CompileParameterList(result, ast.Parameters, deferredActions));

                    if (parameterizedType != result)
                    {
                        var parameterizedDelegate = (DelegateType)parameterizedType;
                        parameterizedDelegate.SetBase(result.Base);
                        parameterizedDelegate.SetReturnType(result.ReturnType);
                        parameterizedDelegate.SetParameters(result.Parameters);
                    }

                    foreach (var action in deferredActions)
                        action();
                },
                populate: x =>
                {
                    var parameterizedType = Parameterize(x);

                    x.Operators.Add(
                        CreateOperator(parameterizedType, OperatorType.Addition, parameterizedType,
                            CreateParameter(ast.Name, parameterizedType, "left"),
                            CreateParameter(ast.Name, parameterizedType, "right")));
                    x.Operators.Add(
                        CreateOperator(parameterizedType, OperatorType.Subtraction, parameterizedType,
                            CreateParameter(ast.Name, parameterizedType, "left"),
                            CreateParameter(ast.Name, parameterizedType, "right")));
                });
        }
    }
}
