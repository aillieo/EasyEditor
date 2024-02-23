// -----------------------------------------------------------------------
// <copyright file="ErrorCheckDrawer.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoUtils.EasyEditor.Editor
{
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

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
            if (string.IsNullOrEmpty(this.attributeInvalidMessage))
            {
                bool drawError = this.Evaluate(property);

                if (this.invert)
                {
                    drawError = !drawError;
                }

                if (drawError)
                {
                    EditorGUILayout.HelpBox(this.errorMessage, MessageType.Error);
                }

                EditorGUILayout.PropertyField(property);
            }
            else
            {
                EditorGUILayout.HelpBox(this.attributeInvalidMessage, MessageType.Error);
                EditorGUILayout.PropertyField(property);
            }
        }

        public override void Init(SerializedProperty property)
        {
            ErrorCheckAttribute ecAttribute = EasyEditorUtils.GetCustomAttribute<ErrorCheckAttribute>(property);

            this.invert = ecAttribute is ErrorIfNotAttribute;

            this.errorMessage = ecAttribute.errorMessage;
            this.condition = ecAttribute.condition;
            this.refValue = ecAttribute.refValue;

            this.attributeInvalidMessage = EasyEditorUtils.ValidateEvaluationParameters(property.serializedObject.targetObject, this.condition, this.refValue);

            Type objectType = property.serializedObject.targetObject.GetType();
            this.memberInfo = objectType.GetMember(this.condition).FirstOrDefault();
        }

        public override void Cleanup()
        {
        }

        private bool Evaluate(SerializedProperty property)
        {
            if (this.memberInfo != null)
            {
                object target = property.serializedObject.targetObject;
                return EasyEditorUtils.Evaluate(target, this.memberInfo, this.refValue);
            }

            return true;
        }
    }
}
