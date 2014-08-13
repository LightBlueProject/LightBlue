using System;
using System.IO;

namespace LightBlue.Tests.Standalone
{
    public class StandaloneAzureTestsBase
    {
        protected readonly string BasePath;

        protected StandaloneAzureTestsBase()
        {
            BasePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        public void Dispose()
        {
            if (!string.IsNullOrWhiteSpace(BasePath) && Directory.Exists(BasePath))
            {
                Directory.Delete(BasePath, true);
            }
        }
    }
}