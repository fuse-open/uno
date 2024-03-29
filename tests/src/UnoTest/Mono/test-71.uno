namespace Mono.test_71
{
    //
    // struct with a constructor
    //
    using Uno;

    class X {

        [Uno.Testing.Test] public static void test_71() { Main(); }
        public static void Main()
        {
            MethodSignature ms = new MethodSignature ("hello", null, null);

            Console.WriteLine ("About to look for: " + ms.Name);
        }
    }

        struct MethodSignature {
            public string Name;
            public Type RetType;
            public Type [] Parameters;

            public MethodSignature (string name, Type ret_type, Type [] parameters)
            {
                Name = name;
                RetType = ret_type;
                Parameters = parameters;
            }
        }
}
