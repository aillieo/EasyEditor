using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using AillieoUtils.CSReflectionUtils;

namespace AillieoUtils.EasyEditor.Editor
{
    [CustomPropertyDrawer(typeof(SerializeReferenceSelectorAttribute))]

    public class SerializeReferenceSelectorDrawer : PropertyDrawer
    {
        private static readonly Dictionary<Type, Dictionary<string, Type>> templateTypes = new Dictionary<Type, Dictionary<string, Type>>();
        private static readonly Dictionary<Type, string[]> templateTypeNames = new Dictionary<Type, string[]>();
        private static readonly Dictionary<Type, Dictionary<string, int>> templateTypeNameLookup = new Dictionary<Type, Dictionary<string, int>>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            EnsureTypeCache();

            return EditorGUI.GetPropertyHeight(property, property.isExpanded);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnsureTypeCache();

            Type keyType = fieldInfo.FieldType;
            Dictionary<string, Type> subTypes = templateTypes[keyType];
            string[] subTypeNames = templateTypeNames[keyType];

            EditorGUI.BeginProperty(position, label, property);

            if (subTypeNames.Length == 0)
            {
                EditorGUI.LabelField(position, $"No compatible types found for {keyType.Name}");
            }
            else
            {
                string managedReferenceFullTypename = property.managedReferenceFullTypename;
                if (string.IsNullOrEmpty(managedReferenceFullTypename))
                {
                    string firstTypeName = subTypeNames[0];
                    Type type = subTypes[firstTypeName];
                    property.serializedObject.Update();
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    string typeName = managedReferenceFullTypename.Substring(managedReferenceFullTypename.IndexOf(' ') + 1);
                    Dictionary<string, int> nameIndexLookup = templateTypeNameLookup[keyType];

                    if (!nameIndexLookup.TryGetValue(typeName, out int templateTypeIndex))
                    {
                        templateTypeIndex = Array.IndexOf(subTypeNames, typeName);
                        nameIndexLookup.Add(typeName, templateTypeIndex);
                    }

                    EditorGUI.BeginChangeCheck();
                    Rect lineRect = position;
                    lineRect.height = EditorGUIUtility.singleLineHeight;
                    templateTypeIndex = EditorGUI.Popup(lineRect, templateTypeIndex, subTypeNames);
                    if (EditorGUI.EndChangeCheck())
                    {
                        string selectedTypeName = subTypeNames[templateTypeIndex];
                        Type selectedType = subTypes[selectedTypeName];
                        property.serializedObject.Update();
                        property.managedReferenceValue = Activator.CreateInstance(selectedType);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }

                property.serializedObject.Update();
                EditorGUI.PropertyField(position, property, property.isExpanded);
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        private void EnsureTypeCache()
        {
            Type keyType = fieldInfo.FieldType;

            if (!templateTypes.TryGetValue(keyType, out Dictionary<string, Type> types))
            {
                types = ReflectionUtils.FindSubTypes(keyType)
                    .ToDictionary(t => t.FullName, t => t, StringComparer.Ordinal);
                templateTypes.Add(keyType, types);
            }

            if (!templateTypeNames.TryGetValue(keyType, out string[] typeNames))
            {
                typeNames = templateTypes[keyType].Keys.ToArray();
                templateTypeNames.Add(keyType, typeNames);
            }

            if (!templateTypeNameLookup.TryGetValue(keyType, out Dictionary<string, int> nameLookup))
            {
                nameLookup = new Dictionary<string, int>(StringComparer.Ordinal);
                templateTypeNameLookup.Add(keyType, nameLookup);
            }
        }
    }
}
