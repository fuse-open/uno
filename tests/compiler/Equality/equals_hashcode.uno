namespace equality
{

    class LacksGetHashCode //$w equality.LacksGetHashCode overrides object.Equals(object o) but does not override object.GetHashCode()
    {

        public override bool Equals(object obj)
        {
            return false;
        }

        public new int GetHashCode()
        {
            return 0;
        }

    }

    class OverridesBoth
    {

        public override bool Equals(object obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }

    }

}
