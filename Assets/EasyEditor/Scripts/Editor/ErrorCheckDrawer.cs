using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AillieoUtils.EasyEditor.Editor
{
    [EasyEditorDrawer(typeof(ErrorCheckAttribute))]
    public class ErrorCheckDrawer : BaseEasyEditorDrawer
    {
        private string errorMessage;
        private string condition;
        private object refValue;

        private bool invert;
        private MemberInfo memberInfo;
        private string attributeInvalidMessage;

        public override void PropertyField(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(attributeInvalidMessage))
            {
                bool drawError = this.Evaluate(property);

                if (invert)
                {
                    drawError = !drawError;
                }

                if (drawError)
                {
                    EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
                }

                EditorGUILayout.PropertyField(property);
            }
            else
            {
                EditorGUILayout.HelpBox(attributeInvalidMessage, MessageType.Error);
                EditorGUILayout.PropertyField(property);
            }
        }

        public override void Init(SerializedProperty property)
        {
            ErrorCheckAttribute ecAttribute = EasyEditorUtils.GetCustomAttribute<ErrorCheckAttribute>(property);

            invert = ecAttribute is ErrorIfNotAttribute;

            errorMessage = ecAttribute.errorMessage;
            condition = ecAttribute.condition;
            refValue = ecAttribute.refValue;

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
