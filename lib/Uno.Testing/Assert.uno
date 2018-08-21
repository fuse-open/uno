using Uno.UX;
using Uno.Collections;
using Uno.Compiler;
using Uno.Text;

namespace Uno.Testing
{
    public static partial class Assert
    {
        public const float ZeroTolerance = 1e-05f;

        private static int maxStringLength = 300;

        public static void Ignore(string message)
        {
            throw new IgnoreException(message);
        }

        // Throw assertions
        private static void ReportFailure(string filePath, int lineNumber, string memberName, string expected, string actual)
        {
            throw new AssertionFailedException(filePath, lineNumber, memberName, expected, actual);
        }

        /**
            Asserts that `method` throws an exception of any type

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static Exception Throws(Action method,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            try
            {
                method();
            }
            catch (Exception e)
            {
                return e;
            }

            ReportFailure(filePath, lineNumber, memberName, "It throws any exception", "It did not throw any exception");
            return null;
        }

        /**
            Asserts that `method` throws an exception of type `T`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static Exception Throws<T>(Action method,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {

            // HACK: This is an ugly work-around for a compiler-bug, see
            // https://github.com/fusetools/Uno/issues/487
            // for details.
            Exception ret = null;

            try
            {
                method();
            }
            catch (Exception e)
            {
                if (!(e is T))
                    throw;
                ret = e;
            }

            if (ret != null)
                return ret;

            ReportFailure(filePath, lineNumber, memberName, "It throws an exception of specific type", "It did not throw");
            return null;
        }

        /**
            Asserts that `method` does not throw any exception

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void DoesNotThrowAny(Action method,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            try
            {
                method();
            }
            catch (Exception e)
            {
                ReportFailure(filePath, lineNumber, memberName, "It does not throw", e.ToString());
            }
        }


        //Contains assertions

        /**
            Asserts that `collection` contains an element equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void Contains<T>(T expected, IEnumerable<T> collection,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (collection == null)
            {
                ReportFailure(filePath, lineNumber, memberName, "It contains " + expected, "Collection was null");
            }
            else
            {
                foreach (T item in collection)
                {
                    if (Uno.Generic.Equals(expected, item))
                    {
                        return;
                    }
                }
                ReportFailure(filePath, lineNumber, memberName, "It contains " + expected, ToLimitedString(collection));
            }
        }

        /**
            Asserts that `collection` contains an element equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void Contains<T>(T expected, T[] collection,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (collection == null)
            {
                ReportFailure(filePath, lineNumber, memberName, "It contains " + expected, "Array was null");
            }
            else
            {
                foreach (T item in collection)
                {
                    if (Uno.Generic.Equals(expected, item))
                    {
                        return;
                    }
                }
                ReportFailure(filePath, lineNumber, memberName, "It contains " + expected, ToLimitedString(collection));
            }
        }

        private static string ToLimitedString<T>(T[] array)
        {
            var limiter = new LimitedStringBuilder();
            foreach (var v in array)
            {
                if (limiter.Append(v))
                    break;
            }
            return limiter.ToString();
        }

        /**
            Asserts that `haystack` contains the substring `needle`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void Contains(string needle, string haystack,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (needle.Length > haystack.Length)
            {
                ReportFailure(filePath, lineNumber, memberName, "It contains \"" + needle + "\"", "\"" + ToLimitedString(haystack) + "\"");
            }
            if (needle.Length == 0)
            {
                return;
            }
            for (int startPos = 0; startPos < haystack.Length; ++startPos)
            {
                int character = 0;
                while (startPos + character < haystack.Length && character < needle.Length)
                {
                    if (haystack[startPos + character] != needle[character])
                    {
                        break;
                    }
                    if (character == needle.Length - 1)
                    {
                        return;
                    }
                    character++;
                }
            }
            ReportFailure(filePath, lineNumber, memberName, "It contains \"" + needle + "\"", "\"" + ToLimitedString(haystack) + "\"");
        }


        //Equality assertions

        /**
            Asserts that `actual` is equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(object expected, object actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected == null && actual == null)
                return;
            if (expected == null)
                ReportFailure(filePath, lineNumber, memberName, "null", ToPrintable(actual));
            if (actual == null)
                ReportFailure(filePath, lineNumber, memberName, ToPrintable(expected), "null");
            if (!expected.Equals(actual))
                ReportFailure(filePath, lineNumber, memberName, ToPrintable(expected), ToPrintable(actual));
        }

        /**
            Asserts that `actual` is equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqualSize(Size expected, Size actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected != actual)
                ReportFailure(filePath, lineNumber, memberName, expected.ToString(), actual.ToString());
        }

        /**
            Asserts that `actual` is equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqualSize2(Size2 expected, Size2 actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected != actual)
                ReportFailure(filePath, lineNumber, memberName, expected.ToString(), actual.ToString());
        }

        /**
            Asserts that `actual` is equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(long expected, long actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected != actual)
                ReportFailure(filePath, lineNumber, memberName, expected.ToString(), actual.ToString());
        }

        /**
            Asserts that `actual` is equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(ulong expected, ulong actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected != actual)
                ReportFailure(filePath, lineNumber, memberName, expected.ToString(), actual.ToString());
        }

        /**
            Asserts that `actual` is equal to `expected`
            
            If `tolerance` is given, asserts that `actual` and `expected` do not differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(double expected, double actual, double tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (DifferMoreThan(expected, actual, tolerance))
                ReportFailure(filePath, lineNumber, memberName, String.Format("{0:F16}", expected), String.Format("{0:F16}", actual));
        }

        /**
            Asserts that `actual` is equal to `expected`
            
            If `tolerance` is given, asserts that `actual` and `expected` do not differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(float expected, float actual, float tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (DifferMoreThan(expected, actual, tolerance))
                ReportFailure(filePath, lineNumber, memberName, String.Format("{0:F7}", expected), String.Format("{0:F7}", actual));
        }

        /**
            Asserts that `actual` is not equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(object expected, object actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected == null && actual == null)
                ReportFailure(filePath, lineNumber, memberName, "Arguments not to be equal", "They are both null");
            if (expected == null || actual == null)
                return;
            if (expected.Equals(actual))
                ReportFailure(filePath, lineNumber, memberName, expected.ToString() + " to differ from " + actual.ToString(), "They compare equal");
        }

        /**
            Asserts that `actual` is not equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(long expected, long actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected == actual)
                ReportFailure(filePath, lineNumber, memberName, expected.ToString() + " to differ from " + actual.ToString(), "They compare equal");
        }

        /**
            Asserts that `actual` is not equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(ulong expected, ulong actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected == actual)
                ReportFailure(filePath, lineNumber, memberName, expected.ToString() + " to differ from " + actual.ToString(), "They compare equal");
        }

        /**
            Asserts that `actual` is not equal to `expected`
            
            If `tolerance` is given, asserts that `actual` and `expected` differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(double expected, double actual, double tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (!DifferMoreThan(expected, actual, tolerance))
                ReportFailure(filePath, lineNumber, memberName, String.Format("{0:F16}", expected) + " to differ from " + String.Format("{0:F16}", actual), "They compare equal");
        }

        /**
            Asserts that `actual` is not equal to `expected`
            
            If `tolerance` is given, asserts that `actual` and `expected` differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(float expected, float actual, float tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (!DifferMoreThan(expected, actual, tolerance))
                ReportFailure(filePath, lineNumber, memberName, String.Format("{0:F7}", expected) + " to differ from " + String.Format("{0:F7}", actual), "They compare equal");
        }


        //
        //Vector/Matrix equality assertions

        /**
            Asserts that `actual` is equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(int2 expected, int2 actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected != actual)
                ReportFailure(filePath, lineNumber, memberName, int2ToString(expected), int2ToString(actual));
        }

        /**
            Asserts that `actual` is equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(int3 expected, int3 actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected != actual)
                ReportFailure(filePath, lineNumber, memberName, int3ToString(expected), int3ToString(actual));
        }

        /**
            Asserts that `actual` is equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(int4 expected, int4 actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected != actual)
                ReportFailure(filePath, lineNumber, memberName, int4ToString(expected), int4ToString(actual));
        }

        /**
            Asserts that `actual` is equal to `expected`
            
            If `tolerance` is given, asserts that each element of `actual` and `expected` do not differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(float2 expected, float2 actual, float tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (DifferMoreThan(expected, actual, tolerance))
                ReportFailure(filePath, lineNumber, memberName, float2ToString(expected), float2ToString(actual));
        }

        /**
            Asserts that `actual` is equal to `expected`
            
            If `tolerance` is given, asserts that each element of `actual` and `expected` do not differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(float3 expected, float3 actual, float tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (DifferMoreThan(expected, actual, tolerance))
                ReportFailure(filePath, lineNumber, memberName, float3ToString(expected), float3ToString(actual));
        }

        /**
            Asserts that `actual` is equal to `expected`
            
            If `tolerance` is given, asserts that each element of `actual` and `expected` do not differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(float4 expected, float4 actual, float tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (DifferMoreThan(expected, actual, tolerance))
                ReportFailure(filePath, lineNumber, memberName, float4ToString(expected), float4ToString(actual));
        }

        /**
            Asserts that `actual` is equal to `expected`
            
            If `tolerance` is given, asserts that each element of `actual` and `expected` do not differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(float3x3 expected, float3x3 actual, float tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (DifferMoreThan(expected[0], actual[0], tolerance)
                || DifferMoreThan(expected[1], actual[1], tolerance)
                || DifferMoreThan(expected[2], actual[2], tolerance))
                ReportFailure(filePath, lineNumber, memberName, float3x3ToString(expected), float3x3ToString(actual));
        }

        /**
            Asserts that `actual` is equal to `expected`
            
            If `tolerance` is given, asserts that each element of `actual` and `expected` do not differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual(float4x4 expected, float4x4 actual, float tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (DifferMoreThan(expected[0], actual[0], tolerance)
                || DifferMoreThan(expected[1], actual[1], tolerance)
                || DifferMoreThan(expected[2], actual[2], tolerance)
                || DifferMoreThan(expected[3], actual[3], tolerance))
                ReportFailure(filePath, lineNumber, memberName, float4x4ToString(expected), float4x4ToString(actual));
        }

        /**
            Asserts that `actual` is not equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(int2 expected, int2 actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected == actual)
                ReportFailure(filePath, lineNumber, memberName, int2ToString(expected) + " to differ from " + int2ToString(actual), "They compare equal");
        }

        /**
            Asserts that `actual` is not equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(int3 expected, int3 actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected == actual)
                ReportFailure(filePath, lineNumber, memberName, int3ToString(expected) + " to differ from " + int3ToString(actual), "They compare equal");
        }

        /**
            Asserts that `actual` is not equal to `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(int4 expected, int4 actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected == actual)
                ReportFailure(filePath, lineNumber, memberName, int4ToString(expected) + " to differ from " + int4ToString(actual), "They compare equal");
        }

        /**
            Asserts that `actual` is not equal to `expected`

            If `tolerance` is given, asserts that at least one element of `actual` and `expected` differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(float2 expected, float2 actual, float tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (!DifferMoreThan(expected, actual, tolerance))
                ReportFailure(filePath, lineNumber, memberName, float2ToString(expected) + " to differ from " + float2ToString(actual), "They compare equal");
        }

        /**
            Asserts that `actual` is not equal to `expected`

            If `tolerance` is given, asserts that at least one element of `actual` and `expected` differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(float3 expected, float3 actual, float tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (!DifferMoreThan(expected, actual, tolerance))
                ReportFailure(filePath, lineNumber, memberName, float3ToString(expected) + " to differ from " + float3ToString(actual), "They compare equal");
        }

        /**
            Asserts that `actual` is not equal to `expected`

            If `tolerance` is given, asserts that at least one element of `actual` and `expected` differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(float4 expected, float4 actual, float tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (!DifferMoreThan(expected, actual, tolerance))
                ReportFailure(filePath, lineNumber, memberName, float4ToString(expected) + " to differ from " + float4ToString(actual), "They compare equal");
        }

        /**
            Asserts that `actual` is not equal to `expected`

            If `tolerance` is given, asserts that at least one element of `actual` and `expected` differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(float3x3 expected, float3x3 actual, float tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (!DifferMoreThan(expected[0], actual[0], tolerance)
                && !DifferMoreThan(expected[1], actual[1], tolerance)
                && !DifferMoreThan(expected[2], actual[2], tolerance))
                ReportFailure(filePath, lineNumber, memberName, float3x3ToString(expected) + " to differ from " + float3x3ToString(actual), "They compare equal");
        }

        /**
            Asserts that `actual` is not equal to `expected`

            If `tolerance` is given, asserts that at least one element of `actual` and `expected` differ by more than `tolerance`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreNotEqual(float4x4 expected, float4x4 actual, float tolerance = ZeroTolerance,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (!DifferMoreThan(expected[0], actual[0], tolerance)
                && !DifferMoreThan(expected[1], actual[1], tolerance)
                && !DifferMoreThan(expected[2], actual[2], tolerance)
                && !DifferMoreThan(expected[3], actual[3], tolerance))
                ReportFailure(filePath, lineNumber, memberName, float4x4ToString(expected) + " to differ from " + float4x4ToString(actual), "They compare equal");
        }


        //Collection and array assertions

        /**
            Asserts that the elements of `actual` are equal to the elements of `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreCollectionsEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            var e = expected.GetEnumerator();
            var a = actual.GetEnumerator();
            int position = 0;
            while (true)
            {
                var eWasLast = !e.MoveNext();
                var aWasLast = !a.MoveNext();
                if (eWasLast && aWasLast)
                    return;
                if (eWasLast && ! aWasLast)
                    ReportFailure(filePath, lineNumber, memberName, "IEnumerables of identical length", "Actual had more items than expected (which had " + position + ")");
                if (!eWasLast && aWasLast)
                    ReportFailure(filePath, lineNumber, memberName, "IEnumerables of identical length", "Expected had more items than actual (which had " + position + ")");
                if (!e.Current.Equals(a.Current))
                    ReportFailure(filePath, lineNumber, memberName, "'" + e.Current + "' in position " + position, "'" + a.Current + "' in position " + position);
                position++;
            }
            throw new Exception("Internal errror");
        }

        /**
            Asserts that the elements of `actual` are equal to the elements of `expected`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void AreEqual<T>(T[] expected, T[] actual,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (expected.Length != actual.Length)
                ReportFailure(filePath, lineNumber, memberName, expected.Length.ToString() + " elements",  actual.Length + " elements");
            for (int i = 0; i < expected.Length; i++)
                if (!expected[i].Equals(actual[i]))
                    ReportFailure(filePath, lineNumber, memberName, "'" + ToPrintable(expected[i]) + "' in position " + i, "'" + ToPrintable(actual[i]) + "' in position " + i);
        }


        //Other assertions

        /**
            Call this method to manually fail a test

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void Fail(string message,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            ReportFailure(filePath, lineNumber, memberName, "", message);
        }

        /**
            Asserts that `val` is true

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void IsTrue(bool val,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (!val)
                ReportFailure(filePath, lineNumber, memberName, "True", "False");
        }

        /**
            Asserts that `val` is false

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void IsFalse(bool val,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (val)
                ReportFailure(filePath, lineNumber, memberName, "False", "True");
        }

        /**
            Asserts that `obj` is of type `T`

            (Do not use the `filePath`, `lineNumber` and `memberName` arguments, as these are filled in automatically)
        */
        public static void OfType<T>(object obj,
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            if (obj is T)
                return;

            var expected = "object of type '" + typeof(T).FullName + "'";
            var actual = (obj == null) ? "null" : "object of type '" + obj.GetType().FullName + "'";

            Assert.ReportFailure(filePath, lineNumber, memberName, expected, actual);
        }

        //Internal helpers

        private static bool DifferMoreThan(double expected, double actual, double tolerance)
        {
            return Math.Abs(expected-actual) > tolerance;
        }

        private static bool DifferMoreThan(float expected, float actual, float tolerance)
        {
            return Math.Abs(expected-actual) > tolerance;
        }

        private static bool DifferMoreThan(float2 expected, float2 actual, float tolerance)
        {
            return DifferMoreThan(expected.X, actual.X, tolerance)
                || DifferMoreThan(expected.Y, actual.Y, tolerance);
        }

        private static bool DifferMoreThan(float3 expected, float3 actual, float tolerance)
        {
            return DifferMoreThan(expected.X, actual.X, tolerance)
                || DifferMoreThan(expected.Y, actual.Y, tolerance)
                || DifferMoreThan(expected.Z, actual.Z, tolerance);
        }

        private static bool DifferMoreThan(float4 expected, float4 actual, float tolerance)
        {
            return DifferMoreThan(expected.X, actual.X, tolerance)
                || DifferMoreThan(expected.Y, actual.Y, tolerance)
                || DifferMoreThan(expected.Z, actual.Z, tolerance)
                || DifferMoreThan(expected.W, actual.W, tolerance);
        }

        private static string int2ToString(int2 i)
        {
            return String.Format("[{0}, {1}]", i[0], i[1]);
        }

        private static string int3ToString(int3 i)
        {
            return String.Format("[{0}, {1}, {2}]", i[0], i[1], i[2]);
        }

        private static string int4ToString(int4 i)
        {
            return String.Format("[{0}, {1}, {2}, {3}]", i[0], i[1], i[2], i[3]);
        }

        private static string float2ToString(float2 f)
        {
            return String.Format("[{0:F7}, {1:F7}]", f[0], f[1]);
        }

        private static string float3ToString(float3 f)
        {
            return String.Format("[{0:F7}, {1:F7}, {2:F7}]", f[0], f[1], f[2]);
        }

        private static string float4ToString(float4 f)
        {
            return String.Format("[{0:F7}, {1:F7}, {2:F7}, {3:F7}]", f[0], f[1], f[2], f[3]);
        }

        private static string float3x3ToString(float3x3 m)
        {
            return "[[" + float3ToString(m[0]) + "], [" + float3ToString(m[1]) + "], [" + float3ToString(m[2]) + "]]";
        }

        private static string float4x4ToString(float4x4 m)
        {
            return "[[" + float4ToString(m[0]) + "], [" + float4ToString(m[1]) + "], [" + float4ToString(m[2]) + "], [" + float4ToString(m[3]) + "]]";
        }

        private static string ToLimitedString<T>(IEnumerable<T> collection)
        {
            var limiter = new LimitedStringBuilder();
            foreach (var v in collection)
            {
                if (limiter.Append(v))
                    break;
            }
            return limiter.ToString();
        }

        private static string ToLimitedString(string originalString)
        {
            if (originalString.Length > maxStringLength)
            {
                return originalString.Substring(0, maxStringLength) + "(...)";
            }
            return originalString;
        }

        internal class LimitedStringBuilder
        {
            private int length = 0;
            private bool first = true;
            private StringBuilder builder = new StringBuilder();

            public bool Append(object o)
            {
                var str = o.ToString();
                length += str.Length + 1;
                if (length > maxStringLength)
                {
                    builder.Append(",(...)");
                    return true;
                }
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(",");
                }
                builder.Append(str);
                return false;
            }

            public override string ToString()
            {
                return builder.ToString();
            }
        }

        private static string ToPrintable(object elm)
        {
            if (elm is char)
                return ToPrintable((char)elm);
            else if (elm is string)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var ch in (string)elm)
                    builder.Append(ToPrintable(ch));
                return builder.ToString();
            }
            return elm.ToString();
        }

        private static string ToPrintable(char ch)
        {
            if (0x20 <= ch && ch <= 0x7e)
                return ch.ToString();
            else
                return String.Format("\\u{0:X4}", (int)ch);
        }
    }
}
