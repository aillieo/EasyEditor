// -----------------------------------------------------------------------
// <copyright file="ButtonAttribute.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.EasyEditor
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ButtonAttribute : BaseEasyEditorAttribute
    {
        public readonly string label;

        public ButtonAttribute(string label = null)
        {
            this.label = label;
        }
    }
}
