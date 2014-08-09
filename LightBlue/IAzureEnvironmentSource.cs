using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightBlue
{
    public interface IAzureEnvironmentSource
    {
        AzureEnvironment CurrentEnvironment { get; }
    }
}
