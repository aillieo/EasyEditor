// -----------------------------------------------------------------------
// <copyright file="ShowIfAttribute.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.EasyEditor
{
    public class ShowIfAttribute : VisibilityControlAttribute
    {
        public ShowIfAttribute(string condition, object refValue)
            : base(condition, refValue)
        {
        }

        public ShowIfAttribute(string condition)
            : base(condition)
        {
        }
    }
}
