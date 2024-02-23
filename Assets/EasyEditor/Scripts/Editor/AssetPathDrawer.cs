// -----------------------------------------------------------------------
// <copyright file="AssetPathDrawer.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.EasyEditor.Editor
{
    using UnityEditor;
    using UnityEngine;

    [EasyEditorDrawer(typeof(AssetPathAttribute))]
    public class AssetPathDrawer : BaseEasyEditorDrawer
    {
        public override void Cleanup()
        {
        }

        public override void Init(SerializedProperty property)
        {
        }

        public override void PropertyField(SerializedProperty property)
        {
            string value = property.stringValue;
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(value);
            EditorGUI.BeginChangeCheck();
            obj = EditorGUILayout.ObjectField(property.displayName, obj, typeof(Object), false);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = AssetDatabase.GetAssetPath(obj);
            }
        }
    }
}
