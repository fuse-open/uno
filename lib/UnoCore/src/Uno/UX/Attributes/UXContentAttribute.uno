using Uno.Compiler.ExportTargetInterop;

namespace Uno.UX
{
    public sealed class UXContentAttribute: Attribute {}

    public sealed class UXVerbatimAttribute: Attribute {}

    /** When used to decorate a property of UX expression type, the expression is in data scope. 
    
        This is intended for parsing of `Key`-paths, of the form <DataBinding Key="userName" Target="..."`
    */
    public sealed class UXDataScopeAttribute: Attribute {}

    /** When used to decorate a property of a UX expression type, identifiers are interpreted
        in data scope, while function calls are mapped to NamedFunctionCall. Arguments to the
        NamedFunctionCall are parsed in the global scope.

        This is intended for parsing of `Model`-paths, of the form `Model="Folder/File(arg0, arg1, ...)"`
    */
    public sealed class UXNameScopeAttribute: Attribute {}
}
