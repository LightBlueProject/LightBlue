using System;
using System.Threading.Tasks;

namespace LightBlue.Infrastructure
{
    public static class RunStateExtensions
    {
        public static bool ShouldRunHost(this RunState runState, RetryMode retryMode)
        {
            if (runState == RunState.NotRun || retryMode == RetryMode.Infinite)
            {
                return true;
            }

            if (retryMode == RetryMode.NoReload)
            {
                return false;
            }

            if (retryMode == RetryMode.SingleThenFreeze)
            {
                Task.Delay(-1).Wait();
            }

            if (retryMode == RetryMode.FreezeOnError)
            {
                if (runState == RunState.ExitedCleanly)
                {
                    return true;
                }

                Task.Delay(-1).Wait();
            }

            throw new InvalidOperationException("Retry mode is unknown. If you see this message submit a bug report.");
        }
    }
}