using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace LightBlue.Standalone
{
    public static class FileLockExtensions
    {
        /// <summary>
        /// 0x80070020 ERROR_SHARING_VIOLATION
        /// </summary>
        const int SharingViolationErrorHResult = -2147024864;

        /// <summary>
        /// Executes an action and attempts retries up to a specified maximum if a sharing violation is detected.
        /// </summary>
        /// <param name="ioAction">The action that could potentially throw an IOException</param>
        /// <param name="timeToAttemptRetry">A duration to wait between try attempts. The default duration is 1 second.</param>
        /// <param name="maximumRetryAttempts">The maximum number of retries to attempt before this method returns.</param>
        /// <param name="whenSharingViolationOccurs">An action to invoke when a file sharing violation is detected. This is passed the remaining number of retries.</param>
        public static void WaitAndRetryOnFileLock(
            Action ioAction,
            TimeSpan? timeToAttemptRetry = null,
            int maximumRetryAttempts = 3,
            Action<int> whenSharingViolationOccurs = null)
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
                    ioAction();
                    break;
                }
                catch (IOException ex)
                {
                    var hResult = Marshal.GetHRForException(ex);
                    if (hResult != SharingViolationErrorHResult)
                    {
                        throw;
                    }
                    if (whenSharingViolationOccurs != null)
                    {
                        whenSharingViolationOccurs(retriesRemaining);
                    }
                    Thread.Sleep(waitTime);
                }
            }
        }
    }
}