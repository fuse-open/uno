namespace Uno.Collections
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this T[] self)
        {
            return new Uno.Internal.ArrayEnumerable<T>(self);
        }

        public static IEnumerable<U> OfType<T, U>(this IEnumerable<T> self)
        {
            return new OfTypeEnumerable<T, U>(self);
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> self, Func<T, int> orderFunction)
        {
            var list = ToList(self);
            list.Sort(new OrderBy<T>(orderFunction).Compare);
            return list;
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> self, Func<T, T, int> orderFunction)
        {
            var list = ToList(self);
            list.Sort(new OrderByCompare<T>(orderFunction).Compare);
            return list;
        }

        public static IEnumerable<TRet> Select<T,TRet>(this IEnumerable<T> self, Func<T,TRet> func)
        {
            return new SelectEnumerable<T, TRet>(self, func);
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<T> self, IEnumerable<T> other)
        {
            return new UnionEnumerable<T>(self, other);
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> self, Predicate<T> predicate)
        {
            return new WhereEnumerable<T>(self, predicate);
        }

        public static int IndexOf<T>(this IEnumerable<T> self, T element)
        {
            int i = 0;
            foreach (var item in self)
            {
                if (item.Equals(element))
                    return i;
                i++;
            }
            return -1;
        }

        class LinkedList<T>
        {
            public T Item;
            public LinkedList<T> Next;

            public LinkedList(T item, LinkedList<T> next)
            {
                Item = item;
                Next = next;
            }
        }

        public static T[] ToArray<T>(this IEnumerable<T> self)
        {
            LinkedList<T> head = null;
            int count = 0;
            foreach (var item in self)
            {
                head = new LinkedList<T>(item, head);
                count++;
            }

            var res = new T[count];
            for (var current = head; current != null; current = current.Next)
                res[--count] = current.Item;
            return res;
        }

        public static List<T> ToList<T>(this IEnumerable<T> self)
        {
            LinkedList<T> head = null;
            int count = 0;
            foreach (var item in self)
            {
                head = new LinkedList<T>(item, head);
                count++;
            }

            var res = new List<T>(count);
            res.AddRange(new T[count]);
            for (var current = head; current != null; current = current.Next)
                res[--count] = current.Item;
            return res;
        }

        public static bool Contains<T>(this IEnumerable<T> self, T lookingFor)
        {
            foreach (var item in self)
                if (item.Equals(lookingFor))
                    return true;
            return false;
        }

        public static int Count<T>(this IEnumerable<T> self)
        {
            int c = 0;
            var enumerator = self.GetEnumerator();
            while (enumerator.MoveNext())
                c++;
            return c;
        }

        public static T Last<T>(this IEnumerable<T> self)
        {
            bool any = false;
            T tmp = default(T);
            foreach (var item in self)
            {
                tmp = item;
                any = true;
            }
            if (!any) throw new Exception();
            return tmp;
        }

        public static T LastOrDefault<T>(this IEnumerable<T> self)
        {
            T tmp = default(T);
            foreach (var item in self)
                tmp = item;
            return tmp;
        }

        public static T First<T>(this IEnumerable<T> self)
        {
            foreach (var item in self)
                return item;
            throw new InvalidOperationException();
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> self, Predicate<T> where)
        {
            foreach (var item in self)
            if (where(item))
                return item;
            return default(T);
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> self)
        {
            foreach (var item in self)
                return item;
            return default(T);
        }

        public static bool Any<T>(this IEnumerable<T> self)
        {
            return self.GetEnumerator().MoveNext();
        }

        public static bool Any<T>(this IEnumerable<T> self, Predicate<T> where)
        {
            foreach (var item in self)
            {
                if (where(item))
                    return true;
            }
            return false;
        }

        public static bool All<T>(this IEnumerable<T> self, Predicate<T> where)
        {
            foreach (var item in self)
            {
                if (!where(item))
                    return false;
            }
            return true;
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> self)
        {
            var res = new List<T>();
            foreach (var item in self)
            {
                if (!res.Contains(item))
                    res.Add(item);
            }

            return res;
        }

        public static IEnumerable<T> Skip<T>(this IEnumerable<T> self, int count)
        {
            var list = ToList(self);

            if (count >= list.Count)
                throw new ArgumentOutOfRangeException("count");

            var res = new List<T>();
            for (int i = count; i < list.Count; i++)
                res.Add(list[i]);

            return res;
        }

        public static IEnumerable<T> Take<T>(this IEnumerable<T> self, int count)
        {
            var list = ToList(self);

            if (count > list.Count)
                throw new ArgumentOutOfRangeException("count");

            var res = new List<T>();
            for (int i = 0; i < count; i++)
                res.Add(list[i]);

            return res;
        }

        public static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null)
                throw new ArgumentNullException("first");

            if (second == null)
                throw new ArgumentNullException("second");

            if (object.ReferenceEquals(first, second))
                return true;

            using (var it1 = first.GetEnumerator())
            using (var it2 = second.GetEnumerator())
            {
                while (true)
                {
                    var next1 = it1.MoveNext();
                    var next2 = it2.MoveNext();
                    if (!next1 && !next2) return true;
                    if (next1 != next2) return false;
                    if (!it1.Current.Equals(it2.Current)) return false;
                }
            }
            throw new Exception("Impossible");
        }
    }

}
