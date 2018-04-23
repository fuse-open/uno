using Uno.Compiler.ExportTargetInterop;
using Uno.Text;

namespace Uno
{
    internal interface ITuple
    {
        void AppendItems(StringBuilder sb);
    }

    [extern(DOTNET) DotNetType("System.Tuple")]
    public static class Tuple
    {
        public static Tuple<T1> Create<T1>(T1 item1) { return new Tuple<T1>(item1); }
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) { return new Tuple<T1, T2>(item1, item2); }
        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) { return new Tuple<T1, T2, T3>(item1, item2, item3); }
        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) { return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4); }
        public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) { return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5); }
        public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) { return new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6); }
        public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) { return new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7); }
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8) { return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8>(item8)); }
    }

    [extern(DOTNET) DotNetType("System.Tuple`1")]
    public class Tuple<T1> : ITuple
    {
        public T1 Item1 { get; private set; }

        public Tuple(T1 item1)
        {
            Item1 = item1;
        }

        public override bool Equals(object other)
        {
            var tother = other as Tuple<T1>;
            return tother != null
                ? (Item1.Equals(tother.Item1))
                : false;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            ((ITuple) this).AppendItems(sb);
            sb.Append(")");
            return sb.ToString();
        }

        void ITuple.AppendItems(StringBuilder sb)
        {
            sb.Append(Item1.ToString());
        }
    }

    [extern(DOTNET) DotNetType("System.Tuple`2")]
    public class Tuple<T1, T2> : ITuple
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }

        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override bool Equals(object other)
        {
            var tother = other as Tuple<T1, T2>;
            return tother != null
                ? (Item1.Equals(tother.Item1) && Item2.Equals(tother.Item2))
                : false;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            ((ITuple) this).AppendItems(sb);
            sb.Append(")");
            return sb.ToString();
        }

        void ITuple.AppendItems(StringBuilder sb)
        {
            sb.Append(Item1.ToString());
            sb.Append(", ");
            sb.Append(Item2.ToString());
        }
    }

    [extern(DOTNET) DotNetType("System.Tuple`3")]
    public class Tuple<T1, T2, T3> : ITuple
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }

        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public override bool Equals(object other)
        {
            var tother = other as Tuple<T1, T2, T3>;
            return tother != null
                ? (Item1.Equals(tother.Item1) && Item2.Equals(tother.Item2) && Item3.Equals(tother.Item3))
                : false;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode() ^ Item3.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            ((ITuple) this).AppendItems(sb);
            sb.Append(")");
            return sb.ToString();
        }

        void ITuple.AppendItems(StringBuilder sb)
        {
            sb.Append(Item1.ToString());
            sb.Append(", ");
            sb.Append(Item2.ToString());
            sb.Append(", ");
            sb.Append(Item3.ToString());
        }
    }

    [extern(DOTNET) DotNetType("System.Tuple`4")]
    public class Tuple<T1, T2, T3, T4> : ITuple
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        public override bool Equals(object other)
        {
            var tother = other as Tuple<T1, T2, T3, T4>;
            return tother != null
                ? (Item1.Equals(tother.Item1) && Item2.Equals(tother.Item2) && Item3.Equals(tother.Item3) && Item4.Equals(tother.Item4))
                : false;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode() ^ Item3.GetHashCode() ^ Item4.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            ((ITuple) this).AppendItems(sb);
            sb.Append(")");
            return sb.ToString();
        }

        void ITuple.AppendItems(StringBuilder sb)
        {
            sb.Append(Item1.ToString());
            sb.Append(", ");
            sb.Append(Item2.ToString());
            sb.Append(", ");
            sb.Append(Item3.ToString());
            sb.Append(", ");
            sb.Append(Item4.ToString());
        }
    }

    [extern(DOTNET) DotNetType("System.Tuple`5")]
    public class Tuple<T1, T2, T3, T4, T5> : ITuple
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }
        public T5 Item5 { get; private set; }

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
        }

        public override bool Equals(object other)
        {
            var tother = other as Tuple<T1, T2, T3, T4, T5>;
            return tother != null
                ? (Item1.Equals(tother.Item1) && Item2.Equals(tother.Item2) && Item3.Equals(tother.Item3) && Item4.Equals(tother.Item4) && Item5.Equals(tother.Item5))
                : false;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode() ^ Item3.GetHashCode() ^ Item4.GetHashCode() ^ Item5.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            ((ITuple) this).AppendItems(sb);
            sb.Append(")");
            return sb.ToString();
        }

        void ITuple.AppendItems(StringBuilder sb)
        {
            sb.Append(Item1.ToString());
            sb.Append(", ");
            sb.Append(Item2.ToString());
            sb.Append(", ");
            sb.Append(Item3.ToString());
            sb.Append(", ");
            sb.Append(Item4.ToString());
            sb.Append(", ");
            sb.Append(Item5.ToString());
        }
    }

    [extern(DOTNET) DotNetType("System.Tuple`6")]
    public class Tuple<T1, T2, T3, T4, T5, T6> : ITuple
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }
        public T5 Item5 { get; private set; }
        public T6 Item6 { get; private set; }

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
        }

        public override bool Equals(object other)
        {
            var tother = other as Tuple<T1, T2, T3, T4, T5, T6>;
            return tother != null
                ? (Item1.Equals(tother.Item1) && Item2.Equals(tother.Item2) && Item3.Equals(tother.Item3) && Item4.Equals(tother.Item4) && Item5.Equals(tother.Item5) && Item6.Equals(tother.Item6))
                : false;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode() ^ Item3.GetHashCode() ^ Item4.GetHashCode() ^ Item5.GetHashCode() ^ Item6.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            ((ITuple) this).AppendItems(sb);
            sb.Append(")");
            return sb.ToString();
        }

        void ITuple.AppendItems(StringBuilder sb)
        {
            sb.Append(Item1.ToString());
            sb.Append(", ");
            sb.Append(Item2.ToString());
            sb.Append(", ");
            sb.Append(Item3.ToString());
            sb.Append(", ");
            sb.Append(Item4.ToString());
            sb.Append(", ");
            sb.Append(Item5.ToString());
            sb.Append(", ");
            sb.Append(Item6.ToString());
        }
    }

    [extern(DOTNET) DotNetType("System.Tuple`7")]
    public class Tuple<T1, T2, T3, T4, T5, T6, T7> : ITuple
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }
        public T5 Item5 { get; private set; }
        public T6 Item6 { get; private set; }
        public T7 Item7 { get; private set; }

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
        }

        public override bool Equals(object other)
        {
            var tother = other as Tuple<T1, T2, T3, T4, T5, T6, T7>;
            return tother != null
                ? (Item1.Equals(tother.Item1) && Item2.Equals(tother.Item2) && Item3.Equals(tother.Item3) && Item4.Equals(tother.Item4) && Item5.Equals(tother.Item5) && Item6.Equals(tother.Item6) && Item7.Equals(tother.Item7))
                : false;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode() ^ Item3.GetHashCode() ^ Item4.GetHashCode() ^ Item5.GetHashCode() ^ Item6.GetHashCode() ^ Item7.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            ((ITuple) this).AppendItems(sb);
            sb.Append(")");
            return sb.ToString();
        }

        void ITuple.AppendItems(StringBuilder sb)
        {
            sb.Append(Item1.ToString());
            sb.Append(", ");
            sb.Append(Item2.ToString());
            sb.Append(", ");
            sb.Append(Item3.ToString());
            sb.Append(", ");
            sb.Append(Item4.ToString());
            sb.Append(", ");
            sb.Append(Item5.ToString());
            sb.Append(", ");
            sb.Append(Item6.ToString());
            sb.Append(", ");
            sb.Append(Item7.ToString());
        }
    }

    [extern(DOTNET) DotNetType("System.Tuple`8")]
    public class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> : ITuple
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }
        public T5 Item5 { get; private set; }
        public T6 Item6 { get; private set; }
        public T7 Item7 { get; private set; }
        public TRest Rest { get; private set; }

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest item8)
        {
            if (!(item8 is ITuple))
                throw new ArgumentException("The last element of an eight element tuple must be a Tuple.");

            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
            Item6 = item6;
            Item7 = item7;
            Rest = item8;
        }

        public override bool Equals(object other)
        {
            var tother = other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>;
            return tother != null
                ? (Item1.Equals(tother.Item1) && Item2.Equals(tother.Item2) && Item3.Equals(tother.Item3) && Item4.Equals(tother.Item4) && Item5.Equals(tother.Item5) && Item6.Equals(tother.Item6) && Item7.Equals(tother.Item7) && Rest.Equals(tother.Rest))
                : false;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode() ^ Item3.GetHashCode() ^ Item4.GetHashCode() ^ Item5.GetHashCode() ^ Item6.GetHashCode() ^ Item7.GetHashCode() ^ Rest.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            ((ITuple) this).AppendItems(sb);
            sb.Append(")");
            return sb.ToString();
        }

        void ITuple.AppendItems(StringBuilder sb)
        {
            sb.Append(Item1.ToString());
            sb.Append(", ");
            sb.Append(Item2.ToString());
            sb.Append(", ");
            sb.Append(Item3.ToString());
            sb.Append(", ");
            sb.Append(Item4.ToString());
            sb.Append(", ");
            sb.Append(Item5.ToString());
            sb.Append(", ");
            sb.Append(Item6.ToString());
            sb.Append(", ");
            sb.Append(Item7.ToString());
            sb.Append(", ");
            ((ITuple) Rest).AppendItems(sb);
        }
    }
}
