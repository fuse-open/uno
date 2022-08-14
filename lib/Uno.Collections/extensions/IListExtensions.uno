
namespace Uno.Collections
{
    public static class IListExtensions
    {
        public static void AddRange<T>(this IList<T> self, IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var item in collection)
                self.Add(item);
        }

        public static T Find<T>(this IList<T> self, Predicate<T> where)
        {
            return self.FirstOrDefault(where);
        }

        public static IList<T> FindAll<T>(this IList<T> self, Predicate<T> where)
        {
            return self.Where(where).ToList();
        }

        public static void InsertRange<T>(this IList<T> self, int index, IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (index > self.Count)
                throw new ArgumentOutOfRangeException("index");

            foreach (var item in collection)
            {
                self.Insert(index, item);
                index++;
            }
        }

        public static T Last<T>(this IList<T> self)
        {
            var i = self.Count - 1;
            return self[i];
        }

        public static T LastOrDefault<T>(this IList<T> self)
        {
            if (self.Count == 0)
                return default(T);

            return self.Last();
        }

        public static T RemoveLast<T>(this IList<T> self)
        {
            var i = self.Count - 1;
            var t = self[i];
            self.RemoveAt(i);

            return t;
        }

        public static IList<T> Reverse<T>(this IList<T> self)
        {
            var res = new List<T>();
            for (int i = self.Count - 1; i >= 0; i--)
                res.Add(self[i]);

            return res;
        }
    }
}