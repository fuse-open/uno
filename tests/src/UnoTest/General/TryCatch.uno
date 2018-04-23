using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class TryCatch
    {
        bool foo = true;

        int method1()
        {
            try
            {
                if (foo)
                    return 1;

                throw new Exception();
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        int method2()
        {
            try
            {
                if (foo)
                    throw new Exception();

                return 1;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        void method3()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                try
                {
                    throw new Exception();
                }
                catch (Exception f)
                {
                    return;
                }
            }
        }

        void method4()
        {
            while (true)
            {
                try
                {
                    break;
                }
                catch (Exception e)
                {
                    continue;
                }
            }

            for (int i = 0;;i++)
            {
                try
                {
                    if (i < 5)
                        continue;

                    throw new Exception();
                }
                catch (Exception e)
                {
                    break;
                }
            }

            do
            {
                try
                {
                    return;
                }
                catch (Exception e)
                {
                }
            }
            while (false);
        }

        // Please uncomment method when #289 is fixed.
        //void method5()
        //{
        //    try
        //    {
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        [Test]
        public void Run()
        {
            Assert.AreEqual(1, method1());
            Assert.AreEqual(-1, method2());

            bool thrown = false;
            try
            {
                int a = 5;
                switch (a)
                {
                    case 5: throw new Exception();
                }
            }
            catch (Exception e)
            {
                thrown = true;
            }
            Assert.IsTrue(thrown);

            thrown = false;
            try
            {
                bool b = false;
                if (b) throw new Exception("No");
                if (!b) throw new Exception("Yes");
                throw new Exception("No way, brah");
            }
            catch (Exception e)
            {
                if (e.Message == "Yes") thrown = true;
            }
            Assert.IsTrue(thrown);

            thrown = false;
            try
            {
                bool b = false;
                while (b)
                {
                    throw new Exception("Not this one");
                }
                do
                {
                    throw new Exception("This is the one");
                } while(b);

                throw new Exception("Not this one either");
            }
            catch (Exception e)
            {
                if (e.Message == "This is the one") thrown = true;
            }
            Assert.IsTrue(thrown);

            thrown = false;
            try
            {
                bool b = true;
                while (b)
                {
                    throw new Exception("Yeah, this looks good");
                }

                b = !b;
                do
                {
                    throw new Exception("Uhmm no");
                }
                while (b);

                throw new Exception("C'mon, man");
            }
            catch (Exception e)
            {
                if (e.Message == "Yeah, this looks good") thrown = true;
            }
            Assert.IsTrue(thrown);
        }
    }
}
