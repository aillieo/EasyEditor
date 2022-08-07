using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.EasyEditor.Editor
{
    public static class ReflectionUtils
    {
        public static T[] GetCustomAttributes<T>(SerializedProperty property) where T : class
        {
            object target = property.serializedObject.targetObject;
            FieldInfo fieldInfo = AillieoUtils.ReflectionUtils.GetFieldEx(target, property.name);
            if (fieldInfo == null)
            {
                return Array.Empty<T>();
            }
            return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
        }

        public static T GetCustomAttribute<T>(SerializedProperty property) where T : class
        {
            return GetCustomAttributes<T>(property).FirstOrDefault();
        }

        public static BaseEasyEditorDrawer TryCreateDrawerInstanceForProperty(SerializedProperty serializedProperty)
        {
            BaseEasyEditorAttribute eeAttribute = GetCustomAttribute<BaseEasyEditorAttribute>(serializedProperty);
            if (eeAttribute != null)
            {
                var subTypes = AillieoUtils.ReflectionUtils.FindSubTypes(typeof(BaseEasyEditorDrawer));
                foreach (var typeOfDrawer in subTypes)
                {
                    EasyEditorDrawerAttribute drawerAttribute = typeOfDrawer.GetCustomAttribute<EasyEditorDrawerAttribute>();
                    if (drawerAttribute != null && drawerAttribute.targetType == eeAttribute.GetType())
                    {
                        BaseEasyEditorDrawer drawer = Activator.CreateInstance(typeOfDrawer) as BaseEasyEditorDrawer;
                        if (drawer != null)
                        {
                            return drawer;
                        }
                    }
                }
                Debug.LogWarning($"Can not find drawer implementation for nameof{eeAttribute.GetType()}");
            }
            return null;
        }
    }
}
