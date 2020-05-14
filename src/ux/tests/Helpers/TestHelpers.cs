using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using Uno.UX.Markup.AST;
using Uno.UX.Markup.Common;

namespace Uno.UX.Markup.Tests.Helpers
{
    public static class TestHelpers
    {
        public static void AssertErrors(string docName, string docCode, IEnumerable<ListMarkupErrorLogEntry> expectedErrors)
        {
            var log = new ListMarkupErrorLog();

            Parse(docName, docCode, log);

            CollectionAssert.AreEqual(expectedErrors, log.Errors);
        }

        public static Element Parse(string docName, string docCode, IMarkupErrorLog log = null)
        {
            if (log == null)
                log = new ListMarkupErrorLog();

            var projectName = "test project";
            var docStream = new MemoryStream(Encoding.UTF8.GetBytes(docCode));
            var doc = XmlHelpers.ReadAllXml(docStream, LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace, true);

            return Parser.Parse(projectName, docName, doc, log);
        }
    }
}
