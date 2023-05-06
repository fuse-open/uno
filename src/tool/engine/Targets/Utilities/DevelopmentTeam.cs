using System;
using System.Collections.Generic;

namespace Uno.Build.Targets.Utilities
{
    class DevTeam
    {
        public readonly string Name;
        public readonly string OrganizationName;
        public readonly string OrganizationalUnit; // Cert: OU

        public DevTeam(string name, string organizationName, string organizationalUnit)
        {
            Name = name;
            OrganizationName = organizationName;
            OrganizationalUnit = organizationalUnit;
        }

        public override string ToString()
        {
            return "{ Name: " + Name + ", OrganizationName: " + OrganizationName + ", OrganizationalUnit: " + OrganizationalUnit + " }";
        }
    }

    class DevelopmentTeamExtractorFailure : Exception
    {
    }

    class DevelopmentTeamExtractor
    {
        public IEnumerable<DevTeam> FindAllDevelopmentTeams()
        {
            // FIXME: Can't use Xamarin.Mac so we need to come up with something
            // better... Time to develop native command-line tool in Swift?
            yield break;
        }
    }
}
