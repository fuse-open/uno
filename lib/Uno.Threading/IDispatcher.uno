using Uno;

namespace Uno.Threading
{
    public interface IDispatcher
    {
        void Invoke(Action action);
    }

    public static class IDispatcherExtensions
    {
        public static void Invoke1<T>(this IDispatcher dispatcher, Action<T> action, T arg)
        {
            dispatcher.Invoke(new Arg1Invoke<T>(action, arg).Run);
        }

        public static void Invoke2<T1,T2>(this IDispatcher dispatcher, Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            dispatcher.Invoke(new Arg2Invoke<T1,T2>(action, arg1, arg2).Run);
        }

        public static void Invoke3<T1,T2,T3>(this IDispatcher dispatcher, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            dispatcher.Invoke(new Arg3Invoke<T1,T2,T3>(action, arg1, arg2, arg3).Run);
        }

        class Arg1Invoke<T>
        {
            Action<T> _action;
            T _arg;
            public Arg1Invoke(Action<T> action, T arg)
            {
                _action = action;
                _arg = arg;
            }
            public void Run() { _action(_arg); }
        }

        class Arg2Invoke<T1, T2>
        {
            Action<T1, T2> _action;
            T1 _arg1;
            T2 _arg2;
            public Arg2Invoke(Action<T1, T2> action, T1 arg1, T2 arg2)
            {
                _action = action;
                _arg1 = arg1;
                _arg2 = arg2;
            }
            public void Run() { _action(_arg1, _arg2); }
        }

        class Arg3Invoke<T1, T2, T3>
        {
            Action<T1, T2, T3> _action;
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            public Arg3Invoke(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
            {
                _action = action;
                _arg1 = arg1;
                _arg2 = arg2;
                _arg3 = arg3;
            }
            public void Run() { _action(_arg1, _arg2, _arg3); }
        }

        public static void Invoke0<TResult>(this IDispatcher dispatcher, Func<TResult> func)
        {
            dispatcher.Invoke(new Arg0InvokeFunc<TResult>(func).Run);
        }

        public static void Invoke1<T,TResult>(this IDispatcher dispatcher, Func<T, TResult> func, T arg)
        {
            dispatcher.Invoke(new Arg1InvokeFunc<T,TResult>(func, arg).Run);
        }

        public static void Invoke2<T1,T2,TResult>(this IDispatcher dispatcher, Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
        {
            dispatcher.Invoke(new Arg2InvokeFunc<T1,T2,TResult>(func, arg1, arg2).Run);
        }

        public static void Invoke3<T1,T2,T3,TResult>(this IDispatcher dispatcher, Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
        {
            dispatcher.Invoke(new Arg3InvokeFunc<T1,T2,T3,TResult>(func, arg1, arg2, arg3).Run);
        }

        class Arg0InvokeFunc<TResult>
        {
            Func<TResult> _action;
            public Arg0InvokeFunc(Func<TResult> action)
            {
                _action = action;
            }
            public void Run() { _action(); }
        }

        class Arg1InvokeFunc<T, TResult>
        {
            Func<T, TResult> _action;
            T _arg;
            public Arg1InvokeFunc(Func<T, TResult> action, T arg)
            {
                _action = action;
                _arg = arg;
            }
            public void Run() { _action(_arg); }
        }

        class Arg2InvokeFunc<T1, T2, TResult>
        {
            Func<T1, T2, TResult> _action;
            T1 _arg1;
            T2 _arg2;
            public Arg2InvokeFunc(Func<T1, T2, TResult> action, T1 arg1, T2 arg2)
            {
                _action = action;
                _arg1 = arg1;
                _arg2 = arg2;
            }
            public void Run() { _action(_arg1, _arg2); }
        }

        class Arg3InvokeFunc<T1, T2, T3, TResult>
        {
            Func<T1, T2, T3, TResult> _action;
            T1 _arg1;
            T2 _arg2;
            T3 _arg3;
            public Arg3InvokeFunc(Func<T1, T2, T3, TResult> action, T1 arg1, T2 arg2, T3 arg3)
            {
                _action = action;
                _arg1 = arg1;
                _arg2 = arg2;
                _arg3 = arg3;
            }
            public void Run() { _action(_arg1, _arg2, _arg3); }
        }
    }
}
