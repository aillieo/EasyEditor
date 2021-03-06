using UnityEditor;

namespace EasyEditor.Editor
{
    public abstract class BaseEasyEditorDrawer
    {
        public abstract void Init(SerializedProperty property);
        public abstract void PropertyField(SerializedProperty property);
        public abstract void CleanUp();
    }
}
