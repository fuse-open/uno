namespace Uno.UX
{
    /** Specifies how the UX compiler interprets child nodes of the decorated type.

        Possible values are:
        * `[UXContentMode("Default")]` - Child nodes are interppreted as instances. This is the default.
        * `[UXContentMode("Template")]` - Child nodes are interpreted as `ux:Template`
        * `[UXContentMode("TemplateIfClass")]` - Child nodes are interpreted as `ux:Template`, if the class is marked with `ux:Class` in the current scope, instances otherwise.

        `Template` is useful if creating a class for which the content nodes should not be directly instantiated, but instead be provided
        as `Uno.UX.Template`s so the class can instantiate them later at its own discretion.

        `TemplateIfClass` is useful if creating a class where you want the content to be templates while defining the class, but if users
        of the class add additional children, these should be interpreted as actual children.
    */
    public class UXContentModeAttribute: Attribute
    {
        public readonly string Mode;

        public UXContentModeAttribute(string mode)
        {
            Mode = mode;
        }
    }
}
