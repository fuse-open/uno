using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST.Expressions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.Syntax.Compilers;

namespace Uno.Compiler.Core.Syntax.Builders
{
    public partial class TypeBuilder
    {
        void CompileFieldInitializers(DataType result, DataType parameterizedType, List<AstExpression> fieldInitializers)
        {
            if (_env.Options.CodeCompletionMode)
                return;

            parameterizedType.PopulateMembers();

            // Compile in reverse order because generated statements are inserted at the front in the constructors
            for (int i = fieldInitializers.Count - 1; i >= 0; i--)
            {
                // error
                if (i >= parameterizedType.Fields.Count)
                {
                    InsertConstructorStatement(result, Expression.Invalid);
                    continue;
                }

                var field = parameterizedType.Fields[i];

                if (field.ReturnType is FixedArrayType)
                {
                    if (fieldInitializers[i] != null)
                        Log.Error(fieldInitializers[i].Source, ErrorCode.E0000, "'fixed' array fields cannot be initialized using field initializer");

                    continue;
                }

                if (fieldInitializers[i] == null)
                    continue;

                var value = _compiler.CompileExpression(fieldInitializers[i], parameterizedType, field.ReturnType);
                var constValue = (value as Constant)?.Value;

                // Ignore default values
                if (value is Default ||
                    value is Constant && (
                        constValue == null ||
                        constValue.Equals(0) ||
                        constValue.Equals(0.0) ||
                        constValue.Equals(0.0f) ||
                        constValue.Equals(false) ||
                        constValue is EnumType && 
                            constValue.Equals(Enum.ToObject(constValue.GetType(), 0))))
                    continue;

                if (field.IsStatic)
                    InsertTypeInitializerStatement(result, new StoreField(field.Source, null, field, value));
                else if (parameterizedType.IsStruct)
                    Log.Error(fieldInitializers[i].Source, ErrorCode.E3008, "Only classes can contain field initializers");
                else
                    InsertConstructorStatement(result, new StoreField(field.Source, new This(field.Source, parameterizedType), field, value));
            }
        }

        void InsertConstructorStatement(DataType dt, Statement statement)
        {
            foreach (var ctor in dt.Constructors)
            {
                if (ctor.Body == null)
                    ctor.SetBody(new Scope(ctor.Source));

                ctor.Body.Statements.Insert(0, statement);
            }
        }

        void InsertTypeInitializerStatement(DataType dt, Statement statement)
        {
            if (dt.Initializer == null)
                dt.Initializer = new Constructor(dt.Source, dt, null, Modifiers.Static | Modifiers.Generated, new Parameter[0]);

            if (dt.Initializer.Body == null)
                dt.Initializer.SetBody(new Scope(dt.Initializer.Source));

            dt.Initializer.Body.Statements.Insert(0, statement);
        }
    }
}
