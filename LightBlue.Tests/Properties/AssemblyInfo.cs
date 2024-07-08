using System;
using System.Runtime.InteropServices;
using Xunit;

[assembly: ComVisible(false)]

[assembly: Guid("0181ae49-63e8-4b10-8123-3711c779aae1")]

[assembly: CLSCompliant(false)]

// Many of our tests reference shared state (StandaloneEnvironment.LightBlueDataDirectory) and cannot be run concurrently
[assembly: CollectionBehavior(DisableTestParallelization = true)]