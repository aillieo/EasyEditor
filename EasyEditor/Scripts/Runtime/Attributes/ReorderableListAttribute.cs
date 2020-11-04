using System;

namespace EasyEditor
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ReorderableListAttribute : BaseEasyEditorAttribute
    {
	}
}
