namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class MemberFlagsViewModel
    {
        public bool UxContent { get; }
        public bool UxPrimary { get; }
        public bool UxComponents { get; }

        public MemberFlagsViewModel(bool uxContent, bool uxPrimary, bool uxComponents)
        {
            UxContent = uxContent;
            UxPrimary = uxPrimary;
            UxComponents = uxComponents;
        }

        public bool ShouldSerialize()
        {
            return UxContent ||
                   UxPrimary ||
                   UxComponents;
        }
    }
}