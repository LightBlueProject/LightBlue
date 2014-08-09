using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Infrastructure
{
    public static class RoleEnvironmentExceptionCreatorFactory
    {
        public static Func<string, RoleEnvironmentException> BuildRoleEnvironmentExceptionCreator()
        {
            var exceptionType = typeof(RoleEnvironmentException);

            var constructor = exceptionType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] {typeof(string)},
                null);

            var exceptionParameters = new[]
            {
                Expression.Parameter(typeof(string), "message")
            };

            var newExpression = Expression.New(constructor, exceptionParameters.Cast<Expression>());

            return Expression
                .Lambda<Func<string, RoleEnvironmentException>>(newExpression, exceptionParameters)
                .Compile();
        }
    }
}