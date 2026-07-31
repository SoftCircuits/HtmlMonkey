// Copyright (c) 2019-2026 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.HtmlMonkey;

namespace HtmlMonkey.Tests
{
    public class HeaderTests
    {
        [Fact]
        public void TestHtmlHeader()
        {
            List<HtmlAttribute>[] htmlHeaders =
            [
                [
                    new("html")
                ],
                [
                    new("HTML"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD HTML 4.01//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/html4/strict.dtd\""),
                ],
                [
                    new HtmlAttribute("html"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD HTML 4.01 Transitional//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/html4/loose.dtd\""),
                ],
                [
                    new HtmlAttribute("html"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD HTML 4.01 Frameset//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/html4/frameset.dtd\""),
                ],
                [
                    new HtmlAttribute("html"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD XHTML 1.0 Strict//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\""),
                ],
                [
                    new HtmlAttribute("html"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD XHTML 1.0 Transitional//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\""),
                ],
                [
                    new HtmlAttribute("html"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD XHTML 1.0 Frameset//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd\""),
                ],
                [
                    new HtmlAttribute("html"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD XHTML 1.1//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\""),
                ],
            ];

            foreach (List<HtmlAttribute> htmlAttributes in htmlHeaders)
            {
                string header = string.Format($"<!DOCTYPE {string.Join(" ", htmlAttributes)}>");
                HtmlDocument document = HtmlDocument.FromHtml(header);
                Assert.Single(document.RootNodes);
                if (document.RootNodes[0] is not HtmlHeaderNode node)
                    Assert.Fail($"Parsed node is not of type {nameof(HtmlHeaderNode)}");
                else
#pragma warning disable IDE0305 // Simplify collection initialization
                    Assert.Equal(htmlAttributes.ToList(), node.Attributes.ToList(), new AttributeComparer());
#pragma warning restore IDE0305 // Simplify collection initialization
            }
        }

        [Fact]
        public void TestXmlHeader()
        {
            List<HtmlAttribute>[] xmlHeaders =
            [
                [
                    new HtmlAttribute("version", "1.0")
                ],
                [
                    new HtmlAttribute("version", "1.0"),
                    new HtmlAttribute("encoding", "UTF-8"),
                ],
                [
                    new HtmlAttribute("version", "1.0"),
                    new HtmlAttribute("encoding", "UTF-8"),
                    new HtmlAttribute("standalone", "no"),
                ],
            ];

            foreach (List<HtmlAttribute> htmlAttributes in xmlHeaders)
            {
                string header = string.Format($"<?XML {string.Join(" ", htmlAttributes)}?>");
                HtmlDocument document = HtmlDocument.FromHtml(header);
                Assert.Single(document.RootNodes);
                if (document.RootNodes[0] is not XmlHeaderNode node)
                    Assert.Fail("Parsed header is not XmlHeaderNode");
                else
#pragma warning disable IDE0305 // Simplify collection initialization
                    Assert.Equal(htmlAttributes.ToList(), node.Attributes.ToList(), new AttributeComparer());
#pragma warning restore IDE0305 // Simplify collection initialization
            }
        }
    }
}
