namespace Uno.Compiler.Backends.UnoDoc
{
    public static class ExportConstants
    {
        internal const string RootNamespaceName = "-";
        internal const string RootDataTypeId = "Uno.Object";
        internal const string VoidTypeName = "void";
        internal const string UxConstructorAttributeName = "Uno.UX.UXConstructorAttribute";
        internal const string UxAttachedPropertySetterAttributeName = "Uno.UX.UXAttachedPropertySetterAttribute";
        internal const string UxAttachedEventAdderAttributeName = "Uno.UX.UXAttachedEventAdderAttribute";
        internal const string UxContentAttributeName = "Uno.UX.UXContentAttribute";
        internal const string UxPrimaryAttributeName = "Uno.UX.UXPrimaryAttribute";
        internal const string UxComponentsAttributeName = "Uno.UX.UXComponentsAttribute";
        internal const string GenericListInterfaceTypeUri = "uno/collections/ilist_1";

        internal static readonly string[] AllowedPackagePrefixes =
        {
            "Experimental",
            "Fuse",
            "FuseJS",
            "FuseCore",
            "Uno",
            "UnoCore"
        };
    }
}