using LightBlue.MultiHost.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace LightBlue.MultiHost.Runners
{
    public static class ProcessExtensions
    {
        public static IEnumerable<Process> GetChildren(this Process process)
        {
            var query = $"Select * From Win32_Process Where ParentProcessID={process.Id}";
            foreach (var mo in new ManagementObjectSearcher(query).Get())
                yield return Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));
        }

        public static void AllocateToMultiHostProcess(this Process process)
        {
            // Allows windows to clean up child process for us when the parent process is killed
            MultiHostProcess.Job.AddProcess(process);
        }

        public static void SetPriority(this Process process, Role role)
        {
            switch (role.ProcessPriority)
            {
                case ProcessPriority.BelowNormal:
                    process.PriorityClass = ProcessPriorityClass.BelowNormal;
                    break;
                case ProcessPriority.Normal:
                    process.PriorityClass = ProcessPriorityClass.Normal;
                    break;
                case ProcessPriority.AboveNormal:
                    process.PriorityClass = ProcessPriorityClass.AboveNormal;
                    break;
            }
        }
    }
}