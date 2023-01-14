using System;
using System.Collections.Generic;
using System.Reflection;
using AillieoUtils.CSReflectionUtils;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.EasyEditor.Editor
{
    internal static class ReflectionUtilsInternal
    {
        public static T[] GetCustomAttributes<T>(SerializedProperty property)
            where T : Attribute
        {
            object target = property.serializedObject.targetObject;
            FieldInfo fieldInfo = ReflectionUtils.GetFieldEx(target.GetType(), property.name);
            if (fieldInfo == null)
            {
                return Array.Empty<T>();
            }

            return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
        }

        public static T GetCustomAttribute<T>(SerializedProperty property)
            where T : Attribute
        {
            object target = property.serializedObject.targetObject;
            FieldInfo fieldInfo = ReflectionUtils.GetFieldEx(target.GetType(), property.name);
            if (fieldInfo == null)
            {
                return null;
            }

            return fieldInfo.GetCustomAttribute(typeof(T), true) as T;
        }

        public static BaseEasyEditorDrawer TryCreateDrawerInstanceForProperty(SerializedProperty serializedProperty)
        {
            BaseEasyEditorAttribute eeAttribute = GetCustomAttribute<BaseEasyEditorAttribute>(serializedProperty);
            if (eeAttribute == null)
            {
                return null;
            }

            Type attributeType = eeAttribute.GetType();

            var attributeTypeChain = ReflectionUtils.GetInheritanceChain(attributeType);
            foreach (var at in attributeTypeChain)
            {
                var subTypes = ReflectionUtils.FindSubTypes(typeof(BaseEasyEditorDrawer));
                foreach (var typeOfDrawer in subTypes)
                {
                    EasyEditorDrawerAttribute drawerAttribute = typeOfDrawer.GetCustomAttribute<EasyEditorDrawerAttribute>();
                    if (drawerAttribute != null && drawerAttribute.targetType == at)
                    {
                        BaseEasyEditorDrawer drawer = Activator.CreateInstance(typeOfDrawer) as BaseEasyEditorDrawer;
                        if (drawer != null)
                        {
                            return drawer;
                        }
                    }
                }
            }

            return new MissingReportDrawer(attributeType);
        }
    }
}
