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
        private string condition;
        private object refValue;

        private bool invert;
        private MemberInfo memberInfo;
        private string attributeInvalidMessage;

        public override void PropertyField(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(attributeInvalidMessage))
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
                EditorGUILayout.HelpBox(attributeInvalidMessage, MessageType.Error);
                EditorGUILayout.PropertyField(property);
            }
        }

        public override void Init(SerializedProperty property)
        {
            VisibilityControlAttribute vcAttribute = EasyEditorUtils.GetCustomAttribute<VisibilityControlAttribute>(property);

            invert = vcAttribute is HideIfAttribute;

            condition = vcAttribute.condition;
            refValue = vcAttribute.refValue;

            attributeInvalidMessage = EasyEditorUtils.ValidateEvaluationParameters(property.serializedObject.targetObject, condition, refValue);

            Type objectType = property.serializedObject.targetObject.GetType();
            this.memberInfo = objectType.GetMember(condition).FirstOrDefault();
        }

        public override void Cleanup()
        {
        }

        private bool Evaluate(SerializedProperty property)
        {
            if (memberInfo != null)
            {
                object target = property.serializedObject.targetObject;
                return EasyEditorUtils.Evaluate(target, memberInfo, refValue);
            }

            return true;
        }
    }
}
