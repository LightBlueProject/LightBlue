using System;
using System.IO;

using LightBlue.Host.Stub;

namespace LightBlue.Host
{
    public static class StubManagement
    {
        public static void CopyStubAssemblyToRoleDirectory(string applicationBase)
        {
            var destinationHostStubPath = Path.Combine(applicationBase, Path.GetFileName(typeof(HostStub).Assembly.Location));
            if (!File.Exists(destinationHostStubPath))
            {
                try
                {
                    File.Copy(typeof(HostStub).Assembly.Location, destinationHostStubPath, true);
                }
                catch (IOException)
                {
                    Console.WriteLine("Could not copy Host Stub. Assuming this is because it already exists and continuing.");
                }
            }
        }
    }
}