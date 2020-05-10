using NUnit.Framework;
using Uno.UX.Markup.UXIL.Expressions;

namespace Uno.UX.Markup.Tests.Helpers
{
    static class AssertExtensions
    {
        public static void AreEqualValues(Expression expected, Expression actual)
        {
            if (!expected.ValueEquals(actual))
                throw new AssertionException("Expected: <" + expected + ">\nBut was:  <" + actual + ">\n");
        }
    }
}
