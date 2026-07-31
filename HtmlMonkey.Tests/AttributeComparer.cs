// Copyright (c) 2019-2026 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.HtmlMonkey;

namespace HtmlMonkey.Tests
{
    public class AttributeComparer : IEqualityComparer<HtmlAttribute>
    {
        public bool Equals(HtmlAttribute? x, HtmlAttribute? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            return x.Name == y.Name && x.Value == y.Value;
        }

        public int GetHashCode(HtmlAttribute obj) =>
            HashCode.Combine(obj.Name, obj.Value);
    }
}
