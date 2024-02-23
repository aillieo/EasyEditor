// -----------------------------------------------------------------------
// <copyright file="ErrorIfAttribute.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

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
