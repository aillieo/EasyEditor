
using System;

namespace EasyEditor
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShowInInspectorAttribute : BaseEasyEditorAttribute
    {
    }
}
