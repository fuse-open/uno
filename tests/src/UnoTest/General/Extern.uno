using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace UnoTest.General
{
    extern(CPLUSPLUS)
    class Extern
    {
        [Test]
        void Main()
        {
            int i = 0;
            var a = [Flags] extern<int>(i) "$0";
            var b = [Flags] extern<int> "0";
            [Flags] extern(i) "$0";
            [Flags] "0";
            [Flags] extern(i)
            @{
                $0;
            @}
            [Flags] extern
            @{
            @}
            [Flags]
            @{
            @}

            // Access struct member #250
            Bar bar;
            extern(bar) "@{$0.String}";
            extern(bar) "@{Bar:Of($0).String}";
            extern(bar) "@{Bar:Of($0).String:Get()}";
            extern(bar) "@{Bar:Of($0).StringProp:Get()}";
        }

        struct Bar
        {
            public string String;
            public string StringProp { get { return String; } }
        }

        [Test]
        [Require("Source.Include", "thread")]
        static void Cpp14()
        @{
            // Standard user-defined literals were introduced in C++14.
            using namespace std::chrono_literals;
            std::this_thread::sleep_for(1ms);
        @}
    }
}
