using UnityEngine;
using UnityEditor;

namespace AillieoUtils.EasyEditor.Editor
{
    [EasyEditorDrawer(typeof(AssetPathAttribute))]
    public class AssetPathDrawer : BaseEasyEditorDrawer
    {
        public override void Cleanup()
        {
        }

        public override void Init(SerializedProperty property)
        {
        }

        public override void PropertyField(SerializedProperty property)
        {
            string value = property.stringValue;
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(value);
            EditorGUI.BeginChangeCheck();
            obj = EditorGUILayout.ObjectField(property.displayName, obj, typeof(Object), false);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = AssetDatabase.GetAssetPath(obj);
            }
        }
    }
}
