using System;
using System.Collections.Generic;
using LightBlue.Testability;
using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Testability
{
    public class UnsetContextBehavioursWillThrowTests
    {
        private static readonly FakeLightBlueContext Context = new FakeLightBlueContext();

        [Theory]
        [MemberData(nameof(ContextActions))]
        public void TheoryMethodName(Action failingAction)
        {
            try
            {
                failingAction();
            }
            catch (NotSupportedException ex)
            {
                Assert.Contains("not set, if you want to use this, set the property first", ex.Message);
            }
        }

        public static IEnumerable<object[]> ContextActions
        {
            get
            {
                return new[]
                {
                    new object[] { new Action(() => Context.GetStorageAccount(null)) },
                    new object[] { new Action(() => Context.GetBlobContainer(null)) },
                    new object[] { new Action(() => Context.GetBlobContainer(null, null)) },
                    new object[] { new Action(() => Context.GetBlockBlob(null)) },
                    new object[] { new Action(() => Context.GetBlockBlob(null, null)) },
                    new object[] { new Action(() => Context.GetQueue(null)) }
                };
            }
        }
    }
}