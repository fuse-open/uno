namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class InheritanceViewModel
    {
        public InheritanceNodeViewModel Root { get; private set; }
        public bool HasInheritance { get; private set; }

        public InheritanceViewModel(InheritanceNodeViewModel root)
        {
            Root = root;
            HasInheritance = root != null;
        }
    }
}
