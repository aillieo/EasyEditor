// -----------------------------------------------------------------------
// <copyright file="GroupAttribute.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.EasyEditor
{
    using System;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class GroupAttribute : BaseEasyEditorAttribute
    {
        public readonly string name;

        internal GroupAttribute(string name)
        {
            this.name = name;
        }
    }
}
