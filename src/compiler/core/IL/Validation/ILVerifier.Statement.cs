using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Validation
{
    public partial class ILVerifier
    {
        public override void Next(Statement e)
        {
            _current = null;
        }

        public override void Begin(ref Statement e)
        {
            _current = null;

            switch (e.StatementType)
            {
                case StatementType.Expression:
                {
                    var s = e as Expression;
                    VerifyExpressionStatement(s);
                    break;
                }
                case StatementType.VariableDeclaration:
                {
                    var s = e as VariableDeclaration;
                    VerifyVariableType(s.Source, s.Variable.ValueType);

                    if (s.Variable.ValueType is FixedArrayType)
                        Log.Error(s.Source, ErrorCode.I0000, "Variable declarations cannot be of 'fixed' array type");

                    if (s.Variable.IsConstant)
                    {
                        for (var var = s.Variable; var != null; var = var.Next)
                        {
                            if (var.OptionalValue == null)
                                Log.Error(s.Source, ErrorCode.E0000, "A const variable requires a value to be provided");
                            else if (var.OptionalValue.ExpressionType != ExpressionType.Constant)
                                Log.Error(var.Source, ErrorCode.E0000, "The expression assigned to " + var.Name.Quote() + " must be constant");                            
                        }
                    }

                    break;
                }
                case StatementType.FixedArrayDeclaration:
                {
                    var s = e as FixedArrayDeclaration;
                    VerifyVariableType(s.Source, s.Variable.ValueType);

                    var fat = s.Variable.ValueType as FixedArrayType;

                    if (fat == null)
                        Log.Error(s.Source, ErrorCode.I0000, "Invalid 'fixed' array declaration");

                    if (s.Variable.OptionalValue != null)
                        Log.Error(s.Source, ErrorCode.E0000, "Invalid init value found on 'fixed' array variable");

                    break;
                }
                case StatementType.Throw:
                {
                    var s = e as Throw;

                    if (!s.Exception.ReturnType.IsSubclassOfOrEqual(Essentials.Exception) && s.Exception.ReturnType != DataType.Invalid)
                        Log.Error(s.Source, ErrorCode.E4044, "Thrown object must be of type Uno.Exception or class derived from it.");

                    break;
                }
                case StatementType.TryCatchFinally:
                {
                    var s = e as TryCatchFinally;

                    foreach (var c in s.CatchBlocks)
                    {
                        if (!c.Exception.ValueType.IsSubclassOfOrEqual(Essentials.Exception) && c.Exception.ValueType != DataType.Invalid)
                            Log.Error(c.Exception.Source, ErrorCode.E4045, "Caught object must be of type Uno.Exception or a class derived from it.");

                        if (c.Exception.OptionalValue != null)
                            Log.Error(s.Source, ErrorCode.I0000, "Invalid init value found on catch block exception variable");
                    }

                    s.OptionalFinallyBody?.Visit(_finallyVerifier);
                    break;
                }
                case StatementType.For:
                {
                    var s = e as For;

                    if (s.OptionalIncrement != null)
                        VerifyExpressionStatement(s.OptionalIncrement);

                    break;
                }
                case StatementType.Switch:
                {
                    var s = e as Switch;

                    for (int i = 0; i < s.Cases.Length; i++)
                    {
                        var c = s.Cases[i];

                        for (int j = 0; j < c.Values.Length; j++)
                        {
                            var v = c.Values[j];

                            for (int k = j - 1; k >= 0; k--)
                                VerifySwitchCaseValue(v, c.Values[k]);

                            for (int k = i - 1; k >= 0; k--)
                            {
                                var c2 = s.Cases[k];

                                for (int l = c2.Values.Length - 1; l >= 0; l--)
                                    VerifySwitchCaseValue(v, c2.Values[l]);

                                if (c.HasDefault && c2.HasDefault)
                                    Log.Error(c.Scope.Source, ErrorCode.E0000, "Case value 'default' is already used at line " + c2.Scope.Source.Line); // TODO: Bad source
                            }
                        }
                    }

                    break;
                }
                case StatementType.Return:
                {
                    var s = e as Return;

                    if (s.Value != null && s.Value.ReturnType.IsFixedArray)
                        Log.Error(s.Source, ErrorCode.E0000, "Cannot return a 'fixed' array");

                    if (MetaProperty != null)
                    {
                        if (MetaProperty.ReturnType != s.Value.ReturnType)
                            Log.Error(s.Source, ErrorCode.I0000, "Returning incompatible type " + s.Value.ReturnType.Quote());
                    }
                    else if (_lambda != null)
                    {
                        var returnType = _lambda.DelegateType.ReturnType;
                        if (s.Value == null)
                        {
                            if (!returnType.IsVoid)
                                Log.Error(s.Source, ErrorCode.E0000, "Non-void lambda must return a value");
                        }
                        else
                        {
                            if (returnType.IsVoid)
                                Log.Error(s.Source, ErrorCode.E0000, "Void lambda cannot return a value");
                            else if (returnType != s.Value.ReturnType)
                                Log.Error(s.Source, ErrorCode.I0000, "Returning incompatible type " + s.Value.ReturnType.Quote());
                        }
                    }
                    else if (Function != null)
                    {
                        if (s.Value == null)
                        {
                            if (!Function.ReturnType.IsVoid)
                                Log.Error(s.Source, ErrorCode.E0000, "Non-void function must return a value");
                        }
                        else
                        {
                            if (Function.ReturnType.IsVoid)
                                Log.Error(s.Source, ErrorCode.E0000, "Void function cannot return a value");
                            else if (Function.ReturnType != s.Value.ReturnType)
                                Log.Error(s.Source, ErrorCode.I0000, "Returning incompatible type " + s.Value.ReturnType.Quote());
                        }
                    }

                    break;
                }
                case StatementType.ExternScope:
                {
                    VerifyExtern(e);
                    break;
                }
            }
        }

        void VerifyExtern(Statement e)
        {
            if (Function is ShaderFunction)
                return;
            if (Backend.Has(FunctionOptions.Bytecode) && !Function.IsPInvokable(Essentials, Log))
                Log.Error(e.Source, ErrorCode.E0000, "This backend does not support 'extern' code");
        }

        void VerifySwitchCaseValue(Constant a, Constant b)
        {
            if (Equals(a.Value, b.Value))
                Log.Error(a.Source, ErrorCode.E0000, "Case value " + a.Value.Quote() + " is already used at line " + b.Source.Line);
        }

        void VerifyExpressionStatement(Expression s)
        {
            if (Environment.IsGeneratingCode)
                return;

            switch (s.ExpressionType)
            {
                case ExpressionType.StoreField:
                case ExpressionType.StoreLocal:
                case ExpressionType.StoreElement:
                case ExpressionType.StoreArgument:
                case ExpressionType.StoreThis:
                case ExpressionType.SetProperty:
                case ExpressionType.CallMethod:
                case ExpressionType.CallConstructor:
                case ExpressionType.CallShader:
                case ExpressionType.AddListener:
                case ExpressionType.RemoveListener:
                case ExpressionType.CallDelegate:
                case ExpressionType.NewObject:
                case ExpressionType.NewArray:
                case ExpressionType.NewDelegate:
                case ExpressionType.FixOp:
                case ExpressionType.ExternOp:
                case ExpressionType.NoOp:
                    return;

                case ExpressionType.SequenceOp:
                {
                    var e = s as SequenceOp;
                    VerifyExpressionStatement(e.Left);
                    VerifyExpressionStatement(e.Right);
                    return;
                }
            }

            Log.Error(s.Source, ErrorCode.E4046, "Statement has no effect. Only assign, increment/decrement, method call or new object expressions can be used as statements. Expression was of type <" + s.ExpressionType + ">");
        }
    }
}
