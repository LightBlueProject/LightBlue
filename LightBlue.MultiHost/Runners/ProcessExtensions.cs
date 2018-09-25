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
    }
}