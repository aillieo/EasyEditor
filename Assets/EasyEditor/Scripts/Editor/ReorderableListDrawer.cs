// -----------------------------------------------------------------------
// <copyright file="ReorderableListDrawer.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.EasyEditor.Editor
{
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [EasyEditorDrawer(typeof(ReorderableListAttribute))]
    public class ReorderableListDrawer : BaseEasyEditorDrawer
    {
        private ReorderableList reorderableList;
        private const float labelWidth = 36f;

        public override void PropertyField(SerializedProperty property)
        {
            if (property.isArray)
            {
                if (this.reorderableList == null)
                {
                    this.reorderableList = new ReorderableList(property.serializedObject, property)
                    {
                        drawHeaderCallback = (Rect rect) =>
                        {
                            EditorGUI.LabelField(rect, $"{property.displayName}[{property.arraySize}]", EditorStyles.boldLabel);
                        },

                        drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                        {
                            SerializedProperty element = property.GetArrayElementAtIndex(index);
                            Rect label = rect;
                            label.width = labelWidth;
                            Rect field = rect;
                            field.x += labelWidth;
                            field.width -= labelWidth;

                            EditorGUI.LabelField(label, new GUIContent($"[{index}]"));
                            EditorGUI.PropertyField(field, element, GUIContent.none, true);
                        },

                        elementHeightCallback = (int index) =>
                        {
                            return EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index));
                        },
                    };
                }

                this.reorderableList.DoLayoutList();
            }
            else
            {
                EditorGUILayout.HelpBox($"{nameof(ReorderableListAttribute)} can be used only on arrays or lists", MessageType.Warning);
                EditorGUILayout.PropertyField(property, true);
            }
        }

        public override void Init(SerializedProperty property)
        {
        }

        public override void Cleanup()
        {
        }
    }
}
