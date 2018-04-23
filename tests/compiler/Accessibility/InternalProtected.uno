namespace InternalProtected
{
    internal class InternalClass
    {
    }

    public class ProtectedUse
    {
        protected InternalClass ProtectedInternalClassField = new InternalClass(); // $E4128

        protected void ProtectedInternalClassMethod(InternalClass foo) // $E4128
        {
        }
    }

    public class PublicUse
    {
        public InternalClass ProtectedInternalClassField = new InternalClass(); // $E4128

        public void ProtectedInternalClassMethod(InternalClass foo) // $E4128
        {
        }
    }

    public class ProtectedInternalUse
    {
        protected internal InternalClass ProtectedInternalClassField = new InternalClass(); // $E4128

        protected internal void ProtectedInternalClassMethod(InternalClass foo) // $E4128
        {
        }
    }

    public class PrivateUse
    {
        private InternalClass ProtectedInternalClassField = new InternalClass();

        private void ProtectedInternalClassMethod(InternalClass foo)
        {
        }
    }

    public class InternalUse
    {
        internal InternalClass ProtectedInternalClassField = new InternalClass();

        internal void ProtectedInternalClassMethod(InternalClass foo)
        {
        }
    }
}
