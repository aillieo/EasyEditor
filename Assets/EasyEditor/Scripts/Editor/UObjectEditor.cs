// -----------------------------------------------------------------------
// <copyright file="UObjectEditor.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.EasyEditor.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AillieoUtils.CSReflectionUtils;
    using UnityEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class UObjectEditor : UnityEditor.Editor
    {
        private readonly Dictionary<string, BaseEasyEditorDrawer> cachedDrawers = new Dictionary<string, BaseEasyEditorDrawer>();
        private IEnumerable<MethodInfo> methodsForButtons;
        private IEnumerable<PropertyInfo> propertiesToDraw;
        private Dictionary<FoldableGroupAttribute, bool> foldoutState;

        protected virtual void OnEnable()
        {
            Type type = this.target.GetType();

            if (this.methodsForButtons == null)
            {
                this.methodsForButtons = ReflectionUtils.GetAllAccessibleMethods(type, m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
            }

            if (this.propertiesToDraw == null)
            {
                this.propertiesToDraw = ReflectionUtils.GetAllAccessibleProperties(type, m => m.GetCustomAttributes(typeof(ShowInInspectorAttribute), true).Length > 0);
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var drawer in this.cachedDrawers)
            {
                if (drawer.Value != null)
                {
                    drawer.Value.Cleanup();
                }
            }

            this.cachedDrawers.Clear();
        }

        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspectorEx();
            this.DrawProperties();
            this.DrawButtons();
        }

        protected void DrawDefaultInspectorEx()
        {
            this.serializedObject.Update();

            IEnumerable<IGrouping<GroupAttribute, SerializedProperty>> groupedProperties = null;

            var serializedProperties = EasyEditorUtils.GetAllSerializedProperties(this.serializedObject);
            if (EasyEditorUtils.HasGroupAttribute(this.target.GetType()))
            {
                groupedProperties = serializedProperties.GroupBy(property => EasyEditorUtils.GetCustomAttribute<GroupAttribute>(property));
            }
            else
            {
                groupedProperties = serializedProperties.GroupBy(property => (GroupAttribute)null);
            }

            foreach (var group in groupedProperties)
            {
                bool drawGroup = true;

                var groupAttribute = group.Key;
                if (groupAttribute != null)
                {
                    switch (groupAttribute)
                    {
                        case FoldableGroupAttribute foldableGroup:
                            if (this.foldoutState == null)
                            {
                                this.foldoutState = new Dictionary<FoldableGroupAttribute, bool>();
                            }

                            if (!this.foldoutState.TryGetValue(foldableGroup, out bool foldout))
                            {
                                foldout = true;
                            }

                            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, foldableGroup.name);
                            drawGroup = foldout;

                            this.foldoutState[foldableGroup] = foldout;

                            break;

                        case BoxGroupAttribute boxGroup:
                            EditorGUILayout.BeginVertical(boxGroup.name);
                            break;
                    }
                }

                if (drawGroup)
                {
                    foreach (var property in group)
                    {
                        if (property.name.Equals("m_Script", System.StringComparison.Ordinal))
                        {
                            using (new EditorGUI.DisabledScope(true))
                            {
                                EditorGUILayout.PropertyField(property, true);
                            }
                        }
                        else
                        {
                            string propertyName = property.name;

                            BaseEasyEditorDrawer drawer;
                            if (!this.cachedDrawers.TryGetValue(propertyName, out drawer))
                            {
                                drawer = EasyEditorUtils.TryCreateDrawerInstanceForProperty(property);
                                if (drawer != null)
                                {
                                    drawer.Init(property);
                                }

                                this.cachedDrawers[propertyName] = drawer;
                            }

                            if (drawer != null)
                            {
                                SerializedProperty copy = property.Copy();
                                drawer.PropertyField(copy);
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(property, true);
                            }
                        }
                    }
                }

                if (groupAttribute != null)
                {
                    switch (groupAttribute)
                    {
                        case FoldableGroupAttribute foldableGroup:
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            break;

                        case BoxGroupAttribute boxGroup:
                            EditorGUILayout.EndVertical();
                            break;
                    }
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        protected void DrawButtons()
        {
            if (this.methodsForButtons.Any())
            {
                EditorGUILayout.Space();

                foreach (var method in this.methodsForButtons)
                {
                    EasyEditorGUILayout.Button(this.serializedObject.targetObject, method);
                }
            }
        }

        protected void DrawProperties()
        {
            if (this.propertiesToDraw.Any())
            {
                EditorGUILayout.Space();

                foreach (var prop in this.propertiesToDraw)
                {
                    EasyEditorGUILayout.DrawProperty(this.serializedObject.targetObject, prop);
                }
            }
        }
    }
}
