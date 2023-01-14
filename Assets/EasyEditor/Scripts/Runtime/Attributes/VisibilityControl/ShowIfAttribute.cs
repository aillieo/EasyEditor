namespace AillieoUtils.EasyEditor
{
    public class ShowIfAttribute : VisibilityControlAttribute
    {
        public ShowIfAttribute(string condition, object refValue)
            : base(condition, refValue)
        {
        }

        public ShowIfAttribute(string condition)
            : base(condition)
        {
        }
    }
}
