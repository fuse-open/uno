using Uno;
using Uno.IO;
using Uno.Collections;
using Uno.Data.Xml;
using Uno.Testing;

namespace XmlTests
{
    public class XmlDocumentTests
    {
        const string xml1 = "<rss version=\"2.0\"><channel><title>Hacker News</title><link>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;</link><description>Links for the intellectually curious, ranked by readers.</description><item><title>Gmail was down</title><link>http:&#x2F;&#x2F;www.google.com&#x2F;appsstatus#hl=en&amp;v=status&amp;ts=1390590318542</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7116764</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7116764\">Comments</a>]]></description></item><item><title>Show HN: I open-sourced my web app alternative to Illustrator</title><link>http:&#x2F;&#x2F;mondrian.io&#x2F;contributing</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7116042</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7116042\">Comments</a>]]></description></item><item><title>Where do you find the time for side projects?</title><link>http:&#x2F;&#x2F;justinjackson.ca&#x2F;where-do-you-find-the-time-for-side-projects&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7117131</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7117131\">Comments</a>]]></description></item><item><title>UK porn filter blocks League of Legends update</title><link>http:&#x2F;&#x2F;www.joystiq.com&#x2F;2014&#x2F;01&#x2F;24&#x2F;uk-porn-filter-blocks-league-of-legends-update-for-sex-in-file&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7116328</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7116328\">Comments</a>]]></description></item><item><title>Why Dogecoin is Important</title><link>http:&#x2F;&#x2F;www.abcoin.net&#x2F;post&#x2F;74401339267&#x2F;why-dogecoin-is-important</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7116517</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7116517\">Comments</a>]]></description></item><item><title>King.com, makers of Candy Crush Saga – Trademark Trolls with a Double Standard?</title><link>http:&#x2F;&#x2F;junkyardsam.com&#x2F;kingcopied&#x2F;#</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7114291</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7114291\">Comments</a>]]></description></item><item><title>Richard Stallman - Re: clang vs free software</title><link>http:&#x2F;&#x2F;gcc.gnu.org&#x2F;ml&#x2F;gcc&#x2F;2014-01&#x2F;msg00247.html</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7116144</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7116144\">Comments</a>]]></description></item><item><title>Uber rival accuses car service of dirty tactics</title><link>http:&#x2F;&#x2F;money.cnn.com&#x2F;2014&#x2F;01&#x2F;24&#x2F;technology&#x2F;social&#x2F;uber-gett&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7115177</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7115177\">Comments</a>]]></description></item><item><title>JQuery 1.11 and 2.1 Released</title><link>http:&#x2F;&#x2F;blog.jquery.com&#x2F;2014&#x2F;01&#x2F;24&#x2F;jquery-1-11-and-2-1-released&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7115195</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7115195\">Comments</a>]]></description></item><item><title>Stephen Hawking: &#x27;There are no black holes&#x27;</title><link>http:&#x2F;&#x2F;www.nature.com&#x2F;news&#x2F;stephen-hawking-there-are-no-black-holes-1.14583</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7114913</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7114913\">Comments</a>]]></description></item><item><title>The 30-Year-Old Macintosh and a Conversation With Steve Jobs</title><link>http:&#x2F;&#x2F;bits.blogs.nytimes.com&#x2F;2014&#x2F;01&#x2F;24&#x2F;the-30-year-old-macintosh-and-a-lost-conversation-with-steve-jobs&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7116027</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7116027\">Comments</a>]]></description></item><item><title>Use SQL subqueries to count distinct 50x faster</title><link>https:&#x2F;&#x2F;periscope.io&#x2F;blog&#x2F;use-subqueries-to-count-distinct-50x-faster.html</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7114310</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7114310\">Comments</a>]]></description></item><item><title>New XAMPP &#x2F; Apache Friends site released</title><link>http:&#x2F;&#x2F;apachefriends.org</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7116880</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7116880\">Comments</a>]]></description></item><item><title>The Macintosh Is 30, and I Was There for Its Birth</title><link>http:&#x2F;&#x2F;www.wired.com&#x2F;wiredenterprise&#x2F;2014&#x2F;01&#x2F;macintosh-30th-anniversary&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7116640</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7116640\">Comments</a>]]></description></item><item><title>Macintosh 128K Teardown</title><link>http:&#x2F;&#x2F;www.ifixit.com&#x2F;Teardown&#x2F;Macintosh+128K+Teardown&#x2F;21422</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7115134</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7115134\">Comments</a>]]></description></item><item><title>AeroFS is Hiring Mobile, Backend, and Support Engineers</title><link>https:&#x2F;&#x2F;www.aerofs.com&#x2F;careers?source=HackerNews</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7116526</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7116526\">Comments</a>]]></description></item><item><title>Ethereum “Dagger” proof-of-work function is flawed</title><link>http:&#x2F;&#x2F;bitslog.wordpress.com&#x2F;2014&#x2F;01&#x2F;17&#x2F;ethereum-dagger-pow-is-flawed&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7115725</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7115725\">Comments</a>]]></description></item><item><title>Thirty Years of Mac [video]</title><link>https:&#x2F;&#x2F;www.apple.com&#x2F;30-years&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7114144</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7114144\">Comments</a>]]></description></item><item><title>Injection of malicious code into jQuery is increasing</title><link>http:&#x2F;&#x2F;blog.spiderlabs.com&#x2F;2014&#x2F;01&#x2F;beware-bats-hide-in-your-jquery-.html</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7115154</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7115154\">Comments</a>]]></description></item><item><title>Ask HN: cheap ways to host your own email server?</title><link>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7117263</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7117263</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7117263\">Comments</a>]]></description></item><item><title>Debunking Princeton</title><link>https:&#x2F;&#x2F;www.facebook.com&#x2F;notes&#x2F;mike-develin&#x2F;debunking-princeton&#x2F;10151947421191849</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7111627</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7111627\">Comments</a>]]></description></item><item><title>JavaScript – The monkeys in 2013</title><link>https:&#x2F;&#x2F;blog.mozilla.org&#x2F;javascript&#x2F;2014&#x2F;01&#x2F;23&#x2F;the-monkeys-in-2013&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7114357</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7114357\">Comments</a>]]></description></item><item><title>How Silicon Valley CEOs conspired to drive down tech engineer wages</title><link>http:&#x2F;&#x2F;pando.com&#x2F;2014&#x2F;01&#x2F;23&#x2F;the-techtopus-how-silicon-valleys-most-celebrated-ceos-conspired-to-drive-down-100000-tech-engineers-wages&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7111531</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7111531\">Comments</a>]]></description></item><item><title>&#x27;Revenge porn&#x27; website former owner Hunter Moore arrested </title><link>http:&#x2F;&#x2F;www.bbc.co.uk&#x2F;news&#x2F;technology-25872322</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7114182</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7114182\">Comments</a>]]></description></item><item><title>The rise of Dogecoin, the internet&#x27;s hottest cryptocurrency</title><link>http:&#x2F;&#x2F;www.theage.com.au&#x2F;technology&#x2F;technology-news&#x2F;the-rise-and-rise-of-dogecoin-the-internets-hottest-cryptocurrency-20140124-31d24.html</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7114813</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7114813\">Comments</a>]]></description></item><item><title>Hawking: Information Preservation and Weather Forecasting for Black Holes</title><link>http:&#x2F;&#x2F;arxiv.org&#x2F;abs&#x2F;1401.5761</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7114762</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7114762\">Comments</a>]]></description></item><item><title>Write an aphorism using valid code</title><link>http:&#x2F;&#x2F;codegolf.stackexchange.com&#x2F;questions&#x2F;18093&#x2F;write-an-aphorism-using-valid-code&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7115827</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7115827\">Comments</a>]]></description></item><item><title>CBSD – FreeBSD Jail Management Tools</title><link>http:&#x2F;&#x2F;www.bsdstore.ru&#x2F;html&#x2F;about_en.html</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7114402</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7114402\">Comments</a>]]></description></item><item><title>MySQL server memory usage troubleshooting tips</title><link>http:&#x2F;&#x2F;www.mysqlperformanceblog.com&#x2F;2014&#x2F;01&#x2F;24&#x2F;mysql-server-memory-usage-2&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7115273</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7115273\">Comments</a>]]></description></item><item><title>Ethereum is a next-generation cryptocurrency platform</title><link>http:&#x2F;&#x2F;ethereum.org&#x2F;</link><comments>https:&#x2F;&#x2F;news.ycombinator.com&#x2F;item?id=7114898</comments><description><![CDATA[<a href=\"https://news.ycombinator.com/item?id=7114898\">Comments</a>]]></description></item></channel></rss>";

        const string xml2 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><note><to>Tove</to><from>Jani</from><heading>Reminder</heading><body>Don't forget me this weekend!</body></note>";

        const string xml3 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><mesh name=\"mesh_root\">    <!-- here is a mesh node -->    some text    <![CDATA[someothertext]]>    some more text    <node attr1=\"value1\" attr2=\"value2\" />    <node>        <innernode/>    </node><node>internal value</node></mesh><?include somedata?>";

        const string xml4 = "<root xmlns=\"http://schemas.datacontract.org/2004/07/Service.Entities\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><node>text</node></root>";

        const string xml5 = "<root><category>This is <empty/><item>some first item</item></category><category>and<item>some second item</item></category><!-- comment here--></root>";

        [Test]
        public void SimpleParse()
        {
            var document = XmlDocument.Load(xml2);
            Assert.IsTrue(document != null);
            Assert.IsTrue(document.DocumentElement != null);
        }

        [Test]
        public void BigXmlParse()
        {
            var document = XmlDocument.Load(xml1);
            Assert.AreEqual(1, document.DocumentElement.Children.Count);

            var rootElement = document.DocumentElement.FirstChild;
            Assert.AreEqual(33, rootElement.FirstChild.Children.Count);

            Assert.AreEqual(1, rootElement.Attributes.Count);

            var rootAttribute = rootElement.Attributes.FirstChild;
            Assert.AreEqual("version", rootAttribute.Name);
            Assert.AreEqual("2.0", rootAttribute.Value.AsString());

            var titleNode = rootElement.FirstChild.FirstChild; //first title node
            Assert.AreEqual("title", titleNode.Name);
            Assert.AreEqual("Hacker News", titleNode.FirstChild.Value.AsString());
        }

        [Test]
        public void XmlDeclaration()
        {
            var document = XmlDocument.Load(xml2);
            Assert.AreEqual(1, document.DocumentElement.Children.Count);
            Assert.IsTrue(document.DocumentElement.FirstChild is XmlElement);

            document = XmlDocument.Load(xml2, ParseOptions.IncludeDeclaration);
            Assert.AreEqual(2, document.DocumentElement.Children.Count);
            Assert.IsTrue(document.DocumentElement.FirstChild is XmlDeclaration);

            var xmlDeclaration = document.DocumentElement.FirstChild as XmlDeclaration;
            Assert.AreEqual(XmlNodeType.Declaration, xmlDeclaration.NodeType);
            Assert.AreEqual("1.0", xmlDeclaration.Version);
            Assert.AreEqual((int)XmlEncoding.Utf8, (int)xmlDeclaration.Encoding);
        }

        [Test]
        public void XmlProcessingInstruction()
        {
            var document = XmlDocument.Load(xml3);
            Assert.AreEqual(1, document.DocumentElement.Children.Count);
            Assert.IsTrue(document.DocumentElement.LastChild is XmlElement);

            document = XmlDocument.Load(xml3, ParseOptions.IncludeProcessingInstruction);
            Assert.AreEqual(2, document.DocumentElement.Children.Count);
            Assert.IsTrue(document.DocumentElement.LastChild is XmlProcessingInstruction);

            var processingInstruction = document.DocumentElement.LastChild as XmlProcessingInstruction;
            Assert.AreEqual(XmlNodeType.ProcessingInstruction, processingInstruction.NodeType);
            Assert.AreEqual("include", processingInstruction.Name);
            Assert.AreEqual("somedata", processingInstruction.Value.AsString());
        }

        [Test]
        public void XmlElement()
        {
            var document = XmlDocument.Load(xml3);
            Assert.AreEqual(1, document.DocumentElement.Children.Count);
            Assert.IsTrue(document.DocumentElement.FirstChild is XmlElement);

            var rootElement = document.DocumentElement.FirstChild as XmlElement;
            Assert.AreEqual(XmlNodeType.Element, rootElement.NodeType);
            Assert.AreEqual("mesh", rootElement.Name);
            Assert.IsTrue(string.IsNullOrEmpty(rootElement.Value.AsString()));
            Assert.AreEqual("internal value", rootElement.LastChild.Value.AsString());
        }

        [Test]
        public void XmlComment()
        {
            var document = XmlDocument.Load(xml3);
            var rootElement = document.DocumentElement.FirstChild;
            Assert.IsTrue(rootElement.FirstChild is XmlText);

            document = XmlDocument.Load(xml3, ParseOptions.IncludeComment);
            rootElement = document.DocumentElement.FirstChild;
            Assert.IsTrue(rootElement.FirstChild is XmlComment);

            var commentNode = rootElement.FirstChild as XmlComment;
            Assert.AreEqual(XmlNodeType.Comment, commentNode.NodeType);
            Assert.AreEqual(" here is a mesh node ", commentNode.Value.AsString());
        }

        [Test]
        public void XmlText()
        {
            var document = XmlDocument.Load(xml3);
            var rootElement = document.DocumentElement.FirstChild;
            Assert.IsTrue(rootElement.FirstChild is XmlText);

            var textNode = rootElement.FirstChild as XmlText;
            Assert.AreEqual(XmlNodeType.Text, textNode.NodeType);
            Assert.AreEqual("    some text    ", textNode.Value.AsString());
        }

        [Test]
        public void XmlCharacterData()
        {
            var document = XmlDocument.Load(xml3);
            var rootElement = document.DocumentElement.FirstChild;
            Assert.IsTrue(rootElement.Children[1] is XmlCharacterData);

            var cdNode = rootElement.Children[1] as XmlCharacterData;
            Assert.AreEqual(XmlNodeType.CharacterData, cdNode.NodeType);
            Assert.AreEqual("someothertext", cdNode.Value.AsString());
        }

        [Test]
        public void ParseOptionsCombinations()
        {
            var document = XmlDocument.Load(xml3);
            Assert.AreEqual(1, document.DocumentElement.Children.Count);
            document = XmlDocument.Load(xml3, ParseOptions.IncludeDeclaration | ParseOptions.IncludeProcessingInstruction);
            Assert.AreEqual(3, document.DocumentElement.Children.Count);

            var rootElement = document.DocumentElement.Children[1];
            Assert.AreEqual(6, rootElement.Children.Count);

            document = XmlDocument.Load(xml3, ParseOptions.IncludeProcessingInstruction | ParseOptions.IncludeComment);
            Assert.AreEqual(2, document.DocumentElement.Children.Count);
            rootElement = document.DocumentElement.FirstChild;
            Assert.AreEqual(7, rootElement.Children.Count);
        }

        [Test]
        public void ComplexParse()
        {
            var document = XmlDocument.Load(xml3, ParseOptions.IncludeDeclaration | ParseOptions.IncludeProcessingInstruction | ParseOptions.IncludeComment);
            Assert.AreEqual(XmlNodeType.Document, document.DocumentElement.NodeType);
            Assert.AreEqual(3, document.DocumentElement.Children.Count);

            Assert.AreEqual(XmlNodeType.Declaration, document.DocumentElement.FirstChild.NodeType);
            XmlDeclaration declaration = document.DocumentElement.FirstChild as XmlDeclaration;
            Assert.AreEqual("1.0", declaration.Version);

            Assert.AreEqual(XmlNodeType.Element, document.DocumentElement.Children[1].NodeType);
            var mesh = document.DocumentElement.Children[1];
            Assert.IsTrue(mesh.HasAttributes);
            Assert.AreEqual(1, mesh.Attributes.Count);
            Assert.AreEqual("name", mesh.Attributes[0].Name);
            Assert.AreEqual("mesh_root", mesh.Attributes[0].Value.AsString());

            var item = mesh.FirstChild;
            Assert.AreEqual(XmlNodeType.Comment, item.NodeType);
            Assert.AreEqual(" here is a mesh node ", item.Value.AsString());

            item = item.NextSibling;
            Assert.AreEqual(XmlNodeType.Text, item.NodeType);
            Assert.AreEqual("    some text    ", item.Value.AsString());

            item = item.NextSibling;
            Assert.AreEqual(XmlNodeType.CharacterData, item.NodeType);
            Assert.AreEqual("someothertext", item.Value.AsString());

            item = item.NextSibling;
            Assert.AreEqual(XmlNodeType.Text, item.NodeType);
            Assert.AreEqual("    some more text    ", item.Value.AsString());

            item = item.NextSibling;
            Assert.AreEqual(XmlNodeType.Element, item.NodeType);
            Assert.IsTrue(string.IsNullOrEmpty(item.Value.AsString()));
            Assert.AreEqual("node", item.Name);
            Assert.AreEqual(0, item.Children.Count);
            Assert.AreEqual(2, item.Attributes.Count);
            Assert.AreEqual("attr1", item.Attributes[0].Name);
            Assert.AreEqual("value1", item.Attributes[0].Value.AsString());
            Assert.AreEqual("attr2", item.Attributes[1].Name);
            Assert.AreEqual("value2", item.Attributes[1].Value.AsString());

            item = item.NextSibling;
            Assert.AreEqual(XmlNodeType.Element, item.NodeType);
            Assert.IsTrue(string.IsNullOrEmpty(item.Value.AsString()));
            Assert.AreEqual("node", item.Name);
            Assert.AreEqual(1, item.Children.Count);
            Assert.AreEqual(0, item.Attributes.Count);
            Assert.AreEqual("innernode", item.FirstChild.Name);
            Assert.AreEqual(null, item.FirstChild.Value.AsString());

            item = item.NextSibling;
            Assert.AreEqual(XmlNodeType.Element, item.NodeType);
            Assert.AreEqual("internal value", item.Value.AsString());
            Assert.AreEqual("node", item.Name);
            Assert.AreEqual(1, item.Children.Count);
            Assert.AreEqual(0, item.Attributes.Count);
            Assert.AreEqual("internal value", item.FirstChild.Value.AsString());

            Assert.AreEqual(null, item.NextSibling);
        }

        [Test]
        public void ParseWithNamespaces()
        {
            var document = XmlDocument.Load(xml4);
            var rootElement = document.DocumentElement.FirstChild;

            Assert.AreEqual("node", rootElement.FirstChild.Name);
        }

        const string whiteSpaceXml = "<root><item>   123    </item>   <item>     <subitem>  333  </subitem>  </item>   222   </root>";

        const string savedWhiteSpaceXml = "<root><item>   123    </item><item><subitem>  333  </subitem></item>   222   </root>";

        [Test]
        public void WhiteSpaceSave()
        {
            var document = XmlDocument.Load(whiteSpaceXml);
            Assert.AreEqual(3, document.DocumentElement.FirstChild.Children.Count);

            string output;
            document.Save(out output);
            Assert.AreEqual(savedWhiteSpaceXml, output);
        }

        const string copyElementXml = "<root><item1>1</item1><item2><subitem>21</subitem></item2></root>";

        const string savedCopyElementXml = "<root><subitem>21</subitem><item1>1<item2><subitem>21</subitem></item2></item1><item2><subitem>21</subitem></item2></root>";

        [Test]
        public void CopyElementSave()
        {
            var document = XmlDocument.Load(copyElementXml);
            var parent = document.DocumentElement.FirstChild;

            Assert.AreEqual(2, parent.Children.Count);

            parent.Children[0].AppendChild(parent.Children[1]);

            Assert.AreEqual(2, parent.Children.Count);

            parent.PrependChild(parent.Children[0].Children[1].Children[0]);

            Assert.AreEqual(3, parent.Children.Count);

            string output;
            document.Save(out output);
            Assert.AreEqual(savedCopyElementXml, output);
        }

        [Test]
        public void XmlNodeComparison()
        {
            var node1 = new XmlElement() { Name = "node1" };
            var node2 = new XmlComment() { Name = "node2" };
            var node3 = node1;
            XmlNode node4 = null;

            Assert.AreEqual(node1, node1);
            Assert.AreEqual(node2, node2);
            Assert.AreEqual(node3, node3);
            Assert.AreEqual(node1, node3);
            Assert.AreEqual(node3, node1);
            Assert.AreNotEqual(node1, node2);
            Assert.AreNotEqual(node2, node1);
            Assert.AreNotEqual(node1, node4);
            Assert.AreNotEqual(node3, node2);
            Assert.AreNotEqual(node3, node4);
            Assert.AreNotEqual(node1, null);
        }

        [Test]
        public void XmlNodeFirstChild()
        {
            var document = new XmlDocument();
            XmlLinkedNode parent = document.DocumentElement;
            XmlLinkedNode child1 = new XmlDeclaration { Encoding = XmlEncoding.Utf8 };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };

            parent.Children.Add(child1);
            parent.Children.Add(child2);

            Assert.AreEqual(child1, parent.FirstChild);
            Assert.AreNotEqual(child2, parent.FirstChild);
        }

        [Test]
        public void XmlNodeLastChild()
        {
            var document = new XmlDocument();
            XmlLinkedNode parent = document.DocumentElement;
            XmlLinkedNode child1 = new XmlDeclaration { Encoding = XmlEncoding.Utf8 };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };

            parent.Children.Add(child1);
            parent.Children.Add(child2);

            Assert.AreNotEqual(child1, parent.LastChild);
            Assert.AreEqual(child2, parent.LastChild);
        }

        [Test]
        public void XmlNodeNextSibling()
        {
            var document = new XmlDocument();
            var parent = new XmlElement();
            document.DocumentElement.AppendChild(parent);
            XmlLinkedNode child1 = new XmlText { Value = new XmlValue("Value") };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };
            XmlLinkedNode child3 = new XmlComment() { Value = new XmlValue("Value") };

            parent.Children.Add(child1);
            parent.Children.Add(child2);
            parent.Children.Add(child3);

            Assert.AreEqual(child2, child1.NextSibling);
            Assert.AreEqual(child3, child2.NextSibling);
            Assert.IsTrue(child3.NextSibling == null);
        }

        [Test]
        public void XmlNodePreviousSibling()
        {
            var document = new XmlDocument();
            var parent = new XmlElement();
            document.DocumentElement.AppendChild(parent);
            XmlLinkedNode child1 = new XmlText { Value = new XmlValue("Value") };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };
            XmlLinkedNode child3 = new XmlComment() { Value = new XmlValue("Value") };

            parent.Children.Add(child1);
            parent.Children.Add(child2);
            parent.Children.Add(child3);

            Assert.AreEqual(child2, child3.PreviousSibling);
            Assert.AreEqual(child1, child2.PreviousSibling);
            Assert.AreEqual(null, child1.PreviousSibling);
        }

        [Test]
        public void XmlNodeParent()
        {
            var document = new XmlDocument();
            XmlLinkedNode parent = document.DocumentElement;
            XmlLinkedNode child1 = new XmlDeclaration { Encoding = XmlEncoding.Utf8 };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };

            parent.Children.Add(child1);
            parent.Children.Add(child2);

            Assert.AreEqual(parent, child1.Parent);
            Assert.AreEqual(parent, child2.Parent);
            Assert.AreEqual(child1.Parent, child2.Parent);
            Assert.IsTrue(parent.Parent == null);
        }

        [Test]
        public void XmlNodeAppend()
        {
            var document = new XmlDocument();
            XmlLinkedNode parent = document.DocumentElement;
            XmlLinkedNode child1 = new XmlDeclaration { Encoding = XmlEncoding.Utf8 };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };

            parent.AppendChild(child1);
            parent.AppendChild(child2);

            Assert.AreEqual(child1, parent.FirstChild);
            Assert.AreEqual(child2, parent.LastChild);
        }

        [Test]
        public void XmlNodePrepend()
        {
            var document = new XmlDocument();
            XmlLinkedNode parent = document.DocumentElement;
            XmlLinkedNode child1 = new XmlDeclaration { Encoding = XmlEncoding.Utf8 };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };

            parent.PrependChild(child1);
            parent.PrependChild(child2);

            Assert.AreEqual(child2, parent.FirstChild);
            Assert.AreEqual(child1, parent.LastChild);
        }

        [Test]
        public void FindXmlNode()
        {
            var document = new XmlDocument();
            var parent = new XmlElement();
            document.DocumentElement.AppendChild(parent);
            XmlLinkedNode child1 = new XmlElement() { Name = "child1", Value = new XmlValue("Value") };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };
            XmlLinkedNode child3 = new XmlElement() { Name = "child3", Value = new XmlValue("Value") };

            parent.AppendChild(child1);
            parent.AppendChild(child2);
            child2.AppendChild(child3);

            Assert.AreEqual(child1, parent.FindByName("child1"));
            Assert.AreEqual(child2, parent.FindByName("child2"));
            Assert.AreEqual(null, parent.FindByName("child3"));
            Assert.AreEqual(child3, parent.FindByName("child3", true));
            Assert.AreEqual(child3, child2.FindByName("child3"));

            Assert.AreNotEqual(child2, parent.FindByName("child1"));
            Assert.AreNotEqual(child1, parent.FindByName("child2"));
            Assert.AreEqual(null, parent.FindByName("aldsf"));
        }

        [Test]
        public void FindAllXmlNodes()
        {
            var document = new XmlDocument();
            var parent = new XmlElement();
            document.DocumentElement.AppendChild(parent);
            XmlLinkedNode child1 = new XmlElement() { Name = "child", Value = new XmlValue("Value") };
            XmlLinkedNode child2 = new XmlElement() { Name = "child", Value = new XmlValue("Value") };
            XmlLinkedNode child3 = new XmlElement() { Name = "child", Value = new XmlValue("Value") };

            parent.AppendChild(child1);
            parent.AppendChild(child2);
            child2.AppendChild(child3);

            Assert.AreEqual(2, parent.FindAllByName("child").Count);
            Assert.AreEqual(3, parent.FindAllByName("child", true).Count);
        }

        [Test]
        public void XmlNodeGetChildren()
        {
            var document = new XmlDocument();
            var parent = new XmlElement();
            document.DocumentElement.AppendChild(parent);
            XmlLinkedNode child1 = new XmlText { Value = new XmlValue("Value") };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };
            XmlLinkedNode child3 = new XmlComment() { Value = new XmlValue("Value") };
            XmlLinkedNode child4 = new XmlCharacterData() { Name = "child4", Data = "Data" };

            parent.AppendChild(child1);
            parent.AppendChild(child2);
            parent.AppendChild(child3);
            parent.AppendChild(child4);

            Assert.AreEqual(child1, parent.Children[0]);
            Assert.AreEqual(child2, parent.Children[1]);
            Assert.AreEqual(child3, parent.Children[2]);
            Assert.AreEqual(child4, parent.Children[3]);
        }

        [Test]
        public void XmlNodeAddBeforeSelf()
        {
            var document = new XmlDocument();
            var parent = new XmlElement();
            document.DocumentElement.AppendChild(parent);
            XmlLinkedNode child1 = new XmlText { Value = new XmlValue("Value") };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };
            XmlLinkedNode child3 = new XmlComment() { Value = new XmlValue("Value") };
            XmlLinkedNode child4 = new XmlElement() { Name = "child4", Value = new XmlValue("Value") };

            parent.AppendChild(child1);
            parent.AppendChild(child2);

            child1.AddBeforeSelf(child3);
            child2.AddBeforeSelf(child4);

            Assert.AreEqual(child3, parent.FirstChild);
            Assert.AreEqual(child3.Name, child1.PreviousSibling.Name);
            Assert.AreEqual(child1, child3.NextSibling);

            Assert.AreEqual(child2, parent.LastChild);
            Assert.AreEqual(child4, child2.PreviousSibling);
            Assert.AreEqual(child2, child4.NextSibling);
        }

        [Test]
        public void XmlNodeAddAfterSelf()
        {
            var document = new XmlDocument();
            var parent = new XmlElement();
            document.DocumentElement.AppendChild(parent);
            XmlLinkedNode child1 = new XmlText { Value = new XmlValue("Value") };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };
            XmlLinkedNode child3 = new XmlComment() { Value = new XmlValue("Value") };
            XmlLinkedNode child4 = new XmlCharacterData() { Name = "child4", Data = "Data" };

            parent.AppendChild(child1);
            parent.AppendChild(child2);

            child1.AddAfterSelf(child3);

            Assert.AreEqual(child3, child1.NextSibling);
            Assert.AreEqual(child1, child3.PreviousSibling);

            child2.AddAfterSelf(child4);

            Assert.AreEqual(child4, parent.LastChild);
            Assert.AreEqual(child4, child2.NextSibling);
            Assert.AreEqual(child2, child4.PreviousSibling);
        }

        [Test]
        public void XmlNodeRemove()
        {
            var document = new XmlDocument();
            var parent = new XmlElement();
            document.DocumentElement.AppendChild(parent);
            XmlLinkedNode child1 = new XmlText { Value = new XmlValue("Value") };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };
            XmlLinkedNode child3 = new XmlElement() { Name = "child3", Value = new XmlValue("Value") };

            parent.AppendChild(child1);
            parent.AppendChild(child2);
            parent.AppendChild(child3);

            Assert.AreEqual(3, parent.Children.Count);

            parent.Children.Remove(child2);

            Assert.AreEqual(null, parent.FindByName("child2"));
            Assert.AreEqual(2, parent.Children.Count);
            Assert.AreEqual(child1, child3.PreviousSibling);
            Assert.AreEqual(child3, child1.NextSibling);

            parent.Children.Remove(child1);

            Assert.AreEqual(null, parent.FindByName("child1"));
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreEqual(null, child3.PreviousSibling);
            Assert.AreEqual(null, child3.NextSibling);

            parent.Children.Remove(child3);
            Assert.AreEqual(0, parent.Children.Count);
        }

        [Test]
        public void XmlNodeRemoveAt()
        {
            var document = new XmlDocument();
            var parent = new XmlElement();
            document.DocumentElement.AppendChild(parent);
            XmlLinkedNode child1 = new XmlText { Value = new XmlValue("Value") };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };
            XmlLinkedNode child3 = new XmlElement() { Name = "child3", Value = new XmlValue("Value") };

            parent.AppendChild(child1);
            parent.AppendChild(child2);
            parent.AppendChild(child3);

            Assert.AreEqual(3, parent.Children.Count);

            parent.Children.RemoveAt(1);

            Assert.AreEqual(null, parent.FindByName("child2"));
            Assert.AreEqual(2, parent.Children.Count);
            Assert.AreEqual(child1, child3.PreviousSibling);
            Assert.AreEqual(child3, child1.NextSibling);
        }

        [Test]
        public void XmlNodeRemoveAllChildren()
        {
            var document = new XmlDocument();
            XmlLinkedNode parent = document.DocumentElement;
            XmlLinkedNode child1 = new XmlDeclaration { Encoding = XmlEncoding.Utf8 };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };

            parent.AppendChild(child1);
            parent.AppendChild(child2);

            Assert.AreEqual(2, parent.Children.Count);
            Assert.IsTrue(parent.HasChildren);

            parent.Children.Clear();

            Assert.IsTrue(!parent.HasChildren);
            Assert.AreEqual(0, parent.Children.Count);
        }

        [Test]
        public void XmlNodeAddAttribute()
        {
            var document = new XmlDocument();
            var root = new XmlElement();
            document.DocumentElement.AppendChild(root);
            XmlAttribute attrib1 = new XmlAttribute() { Name = "attrib1", Value = new XmlValue("Value") };
            XmlAttribute attrib2 = new XmlAttribute() { Name = "attrib2", Value = new XmlValue("Value") };

            root.Attributes.Add(attrib1);
            root.Attributes.Add(attrib2);

            Assert.IsTrue(root.HasAttributes);
            Assert.AreEqual(attrib1, root.Attributes["attrib1"]);
            Assert.AreEqual(attrib2, root.Attributes["attrib2"]);
            Assert.AreEqual(2, root.Attributes.Count);
        }

        [Test]
        public void XmlAttributeComparison()
        {
            var attrib1 = new XmlAttribute() { Name = "attrib1" };
            var attrib2 = new XmlAttribute() { Name = "attrib2" };
            var attrib3 = attrib1;
            XmlAttribute attrib4 = null;

            Assert.AreEqual(attrib1, attrib1);
            Assert.AreEqual(attrib2, attrib2);
            Assert.AreEqual(attrib3, attrib3);
            Assert.AreEqual(attrib1, attrib3);
            Assert.AreEqual(attrib3, attrib1);
            Assert.AreNotEqual(attrib1, attrib2);
            Assert.AreNotEqual(attrib2, attrib1);
            Assert.AreNotEqual(attrib1, attrib4);
            Assert.AreNotEqual(attrib3, attrib2);
            Assert.AreNotEqual(attrib2, attrib3);
            Assert.AreNotEqual(attrib3, attrib4);
            Assert.AreNotEqual(attrib1, null);
        }

        [Test]
        public void RemoveAttributes()
        {
            var document = new XmlDocument();
            var element = new XmlElement();
            document.DocumentElement.AppendChild(element);
            XmlAttribute attrib1 = new XmlAttribute() { Name = "attrib1", Value = new XmlValue("Value") };
            XmlAttribute attrib2 = new XmlAttribute() { Name = "attrib2", Value = new XmlValue("Value") };
            XmlAttribute attrib3 = new XmlAttribute() { Name = "attrib3", Value = new XmlValue("Value") };

            element.Attributes.Add(attrib1);
            element.Attributes.Add(attrib2);
            element.Attributes.Add(attrib3);

            Assert.AreEqual(3, element.Attributes.Count);

            element.Attributes.Remove(attrib2);

            Assert.AreEqual(2, element.Attributes.Count);
            Assert.AreEqual(null, element.Attributes["attrib2"]);
            Assert.AreEqual(attrib3, attrib1.NextSibling);
            Assert.AreEqual(attrib1, attrib3.PreviousSibling);

            element.Attributes.Remove(attrib1);

            Assert.AreEqual(1, element.Attributes.Count);
            Assert.AreEqual(null, element.Attributes["attrib1"]);
            Assert.AreEqual(null, attrib3.NextSibling);
            Assert.AreEqual(null, attrib3.PreviousSibling);

            element.Attributes.Remove(attrib3);

            Assert.AreEqual(0, element.Attributes.Count);
        }

        [Test]
        public void RemoveAllAttributes()
        {
            var document = new XmlDocument();
            var element = new XmlElement();
            document.DocumentElement.AppendChild(element);
            XmlAttribute attrib1 = new XmlAttribute() { Name = "attrib1", Value = new XmlValue("Value") };
            XmlAttribute attrib2 = new XmlAttribute() { Name = "attrib2", Value = new XmlValue("Value") };
            XmlAttribute attrib3 = new XmlAttribute() { Name = "attrib3", Value = new XmlValue("Value") };

            element.Attributes.Add(attrib1);
            element.Attributes.Add(attrib2);
            element.Attributes.Add(attrib3);

            Assert.IsTrue(element.HasAttributes);
            Assert.AreEqual(3, element.Attributes.Count);

            element.Attributes.Clear();

            Assert.IsFalse(element.HasAttributes);
            Assert.AreEqual(0, element.Attributes.Count);
        }

        [Test]
        public void AppendExistingAttribute()
        {
            var document = new XmlDocument();
            var element = new XmlElement();
            document.DocumentElement.AppendChild(element);
            XmlLinkedNode child1 = new XmlElement() { Name = "child1", Value = new XmlValue("Value") };
            XmlAttribute attrib1 = new XmlAttribute() { Name = "attrib1", Value = new XmlValue("Value") };
            XmlAttribute attrib2 = new XmlAttribute() { Name = "attrib2", Value = new XmlValue("Value") };
            XmlAttribute attrib3 = new XmlAttribute() { Name = "attrib3", Value = new XmlValue("Value") };

            element.Attributes.Add(attrib1);
            element.Attributes.Add(attrib2);
            child1.Attributes.Add(attrib3);
            element.Attributes.Add(attrib3);

            Assert.AreEqual(1, child1.Attributes.Count);
            Assert.AreEqual(3, element.Attributes.Count);
        }

        [Test]
        public void XmlNodeAppendExisting()
        {
            var document = new XmlDocument();
            var root = new XmlElement();
            document.DocumentElement.AppendChild(root);
            XmlLinkedNode child1 = new XmlElement() { Name = "child1", Value = new XmlValue("Value") };
            XmlLinkedNode child2 = new XmlElement() { Name = "child2", Value = new XmlValue("Value") };
            XmlLinkedNode child3 = new XmlElement() { Name = "child3", Value = new XmlValue("Value") };
            XmlLinkedNode child4 = new XmlElement() { Name = "child4", Value = new XmlValue("Value") };
            XmlAttribute attrib1 = new XmlAttribute() { Name = "attrib1", Value = new XmlValue("Value") };
            XmlAttribute attrib2 = new XmlAttribute() { Name = "attrib2", Value = new XmlValue("Value") };

            root.AppendChild(child1);
            child1.AppendChild(child2);
            child2.AppendChild(child3);
            child2.AppendChild(child4);
            child2.Attributes.Add(attrib1);
            child3.Attributes.Add(attrib2);
            root.AppendChild(child2);

            Assert.AreEqual(2, root.Children.Count);
            Assert.AreEqual(2, child2.Children.Count);

            Assert.AreEqual("child2", root.Children.LastChild.Name);
            Assert.AreEqual(1, root.Children.LastChild.Attributes.Count);
            Assert.AreEqual(2, root.Children.LastChild.Children.Count);
            Assert.AreEqual("attrib1", root.Children.LastChild.Attributes.FirstChild.Name);
            Assert.AreEqual("child3", root.Children.LastChild.FirstChild.Name);
            Assert.AreEqual("attrib2", root.Children.LastChild.FirstChild.Attributes.FirstChild.Name);
        }

        [Test]
        public void XmlValue()
        {
            var xmlValue = new XmlValue("50");
            Assert.AreEqual("50", xmlValue.AsString());
            Assert.AreEqual(50, xmlValue.AsInt());
            Assert.AreEqual(50L, xmlValue.AsLong());
            Assert.AreEqual(50.0f, xmlValue.AsFloat());
            Assert.AreEqual(50.0, xmlValue.AsDouble());

            xmlValue = new XmlValue("50.2");
            Assert.AreEqual(50.2f, xmlValue.AsFloat());
            Assert.AreEqual(50.2, xmlValue.AsDouble());

            xmlValue = new XmlValue("TrUe");
            Assert.IsTrue(xmlValue.AsBool());

            xmlValue = new XmlValue("False");
            Assert.IsFalse(xmlValue.AsBool());
        }

        [Test]
        public void GetTextContent()
        {
            var document = XmlDocument.Load(xml5, ParseOptions.IncludeComment);

            var rootElement = document.DocumentElement.FirstChild;
            var firstCategory = rootElement.FirstChild;
            var secondCategory = rootElement.Children[1];
            var commentNode = rootElement.LastChild;

            Assert.AreEqual("This is ", firstCategory.FirstChild.TextContent);
            Assert.AreEqual("some first item", firstCategory.LastChild.TextContent);
            Assert.AreEqual("This is some first item", firstCategory.TextContent);

            Assert.AreEqual("and", secondCategory.FirstChild.TextContent);
            Assert.AreEqual("some second item", secondCategory.LastChild.TextContent);
            Assert.AreEqual("and some second item", secondCategory.TextContent);

            Assert.AreEqual(" comment here", commentNode.TextContent);

            Assert.AreEqual("This is some first item and some second item", rootElement.TextContent);
            Assert.AreEqual("This is some first item and some second item", document.DocumentElement.TextContent);
        }

        [Test]
        public void SetTextContent()
        {
            var document = XmlDocument.Load(xml5, ParseOptions.IncludeComment);

            var rootElement = document.DocumentElement.FirstChild;
            var firstCategory = rootElement.FirstChild;

            firstCategory.TextContent = "TextContent for the fitst category was updated";
            Assert.AreEqual(XmlNodeType.Text, firstCategory.FirstChild.NodeType);
            Assert.AreEqual("TextContent for the fitst category was updated", firstCategory.TextContent);

            rootElement.TextContent = "TextContent for the root node was updated";
            Assert.AreEqual(XmlNodeType.Text, rootElement.FirstChild.NodeType);
            Assert.AreEqual("TextContent for the root node was updated", rootElement.TextContent);
        }

        [Test]
        public void GetType()
        {
            var xmlValue = new XmlValue("Some string");
            Assert.AreEqual(XmlValueType.String, xmlValue.Type);

            xmlValue = new XmlValue(true);
            Assert.AreEqual(XmlValueType.Bool, xmlValue.Type);

            xmlValue = new XmlValue(5);
            Assert.AreEqual(XmlValueType.Int, xmlValue.Type);

            xmlValue = new XmlValue(69598494964984L);
            Assert.AreEqual(XmlValueType.Long, xmlValue.Type);

            xmlValue = new XmlValue(3.5f);
            Assert.AreEqual(XmlValueType.Float, xmlValue.Type);

            xmlValue = new XmlValue(3658464984984984984984.5);
            Assert.AreEqual(XmlValueType.Double, xmlValue.Type);
        }

        [Test]
        public void GetTypeFromString()
        {
            var xmlValue = new XmlValue("Some string");
            Assert.AreEqual(XmlValueType.String, xmlValue.Type);

            xmlValue = new XmlValue("true");
            Assert.AreEqual(XmlValueType.Bool, xmlValue.Type);

            xmlValue = new XmlValue("5");
            Assert.AreEqual(XmlValueType.Int, xmlValue.Type);

            xmlValue = new XmlValue("69598494964984");
            Assert.AreEqual(XmlValueType.Long, xmlValue.Type);

            xmlValue = new XmlValue("342343.5123421");
            Assert.AreEqual(XmlValueType.Float, xmlValue.Type);
        }

        [Test]
        public void InvalidEncoding()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"FFF\"?><root><item></item></root>";
            var document = XmlDocument.Load(xml);
        }

        [Test]
        public void DeclarationPositionOnSave()
        {
            string output;
            string xml = "<root><item/></root><?include somedata?>";
            var document = XmlDocument.Load(xml, ParseOptions.IncludeProcessingInstruction);

            document.DocumentElement.AppendChild(new XmlDeclaration("1.0", XmlEncoding.Utf8));
            Assert.AreEqual(XmlNodeType.Declaration, document.DocumentElement.FirstChild.NodeType);
            document.Save(out output);
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?><root><item/></root><?include somedata?>", output);

            document = XmlDocument.Load(xml, ParseOptions.IncludeProcessingInstruction);
            document.DocumentElement.LastChild.AddBeforeSelf(new XmlDeclaration("1.0", XmlEncoding.Utf8));
            Assert.AreEqual(XmlNodeType.Declaration, document.DocumentElement.FirstChild.NodeType);
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?><root><item/></root><?include somedata?>", output);
        }

        [Test]
        public void InvalidDeclarationPosition()
        {
            Assert.Throws(InvalidDeclarationPositionFunc);
        }

        private void InvalidDeclarationPositionFunc()
        {
            string xml = "<root><item/></root><?xml version=\"1.0\" encoding=\"UTF-8\"?><?include somedata?>";
            XmlDocument.Load(xml);
        }

        [Test]
        public void MissingEndOfTag()
        {
            Assert.Throws(MissingEndOfTagFunc);
        }

        private void MissingEndOfTagFunc()
        {
            string xml = "<root><item/><item></item>";
            XmlDocument.Load(xml);
        }

        [Test]
        public void NotClosedTag()
        {
            Assert.Throws<XmlException>(NotClosedTagFunc);
        }

        private void NotClosedTagFunc()
        {
            string xml = "<root><item/><item></root>";
            XmlDocument.Load(xml);
        }

        [Test]
        public void TwoRootElements()
        {
            Assert.Throws<XmlException>(TwoRootElementsFunc);
        }

        private void TwoRootElementsFunc()
        {
            string xml = "<root><item/><item></root><another><item2 /></another>";
            XmlDocument.Load(xml);
        }

        [Test]
        public void TwoDeclarations()
        {
            Assert.Throws<XmlException>(TwoDeclarationsFunc);
        }

        private void TwoDeclarationsFunc()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"><root><item/><item></root><another><item2 /></another>";
            XmlDocument.Load(xml);
        }

        [Test]
        public void InvalidDeclaration()
        {
            Assert.Throws<XmlException>(InvalidDeclarationFunc);
        }

        private void InvalidDeclarationFunc()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><?xml version=\"2.0\" encoding=\"UTF-16\"?><root><item/><item></root>";
            XmlDocument.Load(xml);
        }

        [Test]
        public void DeclarationInside()
        {
            Assert.Throws<XmlException>(DeclarationInsideFunc);
        }

        private void DeclarationInsideFunc()
        {
            string xml = "<root><item/><?xml version=\"1.0\" encoding=\"UTF-8\"?><item></root>";
            XmlDocument.Load(xml);
        }

        [Test]
        [Ignore("fusetools/unolibs#26", "CPlusPlus")]
        public void ForbiddenCharacters()
        {
            Assert.Throws<XmlException>(ForbiddenCharactersFunc);
        }

        private void ForbiddenCharactersFunc()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><root:item><item></item></root:item>";
            XmlDocument.Load(xml);
        }

        [Test]
        public void AddAttributeToXmlDocument()
        {
            Assert.Throws<XmlException>(AddAttributeToXmlDocumentFunc);
        }

        private void AddAttributeToXmlDocumentFunc()
        {
            var document = new XmlDocument();
            document.DocumentElement.Attributes.Add(new XmlAttribute("attr", "value1"));
        }

        [Test]
        public void AddAttributeToXmlComment()
        {
            Assert.Throws<XmlException>(AddAttributeToXmlCommentFunc);
        }

        private void AddAttributeToXmlCommentFunc()
        {
            var comment = new XmlComment();
            comment.Attributes.Add(new XmlAttribute("attr", "value1"));
        }

        [Test]
        public void AddAttributeToDeclaration()
        {
            Assert.Throws<XmlException>(AddAttributeToDeclarationFunc);
        }

        private void AddAttributeToDeclarationFunc()
        {
            var declaration = new XmlDeclaration();
            declaration.Attributes.Add(new XmlAttribute("attr", "value1"));
        }

        [Test]
        public void AddAttributeToXmlText()
        {
            Assert.Throws<XmlException>(AddAttributeToXmlTextFunc);
        }

        private void AddAttributeToXmlTextFunc()
        {
            var text = new XmlText();
            text.Attributes.Add(new XmlAttribute("attr", "value1"));
        }

        [Test]
        public void AddAttributeToXmlCDATA()
        {
            Assert.Throws<XmlException>(AddAttributeToXmlCDATAFunc);
        }

        private void AddAttributeToXmlCDATAFunc()
        {
            var cdata = new XmlCharacterData();
            cdata.Attributes.Add(new XmlAttribute("attr", "value1"));
        }

        [Test]
        public void AddAttributeWithTheSameName()
        {
            Assert.Throws<XmlException>(AddAttributeWithTheSameNameFunc);
        }

        private void AddAttributeWithTheSameNameFunc()
        {
            string xml = "<root></root>";
            var document = XmlDocument.Load(xml);
            var root = document.DocumentElement.FirstChild;
            root.Attributes.Add(new XmlAttribute("attr", "value1"));
            root.Attributes.Add(new XmlAttribute("attr", "value2"));
        }

        [Test]
        public void AddDeclarationInside()
        {
            Assert.Throws<XmlException>(AddDeclarationInsideFunc);
        }

        private void AddDeclarationInsideFunc()
        {
            string xml = "<root></root>";
            var document = XmlDocument.Load(xml);
            document.DocumentElement.FirstChild.AppendChild(new XmlDeclaration());
        }

        [Test]
        public void AddTwoDeclarations()
        {
            Assert.Throws<XmlException>(AddTwoDeclarationsFunc);
        }

        private void AddTwoDeclarationsFunc()
        {
            var document = new XmlDocument();
            document.DocumentElement.AppendChild(new XmlDeclaration());
            document.DocumentElement.AppendChild(new XmlDeclaration());
        }

        [Test]
        public void AddTwoElementToRoot()
        {
            Assert.Throws<XmlException>(AddTwoElementToRootFunc);
        }

        private void AddTwoElementToRootFunc()
        {
            var document = new XmlDocument();
            document.DocumentElement.AppendChild(new XmlElement());
            document.DocumentElement.AppendChild(new XmlElement());
        }

        [Test]
        public void AddTextToRoot()
        {
            Assert.Throws<XmlException>(AddTextToRootFunc);
        }

        private void AddTextToRootFunc()
        {
            var document = new XmlDocument();
            document.DocumentElement.AppendChild(new XmlText());
        }

        [Test]
        public void AddCommentToRoot()
        {
            Assert.Throws<XmlException>(AddCommentToRootFunc);
        }

        private void AddCommentToRootFunc()
        {
            var document = new XmlDocument();
            document.DocumentElement.AppendChild(new XmlComment());
        }

        [Test]
        public void AddCDATAToRoot()
        {
            Assert.Throws<XmlException>(AddCDATAToRootFunc);
        }

        private void AddCDATAToRootFunc()
        {
            var document = new XmlDocument();
            document.DocumentElement.AppendChild(new XmlCharacterData());
        }

        [Test]
        public void AddChildToTextNode()
        {
            Assert.Throws<XmlException>(AddChildToTextNodeFunc);
        }

        private void AddChildToTextNodeFunc()
        {
            var text = new XmlText();
            text.AppendChild(new XmlElement());
        }

        [Test]
        public void AddChildToDeclarationNode()
        {
            Assert.Throws<XmlException>(AddChildToDeclarationNodeFunc);
        }

        private void AddChildToDeclarationNodeFunc()
        {
            var text = new XmlDeclaration();
            text.AppendChild(new XmlElement());
        }

        [Test]
        public void AddChildToCDATANode()
        {
            Assert.Throws<XmlException>(AddChildToCDATANodeFunc);
        }

        private void AddChildToCDATANodeFunc()
        {
            var cdata = new XmlCharacterData();
            cdata.AppendChild(new XmlElement());
        }

        [Test]
        public void AddChildToCommentNode()
        {
            Assert.Throws<XmlException>(AddChildToCommentNodeFunc);
        }

        private void AddChildToCommentNodeFunc()
        {
            var comment = new XmlComment();
            comment.AppendChild(new XmlElement());
        }
    }
}
