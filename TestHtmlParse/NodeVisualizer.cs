using HtmlMonkey;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace TestHtmlMonkey
{
    /// <summary>
    /// Class that provides visualization data for a particular node type.
    /// </summary>
    public class Visualizer
    {
        public Func<HtmlNode, string> ShortDescription { get; set; }
        public Func<HtmlNode, string> LongDescription { get; set; }
        public Action<HtmlNode, ListView> PopulateProperties { get; set; }
    }

    /// <summary>
    /// Class to provide HTML node visualization data.
    /// </summary>
    public class NodeVisualizer
    {
        private static string[] EmptyColumns = { };
        private static string[] AttributeColumns = { "Attribute", "Value" };
        private static string[] ParameterColumns = { "Parameter" };
        private static string[] CDataColumns = { "Delimiter", "Value" };

        // Note: To work correctly, derived types must appear before base types.
        private static Dictionary<Type, Visualizer> VisualizerLookup = new Dictionary<Type, Visualizer>
        {
            [typeof(HtmlCDataNode)] = new Visualizer
            {
                ShortDescription = n => ShortDescriptionCData(n as HtmlCDataNode),
                LongDescription = n => LongDescriptionCData(n as HtmlCDataNode),
                PopulateProperties = (n, lvw) => PopulatePropertiesCData(n as HtmlCDataNode, lvw)
            },
            [typeof(HtmlTextNode)] = new Visualizer
            {
                ShortDescription = n => ShortDescriptionText(n as HtmlTextNode),
                LongDescription = n => LongDescriptionText(n as HtmlTextNode),
                PopulateProperties = (n, lvw) => PopulatePropertiesText(n as HtmlTextNode, lvw)
            },
            [typeof(HtmlElementNode)] = new Visualizer
            {
                ShortDescription = n => ShortDescriptionElement(n as HtmlElementNode),
                LongDescription = n => LongDescriptionElement(n as HtmlElementNode),
                PopulateProperties = (n, lvw) => PopulatePropertiesElement(n as HtmlElementNode, lvw)
            },
            [typeof(XmlHeaderNode)] = new Visualizer
            {
                ShortDescription = n => ShortDescriptionXmlHeader(n as XmlHeaderNode),
                LongDescription = n => LongDescriptionXmlHeader(n as XmlHeaderNode),
                PopulateProperties = (n, lvw) => PopulatePropertiesXmlHeader(n as XmlHeaderNode, lvw)
            },
            [typeof(HtmlHeaderNode)] = new Visualizer
            {
                ShortDescription = n => ShortDescriptionDocTypeHeader(n as HtmlHeaderNode),
                LongDescription = n => LongDescriptionDocTypeHeader(n as HtmlHeaderNode),
                PopulateProperties = (n, lvw) => PopulatePropertiesDocTypeHeader(n as HtmlHeaderNode, lvw)
            }
        };

        /// <summary>
        /// Returns a visualizer for the given node. Returns node if node is not a valid node.
        /// </summary>
        /// <param name="node">Node for which to return a visualizer.</param>
        public static Visualizer GetVisualizer(HtmlNode node)
        {
            if (VisualizerLookup.TryGetValue(node.GetType(), out Visualizer visualizer))
                return visualizer;
            Debug.Assert(false);
            return null;
        }

        private static string ShortDescriptionDocTypeHeader(HtmlHeaderNode node) => "<!doctype>";
        private static string ShortDescriptionXmlHeader(XmlHeaderNode node) => "<?xml>";
        private static string ShortDescriptionElement(HtmlElementNode node) => $"<{node.TagName}>";
        private static string ShortDescriptionText(HtmlTextNode node) => $"\"{TruncateEncoded(node.Text, 32)}\"";
        private static string ShortDescriptionCData(HtmlCDataNode node) => $"\"{TruncateEncoded(node.Html, 32)}\"";

        private static string LongDescriptionDocTypeHeader(HtmlHeaderNode node) => string.Empty;
        private static string LongDescriptionXmlHeader(XmlHeaderNode node) => string.Empty;
        private static string LongDescriptionElement(HtmlElementNode node) => string.Empty;
        private static string LongDescriptionText(HtmlTextNode node) => node.Html;
        private static string LongDescriptionCData(HtmlCDataNode node) => node.Html;

        private static void PopulatePropertiesDocTypeHeader(HtmlHeaderNode node, ListView listView)
        {
            InitializeListView(ParameterColumns, listView);
            foreach (var parm in node.Parameters)
                listView.Items.Add(parm);
        }

        private static void PopulatePropertiesXmlHeader(XmlHeaderNode node, ListView listView)
        {
            InitializeListView(AttributeColumns, listView);
            foreach (var att in node.Attributes)
            {
                var item = listView.Items.Add(att.Key);
                item.SubItems.Add(att.Value != null ? att.Value.Value : "(null)");
            }
        }

        private static void PopulatePropertiesElement(HtmlElementNode node, ListView listView)
        {
            InitializeListView(AttributeColumns, listView);
            foreach (var att in node.Attributes)
            {
                var item = listView.Items.Add(att.Key);
                item.SubItems.Add(att.Value != null ? att.Value.Value : "(null)");
            }
        }

        private static void PopulatePropertiesText(HtmlTextNode node, ListView listView)
        {
            InitializeListView(EmptyColumns, listView);
        }

        private static void PopulatePropertiesCData(HtmlCDataNode node, ListView listView)
        {
            InitializeListView(CDataColumns, listView);
            var item = listView.Items.Add("Prefix");
            item.SubItems.Add(node.Prefix ?? string.Empty);
            item = listView.Items.Add("Suffix");
            item.SubItems.Add(node.Suffix ?? string.Empty);
        }

        private static void InitializeListView(string[] columns, ListView listView)
        {
            listView.Columns.Clear();
            listView.Items.Clear();
            for (int i = 0; i < columns.Length; i++)
            {
                var header = listView.Columns.Add(columns[i]);
                header.Width = 240;
            }
        }

        private static string TruncateEncoded(string s, int maxLength)
        {
            bool isTruncated = false;

            if (s == null)
                return string.Empty;
            if (s.Length > maxLength)
            {
                s = s.Substring(0, maxLength);
                isTruncated = true;
            }
            s = CStringEncoder.EncodeString(s);
            if (isTruncated)
                s += "...";
            return s;
        }
    }

    #region String encoding

    /// <summary>
    /// Class to convert text to C-style strings. For example, replaces new lines with
    /// &quot;\r\n&quot;.
    /// </summary>
    public static class CStringEncoder
    {
        private static Dictionary<char, string> EncodeLookup = new Dictionary<char, string>
        {
            ['\\'] = "\\\\",
            ['"'] = "\\\"",
            ['\a'] = "\\a",
            ['\b'] = "\\b",
            ['\f'] = "\\f",
            ['\n'] = "\\n",
            ['\r'] = "\\r",
            ['\t'] = "\\t",
            ['\v'] = "\\v",
        };

        public static string EncodeString(string s)
        {
            StringBuilder builder = new StringBuilder(s.Length + (s.Length / 4));

            foreach (char c in s)
            {
                if (EncodeLookup.TryGetValue(c, out string value))
                    builder.Append(value);
                else if (char.IsControl(c))
                    builder.Append($"\\u{((int)c).ToString("x4")}");
                else
                    builder.Append(c);
            }
            return builder.ToString();
        }
    }

    #endregion

}
