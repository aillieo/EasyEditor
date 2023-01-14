using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.EasyEditor.Editor
{
    [EasyEditorDrawer(typeof(VisibilityControlAttribute))]
    public class VisibilityControlDrawer : BaseEasyEditorDrawer
    {
        private bool invert;
        private string condition;
        private MemberInfo memberInfo;
        private string errorMessage;

        public override void PropertyField(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                bool show = Evaluate(property);

                if (invert)
                {
                    show = !show;
                }

                if (show)
                {
                    EditorGUILayout.PropertyField(property);
                }
            }
            else
            {
                EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
                EditorGUILayout.PropertyField(property);
            }
        }

        public override void Init(SerializedProperty property)
        {
            VisibilityControlAttribute vcAttribute = ReflectionUtilsInternal.GetCustomAttribute<VisibilityControlAttribute>(property);

            invert = vcAttribute is HideIfAttribute;

            condition = vcAttribute.condition;
            Type objectType = property.serializedObject.targetObject.GetType();
            MemberInfo mi = objectType.GetMember(condition).FirstOrDefault();
            if (mi == null)
            {
                Debug.LogError($"Condition not found: {condition} in {objectType}");
                return;
            }

            switch (mi.MemberType)
            {
                case MemberTypes.Field:
                case MemberTypes.Property:
                case MemberTypes.Method:
                    this.memberInfo = mi;
                    break;
                default:
                    Debug.LogError($"Condition member type not supported: {condition} in {objectType}({mi.MemberType})");
                    break;
            }
        }

        public override void Cleanup()
        {
        }

        private bool Evaluate(SerializedProperty property)
        {
            if (memberInfo != null)
            {
                object target = property.serializedObject.targetObject;

                switch (this.memberInfo)
                {
                    case FieldInfo fieldInfo:
                        return (bool)fieldInfo.GetValue(target);
                    case PropertyInfo propertyInfo:
                        return (bool)propertyInfo.GetValue(target);
                    case MethodInfo methodInfo:
                        return (bool)methodInfo.Invoke(target, null);
                    default:
                        break;
                }
            }

            return true;
        }
    }
}
