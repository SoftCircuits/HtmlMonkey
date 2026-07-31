// Copyright (c) 2019-2026 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.HtmlMonkey;

namespace HtmlMonkey.Tests
{
    public class FindTests
    {
        private readonly string html =
            """
            <!doctype html>
            <html>
              <head>
                <title>Test Document</title>
                <style type="text/css">
                  h1 {color:red;}
                  p {color:blue;}
                </style>
                <script type="text/javascript">
                  document.getElementById("demo").innerHTML = "Hello JavaScript!";
                </script>
                <![CDATA[
                  Ignore this text here!!!
                ]]>
              </head>
              <body>
                <!-- Comment tag -->
                <h1>Test Document</h1>
                <p id="one-of-many" data-id="123">
                  Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt
                  ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation
                  ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in
                  reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur
                  sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id
                  est laborum.
                </p>
                <p data-id="123">
                  Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt
                  ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation
                  ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in
                  reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur
                  sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id
                  est laborum.
                </p>
                <p data-id="123" class="abc last-para def">
                  Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt
                  ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation
                  ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in
                  reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur
                  sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id
                  est laborum.
                </p>
                <p>
                    <input type="text" id="name">
                </p>
                <div id="div1">
                  <div id="div2">
                    <div id="div3">
                      <p id="test-paragraph">Paragraph.</p>
                    </div>
                  </div>
                </div>
              </body>
            </html>
            """;

        [Fact]
        public void Test()
        {
            HtmlDocument document = HtmlDocument.FromHtml(html);

            IEnumerable<HtmlElementNode> elements = document.Find("p");
            Assert.Equal(5, elements.Count());

            elements = document.Find("#one-of-many");
            Assert.Single(elements);

            elements = document.Find("p#one-of-many");
            Assert.Single(elements);

            elements = document.Find(".last-para");
            Assert.Single(elements);

            elements = document.Find("p.last-para");
            Assert.Single(elements);

            elements = document.Find(":text");
            Assert.Single(elements);

            elements = document.Find("input:text");
            Assert.Single(elements);

            elements = document.Find("p[data-id=123]");
            Assert.Equal(3, elements.Count());

            elements = document.Find("p[data-id:=\"123\"]");
            Assert.Equal(3, elements.Count());

            elements = document.Find("p[data-id:=\"[0-9]{3}\"]");
            Assert.Equal(3, elements.Count());

            elements = document.Find("body > h1");
            Assert.Single(elements);
            Assert.Equal("h1", elements.First().TagName);
            Assert.Equal("Test Document", elements.First().Text);

            elements = document.Find("body>h1");
            Assert.Single(elements);
            Assert.Equal("h1", elements.First().TagName);
            Assert.Equal("Test Document", elements.First().Text);

            elements = document.Find("#div3");
            Assert.Single(elements);

            elements = document.Find("#div1   >   p#test-paragraph");
            Assert.Empty(elements);

            elements = document.Find("#div1   p#test-paragraph");
            Assert.Single(elements);

            IEnumerable<HtmlCDataNode> cDataNodes = document.FindOfType<HtmlCDataNode>();
            Assert.Equal(4, cDataNodes.Count());
        }

        [Fact]
        public void TestExtensionMethods()
        {
            HtmlDocument document = HtmlDocument.FromHtml(html);
            IEnumerable<HtmlNode> nodes = [.. document.RootNodes];

            IEnumerable<HtmlElementNode> elements = nodes.Find("p");
            Assert.Equal(5, elements.Count());

            elements = nodes.Find("#one-of-many");
            Assert.Single(elements);

            elements = nodes.Find("p#one-of-many");
            Assert.Single(elements);

            elements = nodes.Find(".last-para");
            Assert.Single(elements);

            elements = nodes.Find("p.last-para");
            Assert.Single(elements);

            elements = nodes.Find(":text");
            Assert.Single(elements);

            elements = nodes.Find("input:text");
            Assert.Single(elements);

            elements = nodes.Find("p[data-id=123]");
            Assert.Equal(3, elements.Count());

            elements = nodes.Find("p[data-id:=\"123\"]");
            Assert.Equal(3, elements.Count());

            elements = nodes.Find("p[data-id:=\"[0-9]{3}\"]");
            Assert.Equal(3, elements.Count());

            elements = nodes.Find("body > h1");
            Assert.Single(elements);
            Assert.Equal("h1", elements.First().TagName);
            Assert.Equal("Test Document", elements.First().Text);

            elements = nodes.Find("body>h1");
            Assert.Single(elements);
            Assert.Equal("h1", elements.First().TagName);
            Assert.Equal("Test Document", elements.First().Text);

            elements = nodes.Find("#div3");
            Assert.Single(elements);

            elements = nodes.Find("#div1   >   p#test-paragraph");
            Assert.Empty(elements);

            elements = document.Find("#div1   p#test-paragraph");
            Assert.Single(elements);

            IEnumerable<HtmlCDataNode> cDataNodes = nodes.FindOfType<HtmlCDataNode>();
            Assert.Equal(4, cDataNodes.Count());
        }
    }
}
