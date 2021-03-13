// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HtmlMonkeyTests
{
    [TestClass]
    public class NavigationTests
    {
        private class NodeInfo
        {
            public Type? Type { get; set; }
            public string? Content { get; set; }
        }

        private readonly List<NodeInfo> ExpectedNodes = new()
        {
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "div" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n    " },
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "table" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n        " },
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "thead" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n            " },
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "tr" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n                " },
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "th" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "Header1" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n                " },
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "th" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "Header2" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n            " },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n        " },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n        " },
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "tbody" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n            " },
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "tr" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n                " },
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "td" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "Cell1-1" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n                " },
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "td" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "Cell1-2" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n            " },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n            " },
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "tr" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n                " },
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "td" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "Cell2-1" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n                " },
            new NodeInfo { Type = typeof(HtmlElementNode), Content = "td" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "Cell2-2" },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n            " },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n        " },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n    " },
            new NodeInfo { Type = typeof(HtmlTextNode), Content = "\r\n" },
        };

        private readonly string html = @"<div>
    <table>
        <thead>
            <tr>
                <th>Header1</th>
                <th>Header2</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Cell1-1</td>
                <td>Cell1-2</td>
            </tr>
            <tr>
                <td>Cell2-1</td>
                <td>Cell2-2</td>
            </tr>
        </tbody>
    </table>
</div>";


        [TestMethod]
        public void Test()
        {
            HtmlDocument document = HtmlDocument.FromHtml(html);

            List<HtmlNode> encounteredNodes = new();
            for (HtmlNode? node = document.RootNodes.First(); node != null; node = node.NavigateNextNode())
                encounteredNodes.Add(node);
            CompareResults(encounteredNodes);

            HtmlNode lastNode = encounteredNodes[^1];
            encounteredNodes.Clear();
            for (HtmlNode? node = lastNode; node != null; node = node.NavigatePrevNode())
                encounteredNodes.Add(node);
            CompareResults(encounteredNodes, true);
        }

        void CompareResults(List<HtmlNode> nodes, bool reverse = false)
        {
            Assert.AreEqual(ExpectedNodes.Count, nodes.Count);
            Func<int, int> getExpectedIndex;
            if (reverse)
                getExpectedIndex = i => ExpectedNodes.Count - 1 - i;
            else
                getExpectedIndex = i => i;

            for (int i = 0; i < ExpectedNodes.Count; i++)
            {
                NodeInfo expected = ExpectedNodes[getExpectedIndex(i)];
                HtmlNode node = nodes[i];

                if (node is HtmlElementNode elementNode)
                {
                    Assert.AreEqual(expected.Type, typeof(HtmlElementNode));
                    Assert.AreEqual(expected.Content, elementNode.TagName);
                }
                else if (node is HtmlTextNode textNode)
                {
                    Assert.AreEqual(expected.Type, typeof(HtmlTextNode));
                    Assert.AreEqual(expected.Content, textNode.Text);
                }
                else Assert.Fail("Unsupported node type");
            }
        }
    }
}
