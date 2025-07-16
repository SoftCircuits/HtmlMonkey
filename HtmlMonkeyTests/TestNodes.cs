using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.HtmlMonkey;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HtmlMonkeyTests
{
    public class TestTagNode(string tagName, params IEnumerable<TestNode> nodes) : TestNode(typeof(HtmlElementNode), nodes)
    {
        public string TagName { get; set; } = tagName;

        public override bool Equals(HtmlNode? other)
        {
            if (other is not HtmlElementNode tagNode)
                return false;
            return tagNode.TagName.Equals(TagName, HtmlRules.TagStringComparison);
        }

        public override string ToString() => $"Tag = \"{TagName}\"";
    }

    public class TestTextNode(string text, params IEnumerable<TestNode> nodes) : TestNode(typeof(HtmlTextNode), nodes)
    {
        public string Text { get; set; } = text;

        public override bool Equals(HtmlNode? other)
        {
            if (other is not HtmlTextNode textNode)
                return false;
            return textNode.Text.Equals(Text);
        }

        public override string ToString() => $"Text = \"{Text}\"";
    }

    public class TestNode(Type type, params IEnumerable<TestNode> nodes)
    {
        public Type Type { get; set; } = type;
        public List<TestNode> Children { get; set; } = [.. nodes];

        public virtual bool Equals(HtmlNode other)
        {
            Type type = other.GetType();
            return type.IsAssignableFrom(Type);
        }

        public override string ToString() => Type.ToString();

        #region Static Methods

        public static void Test(List<TestNode> expectedNodes, HtmlNodeCollection actualNodes, bool ignoreTextNodes = false)
        {
            TestRecursive(expectedNodes, actualNodes, ignoreTextNodes);
        }

        private static void TestRecursive(List<TestNode> expectedNodes, HtmlNodeCollection actualNodes, bool ignoreTextNodes)
        {
            // Ignore text nodes
            if (ignoreTextNodes)
                actualNodes = [.. actualNodes.Where(n => n is not HtmlTextNode)];

            Assert.AreEqual(expectedNodes.Count, actualNodes.Count, "Node count mismatch");
            for (int i = 0; i < expectedNodes.Count; i++)
            {
                Assert.IsTrue(expectedNodes[i].Equals(actualNodes[i]));
                if (actualNodes[i] is HtmlElementNode elementNode)
                {
                    TestRecursive(expectedNodes[i].Children, elementNode.Children, ignoreTextNodes);
                }
                else
                {
                    Assert.AreEqual(0, expectedNodes[i].Children.Count, "Non-element node should have no children");
                }
            }
        }

        #endregion

    }
}
