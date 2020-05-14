using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Compiler.Core.Syntax
{
    public class AstILNode
    {
        public readonly Namespace IL;
        public readonly AstNamespace Ast;
        public readonly List<AstILNode> Nodes = new List<AstILNode>();

        public AstILNode(Namespace il, AstNamespace ast)
        {
            IL = il;
            Ast = ast;
        }

        public override string ToString()
        {
            return Ast.Name.Source.ToString();
        }
    }
}