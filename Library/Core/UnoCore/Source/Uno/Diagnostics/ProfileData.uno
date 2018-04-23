using Uno.Collections;

namespace Uno.Diagnostics
{
    [Obsolete]
    public class ProfileData
    {
        public List<string> FunctionIds = new List<string>();
        public List<ProfileEvent> ProfileEvents = new List<ProfileEvent>();
        public IdMap<string> TypeMap = new IdMap<string>(true);
    }
}
