class Bar {}
{
   public virtual int Property // $E Expected type or block following 'virtual' -- found 'int' (Int)
   {
      set {}
   }
}

public class Foo : Bar
{
   public override int Property
   {

      get { return 0; }
      set {}   // OK
   }
}
