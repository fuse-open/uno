using System;

namespace Uno.Compiler.API.Domain.IL
{
    static class ActionQueue
    {
        public static void Dequeue<T>(ref Action<T> action, T item)
        {
            if (action != null)
            {
                var a = action;
                action = null;
                a(item);
            }
        }
    }
}
