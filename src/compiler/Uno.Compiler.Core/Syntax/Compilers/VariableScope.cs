using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Core.Syntax.Compilers
{
    public class VariableScope
    {
        public readonly Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();
        public readonly List<VariableScope> Scopes = new List<VariableScope>();
    }
}