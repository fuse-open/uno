using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Build.Targets;
using Uno.Logging;

namespace Uno.Build
{
    public static class BuildTargets
    {
        public static readonly BuildTarget Default = new DotNetBuild();
        public static readonly BuildTarget Package = new PackageBuild();

        public static readonly BuildTarget[] All =
        {
            new AndroidBuild(),
            new NativeBuild(),
            new iOSBuild(),
            Default,
            new DocsBuild(),
            new MetadataBuild(),
            new PInvokeBuild(),
            Package,
        };

        public static IEnumerable<BuildTarget> Enumerate(bool experimental = true, bool obsolete = false)
        {
            return All.Where(e => (experimental || !e.IsExperimental) && (obsolete || !e.IsObsolete));
        }

        public static BuildTarget Get(string name, List<string> args = null, string def = null,
                                      bool canReturnNull = false)
        {
            BuildTarget result;
            if (name == null && args != null)
            {
                if (args.Count > 0 && TryGet(args[0], out result) && (
                        string.Compare(args[0], result.Identifier, StringComparison.InvariantCultureIgnoreCase) == 0 ||
                        !Directory.Exists(args[0])
                    ))
                {
                    args.RemoveAt(0);
                    return result;
                }

                name = def ?? (canReturnNull ? null : Default.Identifier);
            }

            if (name != null && TryGet(name, out result))
                return result;

            if (canReturnNull)
                return null;

            throw new ArgumentException(name.Quote() + " is not a valid build target -- see \"uno build --help\" for a list of targets");
        }

        static bool TryGet(string name, out BuildTarget result)
        {
            result = TryGet(name);
            return result != null;
        }

        static BuildTarget TryGet(string name)
        {
            var nameUpper = name.ToUpperInvariant();

            foreach (var t in Enumerate(true, true))
                if (t.Identifier.ToUpperInvariant() == nameUpper)
                    return t;
            foreach (var t in Enumerate(true, true))
            {
                foreach (var n in t.FormerNames)
                {
                    if (n.ToUpperInvariant() == nameUpper)
                    {
                        Log.Default.Warning($"The build target '{n}' is deprecated -- use '{t.Identifier}' to silence this message.");
                        return t;
                    }
                }
            }

            // Fuzzy --target finder
            BuildTarget result = null;
            if (nameUpper.Length > 0)
            {
                foreach (var t in Enumerate())
                {
                    if ((!t.IsExperimental || Log.Default.EnableExperimental) &&
                        t.Identifier.ToUpperInvariant().StartsWith(nameUpper, StringComparison.InvariantCulture))
                    {
                        if (result != null)
                            return null;

                        result = t;
                    }
                }
            }

            return result;
        }
    }
}
