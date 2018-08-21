namespace Uno.Compiler.ExportTargetInterop
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public sealed class ForeignAnnotationAttribute : Attribute
    {
        public readonly Language Language;
        public readonly string[] Annotations;

        public ForeignAnnotationAttribute(Language language, string[] annotations)
        {
            Language = language;
        }

        public ForeignAnnotationAttribute(Language language, string annotation0)
        : this(language, new string[] { annotation0 }) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1)
        : this(language, new string[] { annotation0, annotation1 }) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2)
        : this(language, new string[] { annotation0, annotation1 , annotation2 }) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7, string annotation8)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7, annotation8}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7, string annotation8, string annotation9)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7, annotation8, annotation9}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7, string annotation8, string annotation9, string annotation10)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7, annotation8, annotation9, annotation10}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7, string annotation8, string annotation9, string annotation10, string annotation11)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7, annotation8, annotation9, annotation10, annotation11}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7, string annotation8, string annotation9, string annotation10, string annotation11, string annotation12)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7, annotation8, annotation9, annotation10, annotation11, annotation12}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7, string annotation8, string annotation9, string annotation10, string annotation11, string annotation12, string annotation13)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7, annotation8, annotation9, annotation10, annotation11, annotation12, annotation13}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7, string annotation8, string annotation9, string annotation10, string annotation11, string annotation12, string annotation13, string annotation14)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7, annotation8, annotation9, annotation10, annotation11, annotation12, annotation13, annotation14}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7, string annotation8, string annotation9, string annotation10, string annotation11, string annotation12, string annotation13, string annotation14, string annotation15)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7, annotation8, annotation9, annotation10, annotation11, annotation12, annotation13, annotation14, annotation15}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7, string annotation8, string annotation9, string annotation10, string annotation11, string annotation12, string annotation13, string annotation14, string annotation15, string annotation16)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7, annotation8, annotation9, annotation10, annotation11, annotation12, annotation13, annotation14, annotation15, annotation16}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7, string annotation8, string annotation9, string annotation10, string annotation11, string annotation12, string annotation13, string annotation14, string annotation15, string annotation16, string annotation17)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7, annotation8, annotation9, annotation10, annotation11, annotation12, annotation13, annotation14, annotation15, annotation16, annotation17}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7, string annotation8, string annotation9, string annotation10, string annotation11, string annotation12, string annotation13, string annotation14, string annotation15, string annotation16, string annotation17, string annotation18)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7, annotation8, annotation9, annotation10, annotation11, annotation12, annotation13, annotation14, annotation15, annotation16, annotation17, annotation18}) {}

        public ForeignAnnotationAttribute(Language language, string annotation0, string annotation1, string annotation2, string annotation3, string annotation4, string annotation5, string annotation6, string annotation7, string annotation8, string annotation9, string annotation10, string annotation11, string annotation12, string annotation13, string annotation14, string annotation15, string annotation16, string annotation17, string annotation18, string annotation19)
        : this(language, new string[] { annotation0, annotation1, annotation2, annotation3, annotation4, annotation5, annotation6, annotation7, annotation8, annotation9, annotation10, annotation11, annotation12, annotation13, annotation14, annotation15, annotation16, annotation17, annotation18, annotation19}) {}
    }
}
