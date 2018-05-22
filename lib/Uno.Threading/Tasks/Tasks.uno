using Uno;
using Uno.Collections;
using Uno.Platform;

namespace Uno.Threading//.Tasks
{

    internal class CancellationTokenSource
    {
        public bool IsCancellationRequested
        {
            get { return _token.IsCancellationRequested; }
        }

        public CancellationToken Token
        {
            get { return _token; }
        }

        readonly CancellationToken _token;

        public CancellationTokenSource()
        {
            _token = new CancellationToken();
        }

        public void Cancel()
        {
            _token.SetCancellationRequested();
        }
    }

    internal class CancellationToken
    {
        public bool IsCancellationRequested
        {
            get; private set;
        }

        public CancellationToken()
        {
            IsCancellationRequested = false;
        }

        internal void SetCancellationRequested()
        {
            IsCancellationRequested = true;
        }
    }

    internal enum TaskStatus
    {
        Created,
        Faulted,
        RanToCompletion,
        Running,
        //WaitingToRun,
    }

    internal delegate void TaskDelegate(CancellationToken cancellationToken);

    internal delegate TResult TaskDelegate<TResult>(CancellationToken cancellationToken);

    internal class Task : IDisposable
    {

        public bool IsCompleted
        {
            get { return Status == TaskStatus.RanToCompletion; }
        }

        public bool IsFaulted
        {
            get { return Status == TaskStatus.Faulted; }
        }

        public TaskStatus Status
        {
            get { return _taskStatus; }
            protected set { _taskStatus = value; }
        }

        public AggregateException Exception
        {
            get { return _exception; }
            protected set { _exception = value; }
        }

        public CancellationTokenSource CancellationTokenSource
        {
            get { return _cancellationTokenSource; }
        }

        AggregateException _exception = null;
        TaskStatus _taskStatus = TaskStatus.Created;

        readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        readonly ManualResetEvent _manualResetEvent =
            new ManualResetEvent(false);

        readonly TaskDelegate _taskDelegate;

        public Task(TaskDelegate taskDelegate)
        {
            _taskDelegate = taskDelegate;
        }

        public void Wait()
        {
            _manualResetEvent.WaitOne();
        }

        public bool Wait(int timeoutMillis)
        {
            return _manualResetEvent.WaitOne(timeoutMillis);
        }

        public void Dispose()
        {
            _manualResetEvent.Dispose();
        }

        internal void Execute()
        {
            try
            {
                Status = TaskStatus.Running;
                InvokeTaskDelegate();
                Status = TaskStatus.RanToCompletion;
            }
            catch (Exception e)
            {
                Status = TaskStatus.Faulted;
                Exception = new AggregateException(new Exception[] { e });
            }
            finally
            {
                _manualResetEvent.Set();
            }
        }

        protected virtual void InvokeTaskDelegate()
        {
            if (_taskDelegate != null)
                _taskDelegate(CancellationTokenSource.Token);
        }

        public static Task Run(TaskDelegate taskDelegate, ITaskScheduler scheduler)
        {
            var task = new Task(taskDelegate);
            scheduler.ScheduleTask(task);
            return task;
        }

        public static Task<TResult> Run<TResult>(TaskDelegate<TResult> taskDelegate, ITaskScheduler scheduler)
        {
            var task = new Task<TResult>(taskDelegate);
            scheduler.ScheduleTask(task);
            return task;
        }

        public static Task Run(TaskDelegate taskDelegate)
        {
            return Run(taskDelegate, ThreadPoolTaskScheduler.Default);
        }

        public static Task<TResult> Run<TResult>(TaskDelegate<TResult> taskDelegate)
        {
            return Run<TResult>(taskDelegate, ThreadPoolTaskScheduler.Default);
        }

    }

    internal class Task<TResult> : Task
    {

        public TResult Result
        {
            get
            {
                Wait();
                return _result;
            }
        }

        TResult _result = default(TResult);

        readonly TaskDelegate<TResult> _taskDelegate;

        public Task(TaskDelegate<TResult> taskDelegate) : base(null)
        {
            _taskDelegate = taskDelegate;
        }

        protected override void InvokeTaskDelegate()
        {
            if (_taskDelegate != null)
                _result = _taskDelegate(CancellationTokenSource.Token);
        }

    }

    internal interface ITaskScheduler : IDisposable
    {
        void ScheduleTask(Task task);
    }

    internal class ThreadPoolTaskScheduler : ITaskScheduler
    {

        static ThreadPoolTaskScheduler _default;
        public static ThreadPoolTaskScheduler Default
        {
            get {
                if (_default == null)
                {
                    _default = new ThreadPoolTaskScheduler();
                    Uno.Platform.CoreApp.Terminating += OnAppTerminating;
                }

                return _default;
            }
        }

        static void OnAppTerminating(ApplicationState newState)
        {
            _default.Dispose();
            _default = null;
        }

        readonly ThreadPool _threadPool;

        public ThreadPoolTaskScheduler()
        {
            _threadPool = new ThreadPool(/* Number of logical cores */);
        }

        public void ScheduleTask(Task task)
        {
            _threadPool.QueueAction(task.Execute);
        }

        public void Dispose()
        {
            _threadPool.Dispose();
        }

    }

}
