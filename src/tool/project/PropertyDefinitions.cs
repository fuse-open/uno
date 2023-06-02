using System;
using System.Collections.Generic;

namespace Uno.ProjectFormat
{
    public class PropertyDefinitions : Dictionary<string, Tuple<PropertyType, string>>
    {
        public static readonly PropertyDefinitions Items = new()
        {
            {"outputType", PropertyType.String},
            {"buildCondition", PropertyType.String},
            {"buildDirectory", PropertyType.Path, "build"},
            {"cacheDirectory", PropertyType.Path, ".uno"},
            {"outputDirectory", PropertyType.Path, "$(buildDirectory)/@(configuration:toLower)/@(target)"},
            {"rootNamespace", PropertyType.String, "$(qidentifier)"},
            {"version", PropertyType.String, Environment.GetEnvironmentVariable("npm_package_version") ?? "0.1.0"},
            {"versionCode", PropertyType.Integer, 1},
            {"title", PropertyType.String, "$(name)"},
            {"icon", PropertyType.Path},
            {"url", PropertyType.URL},
            {"description", PropertyType.String},
            {"copyright", PropertyType.String, "Copyright (C) " + DateTime.Now.Year + " $(publisher)"},
            {"publisher", PropertyType.String, "[Publisher]"},
            {"isTransitive", PropertyType.Bool, false},
            {"mobile.keepAlive", PropertyType.Bool, false},
            {"mobile.showStatusbar", PropertyType.Bool, true},
            {"mobile.runsInBackground", PropertyType.Bool, true},
            {"mobile.orientations", PropertyType.String, Orientations.Auto},
            {"android.applicationLabel", PropertyType.String, "$(title)"},
            {"android.architectures.debug", PropertyType.String, "arm64-v8a"},
            {"android.architectures.release", PropertyType.String, "armeabi-v7a\narm64-v8a"},
            {"android.defines", PropertyType.String},
            {"android.versionCode", PropertyType.Integer, "$(versionCode)"},
            {"android.versionName", PropertyType.String, "$(version)"},
            {"android.package", PropertyType.String},
            {"android.description", PropertyType.String, "$(description)"},
            {"android.theme", PropertyType.String},
            {"android.icons.ldpi", PropertyType.Path, "$(icon)"},
            {"android.icons.mdpi", PropertyType.Path, "$(icon)"},
            {"android.icons.hdpi", PropertyType.Path, "$(icon)"},
            {"android.icons.xhdpi", PropertyType.Path, "$(icon)"},
            {"android.icons.xxhdpi", PropertyType.Path, "$(icon)"},
            {"android.icons.xxxhdpi", PropertyType.Path, "$(icon)"},
            {"android.key.store", PropertyType.Path},
            {"android.key.alias", PropertyType.String},
            {"android.key.storePassword", PropertyType.String},
            {"android.key.aliasPassword", PropertyType.String},
            {"android.geo.apiKey", PropertyType.String},
            {"android.ndk.platformVersion", PropertyType.Integer},
            {"android.sdk.buildToolsVersion", PropertyType.String},
            {"android.sdk.compileVersion", PropertyType.Integer},
            {"android.sdk.minVersion", PropertyType.Integer},
            {"android.sdk.targetVersion", PropertyType.Integer},
            {"ios.bundleIdentifier", PropertyType.String},
            {"ios.bundleName", PropertyType.String, "$(title)"},
            {"ios.bundleVersion", PropertyType.String, "$(version)"},
            {"ios.bundleShortVersionString", PropertyType.String, "$(version)"},
            {"ios.statusBarHidden", PropertyType.Bool, "!$(mobile.showStatusbar)"},
            {"ios.statusBarStyle", PropertyType.String, "Default"},
            {"ios.defines", PropertyType.String},
            {"ios.deploymentTarget", PropertyType.String, "11.0"},
            {"ios.developmentTeam", PropertyType.String},
            {"ios.icons.iphone_20_2x", PropertyType.Path, "$(icon)"},
            {"ios.icons.iphone_20_3x", PropertyType.Path, "$(icon)"},
            {"ios.icons.iphone_29_2x", PropertyType.Path, "$(icon)"},
            {"ios.icons.iphone_29_3x", PropertyType.Path, "$(icon)"},
            {"ios.icons.iphone_40_2x", PropertyType.Path, "$(icon)"},
            {"ios.icons.iphone_40_3x", PropertyType.Path, "$(icon)"},
            {"ios.icons.iphone_60_2x", PropertyType.Path, "$(icon)"},
            {"ios.icons.iphone_60_3x", PropertyType.Path, "$(icon)"},
            {"ios.icons.ipad_20_1x", PropertyType.Path, "$(icon)"},
            {"ios.icons.ipad_20_2x", PropertyType.Path, "$(icon)"},
            {"ios.icons.ipad_29_1x", PropertyType.Path, "$(icon)"},
            {"ios.icons.ipad_29_2x", PropertyType.Path, "$(icon)"},
            {"ios.icons.ipad_40_1x", PropertyType.Path, "$(icon)"},
            {"ios.icons.ipad_40_2x", PropertyType.Path, "$(icon)"},
            {"ios.icons.ipad_76_1x", PropertyType.Path, "$(icon)"},
            {"ios.icons.ipad_76_2x", PropertyType.Path, "$(icon)"},
            {"ios.icons.ipad_83.5_2x", PropertyType.Path, "$(icon)"},
            {"ios.icons.ios_marketing_1024_1x", PropertyType.Path, "$(icon)"},
            {"ios.launchScreen.contentMode", PropertyType.String, "scaleAspectFit"},
            {"ios.launchScreen.image", PropertyType.Path, "$(ios.icons.iphone_60_3x)"},
            {"ios.launchScreen.width", PropertyType.Integer, 60},
            {"ios.launchScreen.height", PropertyType.Integer, 60},
            {"ios.launchScreen.backgroundColor.red", PropertyType.String},
            {"ios.launchScreen.backgroundColor.green", PropertyType.String},
            {"ios.launchScreen.backgroundColor.blue", PropertyType.String},
            {"ios.launchScreen.backgroundColor.alpha", PropertyType.String},
            {"dotnet.defines", PropertyType.String},
            {"native.defines", PropertyType.String},
            {"mac.icon", PropertyType.Path},
            {"windows.icon", PropertyType.Path},
        };

        public static readonly Dictionary<string, string> RenamedItems = new()
        {
            {"defaultNamespace", "rootNamespace"},
            {"buildDir", "buildDirectory"},
            {"cacheDir", "cacheDirectory"},
            {"versionCount", "versionCode"},
            {"ios.icons.ios-marketing_1024_1x", "ios.icons.ios_marketing_1024_1x"},
        };

        public void Add(string property, PropertyType type, object defaultValue = null)
        {
            Add(property, Tuple.Create(type, defaultValue is bool
                    ? defaultValue.ToString().ToLower() :
                defaultValue?.ToString()));
        }
    }
}
