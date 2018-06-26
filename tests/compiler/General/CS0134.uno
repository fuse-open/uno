class Foo {}

class Main
{
   const Foo test = new Foo();   // $E0059

   const Foo test2 = null; // Should not give error

   //OK
   const Foo test2 = null;
   const string test3 = "test";
}