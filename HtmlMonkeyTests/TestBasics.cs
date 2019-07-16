// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;
using System;

namespace HtmlMonkeyTests
{
    [TestClass]
    public class TestBasics
    {
        class NodeInfo
        {
            public Type Type { get; set; }
            public string TagName { get; set; }
            public string Text { get; set; }
            public Attribute[] Attributes { get; set; }
            public NodeInfo[] ChildNodes { get; set; }
        }

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
    <p data-id=""123"">
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
  </body>
</html>
";

        /// <summary>
        /// Data that matches HTML string above
        /// </summary>
        private readonly NodeInfo NodeData = new NodeInfo
        {
            Type = typeof(HtmlNode),
            ChildNodes = new NodeInfo[]
            {
                new NodeInfo
                {
                    Type = typeof(HtmlHeaderNode),
                    Attributes = new [] { new Attribute("html") },
                },
                new NodeInfo
                {
                    Type = typeof(HtmlTextNode),
                    Text = "\r\n"
                },
                new NodeInfo
                {
                    Type = typeof(HtmlElementNode),
                    TagName = "html",
                    Attributes = new Attribute[] { },
                    ChildNodes = new NodeInfo[]
                    {
                        new NodeInfo
                        {
                            Type = typeof(HtmlTextNode),
                            Text = "\r\n  "
                        },
                        new NodeInfo
                        {
                            Type = typeof(HtmlElementNode),
                            TagName = "head",
                            Attributes = new Attribute[] { },
                            ChildNodes = new NodeInfo[]
                            {
                                new NodeInfo
                                {
                                    Type = typeof(HtmlTextNode),
                                    Text = "\r\n    ",
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlElementNode),
                                    TagName = "title",
                                    Attributes = new Attribute[] { },
                                    ChildNodes = new NodeInfo[]
                                    {
                                        new NodeInfo
                                        {
                                            Type = typeof(HtmlTextNode),
                                            Text = "Test Document",
                                        },
                                    }
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlTextNode),
                                    Text = "\r\n    ",
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlElementNode),
                                    TagName = "style",
                                    Attributes = new [] { new Attribute("type", "text/css") },
                                    ChildNodes = new NodeInfo[]
                                    {
                                        new NodeInfo
                                        {
                                            Type = typeof(HtmlCDataNode),
                                            Text = @"
      h1 {color:red;}
      p {color:blue;}
    ",
                                        },
                                    }
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlTextNode),
                                    Text = "\r\n    ",
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlElementNode),
                                    TagName = "script",
                                    Attributes = new [] { new Attribute("type", "text/javascript") },
                                    ChildNodes = new NodeInfo[]
                                    {
                                        new NodeInfo
                                        {
                                            Type = typeof(HtmlCDataNode),
                                            Text = @"
      document.getElementById(""demo"").innerHTML = ""Hello JavaScript!"";
    ",
                                        },
                                    },
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlTextNode),
                                    Text = "\r\n    ",
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlCDataNode),
                                    Text = @"
      Ignore this text here!!!
    ",
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlTextNode),
                                    Text = "\r\n  ",
                                },
                            },
                        },
                        new NodeInfo
                        {
                            Type = typeof(HtmlTextNode),
                            Text = "\r\n  "
                        },
                        new NodeInfo
                        {
                            Type = typeof(HtmlElementNode),
                            TagName = "body",
                            Attributes = new Attribute[] { },
                            ChildNodes = new NodeInfo[]
                            {
                                new NodeInfo
                                {
                                    Type = typeof(HtmlTextNode),
                                    Text = "\r\n    "
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlCDataNode),
                                    Text = @" Comment tag ",
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlTextNode),
                                    Text = "\r\n    "
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlElementNode),
                                    TagName = "h1",
                                    Attributes = new Attribute[] { },
                                    ChildNodes = new NodeInfo[]
                                    {
                                        new NodeInfo
                                        {
                                            Type = typeof(HtmlTextNode),
                                            Text = "Test Document"
                                        },
                                    },
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlTextNode),
                                    Text = "\r\n    "
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlElementNode),
                                    TagName = "p",
                                    Attributes = new [] { new Attribute("data-id", "123") },
                                    ChildNodes = new NodeInfo[]
                                    {
                                        new NodeInfo
                                        {
                                            Type = typeof(HtmlTextNode),
                                            Text = @"
      Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt
      ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation
      ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in
      reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur
      sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id
      est laborum.
    ",
                                        }
                                    }
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlTextNode),
                                    Text = "\r\n    "
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlElementNode),
                                    TagName = "p",
                                    Attributes = new [] { new Attribute("data-id", "123") },
                                    ChildNodes = new NodeInfo[]
                                    {
                                        new NodeInfo
                                        {
                                            Type = typeof(HtmlTextNode),
                                            Text = @"
      Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt
      ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation
      ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in
      reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur
      sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id
      est laborum.
    ",
                                        }
                                    }
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlTextNode),
                                    Text = "\r\n    "
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlElementNode),
                                    TagName = "p",
                                    Attributes = new [] { new Attribute("data-id", "123"), new Attribute("class", "last-para") },
                                    ChildNodes = new NodeInfo[]
                                    {
                                        new NodeInfo
                                        {
                                            Type = typeof(HtmlTextNode),
                                            Text = @"
      Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt
      ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation
      ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in
      reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur
      sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id
      est laborum.
    ",
                                        }
                                    }
                                },
                                new NodeInfo
                                {
                                    Type = typeof(HtmlTextNode),
                                    Text = "\r\n  "
                                },
                            },
                        },
                        new NodeInfo
                        {
                            Type = typeof(HtmlTextNode),
                            Text = "\r\n"
                        },
                    },
                },
                new NodeInfo
                {
                    Type = typeof(HtmlTextNode),
                    Text = "\r\n"
                },
            }
        };

        [TestMethod]
        public void Test()
        {
            HtmlMonkeyDocument document = HtmlMonkeyDocument.FromHtml(html);
            NodeInfo info = NodeData;
            TestRecursive(info, document.RootNodes);
        }

        void TestRecursive(NodeInfo parentInfo, HtmlNodeCollection nodes)
        {
            Assert.AreEqual(parentInfo.ChildNodes.Length, nodes.Count);
            if (parentInfo.ChildNodes.Length == nodes.Count)
            {
                for (int i = 0; i < parentInfo.ChildNodes.Length; i++)
                {
                    NodeInfo info = parentInfo.ChildNodes[i];
                    Assert.AreEqual(info.Type, nodes[i].GetType());
                    if (nodes[i] is HtmlHeaderNode headerNode)
                    {
                        Assert.AreEqual(info.Type, typeof(HtmlHeaderNode));
                        Attribute.CompareAttributes(info.Attributes, headerNode.Attributes);
                    }
                    else if (nodes[i] is XmlHeaderNode xmlNode)
                    {
                        Assert.AreEqual(info.Type, typeof(XmlHeaderNode));
                        Attribute.CompareAttributes(info.Attributes, xmlNode.Attributes);
                    }
                    else if (nodes[i] is HtmlElementNode elementNode)
                    {
                        Assert.AreEqual(info.Type, typeof(HtmlElementNode));
                        Assert.AreEqual(info.TagName, elementNode.TagName);
                        Attribute.CompareAttributes(info.Attributes, elementNode.Attributes);
                        if (info.ChildNodes != null)
                            TestRecursive(info, elementNode.Children);
                        else
                            Assert.AreEqual(0, elementNode.Children.Count);
                    }
                    else if (nodes[i] is HtmlTextNode textNode)
                    {
                        Assert.AreEqual(info.Text, textNode.Text);
                    }
                    else if (nodes[i] is HtmlCDataNode cDataNode)
                    {
                        Assert.AreEqual(info.Text, cDataNode.Text);
                    }
                }
            }
        }
    }
}
