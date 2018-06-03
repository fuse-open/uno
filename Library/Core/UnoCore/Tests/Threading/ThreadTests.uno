using Uno;
using Uno.Collections;
using Uno.Threading;
using Uno.Testing;

namespace ThreadingTests
{
    public class ThreadTests
    {
        Thread _mainThread;
        Thread _basicThread;
        bool _currentThreadCorrect;
        void BasicFunc()
        {
            _currentThreadCorrect = Thread.CurrentThread != _mainThread &&
                                    Thread.CurrentThread == _basicThread;
        }

        [Test]
        public void Basic()
        {
            Assert.AreNotEqual(null, Thread.CurrentThread);
            _mainThread = Thread.CurrentThread;

            _basicThread = new Thread(BasicFunc);

            Assert.IsFalse(_currentThreadCorrect);

            _basicThread.Start();
            _basicThread.Join();

            Assert.IsTrue(_currentThreadCorrect);
            Assert.AreEqual(_mainThread, Thread.CurrentThread);
        }

        void Dummy() {}

        void ConstructNull()
        {
            new Thread(null);
        }

        [Test]
        public void Ctor()
        {
            Assert.Throws<ArgumentNullException>(ConstructNull);
        }

        void StartTwice()
        {
            var thread = new Thread(Dummy);
            thread.Start();
            thread.Start();
        }

        void StartCurrentThread()
        {
            Thread.CurrentThread.Start();
        }

        [Test]
        public void Start()
        {
            Assert.Throws<ThreadStateException>(StartTwice);
            Assert.Throws<ThreadStateException>(StartCurrentThread);
        }

        void JoinUnstarted()
        {
            var thread = new Thread(Dummy);
            thread.Join();
        }

        [Test]
        public void Join()
        {
            Assert.Throws<ThreadStateException>(JoinUnstarted);
        }

        bool _currentThreadNameCorrect;
        void NameFunc()
        {
            _currentThreadNameCorrect = Thread.CurrentThread.Name == "nameThread";
        }

        [Test]
        public void Name()
        {
            var thread  = new Thread(NameFunc);

            Assert.IsFalse(_currentThreadNameCorrect);

            thread.Name = "nameThread";
            thread.Start();
            thread.Join();

            Assert.IsTrue(_currentThreadNameCorrect);
        }
    }
}
