using System;

namespace AillieoUtils.EasyEditor
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class GroupAttribute : BaseEasyEditorAttribute
    {
        public readonly string name;

        internal GroupAttribute(string name)
        {
            this.name = name;
        }
    }
}
