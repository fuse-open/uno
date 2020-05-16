using System.Collections.Generic;
using System.Linq;

namespace Uno.UX.Markup
{
    public static class Configuration
    {
        public const string UXNamespace = "http://schemas.fusetools.com/ux";

        public const string DependencyNamespace = "http://schemas.fusetools.com/dep";

        public const string DefaultNamespace = "Fuse, Fuse.Reactive, Fuse.Selection, Fuse.Animations, Fuse.Drawing, Fuse.Entities, Fuse.Controls, Fuse.Layouts, "
            + "Fuse.Elements, Fuse.Effects, Fuse.Triggers, Fuse.Navigation, Fuse.Triggers.Actions, Fuse.Gestures, Fuse.Resources, "
            + "Fuse.Native, Fuse.Physics, Fuse.Vibration, Fuse.Motion, Fuse.Testing, Uno.UX";

        public static IEnumerable<string> DefaultNamespaces
        {
            get { return DefaultNamespace.Split(',').Select(x => x.Trim()); }
        }
    }
}
