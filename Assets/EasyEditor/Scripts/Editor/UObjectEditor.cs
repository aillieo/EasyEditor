using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AillieoUtils.CSReflectionUtils;
using UnityEditor;

namespace AillieoUtils.EasyEditor.Editor
{
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
            Type type = target.GetType();

            if (methodsForButtons == null)
            {
                methodsForButtons = ReflectionUtils.GetAllAccessibleMethods(type, m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
            }

            if (propertiesToDraw == null)
            {
                propertiesToDraw = ReflectionUtils.GetAllAccessibleProperties(type, m => m.GetCustomAttributes(typeof(ShowInInspectorAttribute), true).Length > 0);
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var drawer in cachedDrawers)
            {
                if (drawer.Value != null)
                {
                    drawer.Value.Cleanup();
                }
            }

            cachedDrawers.Clear();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspectorEx();
            DrawProperties();
            DrawButtons();
        }

        protected void DrawDefaultInspectorEx()
        {
            serializedObject.Update();

            IEnumerable<IGrouping<GroupAttribute, SerializedProperty>> groupedProperties = null;

            var serializedProperties = EasyEditorUtils.GetAllSerializedProperties(serializedObject);
            if (EasyEditorUtils.HasGroupAttribute(target.GetType()))
            {
                groupedProperties = serializedProperties.GroupBy(property => EasyEditorUtils.GetCustomAttribute<GroupAttribute>(property));
            }
            else
            {
                groupedProperties = serializedProperties.GroupBy(property => (GroupAttribute)null);
            }

            foreach(var group in groupedProperties)
            {
                bool drawGroup = true;

                var groupAttribue = group.Key;
                if (groupAttribue != null)
                {
                    switch (groupAttribue)
                    {
                        case FoldableGroupAttribute foldableGroup:
                            if (foldoutState == null)
                            {
                                foldoutState = new Dictionary<FoldableGroupAttribute, bool>();
                            }

                            if (!foldoutState.TryGetValue(foldableGroup, out bool foldout))
                            {
                                foldout = true;
                            }

                            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, foldableGroup.name);
                            drawGroup = foldout;

                            foldoutState[foldableGroup] = foldout;

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
                            if (!cachedDrawers.TryGetValue(propertyName, out drawer))
                            {
                                drawer = EasyEditorUtils.TryCreateDrawerInstanceForProperty(property);
                                if (drawer != null)
                                {
                                    drawer.Init(property);
                                }

                                cachedDrawers[propertyName] = drawer;
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

                if (groupAttribue != null)
                {
                    switch (groupAttribue)
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

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawButtons()
        {
            if (methodsForButtons.Any())
            {
                EditorGUILayout.Space();

                foreach (var method in methodsForButtons)
                {
                    EasyEditorGUILayout.Button(serializedObject.targetObject, method);
                }
            }
        }

        protected void DrawProperties()
        {
            if (propertiesToDraw.Any())
            {
                EditorGUILayout.Space();

                foreach (var prop in propertiesToDraw)
                {
                    EasyEditorGUILayout.DrawProperty(serializedObject.targetObject, prop);
                }
            }
        }
    }
}
