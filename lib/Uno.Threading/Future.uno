using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    public abstract class Future : IDisposable
    {
        public FutureState State { get; protected set; }
        public abstract void Wait();
        public abstract void Cancel(bool shutdownGracefully = false);
        public abstract void Dispose();
    }

    public abstract class Future<T> : Future
    {
        protected T Result;
        protected Exception Reason;

        readonly IDispatcher _dispatcher;
        readonly Mutex _mutex = new Mutex();
        readonly Queue<Action<Exception>> _catchables;
        readonly Queue<Action<T>> _thenables;
        readonly Queue<Promise<T>> _chainables;

        protected Future(IDispatcher dispatcher)
        {
            if(dispatcher == null)
                throw new ArgumentNullException("dispatcher");

            _dispatcher = dispatcher;
            State = FutureState.Pending;
            _catchables = new Queue<Action<Exception>>();
            _chainables = new Queue<Promise<T>>();
            _thenables = new Queue<Action<T>>();
        }

        protected Future() : this(new SyncDispatcher()) { }

        bool _isDisposed;
        public override void Dispose()
        {
            if (!_isDisposed)
            {
                _mutex.Dispose();
                _isDisposed = true;
            }
        }

        protected void InternalResolve(T result)
        {
            _mutex.WaitOne();
            if (State != FutureState.Pending)
            {
                _mutex.ReleaseMutex();
                throw new Exception("This promise is already resolved or rejected");
            }

            State = FutureState.Resolved;
            Result = result;

            try
            {
                while (_thenables.Count != 0)
                    Invoke(_thenables.Dequeue(), result);

                while(_chainables.Count != 0)
                    _chainables.Dequeue().Resolve(result);
            }
            catch (Exception exception)
            {
                while (_chainables.Count != 0)
                    _chainables.Dequeue().Reject(exception);
            }

            _mutex.ReleaseMutex();
        }

        protected void InternalReject(Exception reason)
        {
            _mutex.WaitOne();
            if (State != FutureState.Pending)
            {
                _mutex.ReleaseMutex();
                throw new Exception("This promise is already resolved or rejected");
            }

            State = FutureState.Rejected;
            Reason = reason;

            try
            {
                while (_catchables.Count != 0)
                    Invoke(_catchables.Dequeue(), reason);

                while (_chainables.Count != 0)
                    _chainables.Dequeue().Reject(reason);
            }
            catch (Exception exception)
            {
                while (_chainables.Count != 0)
                    _chainables.Dequeue().Reject(exception);
            }

            _mutex.ReleaseMutex();
        }

        void Invoke<T1>(Action<T1> action, T1 arg)
        {
            _dispatcher.Invoke(new Closure<T1>(action, arg).Run);
        }

        public Future<T> Then(Action<T> action)
        {
            return Then(action, null);
        }

        public Future<T> Then(Action<T> fulfilled, Action<Exception> rejected)
        {
            var chainable = new Promise<T>();
            _mutex.WaitOne();
            try
            {
                if (State == FutureState.Resolved)
                {
                    if (fulfilled != null)
                        Invoke(fulfilled, Result);
                }
                else if (State == FutureState.Rejected)
                {
                    if (rejected != null)
                        Invoke(rejected, Reason);
                }
                else
                {
                    _chainables.Enqueue(chainable);

                    if (fulfilled != null)
                        _thenables.Enqueue(fulfilled);

                    if (rejected != null)
                        _catchables.Enqueue(rejected);
                }
            }
            catch (Exception exception)
            {
                chainable.Reject(exception);
            }
            _mutex.ReleaseMutex();
            return chainable;
        }

        public Future<T> Catch(Action<Exception> failure)
        {
            return Then(null, failure);
        }

        class Closure<T>
        {
            readonly Action<T> _action;
            readonly T _result;

            public Closure(Action<T> action, T result)
            {
                _action = action;
                _result = result;
            }

            public void Run() { _action(_result); }
        }
    }
}
