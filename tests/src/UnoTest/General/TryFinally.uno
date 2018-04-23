using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class TryFinally
    {
        [Test]
        public void Run()
        {
            try
            {
                string result = string.Empty;
                try
                {
                    Assert.IsTrue(true);
                    result += "1";
                    throw new Exception();
                    //return;
                }
                catch (InvalidOperationException e)
                {
                    Assert.IsTrue(false);
                    result += "2";
                    throw;
                }
                catch
                {
                    Assert.IsTrue(true);
                    result += "3";
                    throw;
                }
                finally
                {
                    Assert.IsTrue(true);
                    result += "4";
                }
                Assert.AreEqual("134", result);
            }
            catch
            {
                 Assert.IsTrue(true);
            }
        }

        [Test]
        public void Finally()
        {
            int f = 0;
            try
            {
            }
            finally
            {
                ++f;
            }
            Assert.AreEqual(1, f);
        }

        [Test]
        public void WithReturn()
        {
            int f = 0;
            RunWithReturn(ref f);
            Assert.AreEqual(1, f);
        }

        void RunWithReturn(ref int f)
        {
            try
            {
                return;
            }
            finally
            {
                ++f;
            }
        }

        [Test]
        public void WithBreak()
        {
            int f = 0;
            for (int i = 0; i < 10; ++i)
            {
                try
                {
                    break;
                }
                finally
                {
                    ++f;
                }
            }
            Assert.AreEqual(1, f);
        }

        [Test]
        public void WithContinue()
        {
            int f = 0;
            for (int i = 0; i < 1; ++i)
            {
                try
                {
                    continue;
                }
                finally
                {
                    ++f;
                }
            }
            Assert.AreEqual(1, f);
        }

        [Test]
        public void ThrowFromFinally()
        {
            Exception thrown = null;
            try
            {
                try
                {
                    throw new Exception("foo");
                }
                finally
                {
                    throw new Exception("bar");
                }
            }
            catch (Exception e)
            {
                thrown = e;
            }

            Assert.AreNotEqual(null, thrown);
            Assert.AreEqual("bar", thrown.Message);
        }

        [Test]
        public void WithBreakInCatch()
        {
            int f = 0;
            for (int i = 0; i < 10; ++i)
            {
                try
                {
                    throw new Exception("test");
                }
                catch (Exception e)
                {
                    break;
                }
                finally
                {
                    ++f;
                }
            }
            Assert.AreEqual(1, f);
        }

        void RunReturnInsideTryInsideTry(ref int f)
        {
            try
            {
                try
                {
                    return;
                }
                finally
                {
                    ++f;
                }
            }
            finally
            {
                ++f;
            }
        }

        [Test]
        public void ReturnInsideTryInsideTry()
        {
            int f = 0;
            RunReturnInsideTryInsideTry(ref f);
            Assert.AreEqual(2, f);
        }

        int RunReturnStatementRunsBeforeFinalizer()
        {
            int foo = 1;
            try
            {
                return foo;
            }
            finally
            {
                foo = 0;
            }
        }

        [Test]
        public void ReturnStatementRunsBeforeFinalizer()
        {
            int f = RunReturnStatementRunsBeforeFinalizer();
            Assert.AreEqual(1, f);
        }

        [Test]
        public void NestedFinally()
        {
            var innerFinallys = 0;
            var outerCatches = 0;
            try
            {
                try
                {
                }
                finally
                {
                    ++innerFinallys;
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                ++outerCatches;
            }
            Assert.AreEqual(1, innerFinallys);
            Assert.AreEqual(1, outerCatches);
        }
    }
}
