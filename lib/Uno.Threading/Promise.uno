using Uno;
using Uno.Collections;
using Uno.Compiler.ExportTargetInterop;

namespace Uno.Threading
{
    /**
        Uno-side Promise based on the [A+ standard](https://promisesaplus.com/).

        This can be used in multiple ways:

        ## Statically

        You can use the `Run` function to wrap whatever argument-less function you want as a `Promise`, like this:

            bool doStuff()
            {
                //stuff is done
                success = doOtherStuff();
                return success;
            }

            void onSuccess(bool value) 
            {
                //Success!
            }

            void onFail(Exception e)
            {
                // Oh no!
            }
            
            public void DoSomeFancyStuff()
            {
                var promise = Promise.Run(doStuff).Then(onSuccess, onFail);
            }

        ## Make your own promises

        You can also extend `Promise` and handle it yourself. Simply call `Resolve` or `Reject` once you have a result or a failure. The [Native Facebook login example](https://github.com/fusetools/fuse-samples/blob/feature-NativeFacebookLogin/Samples/NativeFacebookLogin/FacebookLogin/FacebookLoginModule.uno) is a good example of this being done in practice.
    
        Notice that `Resolve` and `Reject` are public, so you can also theoretically both resolve and reject promises from elsewhere.

        # Making Promises accessible from JavaScript modules

        A Promise can be wrapped in a @(NativePromise) and fed to a @(NativeModule) through `AddMember`. You can read more about creating custom js modules, and how to populate them with functions and promises, [here](articles:native-interop/native-js-modules.md)
    */
    public class Promise<T> : Future<T>
    {
        public override void Wait()
        {
        }

        public override void Cancel(bool shutdownGracefully = false)
        {
        }

        public Promise(T result)
        {
            Resolve(result);
        }

        public Promise(IDispatcher dispatcher, T result) : base(dispatcher)
        {
            Resolve(result);
        }

        public Promise(IDispatcher dispatcher) : base(dispatcher) { }

        public Promise() { }

        public void Resolve(T result)
        {
           InternalResolve(result);
        }

        public void Reject(Exception reason)
        {
            InternalReject(reason);
        }

        public static Future<T> Run(IDispatcher dispatcher, Func<T> func)
        {
            return new TaskFuture<T>(dispatcher, func);
        }

        public static Future<T> Run(Func<T> func)
        {
            return new TaskFuture<T>(func);
        }
    }

    class TaskFuture<T> : Future<T>
    {

        readonly Func<T> _func;
        readonly Task _task;

        public TaskFuture(IDispatcher dispatcher, Func<T> func) : base(dispatcher)
        {
            _func = func;
            _task = Task.Run(Invoke);
        }

        public TaskFuture(Func<T> func) : base()
        {
            _func = func;
            _task = Task.Run(Invoke);
        }

        void Invoke(CancellationToken cancellationToken)
        {
            try
            {
                var result = default(T);
                if (_func != null)
                    result = _func();
                InternalResolve(result);
            }
            catch (Exception e)
            {
                InternalReject(e);
            }
        }

        public override void Wait()
        {
            _task.Wait();
        }

        public override void Cancel(bool shutdownGracefully = false)
        {
            _task.CancellationTokenSource.Cancel();
        }

        public override void Dispose()
        {
            base.Dispose();
            _task.Dispose();
        }

    }

}
