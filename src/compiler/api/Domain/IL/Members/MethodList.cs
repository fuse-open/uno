using System.Collections.Generic;

namespace Uno.Compiler.API.Domain.IL.Members
{
    public static class MethodList
    {
        public static Method[] Copy(this IReadOnlyList<Method> candidates, CopyState state)
        {
            var result = new Method[candidates.Count];
            for (int i = 0; i < candidates.Count; i++)
                result[i] = state.GetMember(candidates[i]);
            return result;
        }
    }
}