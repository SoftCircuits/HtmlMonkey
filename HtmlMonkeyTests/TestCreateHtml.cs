// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;

namespace HtmlMonkeyTests
{
    [TestClass]
    public class TestCreateHtml
    {
        private static readonly string Html = @"<!doctype html>
<html>
  <head>
    <title>Title</title>
    <meta name=""description"" content=""This is my test meta description node!"" />
  </head>
  <body>
    <p id=""par1"">
      This is my first paragraph
    </p>
    <p id=""par2"">
      This is my second paragraph
    </p>
  </body>
</html>";

        [TestMethod]
        public void Test()
        {
            HtmlDocument document = new HtmlDocument();

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

            Assert.AreEqual(Html, document.ToHtml());
        }
    }
}
