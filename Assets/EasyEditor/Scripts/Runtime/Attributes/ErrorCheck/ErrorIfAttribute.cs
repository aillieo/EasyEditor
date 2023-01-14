namespace AillieoUtils.EasyEditor
{
    public class ErrorIfAttribute : ErrorCheckAttribute
    {
        public ErrorIfAttribute(string errorMessage, string condition, object refValue)
            : base(errorMessage, condition, refValue)
        {
        }

        public ErrorIfAttribute(string errorMessage, string condition)
            : base(errorMessage, condition)
        {
        }
    }
}
