namespace Mono.test_xml_069
{
    // Compiler options: -doc:xml-069.xml
    
    using Uno;
    
    namespace XmlComments
    {
        /// <summary/>
        class Program
        {
            /// <summary/>
            private enum MyEnum
            {
                /// <summary>The first entry</summary>
                One,
            }
    
            /// <summary>
            /// <see cref="MyEnum.One"/>
            /// <see cref="Program.MyEnum.One"/>
            /// <see cref="XmlComments.Program.MyEnum.One"/>
            /// <see cref="F:XmlComments.Program.MyEnum.One"/>
            /// </summary>
            [Uno.Testing.Test] public static void test_xml_069() { Main(new string[0]); }
        public static void Main(string[] args)
            {
            }
        }
    }
}
