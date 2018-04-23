using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API
{
    public interface IILFactory
    {
        IEssentials Essentials { get; }

        SourcePackage TryGetPackage(string packageName);

        DataType GetType(string type);
        DataType GetType(Source src, string type);

        IEntity GetEntity(string expression, params Namescope[] scopes);
        IEntity GetEntity(Source src, string expression, params Namescope[] scopes);

        Expression GetExpression(string expression, DataType expectedType = null);
        Expression GetExpression(Source src, string expression, DataType expectedType = null);
        Expression GetExpression(Source src, string expression, string expectedType);

        Expression NewObject(string type, params Expression[] args);
        Expression NewObject(DataType type, params Expression[] args);
        Expression NewObject(Source src, string type, params Expression[] args);
        Expression NewObject(Source src, DataType type, params Expression[] args);

        Expression CallMethod(string type, string method, params Expression[] args);
        Expression CallMethod(DataType type, string method, params Expression[] args);
        Expression CallMethod(Expression obj, string method, params Expression[] args);
        Expression CallMethod(Source src, string type, string method, params Expression[] args);
        Expression CallMethod(Source src, DataType type, string method, params Expression[] args);
        Expression CallMethod(Source src, Expression obj, string method, params Expression[] args);

        Expression CallOperator(string type, string op, params Expression[] args);
        Expression CallOperator(DataType type, string op, params Expression[] args);
        Expression CallOperator(Source src, string type, string op, params Expression[] args);
        Expression CallOperator(Source src, DataType type, string op, params Expression[] args);

        Expression GetProperty(string type, string property);
        Expression GetProperty(DataType type, string property);
        Expression GetProperty(Expression obj, string property);
        Expression GetProperty(Source src, string type, string property);
        Expression GetProperty(Source src, DataType type, string property);
        Expression GetProperty(Source src, Expression obj, string property);

        Expression SetProperty(string type, string property, Expression value);
        Expression SetProperty(DataType type, string property, Expression value);
        Expression SetProperty(Expression obj, string property, Expression value);
        Expression SetProperty(Source src, string type, string property, Expression value);
        Expression SetProperty(Source src, DataType type, string property, Expression value);
        Expression SetProperty(Source src, Expression obj, string property, Expression value);

        DataType Parameterize(Source src, DataType definition, params DataType[] args);
        Method Parameterize(Source src, Method definition, params DataType[] args);
    }
}
