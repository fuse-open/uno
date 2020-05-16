namespace Uno.Compiler.API.Domain.IL
{
    public interface IVisitor
    {
        bool Visit(Namespace ns);
        bool Visit(DataType dt);
        void Visit(Function f);
    }
}