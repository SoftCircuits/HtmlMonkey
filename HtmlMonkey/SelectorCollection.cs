/////////////////////////////////////////////////////////////
// HTML Monkey
// Copyright (c) 2018 Jonathan Wood
// http://www.softcircuits.com, http://www.blackbeltcoder.com
//
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HtmlMonkey
{
    public class SelectorCollection : List<Selector>
    {
        /// <summary>
        /// Returns the combined results of all the selectors in the collection.
        /// Ensures no duplicate nodes are returned.
        /// </summary>
        /// <param name="nodes">The set of nodes to search.</param>
        /// <returns>A set of nodes that matches this selector collection.</returns>
        public IEnumerable<HtmlElementNode> Find(IEnumerable<HtmlNode> nodes)
        {
            List<HtmlElementNode> results = new List<HtmlElementNode>();

            // Loop through each selector
            foreach (Selector selector in this)
                results.AddRange(selector.Find(nodes));

            // If necessary, remove duplicates between selectors
            return (Count > 1) ? results.Distinct() : results;
        }

        #region Internal methods

        /// <summary>
        /// Adds a new child level selector to the collection, and returns that child selector.
        /// </summary>
        /// <returns>Returns the new child selector.</returns>
        internal Selector AddChildSelector()
        {
            Selector selector = GetLast();
            Debug.Assert(selector.ChildSelector == null);
            selector.ChildSelector = new Selector();
            return selector.ChildSelector;
        }

        /// <summary>
        /// Returns the last selector in the collection. If the collection is empty and <paramref name="createNewIfEmpty"/> is true,
        /// an empty selector is added to the collection and the new selector is returned. If the collection is empty and
        /// <paramref name="createNewIfEmpty"/> is false, then <c>null</c> is returned.
        /// </summary>
        /// <param name="createNewIfEmpty">The true and the collection is empty, an empty selector is added to the collection.</param>
        /// <returns>The last selector in the collection.</returns>
        internal Selector GetLast(bool createNewIfEmpty = false)
        {
            // Create new selector if needed
            if (Count == 0 && createNewIfEmpty)
                Add(new Selector());

            if (Count > 0)
            {
                Selector selector = this[Count - 1];
                while (selector.ChildSelector != null)
                    selector = selector.ChildSelector;
                return selector;
            }
            return null;
        }

        /// <summary>
        /// Removes all selectors and child selectors that do not contain valid data.
        /// </summary>
        internal void RemoveEmpty()
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                Selector selector = RemoveEmptyChain(this[i]);
                if (selector == null)
                    RemoveAt(i);
                else if (this[i] != selector)
                    this[i] = selector;
            }
        }

        private Selector RemoveEmptyChain(Selector selector)
        {
            Selector firstSelector = selector;
            Selector prevSelector = null;

            while (selector != null)
            {
                if (selector.IsEmpty)
                {
                    if (prevSelector == null)
                        firstSelector = selector.ChildSelector;
                    else
                        prevSelector.ChildSelector = selector.ChildSelector;
                }
                else
                {
                    prevSelector = selector;
                }
                selector = selector.ChildSelector;
            }
            return firstSelector;
        }

        #endregion

    }
}
