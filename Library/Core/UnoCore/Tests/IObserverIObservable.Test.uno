using Uno;
using Uno.Testing;

namespace Uno.Test
{

    class IntObserver : IObserver<int>
    {
        public bool Completed = false;
        public Exception Error = null;
        public int Value = 0;

        public void OnCompleted()
        {
            Completed = true;
        }

        public void OnError(Exception error)
        {
            Error = error;
        }

        public void OnNext(int value)
        {
            Value = value;
        }
    }

    class IntObservable : IObservable<int>
    {
        IObserver<int> obs;

        public IDisposable Subscribe(IObserver<int> observer)
        {
            obs = observer;
            return new Disposable();
        }

        public void SetValue(int i)
        {
            obs.OnNext(i);
        }

        public void SetError()
        {
            obs.OnError(new Exception());
        }

        public void SetCompleted()
        {
            obs.OnCompleted();
        }

        private class Disposable : IDisposable
        {
            public void Dispose(){}
        }
    }

    // Provided as an example of how to use those interfaces
    public class IObserverIObservableTest
    {
        IntObserver observer = new IntObserver();
        IntObservable observable = new IntObservable();

        [Test]
        public void OnNext()
        {
            observable.Subscribe(observer);
            Assert.AreEqual(observer.Value, 0);
            observable.SetValue(42);
            Assert.AreEqual(observer.Value, 42);
        }

        [Test]
        public void OnError()
        {
            observable.Subscribe(observer);
            Assert.IsTrue(observer.Error == null);
            observable.SetError();
            Assert.IsFalse(observer.Error == null);
        }

        [Test]
        public void OnCompleted()
        {
            observable.Subscribe(observer);
            Assert.IsFalse(observer.Completed);
            observable.SetCompleted();
            Assert.IsTrue(observer.Completed);
        }
    }

}
