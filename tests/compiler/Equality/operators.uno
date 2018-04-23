namespace equality
{
    class LacksEquals // $w equality.LacksEquals defines operator == or operator != but does not override object.Equals(object o)
    {
        public static bool operator ==(LacksEquals o1, LacksEquals o2)
        {
            return true;
        }

        public static bool operator !=(LacksEquals o1, LacksEquals o2)
        {
            return false;
        }

        public new bool Equals(object obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    class LacksGetHashCode2 //$w equality.LacksGetHashCode2 defines operator == or operator != but does not override object.GetHashCode() $w equality.LacksGetHashCode2 overrides object.Equals(object o) but does not override object.GetHashCode()
    {
        public static bool operator ==(LacksGetHashCode2 o1, LacksGetHashCode2 o2)
        {
            return true;
        }

        public static bool operator !=(LacksGetHashCode2 o1, LacksGetHashCode2 o2)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            return false;
        }

        public new int GetHashCode()
        {
            return 0;
        }

    }

    class HasBoth
    {
        public static bool operator ==(HasBoth o1, HasBoth o2)
        {
            return true;
        }

        public static bool operator !=(HasBoth o1, HasBoth o2)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
