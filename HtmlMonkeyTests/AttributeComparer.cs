// Copyright (c) 2019-2025 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.HtmlMonkey;
using System.Collections.Generic;

namespace HtmlMonkeyTests
{
    public class AttributeComparer : Comparer<HtmlAttribute>
    {
        public override int Compare(HtmlAttribute? x, HtmlAttribute? y)
        {
            if (x != null && y != null)
            {
                int result = x.Name.CompareTo(y.Name);
                if (result != 0)
                    return result;
                return string.Compare(x.Value, y.Value);
            }

            if (x == null && y == null)
                return 0;
            return (y == null) ? 1 : -1;
        }
    }
}
