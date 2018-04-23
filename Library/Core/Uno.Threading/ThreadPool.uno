using Uno;
using Uno.Collections;

namespace Uno.Threading
{

    public class ThreadPool : IDisposable
    {

        abstract class WorkItem
        {
            public abstract void Invoke();
        }

        class DefaultWorkItem : WorkItem
        {
            readonly Action _action;

            public DefaultWorkItem(Action action)
            {
                _action = action;
            }

            public override void Invoke()
            {
                if (_action != null)
                    _action();
            }
        }

        class ParameterizedWorkItem<TState> : WorkItem
        {
            readonly Action<TState> _action;
            readonly TState _state;

            public ParameterizedWorkItem(Action<TState> action, TState state)
            {
                _action = action;
                _state = state;
            }

            public override void Invoke()
            {
                if (_action != null)
                    _action(_state);
            }
        }

        readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);
        readonly ConcurrentQueue<WorkItem> _taskQueue = new ConcurrentQueue<WorkItem>();
        readonly int _poolSize;

        bool _running = true;

        // TODO(Vegard): make the default poolSize equal to the number of logical cores
        // Since we are going to run this code on a lot of systems we cannot test
        // this seems like the best way to avoid preemption
        public ThreadPool(int poolSize = 4)
        {
            _poolSize = poolSize;
            for (var i = 0; i < _poolSize; i++)
            {
                var t = new Thread(WorkerEntrypoint);
                if defined(CIL)
                {
                    // Need this for debug purposes in Visual Studio
                    t.Name = "ThreadPoolWorker #" + i;
                }
                t.Start();
            }
        }

        void WorkerEntrypoint()
        {
            while (_running)
            {
                if defined(CPLUSPLUS)
                    extern "uAutoReleasePool ____pool";

                if (DoTask()) continue;
                _resetEvent.WaitOne();
            }
            _disposeQueue.Enqueue(Thread.CurrentThread);
        }

        bool DoTask()
        {
            WorkItem workItem;
            if (_taskQueue.TryDequeue(out workItem))
            {
                workItem.Invoke();
                return true;
            }
            return false;
        }

        public void QueueAction(Action action)
        {
            _taskQueue.Enqueue(new DefaultWorkItem(action));
            _resetEvent.Set();
        }

        public void QueueAction<TState>(Action<TState> action, TState state)
        {
            _taskQueue.Enqueue(new ParameterizedWorkItem<TState>(action, state));
            _resetEvent.Set();
        }

        bool _isDisposed = false;

        /// Dispose() is kinda icky icky, not to excited about it
        readonly ConcurrentQueue<Thread> _disposeQueue = new ConcurrentQueue<Thread>();
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            while (!_taskQueue.IsEmpty)
                Thread.Sleep(1);

            _running = false;

            var disposeCount = 0;

            while (disposeCount != _poolSize)
            {
                _resetEvent.Set();
                Thread thread;
                if (_disposeQueue.TryDequeue(out thread))
                {
                    thread.Join();
                    disposeCount++;
                }
            }

            _resetEvent.Dispose();
        }

    }

}
