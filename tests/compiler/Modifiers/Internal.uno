namespace A
{
    class Bar
    {
        public Bar()
        {
            var foo = new Outracks.UnoTest.InternalHelpers.IsOhkey();
            var foo1 = new Outracks.UnoTest.InternalHelpers.InternalClass(); // $E3111 Outracks.UnoTest.InternalHelpers does not contain type or namespace 'InternalClass'. Could you be missing a package reference?
        }
    }
}
