// -----------------------------------------------------------------------
// <copyright file="BaseEasyEditorDrawer.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.EasyEditor.Editor
{
    using UnityEditor;

    public abstract class BaseEasyEditorDrawer
    {
        public abstract void Init(SerializedProperty property);

        public abstract void PropertyField(SerializedProperty property);

        public abstract void Cleanup();
    }
}
