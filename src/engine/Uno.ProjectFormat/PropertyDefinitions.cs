using System;
using System.Collections.Generic;

namespace Uno.ProjectFormat
{
    public class PropertyDefinitions : Dictionary<string, Tuple<PropertyType, string>>
    {
        public static readonly PropertyDefinitions Items = new PropertyDefinitions
        {
            {"BuildCondition", PropertyType.String},
            {"BuildDirectory", PropertyType.Path, "build"},
            {"CacheDirectory", PropertyType.Path, ".uno"},
            {"OutputDirectory", PropertyType.Path, "$(BuildDirectory)/@(Target)/@(Configuration)"},
            {"RootNamespace", PropertyType.String, "$(QIdentifier)"},
            {"Version", PropertyType.String, Environment.GetEnvironmentVariable("npm_package_version") ?? "0.1.0"},
            {"VersionCode", PropertyType.Integer, 1},
            {"Title", PropertyType.String, "$(Name)"},
            {"Icon", PropertyType.Path},
            {"URL", PropertyType.URL},
            {"Description", PropertyType.String},
            {"Copyright", PropertyType.String, "Copyright (C) " + DateTime.Now.Year + " $(Publisher)"},
            {"Publisher", PropertyType.String, "[Publisher]"},
            {"UnoCoreReference", PropertyType.Bool, true},
            {"IsTransitive", PropertyType.Bool, false},
            {"Mobile.KeepAlive", PropertyType.Bool, false},
            {"Mobile.ShowStatusbar", PropertyType.Bool, true},
            {"Mobile.RunsInBackground", PropertyType.Bool, true},
            {"Mobile.Orientations", PropertyType.String, Orientations.Auto},
            {"Android.ApplicationLabel", PropertyType.String, "$(Title)"},
            {"Android.Architectures.Debug", PropertyType.String, "armeabi-v7a"},
            {"Android.Architectures.Release", PropertyType.String, "armeabi-v7a\narm64-v8a"},
            {"Android.Defines", PropertyType.String},
            {"Android.VersionCode", PropertyType.Integer, "$(VersionCode)"},
            {"Android.VersionName", PropertyType.String, "$(Version)"},
            {"Android.Package", PropertyType.String},
            {"Android.Description", PropertyType.String, "$(Description)"},
            {"Android.Theme", PropertyType.String},
            {"Android.Icons.LDPI", PropertyType.Path, "$(Icon)"},
            {"Android.Icons.MDPI", PropertyType.Path, "$(Icon)"},
            {"Android.Icons.HDPI", PropertyType.Path, "$(Icon)"},
            {"Android.Icons.XHDPI", PropertyType.Path, "$(Icon)"},
            {"Android.Icons.XXHDPI", PropertyType.Path, "$(Icon)"},
            {"Android.Icons.XXXHDPI", PropertyType.Path, "$(Icon)"},
            {"Android.Key.Store", PropertyType.Path},
            {"Android.Key.Alias", PropertyType.String},
            {"Android.Key.StorePassword", PropertyType.String},
            {"Android.Key.AliasPassword", PropertyType.String},
            {"Android.Geo.ApiKey", PropertyType.String},
            {"Android.NDK.PlatformVersion", PropertyType.Integer},
            {"Android.SDK.BuildToolsVersion", PropertyType.String},
            {"Android.SDK.CompileVersion", PropertyType.Integer},
            {"Android.SDK.MinVersion", PropertyType.Integer},
            {"Android.SDK.TargetVersion", PropertyType.Integer},
            {"iOS.BundleIdentifier", PropertyType.String},
            {"iOS.BundleName", PropertyType.String, "$(Title)"},
            {"iOS.BundleVersion", PropertyType.String, "$(Version)"},
            {"iOS.Defines", PropertyType.String},
            {"iOS.DeploymentTarget", PropertyType.String, "9.0"},
            {"iOS.DevelopmentTeam", PropertyType.String},
            {"iOS.Icons.iPhone_20_2x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPhone_20_3x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPhone_29_2x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPhone_29_3x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPhone_40_2x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPhone_40_3x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPhone_60_2x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPhone_60_3x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPad_20_1x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPad_20_2x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPad_29_1x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPad_29_2x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPad_40_1x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPad_40_2x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPad_76_1x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPad_76_2x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iPad_83.5_2x", PropertyType.Path, "$(Icon)"},
            {"iOS.Icons.iOS-Marketing_1024_1x", PropertyType.Path, "$(Icon)"},
            {"iOS.LaunchImages.iPhone_Portrait_iPhoneX_3x", PropertyType.Path},
            {"iOS.LaunchImages.iPhone_Landscape_iPhoneX_3x", PropertyType.Path},
            {"iOS.LaunchImages.iPhone_Portrait_2x", PropertyType.Path},
            {"iOS.LaunchImages.iPhone_Portrait_R4", PropertyType.Path},
            {"iOS.LaunchImages.iPhone_Portrait_R47", PropertyType.Path},
            {"iOS.LaunchImages.iPhone_Portrait_R55", PropertyType.Path},
            {"iOS.LaunchImages.iPhone_Landscape_R55", PropertyType.Path},
            {"iOS.LaunchImages.iPad_Portrait_1x", PropertyType.Path},
            {"iOS.LaunchImages.iPad_Portrait_2x", PropertyType.Path},
            {"iOS.LaunchImages.iPad_Landscape_1x", PropertyType.Path},
            {"iOS.LaunchImages.iPad_Landscape_2x", PropertyType.Path},
            {"DotNet.Defines", PropertyType.String},
            {"Native.Defines", PropertyType.String},
            {"Mac.Icon", PropertyType.Path},
            {"Windows.Icon", PropertyType.Path},
        };

        public static readonly Dictionary<string, string> RenamedItems = new Dictionary<string, string>
        {
            {"DefaultNamespace", "RootNamespace"},
            {"BuildDir", "BuildDirectory"},
            {"CacheDir", "CacheDirectory"},
            {"VersionCount", "VersionCode"},
            {"ReferenceUnoCore", "UnoCoreReference"},
        };

        public void Add(string property, PropertyType type, object defaultValue = null)
        {
            Add(property, Tuple.Create(type, defaultValue is bool
                    ? defaultValue.ToString().ToLower() :
                defaultValue?.ToString()));
        }
    }
}
