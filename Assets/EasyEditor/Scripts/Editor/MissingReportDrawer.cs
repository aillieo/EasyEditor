using System;
using UnityEditor;

namespace AillieoUtils.EasyEditor.Editor
{
    public class MissingReportDrawer : BaseEasyEditorDrawer
    {
        public MissingReportDrawer(Type type)
        {
            this.type = type;
        }

        private readonly Type type;

        public override void Cleanup()
        {
        }

        public override void Init(SerializedProperty property)
        {
        }

        public override void PropertyField(SerializedProperty property)
        {
            EditorGUILayout.HelpBox($"Default drawer will be used due to missing drawer implementation for\n[{type.Name}]{property.name}", MessageType.Warning);
            EditorGUILayout.PropertyField(property);
        }
    }
}
