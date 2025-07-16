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
        [TestInitialize]
        public void Initialize()
        {
            // Set HTML tag rules for testing
            HtmlRules.TagRules.ClearNestingRules();
            HtmlRules.TagRules.SetNestingRule("html", []);
            HtmlRules.TagRules.SetNestingRule("head", ["html"]);
            HtmlRules.TagRules.SetNestingRule("body", ["html"]);
            HtmlRules.TagRules.SetNestingRule("thead", ["table"]);
            HtmlRules.TagRules.SetNestingRule("tbody", ["table"]);
            HtmlRules.TagRules.SetNestingRule("tfoot", ["table"]);
            HtmlRules.TagRules.SetNestingRule("tr", ["table", "thead", "tbody"]);
            HtmlRules.TagRules.SetNestingRule("td", ["tr"]);
            HtmlRules.TagRules.SetNestingRule("th", ["tr"]);
            HtmlRules.TagRules.SetNestingRule("li", ["ol", "ul"]);
            HtmlRules.TagRules.SetNestingRule("option", ["select", "optgroup"]);
            HtmlRules.TagRules.SetNestingRule("optgroup", ["select"]);
            HtmlRules.TagRules.SetNestingRule("dt", ["dl"]);
            HtmlRules.TagRules.SetNestingRule("dd", ["dl"]);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Reset HTML rules
            HtmlRules.TagRules.ClearNestingRules();
        }

        [TestMethod]
        public void MismatchedTagsTests()
        {
            // <div><p>Test</div></p> -> <div><p>Test</p></div>
            HtmlDocument document = HtmlDocument.FromHtml("<div><p>Test</div></p>");

            TestNode.Test(
                [
                    new TestTagNode("div",
                        new TestTagNode("p",
                            new TestTextNode("Test"))),
                ],
                document.RootNodes);

            // <div>Test</p> -> <div>Test</div>
            document = HtmlDocument.FromHtml("<div>Test</p>");

            TestNode.Test(
                [
                    new TestTagNode("div",
                        new TestTextNode("Test")),
                ],
                document.RootNodes);

            // <div><p><span>Test</div> -> <div><p><span>Test</span></p></div>
            document = HtmlDocument.FromHtml("<div><p><span>Test</div>");

            TestNode.Test(
                [
                    new TestTagNode("div",
                        new TestTagNode("p",
                            new TestTagNode("span",
                                new TestTextNode("Test")))),
                ],
                document.RootNodes);
        }

        [TestMethod]
        public void InvalidNestingTests()
        {
            // <div><html>Test</html></div> -> <div></div><html>Test</html>
            HtmlDocument document = HtmlDocument.FromHtml("<div><html>Test</html></div>");

            TestNode.Test(
                [
                    new TestTagNode("div"),
                    new TestTagNode("html",
                        new TestTextNode("Test")),
                ],
                document.RootNodes);
        }

        [TestMethod]
        public void NoChildrenTests()
        {
            // <br><p>Test</p></br> -> <br /><p>Test</p>
            HtmlDocument document = HtmlDocument.FromHtml("<br><p>Test</p></br>");

            TestNode.Test(
                [
                    new TestTagNode("br"),
                    new TestTagNode("p",
                        new TestTextNode("Test")),
                ],
                document.RootNodes);
        }

        [TestMethod]
        public void NoNestingTests()
        {
            // <a><a>Test</a></a> -> <a></a><a>Test</a>
            HtmlDocument document = HtmlDocument.FromHtml("<a><a>Test</a></a>");

            TestNode.Test(
                [
                    new TestTagNode("a"),
                    new TestTagNode("a",
                        new TestTextNode("Test")),
                ],
                document.RootNodes);
        }

        [TestMethod]
        public void NoSelfClosingTests()
        {
            // <div><textarea />Test</div> -> <div><textarea>Test</textarea></div>
            HtmlDocument document = HtmlDocument.FromHtml("<div><textarea />Test</div>");

            TestNode.Test(
                [
                    new TestTagNode("div",
                        new TestTagNode("textarea",
                            new TestTextNode("Test"))),
                ],
                document.RootNodes);
        }
    }
}
