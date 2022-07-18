namespace Uno.Time
{
    internal static class Preconditions
    {
        internal static T CheckNotNull<T>(T argument, string paramName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }
            return argument;
        }

        internal static void CheckArgumentRange(string paramName, long value, long minInclusive, long maxInclusive)
        {
            if (value < minInclusive || value > maxInclusive)
            {
                throw new ArgumentOutOfRangeException(paramName,
                    "Value should be in range [" + minInclusive + "-" + maxInclusive + "]");
            }
        }

        internal static void CheckArgumentRange(string paramName, int value, int minInclusive, int maxInclusive)
        {
            if (value < minInclusive || value > maxInclusive)
            {
                throw new ArgumentOutOfRangeException(paramName,
                    "Value should be in range [" + minInclusive + "-" + maxInclusive + "]");
            }
        }

        internal static void CheckArgument(bool expression, string parameter, string message)
        {
            if (!expression)
            {
                throw new ArgumentException(message, parameter);
            }
        }

        internal static void CheckArgument<T>(bool expression, string parameter, string messageFormat, T messageArg)
        {
            if (!expression)
            {
                string message = string.Format(messageFormat, messageArg);
                throw new ArgumentException(message, parameter);
            }
        }

        internal static void CheckArgument<T1, T2>(bool expression, string parameter, string messageFormat, T1 messageArg1, T2 messageArg2)
        {
            if (!expression)
            {
                string message = string.Format(messageFormat, messageArg1, messageArg2);
                throw new ArgumentException(message, parameter);
            }
        }

        internal static void CheckArgument(bool expression, string parameter, string messageFormat, params object[] messageArgs)
        {
            if (!expression)
            {
                string message = string.Format(messageFormat, messageArgs);
                throw new ArgumentException(message, parameter);
            }
        }
    }
}
