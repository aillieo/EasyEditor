namespace AillieoUtils.EasyEditor
{
    public class ErrorIfNotAttribute : ErrorCheckAttribute
    {
        public ErrorIfNotAttribute(string errorMessage, string condition, object refValue)
            : base(errorMessage, condition, refValue)
        {
        }

        public ErrorIfNotAttribute(string errorMessage, string condition)
            : base(errorMessage, condition)
        {
        }
    }
}
