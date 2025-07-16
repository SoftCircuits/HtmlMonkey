# HtmlMonkey

[![NuGet version (SoftCircuits.HtmlMonkey)](https://img.shields.io/nuget/v/SoftCircuits.HtmlMonkey.svg?style=flat-square)](https://www.nuget.org/packages/SoftCircuits.HtmlMonkey/)

```
Install-Package SoftCircuits.HtmlMonkey
```

## Overview

HtmlMonkey is a lightweight HTML/XML parser written in C#. It parses HTML or XML into a hierarchy of node objects, which can then be traversed. It also supports searching those nodes using jQuery-like selectors. The library can also be used to create and modify the nodes. And it can generate new HTML or XML from the current nodes.

## Getting Started

You can use either of the static methods `HtmlDocument.FromHtml()` or `HtmlDocument.FromFile()` to parse HTML and create an `HtmlDocument` object. (Note: If you're using WinForms, watch out for conflict with `System.Windows.Forms.HtmlDocument`.)

#### Parse an HTML Document

```cs
string html = "...";   // HTML markup
HtmlDocument document = HtmlDocument.FromHtml(html);
```

This code parses the HTML document into a hierarchy of nodes and returns a new `HtmlDocument` object. The `HtmlDocument.RootNodes` property contains the top-level nodes that were parsed.

#### Types of Nodes

The parsed nodes can include several different types of nodes, as outlined in the table below. All node types derive from the abstract class `HtmlNode`.

| Node Type | Description |
| --------- | ----------- |
| `HtmlElementNode` | Represents an HTML element, or tag. This is the only node type that can contain child nodes. |
| `HtmlTextNode` | Represents raw text in the document. |
| `HtmlCDataNode` | Represents any block of data like a comment or CDATA section. The library creates a node for these blocks but does not parse their contents. |
| `HtmlHeaderNode` | Represents an HTML document header. |
| `XmlHeaderNode` | Represents an XML document header. |

## Navigating Parsed Nodes

HtmlMonkey provides several ways to navigate parsed nodes. Each `HtmlElementNode` node includes a `Children` property, which can be used to access that node's children. In addition, all nodes have `NextNode`, `PrevNode`, and `ParentNode` properties, which you can use to navigate the nodes in every direction.

The `HtmlDocument` class also includes a `Find()` method, which accepts a predicate argument. This method will recursively find all the nodes in the document for which the predicate returns true, and return those nodes in a flat list.

```cs
// Returns all nodes that are the first node of its parent
IEnumerable<HtmlNode> nodes = document.Find(n => n.PrevNode == null);
```

You can also use the `FindOfType()` method. This method traverses the entire document tree to find all the nodes of the specified type.

```cs
// Returns all text nodes
IEnumerable<HtmlTextNode> nodes = document.FindOfType<HtmlTextNode>();
```

The `FindOfType()` method is also overloaded to accept an optional predicate argument.

```cs
// Returns all HtmlElementNodes that have children
IEnumerable<HtmlElementNode> nodes = document.FindOfType<HtmlElementNode>(n => n.Children.Any());
```

## Using Selectors

The `HtmlDocument.Find()` method also has an overload that supports using jQuery-like selectors to find nodes. Selectors provide a powerful and flexible way to locate nodes.

#### Specifying Tag Names

You can specify a tag name to return all the nodes with that tag.

```cs
// Get all <p> tags in the document
// Search is not case-sensitive
IEnumerable<HtmlElementNode> nodes = document.Find("p");

// Get all HtmlElementNode nodes (tags) in the document
// Same result as not specifying the tag name
// Also the same result as document.FindOfType<HtmlElementNode>();
nodes = document.Find("*");
```

#### Specifying Attributes

There are several ways to search for nodes with specific attributes. You can use the pound (#), period (.) or colon (:) to specify a value for the `id`, `class` or `type` attribute, respectively.

```cs
// Get any nodes with the attribute id="center-ad"
IEnumerable<HtmlElementNode> nodes = document.Find("#center-ad");

// Get any <div> tags with the attribute class="align-right"
nodes = document.Find("div.align-right");

// Returns all <input> tags with the attribute type="button"
nodes = document.Find("input:button");
```

For greater control over attributes, you can use square brackets ([]). This is similar to specifying attributes in jQuery, but there are some differences. The first difference is that all the variations for finding a match at the start, middle or end are not supported by HtmlMonkey. Instead, HtmlMonkey allows you to use the `:=` operator to specify that the value is a regular expression and the code will match if the attribute value matches that regular expression.

```cs
// Get any <p> tags with the attribute id="center-ad"
IEnumerable<HtmlElementNode> nodes = document.Find("p[id=\"center-ad\"]");

// Get any <p> tags that have both attributes id="center-ad" and class="align-right"
// Quotes within the square brackets are optional if the value contains no whitespace or most punctuation.
nodes = document.Find("p[id=center-ad][class=align-right]");

// Returns all <a> tags that have an href attribute
// The value of that attribute does not matter
nodes = document.Find("a[href]");

// Get any <p> tags with the attribute data-id with a value that matches the regular
// expression "abc-\d+"
// Not case-sensitive
nodes = document.Find("p[data-id:=\"abc-\\d+\"]");

// Finds all <a> links that link to blackbeltcoder.com
// Uses a regular expression to allow optional http:// or https://, and www. prefix
// This example is also not case-sensitive
nodes = document.Find("a[href:=\"^(http:\\/\\/|https:\\/\\/)?(www\\.)?blackbeltcoder.com\"]");
```

Note that there is one key difference when using square brackets. When using a pound (#), period (.) or colon (:) to specify an attribute value, it is considered a match if it matches any value within that attribute. For example, the selector `div.right-align` would match the attribute `class="main-content right-align"`. When using square brackets, it must match the entire value (although there are exceptions to this when using regular expressions).

#### Multiple Selectors

There are several cases where you can specify multiple selectors.

```cs
// Returns all <a>, <div> and <p> tags
IEnumerable<HtmlElementNode> nodes = document.Find("a, div, p");

// Returns all <span> tags that are descendants of a <div> tag
nodes = document.Find("div span");

// Returns all <span> tags that are a direct descendant of a <div> tag
nodes = document.Find("div > span");
```

#### Selector Performance

Obviously, there is some overhead parsing selectors. If you want to use the same selectors more than once, you can optimize your code by parsing the selectors into data structures and then passing those data structures to the find methods. The following code is further optimized by first finding a set of container nodes, and then potentially performing multiple searches against those container nodes.

```cs
// Parse selectors into SelectorCollections
SelectorCollection containerSelectors = Selector.ParseSelector("div.container");
SelectorCollection itemSelectors = Selector.ParseSelector("p.item");

// Search document for container nodes
IEnumerable<HtmlElementNode> containerNodes = containerSelectors.Find(document.RootNodes);

// Finally, search container nodes for item nodes
IEnumerable<HtmlElementNode> itemNodes = itemSelectors.Find(containerNodes);
```

#### HTML Rules

There are a lot of rules that can apply to HTML and XML documents. These rules can determine how the markup is parsed. For example,
`<a>` tags cannot be nested. And `<li>` tags must be a child of either an `<ol>` or `<ul>` tag. If these rules are set, HTMLMonkey
will terminate the previous tag before starting the new tag when that new tag is not valid as a child of the previous tag.

These rules can be accessed and modified using the `TagRules` property of the `HtmlRules` class. `TagRules` tracks two kinds of rules:
It defines attributes of HTML tags, and it defines nesting rules for HTML tags. The attributes include whether the tag is a self-closing
tag, whether it can have children, whether it can be nested, etc. The nesting rules define which tags can be nested within other tags.
You can specify all the tags that a particular HTML tag can be a child of.

Tags with no attributes set, default to `HtmlTagAttributes.None`. Tags with no nesting rules set, default to no restrictions on which
tags those tags can be a child of. (Note that this is different from having an empty list of nesting rules, which means that the tag
cannot be a child of any other tag.)

By default, attributes are set for common HTML tags, and no nesting rules are set. This means that, by default, all HTML tags can be
nested within any other HTML tag. You can modify these rules to suit your needs. For example, the following code clears any existing
nesting rules and then sets some common HTML nesting rules.

```cs
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
```
