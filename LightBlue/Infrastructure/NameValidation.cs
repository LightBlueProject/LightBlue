using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LightBlue.Infrastructure
{
    public static class NameValidation
    {
        private static readonly Regex _regex = new Regex(@"^[a-z0-9](([a-z0-9\-[^\-])){1,61}[a-z0-9]$");

        public static void Container(string name, string parameterName)
        {
            if (!_regex.IsMatch(name))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Container name '{0}' is invalid. Names may be 3-63 characters comprising number, letters and dashes only with the first and last characters numbers or letters.",
                        name),
                    parameterName);
            }
        }
    }
}