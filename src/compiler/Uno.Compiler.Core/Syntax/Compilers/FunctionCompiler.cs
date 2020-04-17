using System;
using System.Collections.Generic;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.AST.Statements;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Core.IL.Validation;
using Uno.Compiler.Core.Syntax.Binding;
using Uno.Compiler.Core.Syntax.Builders;
using Uno.Logging;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public sealed partial class FunctionCompiler : LogObject
    {
        static readonly Scope _dummyScope = new Scope();

        public readonly BuildEnvironment Environment;
        public readonly Essentials Essentials;
        public readonly ILFactory ILFactory;
        public readonly ILVerifier ILVerifier;
        public readonly TypeBuilder TypeBuilder;
        public readonly NameResolver NameResolver;
        public readonly Compiler Compiler;

        public readonly Function Function;

        // For nested lambdas
        public readonly Stack<Lambda> Lambdas = new Stack<Lambda>();

        public readonly AstScope Body;
        public readonly MetaProperty MetaProperty;
        public readonly Namescope Namescope;

        public readonly List<bool> CheckCastStack = new List<bool>();
        public readonly List<DataType> ImplicitCastStack = new List<DataType>();
        public readonly List<VariableScope> VariableScopeStack = new List<VariableScope>() { new VariableScope() };

        List<ReqStatement> _reqStatements;
        public List<ReqStatement> ReqStatements => _reqStatements ?? (_reqStatements = new List<ReqStatement>());
        public VariableScope CurrentVariableScope => VariableScopeStack[VariableScopeStack.Count - 1];
        public bool CheckCast => CheckCastStack.Count == 0 || CheckCastStack[CheckCastStack.Count - 1];
        public bool IsFunctionScope => Function != null && Namescope is DataType && Function.DeclaringType.MasterDefinition == (Namescope as DataType).MasterDefinition ||
                                       (Function as Method)?.GenericType == Namescope;

        public FunctionCompiler(Compiler compiler, Function func, DataType parameterizedParent, AstScope body)
            : this(compiler)
        {
            Function = func;
            Namescope = func.GenericType ?? parameterizedParent;
            Body = body;
        }

        public FunctionCompiler(Compiler compiler, DataType dt, Expression obj)
            : this(compiler)
        {
            Function = new Method(dt.Source, dt, null, obj == null ? Modifiers.Static : 0, ".member_init", DataType.Void, new Parameter[0]);
            Namescope = dt;
        }

        public FunctionCompiler(Compiler compiler, MetaProperty mp)
            : this(compiler)
        {
            MetaProperty = mp;
            Function = new Method(mp.Source, mp.Parent.TryFindTypeParent() ?? Essentials.Object, null, Modifiers.Static, ".meta_property", mp.ReturnType, new Parameter[0]);
            Namescope = mp.Parent;
        }

        public FunctionCompiler(Compiler compiler, Namescope namescope)
            : this(compiler)
        {
            Function = new Method(Source.Unknown, namescope as DataType ?? Essentials.Object, null, Modifiers.Static, ".member_init", DataType.Void, new Parameter[0]);
            Namescope = namescope;
        }

        FunctionCompiler(Compiler compiler)
            : base(compiler)
        {
            Environment = compiler.Environment;
            Essentials = compiler.Essentials;
            ILFactory = compiler.ILFactory;
            ILVerifier = compiler.ILVerifier;
            TypeBuilder = compiler.TypeBuilder;
            NameResolver = compiler.NameResolver;
            Compiler = compiler;
        }

        public void AddReqStatement(ReqStatement s)
        {
            if (MetaProperty == null)
            {
                Log.Error(s.Source, ErrorCode.E0030, "'req' is not allowed in this scope");
                return;
            }

            var reqStatements = ReqStatements;
            var creq = s as ReqProperty;
            var oreq = s as ReqObject;
            
            if (creq != null)
            {
                for (int i = 0; i < reqStatements.Count; i++)
                {
                    if (reqStatements[i] is ReqProperty)
                    {
                        var lreq = reqStatements[i] as ReqProperty;

                        if (lreq.PropertyName == creq.PropertyName)
                        {
                            var dt = creq.PropertyType ?? lreq.PropertyType;

                            if (creq.PropertyType != null && lreq.PropertyType != null && !creq.PropertyType.Equals(lreq.PropertyType))
                                Log.Error(creq.Source, ErrorCode.E0032, "Data type must be " + lreq.PropertyType.Quote() + ", same as the previous definition of " + lreq.PropertyName.Quote() + " declared at " + lreq.Source);

                            if (creq.Tag != null && lreq.Tag != null)
                                continue;

                            reqStatements[i] = new ReqProperty(creq.Source, creq.PropertyName, dt, Math.Max(creq.Offset, lreq.Offset), lreq.Tag);
                            return;
                        }
                    }
                }
            }
            else if (oreq != null)
            {
                foreach (var r in reqStatements)
                    if ((r as ReqObject)?.ObjectType.Equals(oreq.ObjectType) == true)
                        return;
            }

            reqStatements.Add(s);
        }

        public void Compile(bool force = false)
        {
            // Set _dummyScope so that function.HasBody is true in CanLink()
            if (Function.Body == null)
                Function.SetBody(_dummyScope);

            if (!force &&
                !Function.IsGenerated &&
                Compiler.Backend.CanLink(Function))
            {
                Function.Stats |= EntityStats.CanLink;
                Function.SetBody(null);
            }
            else
            {
                if (Function.Stats.HasFlag(EntityStats.IsCompiled))
                    return;

                Function.Stats |= EntityStats.IsCompiled;

                var result = CompileScope(Body);

                if (Function.Body.Statements.Count > 0)
                    result.Statements.InsertRange(0, Function.Body.Statements);

                Function.SetBody(result);
            }
        }

        public Namescope[] GetUsings(Source src)
        {
            return NameResolver.GetUsings(Namescope, src);
        }

        Expression Error(Source src, ErrorCode code, string msg)
        {
            Log.Error(src, code, msg);
            return Expression.Invalid;
        }

        PartialExpression PartialError(Source src, ErrorCode code, string msg)
        {
            Log.Error(src, code, msg);
            return PartialExpression.Invalid;
        }
    }
}
