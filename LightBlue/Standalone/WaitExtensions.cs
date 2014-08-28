using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace LightBlue.Standalone
{
    public static class WaitExtensions
    {
        public static void WaitAndRetryOnFileLock(Action func, TimeSpan? timeToAttemptRetry = null, int maximumRetryAttempts = 3, Action<int> whenSharingViolationOccurs = null)
        {
            var waitTime = TimeSpan.FromSeconds(1);
            if (timeToAttemptRetry.HasValue)
            {
                waitTime = timeToAttemptRetry.Value;
            }
            
            var retriesRemaining = maximumRetryAttempts;
            while (retriesRemaining-- >= 0)
            {
                try
                {
                    func();
                    break;
                }
                catch (IOException ex)
                {
                    var hResult = Marshal.GetHRForException(ex);
                    if (hResult != -2147024864) // 0x80070020 ERROR_SHARING_VIOLATION
                        throw;
                    if (whenSharingViolationOccurs != null)
                        whenSharingViolationOccurs(retriesRemaining);
                    Thread.Sleep(waitTime);
                }
            }
        }
    }
}