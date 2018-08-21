namespace Uno.Collections
{
    class OrderBy<T>
    {
        readonly Func<T, int> _orderFunction;

        public OrderBy(Func<T, int> orderFunction)
        {
            _orderFunction = orderFunction;
        }

        public int Compare(T a, T b)
        {
            return _orderFunction(a) - _orderFunction(b);
        }
    }

    class OrderByCompare<T>
    {
        readonly Func<T, T, int> _orderFunction;

        public OrderByCompare(Func<T, T, int> orderFunction)
        {
            _orderFunction = orderFunction;
        }

        public int Compare(T a, T b)
        {
            return _orderFunction(a, b);
        }
    }
}
