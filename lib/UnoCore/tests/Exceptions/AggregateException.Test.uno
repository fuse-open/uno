using Uno;
using Uno.Diagnostics;
using Uno.Testing;
using Uno.Collections;

namespace Exceptions.Test
{
    public class AggregateExceptionTest
    {
        void ThrowAggregateException()
        {
            var exceptions = new List<Exception>();
            for (int i = 0; i < 3; ++i)
            {
                try
                {
                    throw new Exception("inner-exception");
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            throw new AggregateException("aggregate-exception", exceptions.ToArray());
        }

        [Test]
        public void InnerExceptions()
        {
            var e = Assert.Throws(ThrowAggregateException);
            var ae = e as AggregateException;
            Assert.AreEqual(ae.InnerExceptions.Count, 3);

            foreach (var innerException in ae.InnerExceptions)
                Assert.AreEqual(innerException.Message, "inner-exception");
        }

        List<string> ParseInnerExceptions(string s)
        {
            int pos = 0;
            var innerExceptions = new List<string>();
            while (true)
            {
                int innerStart = s.IndexOf(string.Format("---> (Inner Exception #{0}) ", innerExceptions.Count), pos);
                if (innerStart < 0)
                    break;

                int innerEnd = s.IndexOf("<---", innerStart);
                if (innerEnd < 0)
                {
                    debug_log "string:\n---8<---" + s + "\n---8<---";
                    throw new Exception(string.Format("unterminated inner exception #{0}!", innerExceptions.Count));
                }

                innerExceptions.Add(s.Substring(innerStart, innerEnd - innerStart));
                pos = innerEnd;
            }
            return innerExceptions;
        }

        [Test]
        new public void ToString()
        {
            var e = Assert.Throws(ThrowAggregateException);
            var ae = e as AggregateException;

            var innerExceptionStrings = ParseInnerExceptions(e.ToString());
            Assert.AreEqual(innerExceptionStrings.Count, ae.InnerExceptions.Count);

            for (int i = 0; i < innerExceptionStrings.Count; ++i)
                Assert.Contains(ae.InnerExceptions[i].Message, innerExceptionStrings[i]);
        }
    }
}
