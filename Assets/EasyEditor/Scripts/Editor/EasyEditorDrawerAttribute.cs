// -----------------------------------------------------------------------
// <copyright file="EasyEditorDrawerAttribute.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.EasyEditor.Editor
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class EasyEditorDrawerAttribute : Attribute
    {
        public readonly Type targetType;

        public EasyEditorDrawerAttribute(Type targetType)
        {
            if (!targetType.IsSubclassOf(typeof(BaseEasyEditorAttribute)))
            {
                UnityEngine.Debug.LogError($"A {nameof(EasyEditorDrawerAttribute)} can only work with sub-types of {nameof(BaseEasyEditorAttribute)}!");
                return;
            }

            this.targetType = targetType;
        }
    }
}
