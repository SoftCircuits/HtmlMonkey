// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;
using System.Collections.Generic;
using System.Linq;

namespace HtmlMonkeyTests
{
    [TestClass]
    public class TestFind
    {
        private readonly string html = @"<!doctype html>
<html>
  <head>
    <title>Test Document</title>
    <style type=""text/css"">
      h1 {color:red;}
      p {color:blue;}
    </style>
    <script type=""text/javascript"">
      document.getElementById(""demo"").innerHTML = ""Hello JavaScript!"";
    </script>
    <![CDATA[
      Ignore this text here!!!
    ]]>
  </head>
  <body>
    <!-- Comment tag -->
    <h1>Test Document</h1>
    <p id=""one-of-many"" data-id=""123"">
      Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt
      ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation
      ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in
      reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur
      sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id
      est laborum.
    </p>
    <p data-id=""123"">
      Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt
      ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation
      ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in
      reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur
      sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id
      est laborum.
    </p>
    <p data-id=""123"" class=""last-para"">
      Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt
      ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation
      ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in
      reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur
      sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id
      est laborum.
    </p>
    <div id=""div1"">
      <div id=""div2"">
        <div id=""div3"">
          <p id=""test-paragraph"">Paragraph.</p>
        </div>
      </div>
    </div>
  </body>
</html>
";

        [TestMethod]
        public void Test()
        {
            HtmlDocument document = HtmlDocument.FromHtml(html);

            IEnumerable<HtmlElementNode> elements = document.Find("p");
            Assert.AreEqual(4, elements.Count());

            elements = document.Find("#one-of-many");
            Assert.AreEqual(1, elements.Count());

            elements = document.Find("p#one-of-many");
            Assert.AreEqual(1, elements.Count());

            elements = document.Find("p.last-para");
            Assert.AreEqual(1, elements.Count());

            elements = document.Find("p[data-id=123]");
            Assert.AreEqual(3, elements.Count());

            elements = document.Find("p[data-id:=\"123\"]");
            Assert.AreEqual(3, elements.Count());

            elements = document.Find("p[data-id:=\"[0-9]{3}\"]");
            Assert.AreEqual(3, elements.Count());

            elements = document.Find("body > h1");
            Assert.AreEqual(1, elements.Count());
            Assert.AreEqual("h1", elements.First().TagName);
            Assert.AreEqual("Test Document", elements.First().Text);

            elements = document.Find("#div3");
            Assert.AreEqual(1, elements.Count());

            elements = document.Find("#div1   >   p#test-paragraph");
            Assert.AreEqual(0, elements.Count());

            elements = document.Find("#div1   p#test-paragraph");
            Assert.AreEqual(1, elements.Count());

            IEnumerable<HtmlCDataNode> cDataNodes = document.FindOfType<HtmlCDataNode>();
            Assert.AreEqual(4, cDataNodes.Count());
        }
    }
}
