class Main
{
    public static void Main()
    {
        double d = 3.2;
        int i1 = d; // $E2047
        int i2 = (int)d;

        object obj = "MyString";
        Main myClass = obj; // $E2047
        var c = (Main)obj;

        var mc = new Main();
        var dc = new DerivedClass();
        dc = mc; // $E2047
        dc = (DerivedClass)mc;
    }
}

class DerivedClass : Main {}