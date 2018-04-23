namespace Uno.Compiler.ExportTargetInterop
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ForeignIncludeAttribute : Attribute
    {
        public readonly Language Language;
        public readonly string[] Includes;

        // look..I know. I'm sorry, but at the time attributes didnt like param args
        public ForeignIncludeAttribute(Language language, string[] includes)
        {
            Language = language;
            Includes = includes;
        }

        public ForeignIncludeAttribute(Language language, string include0)
        : this(language, new string[] { include0 }) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1)
        : this(language, new string[] { include0, include1 }) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2)
        : this(language, new string[] { include0, include1 , include2 }) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3)
        : this(language, new string[] { include0, include1, include2, include3}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4)
        : this(language, new string[] { include0, include1, include2, include3, include4}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7, include8}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7, include8, include9}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7, include8, include9, include10}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10, string include11)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7, include8, include9, include10, include11}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10, string include11, string include12)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7, include8, include9, include10, include11, include12}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10, string include11, string include12, string include13)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7, include8, include9, include10, include11, include12, include13}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10, string include11, string include12, string include13, string include14)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7, include8, include9, include10, include11, include12, include13, include14}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10, string include11, string include12, string include13, string include14, string include15)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7, include8, include9, include10, include11, include12, include13, include14, include15}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10, string include11, string include12, string include13, string include14, string include15, string include16)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7, include8, include9, include10, include11, include12, include13, include14, include15, include16}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10, string include11, string include12, string include13, string include14, string include15, string include16, string include17)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7, include8, include9, include10, include11, include12, include13, include14, include15, include16, include17}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10, string include11, string include12, string include13, string include14, string include15, string include16, string include17, string include18)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7, include8, include9, include10, include11, include12, include13, include14, include15, include16, include17, include18}) {}

        public ForeignIncludeAttribute(Language language, string include0, string include1, string include2, string include3, string include4, string include5, string include6, string include7, string include8, string include9, string include10, string include11, string include12, string include13, string include14, string include15, string include16, string include17, string include18, string include19)
        : this(language, new string[] { include0, include1, include2, include3, include4, include5, include6, include7, include8, include9, include10, include11, include12, include13, include14, include15, include16, include17, include18, include19}) {}

    }
}
