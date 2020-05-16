using NUnit.Framework;
using Uno.UX.Markup.Tests.Helpers;

namespace Uno.UX.Markup.Tests.ParserTests
{
    [TestFixture]
    public class ErrorTests
    {
        [Test]
        public void CaseIsDeprecated()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Case=\"someCase\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("ux:Case is deprecated, use ux:Template instead (means the same)", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("Invalid UX attribute: ux:Case", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void DefaultCaseIsDeprecated()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:DefaultCase=\"someCase\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("ux:DefaultCase is deprecated, use ux:DefaultTemplate instead (means the same)", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("Invalid UX attribute: ux:DefaultCase", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyBothUxDependencyAndUxName()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Dependency=\"hi\" ux:Name=\"fail\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("Cannot specify both ux:Dependency and ux:Name", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyBothUxPropertyAndUxName()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Property=\"hi\" ux:Name=\"fail\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("Cannot specify both ux:Property and ux:Name", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyThisAsUxName()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Name=\"this\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'this\' is the implicit name of class nodes and can not be specified explicitly", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'this\' is a Uno language keyword, and can not be used as ux:Name or ux:Global", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyUnoKeywordAsUxName1()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Name=\"private\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'private\' is a Uno language keyword and can not be used as name", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'private\' is a Uno language keyword, and can not be used as ux:Name or ux:Global", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyUnoKeywordAsUxName2()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Name=\"public\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'public\' is a Uno language keyword and can not be used as name", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'public\' is a Uno language keyword, and can not be used as ux:Name or ux:Global", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyThisAsUxTemplate()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Template=\"this\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'this\' is the implicit name of class nodes and can not be specified explicitly", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'this\' is a Uno language keyword, and can not be used as ux:Name or ux:Global", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyUnoKeywordAsUxTemplate1()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Template=\"int\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'int\' is a Uno language keyword and can not be used as name", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'int\' is a Uno language keyword, and can not be used as ux:Name or ux:Global", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyUnoKeywordAsUxTemplate2()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Template=\"bool\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'bool\' is a Uno language keyword and can not be used as name", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'bool\' is a Uno language keyword, and can not be used as ux:Name or ux:Global", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyThisAsUxDependency()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Dependency=\"this\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'this\' is the implicit name of class nodes and can not be specified explicitly", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'this\' is a Uno language keyword, and can not be used as ux:Name or ux:Global", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyUnoKeywordAsUxDependency1()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Dependency=\"float\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'float\' is a Uno language keyword and can not be used as name", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'float\' is a Uno language keyword, and can not be used as ux:Name or ux:Global", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyUnoKeywordAsUxDependency2()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Dependency=\"double\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'double\' is a Uno language keyword and can not be used as name", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'double\' is a Uno language keyword, and can not be used as ux:Name or ux:Global", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyThisAsUxProperty()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Property=\"this\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'this\' is the implicit name of class nodes and can not be specified explicitly", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'this\' is a Uno language keyword, and can not be used as ux:Name or ux:Global", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'this\' is a Uno language keyword, and can not be used as ux:Property", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyUnoKeywordAsUxProperty1()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Property=\"float4\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'float4\' is a Uno language keyword and can not be used as name", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'float4\' is a Uno language keyword, and can not be used as ux:Name or ux:Global", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'float4\' is a Uno language keyword, and can not be used as ux:Property", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void CannotSpecifyUnoKeywordAsUxProperty2()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Property=\"float2x2\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'float2x2\' is a Uno language keyword and can not be used as name", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'float2x2\' is a Uno language keyword, and can not be used as ux:Name or ux:Global", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("\'float2x2\' is a Uno language keyword, and can not be used as ux:Property", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void UxResourceIsDeprecated()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Resource=\"someResource\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("'ux:Resource' is deprecated - use 'ux:Ref' instead (works for both local and global references now)", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void InvalidUxAttributes()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Derp=\"asdf\" ux:Jeff=\"asdf\" ux:Ux=\"asdf\" ux:Compiler=\"asdf\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("Invalid UX attribute: ux:Derp", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("Invalid UX attribute: ux:Jeff", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("Invalid UX attribute: ux:Ux", new ListMarkupErrorLogSource(docName, 1)),
                new ListMarkupErrorLogEntry("Invalid UX attribute: ux:Compiler", new ListMarkupErrorLogSource(docName, 1))
            });
        }

        [Test]
        public void UxPropertyCantContainNonIdentifierChars()
        {
            const string docName = "a.ux";
            const string docCode = "<Base ux:Property=\"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0987654321_:\" />";

            TestHelpers.AssertErrors(docName, docCode, new[]
            {
                new ListMarkupErrorLogEntry("\'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0987654321_:\' contains the character \':\' which is not a valid Uno identifier character. This name can not be used as ux:Property", new ListMarkupErrorLogSource(docName, 1))
            });
        }
    }
}
