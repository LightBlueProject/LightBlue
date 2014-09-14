using System;

namespace LightBlue.Infrastructure
{
    public static class StringValidation
    {
        public static void NotNullOrEmpty(string value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
            if (value.Length == 0)
            {
                throw new ArgumentException(
                    "The argument must not be an empty string",
                    name);
            }
        }

        public static void NotNullOrWhitespace(string value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
            if (value.Trim().Length == 0)
            {
                throw new ArgumentException(
                    "The argument must not be an empty string",
                    name);
            }
        }
    }
}