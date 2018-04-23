using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class Switch
    {
        public static string Example(int i, int j)
        {
            string retval = "";
            switch (i)
            {
                default:
                    switch (j)
                    {
                        default: retval = "blah";
                        break;
                    }
                break;
            }
            return retval;
        }

        [Test]
        public void Run()
        {
            int d = 8;
            switch (d)
            {
                case 0:
                    Assert.IsTrue(false);
                    break;

                case 1:
                case 2:
                case 3:
                case 4:
                    Assert.IsTrue(false);
                    break;

                case 5:
                case 6:
                case 7:
                    Assert.IsTrue(false);
                    break;

                case 8:
                    Assert.IsTrue(true);
                    break;

                default:
                    Assert.IsTrue(false);
                    break;
            }

            char e = 'a';
            switch (e)
            {
                case 'A': Assert.IsTrue(false); break;
                case 'E': Assert.IsTrue(false); break;
                case 'D': Assert.IsTrue(false); break;
                case 'C': Assert.IsTrue(false); break;
                case 'B': Assert.IsTrue(false); break;
                case 'a': Assert.IsTrue(true); break;
                //case 'A': Assert.IsTrue(false); break;
                case 'P':
                case '"':
                case ' ': Assert.IsTrue(false); break;
                default: Assert.IsTrue(false); break;
            }

            Example(0,0);

            for (var i = 0; i != i; i++)
            {
                switch (i)
                {
                    case 4:
                        break;

                    default:
                        continue;
                }

                break;
            }
        }
    }
}
