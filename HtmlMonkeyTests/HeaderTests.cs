// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;
using System.Collections.Generic;
using System.Linq;

namespace HtmlMonkeyTests
{
    [TestClass]
    public class HeaderTests
    {
        [TestMethod]
        public void TestHtmlHeader()
        {
            List<HtmlAttribute>[] htmlHeaders = new List<HtmlAttribute>[]
            {
                new List<HtmlAttribute> { new HtmlAttribute("html") },
                new List<HtmlAttribute> { new HtmlAttribute("HTML"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD HTML 4.01//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/html4/strict.dtd\""),
                },
                new List<HtmlAttribute> { new HtmlAttribute("html"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD HTML 4.01 Transitional//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/html4/loose.dtd\""),
                },
                new List<HtmlAttribute> { new HtmlAttribute("html"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD HTML 4.01 Frameset//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/html4/frameset.dtd\""),
                },
                new List<HtmlAttribute> { new HtmlAttribute("html"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD XHTML 1.0 Strict//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\""),
                },
                new List<HtmlAttribute> { new HtmlAttribute("html"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD XHTML 1.0 Transitional//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\""),
                },
                new List<HtmlAttribute> { new HtmlAttribute("html"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD XHTML 1.0 Frameset//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd\""),
                },
                new List<HtmlAttribute> { new HtmlAttribute("html"),
                    new HtmlAttribute("PUBLIC"),
                    new HtmlAttribute("\"-//W3C//DTD XHTML 1.1//EN\""),
                    new HtmlAttribute("\"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\""),
                },
            };

            foreach (List<HtmlAttribute> htmlAttributes in htmlHeaders)
            {
                string header = string.Format($"<!DOCTYPE {string.Join(" ", htmlAttributes)}>");
                HtmlDocument document = HtmlDocument.FromHtml(header);
                Assert.AreEqual(1, document.RootNodes.Count);
                if (document.RootNodes[0] is not HtmlHeaderNode node)
                    Assert.Fail();
                else
                    CollectionAssert.AreEqual(htmlAttributes.ToList(), node.Attributes.ToList(), new AttributeComparer());
            }
        }

        [TestMethod]
        public void TestXmlHeader()
        {
            List<HtmlAttribute>[] xmlHeaders = new List<HtmlAttribute>[]
            {
                new List<HtmlAttribute> { new HtmlAttribute("version", "1.0") },
                new List<HtmlAttribute> { new HtmlAttribute("version", "1.0"),
                    new HtmlAttribute("encoding", "UTF-8"),
                },
                new List<HtmlAttribute> { new HtmlAttribute("version", "1.0"),
                    new HtmlAttribute("encoding", "UTF-8"),
                    new HtmlAttribute("standalone", "no"),
                },
            };

            foreach (List<HtmlAttribute> htmlAttributes in xmlHeaders)
            {
                string header = string.Format($"<?XML {string.Join(" ", htmlAttributes)}?>");
                HtmlDocument document = HtmlDocument.FromHtml(header);
                Assert.AreEqual(1, document.RootNodes.Count);
                if (document.RootNodes[0] is not XmlHeaderNode node)
                    Assert.Fail();
                else
                    CollectionAssert.AreEqual(htmlAttributes.ToList(), node.Attributes.ToList(), new AttributeComparer());
            }
        }
    }
}
