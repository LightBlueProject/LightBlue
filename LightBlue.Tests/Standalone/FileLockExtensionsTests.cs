using System;
using System.Collections.Generic;
using System.IO;
using LightBlue.Standalone;
using Xunit;
using Xunit.Extensions;

namespace LightBlue.Tests.Standalone
{
    public class FileLockExtensionsTests
    {
        protected readonly string FilePath;
        protected readonly string BasePath;

        public FileLockExtensionsTests()
        {
            BasePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            FilePath = Path.Combine(BasePath, String.Format("{0}.txt", Guid.NewGuid()));
            Directory.CreateDirectory(BasePath);
        }
        
        [Theory]
        [InlineData(5,6)]
        [InlineData(0, 1)]
        public void RetriesASpecifiedNumberOfTimes(int maxRetryAttempts, int callCountExpected)
        {
            // arrange
            var path = FilePath;
            var callCount = 0;
            
            // act
            using (File.CreateText(path))
            {
                FileLockExtensions.WaitAndRetryOnFileLock(() =>
                {
                    callCount++;
                    File.OpenText(path);
                }, TimeSpan.FromTicks(1), maxRetryAttempts);
            }

            // assert
            Assert.Equal(callCountExpected, callCount);
        }

        [Fact]
        public void WaitsForASpecifiedPeriod()
        {
            // arrange
            const int maximumRetryAttempts = 4;
            var path = FilePath;
            var waitTime = TimeSpan.FromMilliseconds(250);
            var lapTimes = new List<DateTime>();
            
            // act
            using (File.CreateText(path))
            {
                FileLockExtensions.WaitAndRetryOnFileLock(() =>
                {
                    lapTimes.Add(DateTime.Now);
                    File.OpenText(path);
                }, waitTime, maximumRetryAttempts);
            }

            // assert
            for (var i = 0; (i+1) < maximumRetryAttempts; i++)
            {
                var nextTimeIndex = i + 1;
                var timeElapsed = lapTimes[nextTimeIndex].Subtract(lapTimes[i]);
                Assert.InRange(timeElapsed, waitTime, waitTime.Add(TimeSpan.FromSeconds(1)));
            }
        }

        [Fact]
        public void CallsActionWhenSharingViolationOccurs()
        {
            // arrange
            var path = FilePath;
            var whenSharingViolationOccursCalled = false;

            // act
            using (File.CreateText(path))
            {
                FileLockExtensions.WaitAndRetryOnFileLock(() => File.OpenText(path), 
                    maximumRetryAttempts:0,  
                    whenSharingViolationOccurs: (retryAttemptsRemaining) => whenSharingViolationOccursCalled = true 
                );
            }

            // assert
            Assert.True(whenSharingViolationOccursCalled);
        }
    }
}