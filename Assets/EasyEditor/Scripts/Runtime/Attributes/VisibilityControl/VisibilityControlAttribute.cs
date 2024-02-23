// -----------------------------------------------------------------------
// <copyright file="VisibilityControlAttribute.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.EasyEditor
{
    using System;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class VisibilityControlAttribute : BaseEasyEditorAttribute
    {
        public readonly string condition;
        public readonly object refValue;

        internal VisibilityControlAttribute(string condition, object refValue)
        {
            this.condition = condition;
            this.refValue = refValue;
        }

        internal VisibilityControlAttribute(string condition)
            : this(condition, true)
        {
        }
    }
}
