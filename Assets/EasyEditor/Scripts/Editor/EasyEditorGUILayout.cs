using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.EasyEditor.Editor
{
    public static class EasyEditorGUILayout
    {
        public static void Button(UnityEngine.Object target, MethodInfo methodInfo)
        {
            if (!methodInfo.GetParameters().Any(p => !p.IsOptional))
            {
                ButtonAttribute buttonAttribute = (ButtonAttribute)methodInfo.GetCustomAttributes(typeof(ButtonAttribute), true)[0];
                string buttonText = string.IsNullOrEmpty(buttonAttribute.label) ? ObjectNames.NicifyVariableName(methodInfo.Name) : buttonAttribute.label;

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(buttonText, new GUIStyle("button"), GUILayout.Width(220)))
                {
                    object[] defaultParams = methodInfo.GetParameters().Select(p => p.DefaultValue).ToArray();
                    methodInfo.Invoke(target, defaultParams);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.HelpBox($"{nameof(ButtonAttribute)} can only be used for methods with no args", MessageType.Warning);
            }
        }

        public static void DrawProperty(UnityEngine.Object tar, PropertyInfo propertyInfo)
        {
            var value = propertyInfo.GetValue(tar);
            EditorGUI.BeginChangeCheck();
            value = AnyTypeField(propertyInfo.Name, value, $"{nameof(ShowInInspectorAttribute)} can not be used on type {value.GetType()}");
            if (EditorGUI.EndChangeCheck())
            {
                propertyInfo.SetValue(tar, value);
            }
        }

        public static object AnyTypeField(string label, object value, string messageOnFail = null)
        {
            switch (value)
            {
                case int intValue:
                    return EditorGUILayout.IntField(label, intValue);
                case bool boolValue:
                    return EditorGUILayout.Toggle(label, boolValue);
                case long longValue:
                    return EditorGUILayout.LongField(label, longValue);
                case float floatValue:
                    return EditorGUILayout.FloatField(label, floatValue);
                case double doubleValue:
                    return EditorGUILayout.DoubleField(label, doubleValue);
                case string stringValue:
                    return EditorGUILayout.TextField(label, stringValue);
                case Vector2 vector2Value:
                    return EditorGUILayout.Vector2Field(label, vector2Value);
                case Vector3 vector3Value:
                    return EditorGUILayout.Vector3Field(label, vector3Value);
                case Vector4 vector4Value:
                    return EditorGUILayout.Vector4Field(label, vector4Value);
                case Vector2Int vector2IntValue:
                    return EditorGUILayout.Vector2IntField(label, vector2IntValue);
                case Vector3Int vector3IntValue:
                    return EditorGUILayout.Vector3IntField(label, vector3IntValue);
                case Rect rectValue:
                    return EditorGUILayout.RectField(label, rectValue);
                case RectInt rectIntValue:
                    return EditorGUILayout.RectIntField(label, rectIntValue);
                case Bounds boundsValue:
                    return EditorGUILayout.BoundsField(label, boundsValue);
                case BoundsInt boundsIntValue:
                    return EditorGUILayout.BoundsIntField(label, boundsIntValue);
                case Color colorValue:
                    return EditorGUILayout.ColorField(label, colorValue);
                case Gradient gradientValue:
                    return EditorGUILayout.GradientField(label, gradientValue);
                case AnimationCurve animationCurveValue:
                    return EditorGUILayout.CurveField(label, animationCurveValue);
                default:
                    break;
            }

            if (value.GetType().BaseType == typeof(Enum))
            {
                return EditorGUILayout.EnumPopup(label, (Enum)value);
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(value.GetType()))
            {
                return EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, value.GetType(), true);
            }

            if (string.IsNullOrEmpty(messageOnFail))
            {
                messageOnFail = $"{value.GetType()} is not supported by method {nameof(AnyTypeField)}";
            }

            EditorGUILayout.HelpBox(messageOnFail, MessageType.Warning);

            return value;
        }
    }
}
