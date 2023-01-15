using System;
using System.Collections.Generic;
using System.Reflection;
using AillieoUtils.CSReflectionUtils;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.EasyEditor.Editor
{
    internal static class EasyEditorUtils
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

        internal static string ValidateEvaluationParameters(object targetObject, string condition, object refValue)
        {
            Type objectType = targetObject.GetType();
            MemberInfo[] members = objectType.GetMember(condition);
            if (members == null || members.Length == 0)
            {
                return $"Condition not found: {condition} in {objectType}";
            }

            MemberInfo memberInfo = members[0];
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo fieldInfo = memberInfo as FieldInfo;
                    if (!fieldInfo.FieldType.IsAssignableFrom(refValue.GetType()))
                    {
                        return $"Type not match: field condition({fieldInfo.FieldType}) while ref value({refValue.GetType()})";
                    }

                    break;
                case MemberTypes.Property:
                    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                    if (!propertyInfo.PropertyType.IsAssignableFrom(refValue.GetType()))
                    {
                        return $"Type not match: property condition({propertyInfo.PropertyType}) while ref value({refValue.GetType()})";
                    }

                    break;
                case MemberTypes.Method:
                    MethodInfo methodInfo = memberInfo as MethodInfo;
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    if (parameters != null && parameters.Length > 0)
                    {
                        return $"Only methods with 0 arguement can be supported: {condition} {parameters.Length}";
                    }

                    if (!methodInfo.ReturnType.IsAssignableFrom(refValue.GetType()))
                    {
                        return $"Type not match: method condition({methodInfo.ReturnType}) while ref value({refValue.GetType()})";
                    }

                    break;
                default:
                    return $"Condition member type not supported: {condition} in {objectType}({memberInfo.MemberType})";
            }

            return string.Empty;
        }

        internal static bool Evaluate(object targetObject, MemberInfo conditionMember, object refValue)
        {
            object result = default;

            switch (conditionMember)
            {
                case FieldInfo fieldInfo:
                    result = fieldInfo.GetValue(targetObject);
                    break;
                case PropertyInfo propertyInfo:
                    result = propertyInfo.GetValue(targetObject);
                    break;
                case MethodInfo methodInfo:
                    result = methodInfo.Invoke(targetObject, null);
                    break;
                default:
                    break;
            }

            if ((result == null && refValue != null) || (result != null && refValue == null))
            {
                return false;
            }

            Type resultType = result.GetType();

            if (typeof(IComparable).IsAssignableFrom(resultType) || resultType.IsValueType || resultType.IsPrimitive)
            {
                IComparable valueComparer = result as IComparable;
                if (valueComparer != null && valueComparer.CompareTo(refValue) != 0)
                {
                    result = false;
                }

                if (object.Equals(result, refValue))
                {
                    return true;
                }
            }

            return object.ReferenceEquals(result, refValue);
        }
    }
}