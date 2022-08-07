using System;

namespace AillieoUtils.EasyEditor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ButtonAttribute : BaseEasyEditorAttribute
    {
        public readonly string label;
        public ButtonAttribute(string label = null)
        {
            this.label = label;
        }
    }
}
