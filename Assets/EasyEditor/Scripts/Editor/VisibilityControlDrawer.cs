// -----------------------------------------------------------------------
// <copyright file="VisibilityControlDrawer.cs" company="AillieoTech">
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
            if (string.IsNullOrEmpty(this.attributeInvalidMessage))
            {
                bool show = this.Evaluate(property);

                if (this.invert)
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
                EditorGUILayout.HelpBox(this.attributeInvalidMessage, MessageType.Error);
                EditorGUILayout.PropertyField(property);
            }
        }

        public override void Init(SerializedProperty property)
        {
            VisibilityControlAttribute vcAttribute = EasyEditorUtils.GetCustomAttribute<VisibilityControlAttribute>(property);

            this.invert = vcAttribute is HideIfAttribute;

            this.condition = vcAttribute.condition;
            this.refValue = vcAttribute.refValue;

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
