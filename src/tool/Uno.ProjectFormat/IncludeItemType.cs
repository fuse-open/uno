namespace Uno.ProjectFormat
{
    public enum IncludeItemType
    {
        Glob = 0,
        Folder = 1,
        File = 2,
        Stuff = 3,
        Source = 4,
        Java = 5,
        CSource = 6,
        CHeader = 7,
        Extensions = 8,
        UX = 9,
        Bundle = 10,
        ObjCSource = 11,
        ObjCHeader = 12,
        Swift = 13,
        FuseJS = 14,

        // Deprecated
        StuffFile = 103,
        SourceFile = 104,
        JavaFile = 105,
        CSourceFile = 106,
        CHeaderFile = 107,
        ExtensionsFile = 108,
        UXFile = 109,
    }
}