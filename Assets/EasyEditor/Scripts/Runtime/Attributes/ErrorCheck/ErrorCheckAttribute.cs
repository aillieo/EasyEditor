using System;

namespace AillieoUtils.EasyEditor
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public abstract class ErrorCheckAttribute : BaseEasyEditorAttribute
    {
        public readonly string errorMessage;
        public readonly string condition;
        public readonly object refValue;

        internal ErrorCheckAttribute(string errorMessage, string condition, object refValue)
        {
            this.errorMessage = errorMessage;
            this.condition = condition;
            this.refValue = refValue;
        }

        internal ErrorCheckAttribute(string errorMessage, string condition)
            : this(errorMessage, condition, true)
        {
        }
    }
}
