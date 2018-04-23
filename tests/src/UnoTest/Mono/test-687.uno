namespace Mono.test_687
{
    using Uno;
    
    struct XUnit
    {
        public XUnit(double point)
        {
            this.Point = point;
        }
    
        double Point;
    
        public static implicit operator XUnit(double value)
        {
            XUnit unit;
            unit.Point = value;
            return unit;
        }
    
        public static implicit operator double(XUnit value)
        {
            return value.Point;
        }
    }
    
    struct Unit
    {
        public Unit(double point)
        {
            this.Point = point;
        }
    
        double Point;
    
        public static implicit operator Unit(double value)
        {
            Unit unit;
            unit.Point = value;
            return unit;
        }
    
        public static implicit operator double(Unit value)
        {
            return value.Point;
        }
    }
    
    class Test
    {
        [Uno.Testing.Test] public static void test_687() { Uno.Testing.Assert.AreEqual(0, Main()); }
        public static int Main()
        {
            XUnit xunit = new XUnit();
            Unit unit = new Unit();
            Unit uu = unit + xunit;
            unit += xunit;
            return 0;
        }
    }
}
