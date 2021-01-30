# HtmlMonkey

[![NuGet version (SoftCircuits.HtmlMonkey)](https://img.shields.io/nuget/v/SoftCircuits.HtmlMonkey.svg?style=flat-square)](https://www.nuget.org/packages/SoftCircuits.HtmlMonkey/)

```
Install-Package SoftCircuits.HtmlMonkey
```

## Overview

HtmlMonkey is a lightweight HTML/XML parser written in C#. It allows you to parse HTML or XML into a hierarchy of document node objects, which can then be traversed, or queried using jQuery-like selectors. The node objects can be modified or even built from scratch using code. Finally, you can use the classes to generate HTML or XML strings from the data.

The code also include a WinForms application to display the parsed data nodes. This was mostly done for testing the parser, but offers some functionality that may be useful for inspecting the original markup.

## Getting Started

You can use either of the static methods `HtmlDocument.FromHtml()` or `HtmlDocument.FromFile()` to parse HTML and create an `HtmlDocument` object. (Note: If you're using WinForms, watch out for conflict with `System.Windows.Forms.HtmlDocument`.)

#### Parse an HTML Document

```cs
string html = "...";   // HTML markup
HtmlDocument document = HtmlDocument.FromHtml(html);
```

This code parses the HTML document into a hierarchy of nodes, which are then stored in the `HtmlDocument` object.

The node types include `HtmlElementNode`, which represents an HTML tag with attributes and any number of child nodes. `HtmlTextNode` nodes contain only text. And `HtmlCDataNode` nodes contain text from the document that was parsed but is otherwise ignored. Examples of content placed in `HtmlCDataNode` nodes include CDATA content, comments and the content of `<script>` tags.

The code also supports the specialized `HtmlHeaderNode` and `XmlHeaderNode` nodes.

## Navigating Parsed Nodes

HtmlMonkey provides a number of ways to navigate parsed nodes. The `HtmlDocument.RootNodes` property contains the root nodes in the document. Each `HtmlElementNode` node includes a `Children` property, which can be used to access all the other nodes in the document. In addition, all nodes have `NextNode`, `PrevNode`, and `ParentNode` properties, which you can use to navigate the nodes in every direction.

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

// Get all nodes in the document
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

For greater control over attributes, you can use square brackets ([]). This is similar to specifying attributes in jQuery, but there are some differences. The first difference is that all the variations for finding a match at the start, middle or end are not supported by HtmlMonkey. However, to make up for this limitation, you can use the `:=` operator to specify that the value is a regular expression and the code will match if the attribute value matches that regular expression.

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

## Enhancing the Library

This is my initial attempt at this library and I would appreciate and be responsive to any feedback from people working with it. I want to keep the library small but would like to see more testing done on a wide variety of input markup. What sort of scenarios does the library not handle correctly? This is the type of information I'd be curious about.
