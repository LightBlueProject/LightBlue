using System;
using System.Threading.Tasks;

namespace LightBlue.Host
{
    public static class RunStateExtensions
    {
        public static bool ShouldRunHost(this RunState runState, HostArgs hostArgs)
        {
            if (runState == RunState.NotRun || hostArgs.RetryMode == RetryMode.Infinite)
            {
                return true;
            }

            if (hostArgs.RetryMode == RetryMode.NoReload)
            {
                return false;
            }

            if (hostArgs.RetryMode == RetryMode.SingleThenFreeze)
            {
                Task.Delay(-1).Wait();
            }

            if (hostArgs.RetryMode == RetryMode.FreezeOnError)
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