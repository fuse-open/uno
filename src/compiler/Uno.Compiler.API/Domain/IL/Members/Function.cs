using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.API.Domain.IL
{
    public abstract class Function : ParametersMember
    {
        public static readonly Method Null = new Method(Source.Unknown, DataType.Void, null, 
                Modifiers.Generated | Modifiers.Static, ".null", DataType.Void, ParameterList.Empty,
                new Scope(Source.Unknown, new NoOp(Source.Unknown)));

        Scope _optionalBody;

        protected Function(
            Source src,
            DataType owner,
            string name,
            DataType returnType,
            Parameter[] parameters,
            Scope optionalBody,
            string comment,
            Modifiers modifiers)
            : base(src, comment, modifiers, name, owner, returnType, parameters)
        {
            _optionalBody = optionalBody;
        }

        public bool HasBody => Body != null;
        public bool IsCast => MemberType == MemberType.Cast;
        public bool IsMethod => MemberType == MemberType.Method;
        public bool IsOperator => MemberType == MemberType.Operator;
        public bool IsConstructor => MemberType == MemberType.Constructor;
        public bool IsFinalizer => MemberType == MemberType.Finalizer;
        public bool IsGenericMethod => GenericType != null;
        public bool CanLink => Prototype.MasterDefinition.Stats.HasFlag(EntityStats.CanLink);

        public ClassType GenericType => IsMethod ? ((Method) this).GenericType : null;

        public string NameAndParameterList => (IsOperator
            ? OperatorString
            : UnoName + GenericSuffix
            ) + Parameters.BuildString() + (
                IsCast
                    ? "~" + ReturnType
                    : ""
                );

        public string NameAndSuffix => UnoName + GenericSuffix;

        public string OperatorString => IsOperator ? ((Operator) this).Symbol : "?";

        public string GenericSuffix => IsMethod ? DataType.GetGenericSuffix(((Method) this).GenericType) : "";

        public new Function Prototype => _prototype as Function ?? this;

        public new Function MasterDefinition => _master as Function ?? this;

        public Scope Body
        {
            get
            {
                var method = this as Method;
                return method != null && method.IsGenericParameterization
                        ? method.GenericDefinition.Body
                        : _optionalBody ?? ((Function) _master)?._optionalBody;
            }
        }

        public void SetBody(Scope body)
        {
            _optionalBody = body;
        }

        public void Visit(Pass p)
        {
            if (!p.Begin(this))
                return;

            var oldFunction = p.Function;
            p.Function = this;
            _optionalBody?.Visit(p);

            var shaderFunc = this as ShaderFunction;
            if (shaderFunc != null)
            {
                if (shaderFunc.Shader.Entrypoint == this)
                {
                    if (shaderFunc.Shader is VertexShader)
                        shaderFunc.Shader.State.VisitVaryings(shaderFunc.Shader.Entrypoint.Body, p);

                    shaderFunc.Shader.VisitTerminals(shaderFunc.Shader.Entrypoint.Body, p);
                }
            }

            var method = this as Method;
            if (method != null)
                foreach (var db in method.DrawBlocks)
                    db.Visit(p);

            p.Function = oldFunction;
            p.End(this);
        }

        public override string ToString()
        {
            return DeclaringType + "." + NameAndParameterList;
        }
    }
}
