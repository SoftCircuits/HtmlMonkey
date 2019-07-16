// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;
using System.Collections.Generic;

namespace HtmlMonkeyTests
{
    [TestClass]
    public class TestHeaders
    {
        [TestMethod]
        public void TestHtmlHeader()
        {
            List<Attribute>[] htmlHeaders = new List<Attribute>[]
            {
                new List<Attribute> { new Attribute("html") },
                new List<Attribute> { new Attribute("HTML"),
                    new Attribute("PUBLIC"),
                    new Attribute("\"-//W3C//DTD HTML 4.01//EN\""),
                    new Attribute("\"http://www.w3.org/TR/html4/strict.dtd\""),
                },
                new List<Attribute> { new Attribute("html"),
                    new Attribute("PUBLIC"),
                    new Attribute("\"-//W3C//DTD HTML 4.01 Transitional//EN\""),
                    new Attribute("\"http://www.w3.org/TR/html4/loose.dtd\""),
                },
                new List<Attribute> { new Attribute("html"),
                    new Attribute("PUBLIC"),
                    new Attribute("\"-//W3C//DTD HTML 4.01 Frameset//EN\""),
                    new Attribute("\"http://www.w3.org/TR/html4/frameset.dtd\""),
                },
                new List<Attribute> { new Attribute("html"),
                    new Attribute("PUBLIC"),
                    new Attribute("\"-//W3C//DTD XHTML 1.0 Strict//EN\""),
                    new Attribute("\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\""),
                },
                new List<Attribute> { new Attribute("html"),
                    new Attribute("PUBLIC"),
                    new Attribute("\"-//W3C//DTD XHTML 1.0 Transitional//EN\""),
                    new Attribute("\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\""),
                },
                new List<Attribute> { new Attribute("html"),
                    new Attribute("PUBLIC"),
                    new Attribute("\"-//W3C//DTD XHTML 1.0 Frameset//EN\""),
                    new Attribute("\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd\""),
                },
                new List<Attribute> { new Attribute("html"),
                    new Attribute("PUBLIC"),
                    new Attribute("\"-//W3C//DTD XHTML 1.1//EN\""),
                    new Attribute("\"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\""),
                },
            };

            foreach (List<Attribute> attributes in htmlHeaders)
            {
                string header = string.Format($"<!DOCTYPE {string.Join(" ", attributes)}>");
                HtmlMonkeyDocument document = HtmlMonkeyDocument.FromHtml(header);
                Assert.AreEqual(1, document.RootNodes.Count);
                HtmlHeaderNode node = document.RootNodes[0] as HtmlHeaderNode;
                Assert.AreNotEqual(null, node);
                Attribute.CompareAttributes(attributes, node.Attributes);
            }
        }

        [TestMethod]
        public void TestXmlHeader()
        {
            List<Attribute>[] xmlHeaders = new List<Attribute>[]
            {
                new List<Attribute> { new Attribute("version", "1.0") },
                new List<Attribute> { new Attribute("version", "1.0"),
                    new Attribute("encoding", "UTF-8"),
                    new Attribute("standalone", "no"),
                },
            };

            foreach (List<Attribute> attributes in xmlHeaders)
            {
                string header = string.Format($"<?xml {string.Join(" ", attributes)}?>");
                HtmlMonkeyDocument document = HtmlMonkeyDocument.FromHtml(header);
                Assert.AreEqual(1, document.RootNodes.Count);
                XmlHeaderNode node = document.RootNodes[0] as XmlHeaderNode;
                Assert.AreNotEqual(null, node);
                Attribute.CompareAttributes(attributes, node.Attributes);
            }
        }
    }
}
