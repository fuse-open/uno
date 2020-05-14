namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class SyntaxViewModel
    {
        public string Uno { get; }
        public string Ux { get; }

        public SyntaxViewModel(string uno, string ux)
        {
            Uno = uno;
            Ux = ux;
        }

        public bool ShouldSerialize()
        {
            return !string.IsNullOrWhiteSpace(Uno) ||
                   !string.IsNullOrWhiteSpace(Ux);
        }
    }
}