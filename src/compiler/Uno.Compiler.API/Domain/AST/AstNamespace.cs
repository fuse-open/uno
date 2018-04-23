using System.Collections.Generic;
using Uno.Compiler.API.Domain.AST.Expressions;

namespace Uno.Compiler.API.Domain.AST
{
    public class AstNamespace
    {
        public readonly AstIdentifier Name;
        public readonly List<AstNamespace> Namespaces = new List<AstNamespace>();
        public readonly List<AstBlockBase> Blocks = new List<AstBlockBase>();
        public readonly List<AstUsingDirective> Usings = new List<AstUsingDirective>();

        public AstNamespace(AstIdentifier name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.Symbol;
        }
    }
}
