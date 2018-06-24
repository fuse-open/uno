using Uno;

class Main
{
    public void TryWithBreakInsideFinally()
    {
        int f = 0;
        for (int i = 0; i < 10; ++i)
        {
            try
            {
            }
            finally
            {
                try
                {
                    break; // $E0000
                }
                finally
                {
                    ++f;
                }
            }
        }
    }

    public void WithBreakInFinally()
    {
        int f = 0;
        for (int i = 0; i < 10; ++i)
        {
            try
            {
            }
            catch (Exception e)
            {
            }
            finally
            {
                ++f;
                break; // $E0000
            }
        }
    }
}
