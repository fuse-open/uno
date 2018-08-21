using Uno;
using Uno.Diagnostics;
using Uno.Testing;

namespace Diagnostics.Test
{
    public class ProfileTest
    {
        [Test]
        [Ignore("Disabled, but keep it around for manual testing")]
        public void ManualIntegrationTest()
        {
            Profile.AddFunctionId("foo");
            Profile.AddFunctionId("bar");
            Profile.Enter(1);
            Profile.Enter(2);
            Profile.Exit();
            Profile.Enter(2);
            Profile.Exit();
            Profile.Exit();
            Profile.Serialize("MyApp");
        }
    }
}
