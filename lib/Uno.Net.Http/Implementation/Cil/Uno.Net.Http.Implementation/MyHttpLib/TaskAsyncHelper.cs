using System;
using System.Threading.Tasks;

namespace MyHttpLib
{
    // http://blogs.msdn.com/b/pfxteam/archive/2010/11/21/10094564.aspx

    internal static class TaskAsyncHelper
    {
        static readonly Task _emptyTask = MakeEmpty();

        static Task MakeEmpty()
        {
            return FromResult<object>(null);
        }

        public static Task Empty
        {
            get { return _emptyTask; }
        }

        // Then extesions
        public static Task Then(this Task task, Action successor)
        {
            switch (task.Status)
            {
                case TaskStatus.Faulted:
                    return FromError<object>(task.Exception);

                case TaskStatus.Canceled:
                    return Canceled<object>();

                case TaskStatus.RanToCompletion:
                    return FromMethod(successor);

                default:
                    return RunTask(task, successor);
            }
        }


        public static Task<TResult> Then<TResult>(this Task task, Func<Task<TResult>> successor)
        {
            switch (task.Status)
            {
                case TaskStatus.Faulted:
                    return FromError<TResult>(task.Exception);

                case TaskStatus.Canceled:
                    return Canceled<TResult>();

                case TaskStatus.RanToCompletion:
                    return FromMethod(successor).FastUnwrap();

                default:
                    return TaskRunners<object, Task<TResult>>.RunTask(task, successor)
                                                             .FastUnwrap();
            }
        }

        public static Task Then<TResult>(this Task<TResult> task, Action<TResult> successor)
        {
            switch (task.Status)
            {
                case TaskStatus.Faulted:
                    return FromError<object>(task.Exception);

                case TaskStatus.Canceled:
                    return Canceled<object>();

                case TaskStatus.RanToCompletion:
                    return FromMethod(successor, task.Result);

                default:
                    return TaskRunners<TResult, object>.RunTask(task, successor);
            }
        }

        public static Task Then<TResult>(this Task<TResult> task, Func<TResult, Task> successor)
        {
            switch (task.Status)
            {
                case TaskStatus.Faulted:
                    return FromError<object>(task.Exception);

                case TaskStatus.Canceled:
                    return Canceled<object>();

                case TaskStatus.RanToCompletion:
                    return FromMethod(successor, task.Result).FastUnwrap();

                default:
                    return TaskRunners<TResult, Task>.RunTask(task, t => successor(t.Result))
                                                     .FastUnwrap();
            }
        }


        static Task FastUnwrap(this Task<Task> task)
        {
            var innerTask = (task.Status == TaskStatus.RanToCompletion) ? task.Result : null;
            return innerTask ?? task.Unwrap();
        }

        static Task<T> FastUnwrap<T>(this Task<Task<T>> task)
        {
            var innerTask = (task.Status == TaskStatus.RanToCompletion) ? task.Result : null;
            return innerTask ?? task.Unwrap();
        }

        public static Task FromMethod(Action func)
        {
            try
            {
                func();
                return Empty;
            }
            catch (Exception ex)
            {
                return FromError<object>(ex);
            }
        }

        public static Task FromMethod<T1>(Action<T1> func, T1 arg)
        {
            try
            {
                func(arg);
                return Empty;
            }
            catch (Exception ex)
            {
                return FromError<object>(ex);
            }
        }

        public static Task<TResult> FromMethod<TResult>(Func<TResult> func)
        {
            try
            {
                return FromResult(func());
            }
            catch (Exception ex)
            {
                return FromError<TResult>(ex);
            }
        }

        public static Task<TResult> FromMethod<T1, TResult>(Func<T1, TResult> func, T1 arg)
        {
            try
            {
                return FromResult(func(arg));
            }
            catch (Exception ex)
            {
                return FromError<TResult>(ex);
            }
        }


        public static Task<T> FromResult<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }

        internal static Task<T> FromError<T>(Exception e)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(e);
            return tcs.Task;
        }

        static Task<T> Canceled<T>()
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetCanceled();
            return tcs.Task;
        }


        static Task RunTask(Task task, Action successor)
        {
            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.SetException(t.Exception);
                else if (t.IsCanceled)
                    tcs.SetCanceled();
                else
                {
                    try
                    {
                        successor();
                        tcs.SetResult(null);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }
            }, HttpHelper._scheduler);

            return tcs.Task;
        }

        static class TaskRunners<T, TResult>
        {
            internal static Task RunTask(Task<T> task, Action<T> successor)
            {
                var tcs = new TaskCompletionSource<object>();
                task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        tcs.SetException(t.Exception);
                    else if (t.IsCanceled)
                        tcs.SetCanceled();
                    else
                    {
                        try
                        {
                            successor(t.Result);
                            tcs.SetResult(null);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }
                }, HttpHelper._scheduler);

                return tcs.Task;
            }

            internal static Task<TResult> RunTask(Task task, Func<TResult> successor)
            {
                var tcs = new TaskCompletionSource<TResult>();
                task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        tcs.SetException(t.Exception);
                    else if (t.IsCanceled)
                        tcs.SetCanceled();
                    else
                    {
                        try
                        {
                            tcs.SetResult(successor());
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }
                }, HttpHelper._scheduler);

                return tcs.Task;
            }

            internal static Task<TResult> RunTask(Task<T> task, Func<Task<T>, TResult> successor)
            {
                var tcs = new TaskCompletionSource<TResult>();
                task.ContinueWith(t =>
                {
                    if (task.IsFaulted)
                        tcs.SetException(t.Exception);
                    else if (task.IsCanceled)
                        tcs.SetCanceled();
                    else
                    {
                        try
                        {
                            tcs.SetResult(successor(t));
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }
                }, HttpHelper._scheduler);

                return tcs.Task;
            }
        }
    }
}