using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Uno.Configuration;
using Uno.Diagnostics;

namespace Uno.Build.Targets.Utilities
{
    class DevTeam
    {
        public string name;
        public string organizationName;
        public string organizationalUnit; // Cert: OU

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    class DevelopmentTeamExtractorFailure : Exception
    {
    }

    class DevelopmentTeamExtractor
    {
        public IEnumerable<DevTeam> FindAllDevelopmentTeams(Shell shell)
        {
            try
            {
                var json = shell.GetOutput(
                    Path.Combine(UnoConfig.Current.GetNodeModuleDirectory("xcode-devteams"), "bin", "xcode-devteams"),
                    "",
                    RunFlags.NoOutput);

                return JsonConvert.DeserializeObject<DevTeam[]>(json);
            }
            catch (Exception e)
            {
                shell.Log.Trace(e);
                throw new DevelopmentTeamExtractorFailure();
            }
        }
    }
}
