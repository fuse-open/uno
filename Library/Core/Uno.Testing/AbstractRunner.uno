namespace Uno.Testing
{
    abstract class AbstractRunner
    {
        private abstract void TestStarting(string name);
        private abstract void TestPassed();
        private abstract void TestIgnored(string reason);
        private abstract void AssertionFailed(AssertionFailedException e);
        private abstract void ExceptionThrown(Exception e);
        private abstract void Start();
        private abstract void Stop();

        protected readonly Registry _registry;

        private bool started = false;
        private int _testIndex;

        protected int TestIndex { get { return _testIndex; } }
        protected int TestCount { get { return _registry.Count; } }

        private void ScheduleTest(int testIndex)
        {
            if (testIndex < _registry.Count)
            {
                _testIndex = testIndex;
                NextTest = _registry[testIndex];
            }
            else
                Stop();
        }

        protected void SheduleNextTest()
        {
            ScheduleTest(_testIndex + 1);
        }

        public NamedTestMethod _nextTest;
        public NamedTestMethod NextTest
        {
            get { return _nextTest; }
            set
            {
                _nextTest = value;
            }
        }

        protected AbstractRunner(Registry registry)
        {
            _registry = registry;
        }

        public void Update()
        {
            EnsureStarted();
            if (NextTest != null)
            {
                var test = NextTest;
                NextTest = null;
                RunTest(test);
            }
        }

        public void EnsureStarted()
        {
            if (!started)
            {
                started = true;
                Start();

                if (_registry.Count > 0)
                    ScheduleTest(0);
                else
                    Stop();
            }
        }

        void OnException(Exception e)
        {
            var assertionFailedException = e as AssertionFailedException;
            if (assertionFailedException != null)
            {
                AssertionFailed(assertionFailedException);
                return;
            }

            var ignoreException = e as IgnoreException;
            if (ignoreException != null)
            {
                TestIgnored(ignoreException.Message);
                return;
            }

            ExceptionThrown(e);
        }

        public void RunTest(NamedTestMethod test)
        {
            TestStarting(test.Name);

            if (test.Ignore)
            {
                TestIgnored(test.IgnoreReason);
                return;
            }

            try
            {
                test.Method();
                TestPassed();
            }
            catch (Exception e)
            {
                OnException(e);
            }
        }
    }
}
