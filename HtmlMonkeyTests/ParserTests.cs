// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;
using System.Linq;
using System.Threading.Tasks;

namespace HtmlMonkeyTests
{
    [TestClass]
    public class ParserTests
    {
        private readonly static string Html = @"<!doctype html>
<html>
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Home Page - SoftCircuits Programming</title>
    <link href=""/Content/css?v=ZuEFEXhF0MuCq9xXg11nzmzR2Ax_M-qKsw3zKCy9i8U1"" rel=""stylesheet"" />
    <script src=""/bundles/modernizr?v=wBEWDufH_8Md-Pbioxomt90vm6tJN2Pyy9u9zHtWsPo1"" />
</head>
<body>
    <div class=""navbar navbar-inverse navbar-fixed-top"">
        <div class=""container"">
            <div class=""navbar-header"">
                <button type=""button"" class=""navbar-toggle"" data-toggle=""collapse"" data-target="".navbar-collapse"">
                    <span class=""icon-bar"" />
                    <span class=""icon-bar"" />
                    <span class=""icon-bar"" />
                </button>
                <a class=""navbar-brand"" href=""/"">SoftCircuits</a>
            </div>
            <div class=""navbar-collapse collapse"">
                <ul class=""nav navbar-nav"">
                    <li><a href=""/Consulting"">Consulting</a></li>
                    <li><a href=""/Products"">Products</a></li>
                    <li><a href=""/Company/About"">About</a></li>
                </ul>
            </div>
        </div>
    </div>
    <div class=""container body-content"">
        <div class=""company-logo"">
            <a href=""http://www.softcircuits.com"">
            </a>
        </div>
    <div>

    <h1>SoftCircuits</h1>
    <p>SoftCircuits develops desktop and website applications for the Windows platform.</p>

    <h2>Software Consulting</h2>
    <p>
        SoftCircuits has decades of experience developing websites and desktop software for the
        Windows platform.We also work with a network of developers with expertise in various
        technologies. Please<a href=""/Company/Contact"">contact us</a> if you'd like to discuss
        one of your own projects.We'd be happy to help with projects big or small.
    </p>
    <p><a class=""btn btn-primary"" href=""/Consulting"">Learn more &raquo;</a></p>

    <h2>Products</h2>
    <p>
        SoftCircuits currently has a range of products available for the Windows platform.Some of
        them are available in the Microsoft store.These products include our Cygnus Hex Editor,
        Snippets text database and several other Windows applications.
    </p>
    <p><a class=""btn btn-primary"" href=""/Products"">Learn more &raquo;</a></p>

    <h2>Websites</h2>
    <p>
        SoftCircuits also maintains a number of web properties under the
        <a href=""http://www.scwebgroup.com"">SC Web Group brand</a>.
    </p>
    <p><a class=""btn btn-primary"" href=""http://www.scwebgroup.com"">Learn more &raquo;</a></p>

</div>

        <hr />
        <footer>
            <p>
                Copyright &copy; 2024 <a href=""http://www.softcircuits.com"">SoftCircuits</a><br />
                <a href=""http://www.scwebgroup.com"">SC Web Group</a> |
                <a href=""http://www.insiderarticles.com""> Insider Articles</a>
            </p>
        </footer>
    </div>

    <script src=""/bundles/jquery?v=FVs3ACwOLIVInrAl5sdzR2jrCDmVOWFbZMY6g6Q0ulE1"" />

    <script src=""/bundles/bootstrap?v=2Fz3B0iizV2NnnamQFrx-NbYJNTFeBJ2GM05SilbtQU1"" />

</body>
</html>
";

        private readonly string Xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<breakfast_menu>
  <food>
    <name>Belgian Waffles</name>
    <price>$5.95</price>
    <description>Two of our famous Belgian Waffles with plenty of real maple syrup</description>
    <calories>650</calories>
  </food>
  <food>
    <name>Strawberry Belgian Waffles</name>
    <price>$7.95</price>
    <description>Light Belgian waffles covered with strawberries and whipped cream</description>
    <calories>900</calories>
  </food>
  <food>
    <name>Berry-Berry Belgian Waffles</name>
    <price>$8.95</price>
    <description>Light Belgian waffles covered with an assortment of fresh berries and whipped cream</description>
    <calories>900</calories>
  </food>
  <food>
    <name>French Toast</name>
    <price>$4.50</price>
    <description>Thick slices made from our homemade sourdough bread</description>
    <calories>600</calories>
  </food>
  <food>
    <name>Homestyle Breakfast</name>
    <price>$6.95</price>
    <description>Two eggs, bacon or sausage, toast, and our ever-popular hash browns</description>
    <calories>950</calories>
  </food>
</breakfast_menu>
";

        [TestMethod]
        public void ConvertFromMarkupTests()
        {
            // Convert from HTML and back to HTML
            HtmlDocument htmlDoc = HtmlDocument.FromHtml(Html);
            string html = htmlDoc.ToHtml();
            Assert.AreEqual(Html, html);

            // Convert from XML and back to XML
            HtmlDocument xmlDoc = HtmlDocument.FromHtml(Xml);
            string xml = xmlDoc.ToHtml();
            Assert.AreEqual(Xml, xml);
        }

        [TestMethod]
        public async Task ConvertFromMarkupTestsAsync()
        {
            // Convert from HTML and back to HTML
            HtmlDocument htmlDoc = await HtmlDocument.FromHtmlAsync(Html);
            string html = htmlDoc.ToHtml();
            Assert.AreEqual(Html, html);

            // Convert from XML and back to XML
            HtmlDocument xmlDoc = await HtmlDocument.FromHtmlAsync(Xml);
            string xml = xmlDoc.ToHtml();
            Assert.AreEqual(Xml, xml);
        }

        [TestMethod]
        public void ConvertToMarkupTests()
        {
            // Convert from nodes and back to nodes
            HtmlDocument htmlDoc = BuildHtmlDocument();
            string html = htmlDoc.ToHtml();
            AssertNodesAreEqual(htmlDoc.RootNodes, HtmlDocument.FromHtml(html).RootNodes);

            // Convert from nodes and back to nodes
            HtmlDocument xmlDoc = BuildXmlDocument();
            string xml = xmlDoc.ToHtml();
            AssertNodesAreEqual(xmlDoc.RootNodes, HtmlDocument.FromHtml(xml).RootNodes);
        }

        public static HtmlDocument BuildHtmlDocument()
        {
            HtmlDocument document = new();

            // HTML header
            document.RootNodes.Add(new HtmlHeaderNode(new HtmlAttributeCollection { new HtmlAttribute("html") }));
            document.RootNodes.Add(new HtmlTextNode("\r\n"));

            // HTML element
            HtmlElementNode htmlNode = document.RootNodes.Add(new HtmlElementNode("html"));
            htmlNode.Children.Add(new HtmlTextNode("\r\n  "));

            // Head element
            HtmlElementNode headNode = htmlNode.Children.Add(new HtmlElementNode("head"));
            headNode.Children.Add(new HtmlTextNode("\r\n    "));

            // Title element
            HtmlElementNode node = headNode.Children.Add(new HtmlElementNode("title"));
            node.Children.Add(new HtmlTextNode("Title"));

            // Meta element
            headNode.Children.Add(new HtmlTextNode("\r\n    "));
            headNode.Children.Add(new HtmlElementNode("meta", new HtmlAttributeCollection
            {
                new HtmlAttribute("name", "description"),
                new HtmlAttribute("content", "This is my test meta description node!")
            }));
            headNode.Children.Add(new HtmlTextNode("\r\n  "));

            // Body element
            htmlNode.Children.Add(new HtmlTextNode("\r\n  "));
            HtmlElementNode bodyNode = htmlNode.Children.Add(new HtmlElementNode("body"));

            // Comment
            bodyNode.Children.Add(new HtmlTextNode("\r\n    "));
            bodyNode.Children.Add(new HtmlCDataNode("<!--", "-->", " Here's a comment! "));

            // First paragraph
            bodyNode.Children.Add(new HtmlTextNode("\r\n    "));
            node = bodyNode.Children.Add(new HtmlElementNode("p", new HtmlAttributeCollection
            {
                new HtmlAttribute("id", "par1")
            }));
            node.Children.Add(new HtmlTextNode("\r\n      "));
            node.Children.Add(new HtmlTextNode("This is my first paragraph"));
            node.Children.Add(new HtmlTextNode("\r\n    "));

            // Second paragraph
            bodyNode.Children.Add(new HtmlTextNode("\r\n    "));
            node = bodyNode.Children.Add(new HtmlElementNode("p", new HtmlAttributeCollection
            {
                new HtmlAttribute("id", "par2")
            }));
            node.Children.Add(new HtmlTextNode("\r\n      "));
            node.Children.Add(new HtmlTextNode("This is my second paragraph"));
            node.Children.Add(new HtmlTextNode("\r\n    "));

            bodyNode.Children.Add(new HtmlTextNode("\r\n  "));
            htmlNode.Children.Add(new HtmlTextNode("\r\n"));

            return document;
        }

        public static HtmlDocument BuildXmlDocument()
        {
            HtmlDocument document = new();

            // XML header
            document.RootNodes.Add(new XmlHeaderNode(new HtmlAttributeCollection
            {
                new HtmlAttribute("xml"),
                new HtmlAttribute("version", "1.0"),
                new HtmlAttribute("encoding", "UTF-8")
            }));
            document.RootNodes.Add(new HtmlTextNode("\r\n"));

            // Catalog element
            HtmlElementNode catalogNode = document.RootNodes.Add(new HtmlElementNode("catalog"));
            catalogNode.Children.Add(new HtmlTextNode("\r\n  "));

            // Item element
            HtmlElementNode xmlNode = new("plant");
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("common", null, new HtmlNodeCollection { new HtmlTextNode("Bloodroot") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("botanical", null, new HtmlNodeCollection { new HtmlTextNode("Sanguinaria canadensis") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("zone", null, new HtmlNodeCollection { new HtmlTextNode("4") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("light", null, new HtmlNodeCollection { new HtmlTextNode("Mostly Shady") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("price", null, new HtmlNodeCollection { new HtmlTextNode("$2.44") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n  "));
            catalogNode.Children.Add(xmlNode);
            catalogNode.Children.Add(new HtmlTextNode("\r\n  "));

            // Item element
            xmlNode = new("plant");
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("common", null, new HtmlNodeCollection { new HtmlTextNode("Columbine") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("botanical", null, new HtmlNodeCollection { new HtmlTextNode("Aquilegia canadensis") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("zone", null, new HtmlNodeCollection { new HtmlTextNode("3") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("light", null, new HtmlNodeCollection { new HtmlTextNode("Mostly Shady") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("price", null, new HtmlNodeCollection { new HtmlTextNode("$9.37") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n  "));
            catalogNode.Children.Add(xmlNode);
            catalogNode.Children.Add(new HtmlTextNode("\r\n  "));

            // Item element
            xmlNode = new("plant");
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("common", null, new HtmlNodeCollection { new HtmlTextNode("Marsh Marigold") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("botanical", null, new HtmlNodeCollection { new HtmlTextNode("Caltha palustris") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("zone", null, new HtmlNodeCollection { new HtmlTextNode("4") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("light", null, new HtmlNodeCollection { new HtmlTextNode("Mostly Sunny") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("price", null, new HtmlNodeCollection { new HtmlTextNode("$6.81") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n  "));
            catalogNode.Children.Add(xmlNode);
            catalogNode.Children.Add(new HtmlTextNode("\r\n  "));

            // Item element
            xmlNode = new("plant");
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("common", null, new HtmlNodeCollection { new HtmlTextNode("Dutchman's-Breeches") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("botanical", null, new HtmlNodeCollection { new HtmlTextNode("Dicentra cucullaria") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("zone", null, new HtmlNodeCollection { new HtmlTextNode("3") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("light", null, new HtmlNodeCollection { new HtmlTextNode("Mostly Shady") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("price", null, new HtmlNodeCollection { new HtmlTextNode("$6.44") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n  "));
            catalogNode.Children.Add(xmlNode);
            catalogNode.Children.Add(new HtmlTextNode("\r\n  "));

            // Item element
            xmlNode = new("plant");
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("common", null, new HtmlNodeCollection { new HtmlTextNode("Ginger, Wild") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("botanical", null, new HtmlNodeCollection { new HtmlTextNode("Asarum canadense") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("zone", null, new HtmlNodeCollection { new HtmlTextNode("3") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("light", null, new HtmlNodeCollection { new HtmlTextNode("Mostly Shady") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n    "));
            xmlNode.Children.Add(new HtmlElementNode("price", null, new HtmlNodeCollection { new HtmlTextNode("$9.03") }));
            xmlNode.Children.Add(new HtmlTextNode("\r\n  "));
            catalogNode.Children.Add(xmlNode);
            catalogNode.Children.Add(new HtmlTextNode("\r\n"));

            document.RootNodes.Add(new HtmlTextNode("\r\n"));

            return document;
        }

        public static void AssertNodesAreEqual(HtmlNodeCollection nodes, HtmlNodeCollection nodes2)
        {
            Assert.AreEqual(nodes.Count, nodes2.Count);

            for (int i = 0; i < nodes.Count; i++)
            {
                HtmlNode currNode = nodes[i];
                HtmlNode currNode2 = nodes2[i];

                Assert.AreEqual(currNode.GetType(), currNode2.GetType());

                if (currNode is HtmlHeaderNode headerNode)
                {
                    if (currNode2 is HtmlHeaderNode headerNode2)
                        CollectionAssert.AreEqual(headerNode.Attributes.ToList(), headerNode2.Attributes.ToList(), new AttributeComparer());
                }
                else if (currNode is XmlHeaderNode xmlNode)
                {
                    if (currNode2 is XmlHeaderNode xmlNode2)
                        CollectionAssert.AreEqual(xmlNode.Attributes.ToList(), xmlNode2.Attributes.ToList(), new AttributeComparer());
                }
                else if (currNode is HtmlElementNode elementNode)
                {
                    if (currNode2 is HtmlElementNode elementNode2)
                    {
                        Assert.AreEqual(elementNode.TagName, elementNode2.TagName);
                        CollectionAssert.AreEqual(elementNode.Attributes.ToList(), elementNode2.Attributes.ToList(), new AttributeComparer());
                        AssertNodesAreEqual(elementNode.Children, elementNode2.Children);
                    }
                }
                else if (currNode is HtmlCDataNode cDataNode)
                {
                    if (currNode2 is HtmlCDataNode cDataNode2)
                        Assert.AreEqual(cDataNode.InnerHtml, cDataNode2.InnerHtml);
                }
                else if (currNode is HtmlTextNode textNode)
                {
                    if (currNode2 is HtmlCDataNode textNode2)
                        Assert.AreEqual(textNode.Text, textNode2.Text);
                }
                else Assert.Fail("Unknown HtmlNode type!");
            }
        }
    }
}
