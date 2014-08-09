using ExpectedObjects;

using LightBlue.Infrastructure;

using Xunit;

namespace LightBlue.Tests
{
    public class RoleEnvironmentExceptionCreatorFactoryTests
    {
        [Fact]
        public void CanCreateARoleEnvironmentException()
        {
            var creator = RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator();

            var roleEnvironmentException = creator("Test message");

            new
            {
                Message = "Test message"
            }.ToExpectedObject().ShouldMatch(roleEnvironmentException);
        } 
    }
}