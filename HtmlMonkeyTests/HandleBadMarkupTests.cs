// Copyright (c) 2019-2025 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;

namespace HtmlMonkeyTests
{
    [TestClass]
    public class HandleBadMarkupTests
    {
        [TestMethod]
        public void MismatchedTagsTests()
        {
            HtmlElementNode elementNode;
            HtmlTextNode textNode;

            // <div><p>Test</div></p> -> <div><p>Test</p></div>
            HtmlDocument document = HtmlDocument.FromHtml("<div><p>Test</div></p>");

            Assert.AreEqual(1, document.RootNodes.Count);
            elementNode = (HtmlElementNode)document.RootNodes[0];
            Assert.AreEqual("div", elementNode.TagName);

            Assert.AreEqual(1, elementNode.Children.Count);
            elementNode = (HtmlElementNode)elementNode.Children[0];
            Assert.AreEqual("p", elementNode.TagName);

            Assert.AreEqual(1, elementNode.Children.Count);
            textNode = (HtmlTextNode)elementNode.Children[0];
            Assert.AreEqual("Test", textNode.Text);


            // <div>Test</p> -> <div>Test</div>
            document = HtmlDocument.FromHtml("<div>Test</p>");

            Assert.AreEqual(1, document.RootNodes.Count);
            elementNode = (HtmlElementNode)document.RootNodes[0];
            Assert.AreEqual("div", elementNode.TagName);

            Assert.AreEqual(1, elementNode.Children.Count);
            textNode = (HtmlTextNode)elementNode.Children[0];
            Assert.AreEqual("Test", textNode.Text);


            // <div><p><span>Test</div> -> <div><p><span>Test</span></p></div>
            document = HtmlDocument.FromHtml("<div><p><span>Test</div>");

            Assert.AreEqual(1, document.RootNodes.Count);
            elementNode = (HtmlElementNode)document.RootNodes[0];
            Assert.AreEqual("div", elementNode.TagName);

            Assert.AreEqual(1, elementNode.Children.Count);
            elementNode = (HtmlElementNode)elementNode.Children[0];
            Assert.AreEqual("p", elementNode.TagName);

            Assert.AreEqual(1, elementNode.Children.Count);
            elementNode = (HtmlElementNode)elementNode.Children[0];
            Assert.AreEqual("span", elementNode.TagName);

            Assert.AreEqual(1, elementNode.Children.Count);
            textNode = (HtmlTextNode)elementNode.Children[0];
            Assert.AreEqual("Test", textNode.Text);
        }

        [TestMethod]
        public void InvalidNestingTests()
        {
            HtmlElementNode elementNode;
            HtmlTextNode textNode;

            // <div><html>Test</html></div> -> <div></div><html>Test</html>
            HtmlDocument document = HtmlDocument.FromHtml("<div><html>Test</html></div>");

            Assert.AreEqual(2, document.RootNodes.Count);
            elementNode = (HtmlElementNode)document.RootNodes[0];
            Assert.AreEqual("div", elementNode.TagName);

            elementNode = (HtmlElementNode)document.RootNodes[1];
            Assert.AreEqual("html", elementNode.TagName);

            Assert.AreEqual(1, elementNode.Children.Count);
            textNode = (HtmlTextNode)elementNode.Children[0];
            Assert.AreEqual("Test", textNode.Text);
        }

        [TestMethod]
        public void NoChildrenTests()
        {
            HtmlElementNode elementNode;
            HtmlTextNode textNode;

            // <br><p>Test</p></br> -> <br /><p>Test</p>
            HtmlDocument document = HtmlDocument.FromHtml("<br><p>Test</p></br>");

            Assert.AreEqual(2, document.RootNodes.Count);
            elementNode = (HtmlElementNode)document.RootNodes[0];
            Assert.AreEqual("br", elementNode.TagName);

            elementNode = (HtmlElementNode)document.RootNodes[1];
            Assert.AreEqual("p", elementNode.TagName);

            Assert.AreEqual(1, elementNode.Children.Count);
            textNode = (HtmlTextNode)elementNode.Children[0];
            Assert.AreEqual("Test", textNode.Text);
        }

        [TestMethod]
        public void NoNestingTests()
        {
            HtmlElementNode elementNode;
            HtmlTextNode textNode;

            // <a><a>Test</a></a> -> <a></a><a>Test</a>
            HtmlDocument document = HtmlDocument.FromHtml("<a><a>Test</a></a>");

            Assert.AreEqual(2, document.RootNodes.Count);
            elementNode = (HtmlElementNode)document.RootNodes[0];
            Assert.AreEqual("a", elementNode.TagName);

            elementNode = (HtmlElementNode)document.RootNodes[1];
            Assert.AreEqual("a", elementNode.TagName);

            Assert.AreEqual(1, elementNode.Children.Count);
            textNode = (HtmlTextNode)elementNode.Children[0];
            Assert.AreEqual("Test", textNode.Text);
        }

        [TestMethod]
        public void NoSelfClosingTests()
        {
            HtmlElementNode elementNode;
            HtmlTextNode textNode;

            // <div><textarea />Test</div> -> <div><textarea>Test</textarea></div>
            HtmlDocument document = HtmlDocument.FromHtml("<div><textarea />Test</div>");

            Assert.AreEqual(1, document.RootNodes.Count);
            elementNode = (HtmlElementNode)document.RootNodes[0];
            Assert.AreEqual("div", elementNode.TagName);

            Assert.AreEqual(1, elementNode.Children.Count);
            elementNode = (HtmlElementNode)elementNode.Children[0];
            Assert.AreEqual("textarea", elementNode.TagName);

            Assert.AreEqual(1, elementNode.Children.Count);
            textNode = (HtmlTextNode)elementNode.Children[0];
            Assert.AreEqual("Test", textNode.Text);
        }
    }
}
