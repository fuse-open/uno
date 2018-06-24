using Uno;

namespace UnoTest.General
{
    public partial class PartialClass : IDisposable
    {

    }

    public partial class PartialClass : object
    {
        partial interface IFoo
        {

        }
    }

    public partial class PartialClass
    {
        public void Dispose()
        {
        }
    }

    partial class PartialClass : object
    {
        public partial interface IFoo
        {

        }


    }

    extern(IOS)
    partial class PartialClass
    {
        // iOS specific members here...
    }

    extern(ANDROID)
    partial class PartialClass
    {
        // Android specific members here...
    }
}
