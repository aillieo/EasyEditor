using System;

namespace AillieoUtils.EasyEditor
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class VisibilityControlAttribute : BaseEasyEditorAttribute
    {
        public readonly string condition;
        public readonly object refValue;

        internal VisibilityControlAttribute(string condition, object refValue)
        {
            this.condition = condition;
            this.refValue = refValue;
        }

        internal VisibilityControlAttribute(string condition)
            : this(condition, true)
        {
        }
    }
}
