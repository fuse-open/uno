using Uno.Compiler.ExportTargetInterop;
using System.Collections.Generic;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Array")]
    [extern(CPLUSPLUS) Set("TypeName", "uArray*")]
    public class Array
    {
        [extern(CPLUSPLUS) Set("IsIntrinsic", "true")]
        public extern int Length { get; }

        [DotNetOverride]
        public static void Copy<T>(T[] sourceArray, T[] destinationArray, int length)
        {
            Copy(sourceArray, 0, destinationArray, 0, length);
        }

        [DotNetOverride]
        public static void Copy<T>(T[] sourceArray, int sourceIndex,
                                   T[] destinationArray, int destinationIndex,
                                   int length)
        {
            if defined(DOTNET)
                Copy((Array) sourceArray, sourceIndex, (Array) destinationArray, destinationIndex, length);
            else
            {
                if (sourceArray == null)
                    throw new ArgumentNullException(nameof(sourceArray));
                if (destinationArray == null)
                    throw new ArgumentNullException(nameof(destinationArray));
                if (sourceIndex < 0)
                    throw new ArgumentOutOfRangeException("Non-negative number required.", nameof(sourceIndex));
                if (destinationIndex < 0)
                    throw new ArgumentOutOfRangeException("Non-negative number required.", nameof(destinationIndex));
                if (length < 0)
                    throw new ArgumentOutOfRangeException("Non-negative number required.", nameof(length));
                if (sourceArray.Length < sourceIndex + length)
                    throw new ArgumentException("Destination array was not long enough. Check destinationIndex and length, and the array's lower bounds." + sourceArray.Length + ", "  + destinationIndex + ", " + length);
                if (destinationArray.Length < destinationIndex + length)
                    throw new ArgumentException("Source array was not long enough. Check sourceIndex and length, and the array's lower bounds.");

                for (int i = 0; i < length; i++)
                    destinationArray[destinationIndex + i] = sourceArray[sourceIndex + i];
            }
        }

        [DotNetOverride]
        public static void Sort<T>(T[] elements, Comparison<T> comparison)
        {
            if (elements != null)
                Sort(elements, 0, elements.Length, comparison);
        }

        [DotNetOverride]
        public static void Sort<T>(T[] elements, int index, int length, Comparison<T> comparison)
        {
            if defined(DOTNET)
                Sort(elements, index, length, Comparer<T>.Create(comparison));
            else
            {
                if (length == 0)
                    return;
                if (elements == null)
                    throw new ArgumentNullException(nameof(elements));
                if (index < 0)
                    throw new ArgumentOutOfRangeException("Non-negative number required.", nameof(index));
                if (length < 0)
                    throw new ArgumentOutOfRangeException("Non-negative number required.", nameof(length));
                if (elements.Length < index + length)
                    throw new ArgumentException("Index and length do not specify a valid range in elements.");
                if (comparison == null)
                    throw new InvalidOperationException("Comparer is null.");

                if (length > 280)
                    QuickSort(elements, comparison, index, index + length);
                else if (length > 0)
                    ShellSort(elements, comparison, index, index + length);
            }
        }

        static void ShellSort<T>(T[] data, Comparison<T> comparison, int left, int right)
        {
            const float s = 2.8f;

            int size = right - left;
            int increment = size / 2;

            while (increment > 0)
            {
                for (int i = left + increment; i < size; i++)
                {
                    int j = i;
                    var temp = data[i];

                    while ((j >= increment) && comparison(temp, data[j - increment]) < 0)
                    {
                        Swap(data, j, j - increment);
                        j -= increment;
                    }

                    data[j] = temp;
                }

                if (increment < s && increment > 1)
                    increment = 1;
                else
                    increment = (int)((float)increment / s);
            }
        }

        static void QuickSort<T>(T[] data, Comparison<T> comparison, int left, int right)
        {
            if (left < right)
            {
                int rightIndex = right - 1;
                int pivotIndex = left;

                var pivotValue = data[pivotIndex];
                Swap(data, pivotIndex, rightIndex);

                for (int i = left; i < rightIndex; i++)
                {
                    if (comparison(data[i], pivotValue) < 0)
                    {
                        Swap(data, i, pivotIndex);
                        pivotIndex++;
                    }
                }

                Swap(data, pivotIndex, rightIndex);

                QuickSort(data, comparison, left, pivotIndex);
                QuickSort(data, comparison, pivotIndex + 1, right);
            }
        }

        static void Swap<T>(T[] data, int a, int b)
        {
            var temp = data[a];
            data[a] = data[b];
            data[b] = temp;
        }

        public static int IndexOf<T>(T[] array, T value)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            return IndexOfUnchecked(array, value, 0, array.Length);
        }

        public static int IndexOf<T>(T[] array, T value, int startIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (startIndex < 0 || startIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return IndexOfUnchecked(array, value, startIndex, array.Length - startIndex);
        }

        public static int IndexOf<T>(T[] array, T value, int startIndex, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (startIndex < 0 || startIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0 || count > array.Length - startIndex)
                throw new ArgumentOutOfRangeException(nameof(count));

            return IndexOfUnchecked(array, value, startIndex, count);
        }

        static int IndexOfUnchecked<T>(T[] array, T value, int startIndex, int count)
        {
            for (int i = 0; i < count; i++)
                if (Generic.Equals(array[startIndex + i], value))
                    return startIndex + i;

            return -1;
        }

        public static int LastIndexOf<T>(T[] array, T value)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            return LastIndexOfUnchecked(array, value, array.Length - 1, array.Length);
        }

        public static int LastIndexOf<T>(T[] array, T value, int startIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (startIndex < 0 || startIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return LastIndexOfUnchecked(array, value, startIndex, startIndex + 1);
        }

        public static int LastIndexOf<T>(T[] array, T value, int startIndex, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (startIndex < 0 || startIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0 || count > startIndex + 1)
                throw new ArgumentOutOfRangeException(nameof(count));

            return LastIndexOfUnchecked(array, value, startIndex, count);
        }

        static int LastIndexOfUnchecked<T>(T[] array, T value, int startIndex, int count)
        {
            for (int i = 0; i < count; i++)
                if (Generic.Equals(array[startIndex - i], value))
                    return startIndex - i;

            return -1;
        }

        extern(DOTNET) internal static void Sort<T>(T[] array, int index, int length, IComparer<T> comparer);
        extern(DOTNET) internal static void Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length);
        extern(DOTNET) internal static void Reverse(Array array, int index, int length);
        extern(DOTNET) internal static void Reverse(Array array);
    }
}

namespace System.Collections.Generic
{
    using Uno;

    [DotNetType]
    extern(DOTNET) interface IComparer<T>
    {
    }

    [DotNetType]
    extern(DOTNET) abstract class Comparer<T> : IComparer<T>
    {
        public static extern Comparer<T> Create(Comparison<T> comparison);
    }
}
