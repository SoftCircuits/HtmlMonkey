// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;
using System;
using System.Diagnostics;

namespace HtmlMonkeyTests
{
    [TestClass]
    public class TestParser
    {
        class NodeInfo
        {
            public Type Type { get; set; }
            public string TagName { get; set; }
            public string Text { get; set; }
            public Attribute[] Attributes { get; set; }
            public NodeInfo[] ChildNodes { get; set; }
        }

        private readonly string[] Markup = new string[] {
            @"<!doctype html>
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
",
            @"<?xml version=""1.0""?>
<catalog>
  <book id=""bk101"">
    <author>Gambardella, Matthew</author>
    <title>XML Developer's Guide</title>
    <genre>Computer</genre>
    <price>44.95</price>
    <publish_date>2000-10-01</publish_date>
    <description>An in-depth look at creating applications
    with XML.</description>
  </book>
  <book id=""bk102"">
    <author>Ralls, Kim</author>
    <title>Midnight Rain</title>
    <genre>Fantasy</genre>
    <price>5.95</price>
    <publish_date>2000-12-16</publish_date>
    <description>A former architect battles corporate zombies,
    an evil sorceress, and her own childhood to become queen
    of the world.</description>
  </book>
  <book id=""bk103"">
    <author>Corets, Eva</author>
    <title>Maeve Ascendant</title>
    <genre>Fantasy</genre>
    <price>5.95</price>
    <publish_date>2000-11-17</publish_date>
    <description>After the collapse of a nanotechnology
    society in England, the young survivors lay the
    foundation for a new society.</description>
  </book>
  <book id=""bk104"">
    <author>Corets, Eva</author>
    <title>Oberon's Legacy</title>
    <genre>Fantasy</genre>
    <price>5.95</price>
    <publish_date>2001-03-10</publish_date>
    <description>In post-apocalypse England, the mysterious
    agent known only as Oberon helps to create a new life
    for the inhabitants of London. Sequel to Maeve
    Ascendant.</description>
  </book>
</catalog>",
        };

        /// <summary>
        /// Data that matches HTML string above
        /// </summary>
        private readonly NodeInfo[] NodeData = new NodeInfo[]
        {
            new NodeInfo
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
                },
            },
            new NodeInfo
            {
                Type = typeof(HtmlNode),
                ChildNodes = new NodeInfo[]
                {
                    new NodeInfo
                    {
                        Type = typeof(XmlHeaderNode),
                        Attributes = new [] { new Attribute("version", "1.0") },
                    },
                    new NodeInfo
                    {
                        Type = typeof(HtmlTextNode),
                        Text = "\r\n"
                    },
                    new NodeInfo
                    {
                        Type = typeof(HtmlElementNode),
                        TagName = "catalog",
                        ChildNodes = new NodeInfo[]
                        {
                            new NodeInfo
                            {
                                Type = typeof(HtmlTextNode),
                                Text = "\r\n  ",
                            },
                            new NodeInfo
                            {
                                Type = typeof(HtmlElementNode),
                                TagName = "book",
                                Attributes = new [] { new Attribute("id", "bk101") },
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
                                        TagName = "author",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "Gambardella, Matthew"
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
                                        TagName = "title",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "XML Developer's Guide"
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
                                        TagName = "genre",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "Computer"
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
                                        TagName = "price",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "44.95"
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
                                        TagName = "publish_date",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "2000-10-01"
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
                                        TagName = "description",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = @"An in-depth look at creating applications
    with XML."
                                            },
                                        }
                                    },
                                    new NodeInfo
                                    {
                                        Type = typeof(HtmlTextNode),
                                        Text = "\r\n  ",
                                    },
                                }
                            },
                            new NodeInfo
                            {
                                Type = typeof(HtmlTextNode),
                                Text = "\r\n  "
                            },
                            new NodeInfo
                            {
                                Type = typeof(HtmlElementNode),
                                TagName = "book",
                                Attributes = new [] { new Attribute("id", "bk102") },
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
                                        TagName = "author",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "Ralls, Kim"
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
                                        TagName = "title",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "Midnight Rain"
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
                                        TagName = "genre",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "Fantasy"
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
                                        TagName = "price",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "5.95"
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
                                        TagName = "publish_date",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "2000-12-16"
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
                                        TagName = "description",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = @"A former architect battles corporate zombies,
    an evil sorceress, and her own childhood to become queen
    of the world."
                                            },
                                        }
                                    },
                                    new NodeInfo
                                    {
                                        Type = typeof(HtmlTextNode),
                                        Text = "\r\n  ",
                                    },
                                }
                            },
                            new NodeInfo
                            {
                                Type = typeof(HtmlTextNode),
                                Text = "\r\n  "
                            },
                            new NodeInfo
                            {
                                Type = typeof(HtmlElementNode),
                                TagName = "book",
                                Attributes = new [] { new Attribute("id", "bk103") },
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
                                        TagName = "author",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "Corets, Eva"
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
                                        TagName = "title",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "Maeve Ascendant"
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
                                        TagName = "genre",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "Fantasy"
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
                                        TagName = "price",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "5.95"
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
                                        TagName = "publish_date",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "2000-11-17"
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
                                        TagName = "description",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = @"After the collapse of a nanotechnology
    society in England, the young survivors lay the
    foundation for a new society."
                                            },
                                        }
                                    },
                                    new NodeInfo
                                    {
                                        Type = typeof(HtmlTextNode),
                                        Text = "\r\n  ",
                                    },
                                }
                            },
                            new NodeInfo
                            {
                                Type = typeof(HtmlTextNode),
                                Text = "\r\n  "
                            },
                            new NodeInfo
                            {
                                Type = typeof(HtmlElementNode),
                                TagName = "book",
                                Attributes = new [] { new Attribute("id", "bk104") },
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
                                        TagName = "author",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "Corets, Eva"
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
                                        TagName = "title",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "Oberon's Legacy"
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
                                        TagName = "genre",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "Fantasy"
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
                                        TagName = "price",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "5.95"
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
                                        TagName = "publish_date",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = "2001-03-10"
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
                                        TagName = "description",
                                        ChildNodes = new NodeInfo[]
                                        {
                                            new NodeInfo
                                            {
                                                Type = typeof(HtmlTextNode),
                                                Text = @"In post-apocalypse England, the mysterious
    agent known only as Oberon helps to create a new life
    for the inhabitants of London. Sequel to Maeve
    Ascendant."
                                            },
                                        }
                                    },
                                    new NodeInfo
                                    {
                                        Type = typeof(HtmlTextNode),
                                        Text = "\r\n  ",
                                    },
                                }
                            },
                            new NodeInfo
                            {
                                Type = typeof(HtmlTextNode),
                                Text = "\r\n"
                            },
                        }
                    }
                }
            },
        };

        [TestMethod]
        public void Test()
        {
            Debug.Assert(NodeData.Length == Markup.Length);

            for (int i = 0; i < Markup.Length; i++)
            {
                HtmlDocument document = HtmlDocument.FromHtml(Markup[i]);
                NodeInfo info = NodeData[i];
                TestRecursive(info, document.RootNodes);
            }
        }

        void TestRecursive(NodeInfo parentInfo, HtmlNodeCollection nodes)
        {
            Assert.AreNotEqual(null, parentInfo);
            Assert.AreNotEqual(null, nodes);
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
                    else if (nodes[i] is HtmlCDataNode cDataNode)
                    {
                        Assert.AreEqual(info.Text, cDataNode.InnerHtml);
                    }
                    else if (nodes[i] is HtmlTextNode textNode)
                    {
                        Assert.AreEqual(info.Text, textNode.Text);
                    }
                    else Assert.Fail("Unknown HtmlNode type!");
                }
            }
        }
    }
}
