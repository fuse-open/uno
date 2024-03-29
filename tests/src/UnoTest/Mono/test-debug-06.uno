namespace Mono.test_debug_06
{
    using Uno;

    class Test
    {
        [Uno.Testing.Test] public static void test_debug_06() { Main(); }
        public static void Main()
        {
        }

        void TryCatch_1 ()
        {
            try
            {
            }
            catch
            {
            }
        }

        void TryCatch_2 ()
        {
            try
            {
            }
            catch (Exception e)
            {
                e = null;
            }
        }

        void TryCatch_3 ()
        {
            try
            {
            }
            catch (ArgumentException e)
            {
                e = null;
            }
            catch (Exception e)
            {
                return;
            }
        }

        void TryFinally_1 ()
        {
            try
            {
            }
            finally
            {
            }
        }

        void TryFinally_2 ()
        {
            try
            {
            }
            catch (Exception e)
            {
                e = null;
            }
            finally
            {
            }
        }
    }
}
