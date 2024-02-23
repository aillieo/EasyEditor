// -----------------------------------------------------------------------
// <copyright file="HideIfAttribute.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.EasyEditor
{
    public class HideIfAttribute : VisibilityControlAttribute
    {
        public HideIfAttribute(string condition, object refValue)
            : base(condition, refValue)
        {
        }

        public HideIfAttribute(string condition)
            : base(condition)
        {
        }
    }
}
