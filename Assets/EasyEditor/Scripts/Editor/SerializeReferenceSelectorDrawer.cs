// -----------------------------------------------------------------------
// <copyright file="SerializeReferenceSelectorDrawer.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.EasyEditor.Editor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AillieoUtils.CSReflectionUtils;
    using UnityEditor;
    using UnityEngine;

    [EasyEditorDrawer(typeof(SerializeReferenceSelectorAttribute))]

    public class SerializeReferenceSelectorDrawer : BaseEasyEditorDrawer
    {
        private static readonly Dictionary<string, Type> fieldTypeNameToTypeCache = new Dictionary<string, Type>();
        private static readonly Dictionary<Type, Dictionary<string, Type>> templateTypes = new Dictionary<Type, Dictionary<string, Type>>();
        private static readonly Dictionary<Type, string[]> templateTypeNames = new Dictionary<Type, string[]>();
        private static readonly Dictionary<Type, Dictionary<string, int>> templateTypeNameLookup = new Dictionary<Type, Dictionary<string, int>>();

        public override void Init(SerializedProperty property)
        {
            Type keyType = this.GetKeyType(property);
            this.EnsureTypeCache(keyType);
        }

        public override void Cleanup()
        {
        }

        public override void PropertyField(SerializedProperty property)
        {
            Type keyType = this.GetKeyType(property);

            this.EnsureTypeCache(keyType);

            Dictionary<string, Type> subTypes = templateTypes[keyType];
            string[] subTypeNames = templateTypeNames[keyType];

            if (subTypeNames.Length == 1 && subTypeNames[0] == "<null>")
            {
                EditorGUILayout.LabelField($"No compatible types found for {keyType.Name}");
            }
            else
            {
                int templateTypeIndex = 0;
                string managedReferenceFullTypename = property.managedReferenceFullTypename;
                if (!string.IsNullOrEmpty(managedReferenceFullTypename))
                {
                    string typeName = managedReferenceFullTypename.Substring(managedReferenceFullTypename.IndexOf(' ') + 1);
                    Dictionary<string, int> nameIndexLookup = templateTypeNameLookup[keyType];

                    if (!nameIndexLookup.TryGetValue(typeName, out templateTypeIndex))
                    {
                        templateTypeIndex = Array.IndexOf(subTypeNames, typeName);
                        if (templateTypeIndex >= 0)
                        {
                            nameIndexLookup.Add(typeName, templateTypeIndex);
                        }
                        else
                        {
                            templateTypeIndex = 0;
                        }
                    }
                }

                int newTemplateTypeIndex = EditorGUILayout.Popup(property.name, templateTypeIndex, subTypeNames);
                if (newTemplateTypeIndex != templateTypeIndex)
                {
                    templateTypeIndex = newTemplateTypeIndex;
                    string selectedTypeName = subTypeNames[templateTypeIndex];
                    object newInstance = null;

                    if (templateTypeIndex != 0)
                    {
                        Type selectedType = subTypes[selectedTypeName];
                        newInstance = Activator.CreateInstance(selectedType);
                    }

                    property.serializedObject.Update();
                    property.managedReferenceValue = newInstance;
                    property.serializedObject.ApplyModifiedProperties();
                }

                property.serializedObject.Update();

                if (property.hasVisibleChildren)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(property, new GUIContent(subTypeNames[templateTypeIndex]), property.isExpanded);
                    EditorGUI.indentLevel--;
                }

                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private void EnsureTypeCache(Type keyType)
        {
            if (!templateTypes.TryGetValue(keyType, out Dictionary<string, Type> types))
            {
                types = ReflectionUtils.FindSubTypes(keyType)
                    .ToDictionary(t => t.FullName, t => t, StringComparer.Ordinal);
                templateTypes.Add(keyType, types);
            }

            if (!templateTypeNames.TryGetValue(keyType, out string[] typeNames))
            {
                typeNames = new string[types.Count + 1];
                typeNames[0] = "<null>";
                int i = 1;
                foreach (var pair in types)
                {
                    typeNames[i++] = pair.Key;
                }

                templateTypeNames.Add(keyType, typeNames);
            }

            if (!templateTypeNameLookup.TryGetValue(keyType, out Dictionary<string, int> nameLookup))
            {
                nameLookup = new Dictionary<string, int>(StringComparer.Ordinal);
                templateTypeNameLookup.Add(keyType, nameLookup);
            }
        }

        private Type GetKeyType(SerializedProperty property)
        {
            string managedReferenceFieldTypename = property.managedReferenceFieldTypename;
            if (!fieldTypeNameToTypeCache.TryGetValue(managedReferenceFieldTypename, out Type type))
            {
                string[] pair = managedReferenceFieldTypename.Split(' ');
                type = Type.GetType($"{pair[1]},{pair[0]}");
                fieldTypeNameToTypeCache.Add(managedReferenceFieldTypename, type);
            }

            return type;
        }
    }
}
