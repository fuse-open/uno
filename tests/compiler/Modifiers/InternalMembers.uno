namespace A
{
    class Bar
    {
        public Bar()
        {
            var foo2 = new Outracks.UnoTest.InternalHelpers.InternalMembers();
            foo2.Field = true; // $E4040
            foo2.Property = true; // $E4040
            foo2.InternalMethod(); // $E4040
        }
    }
}
