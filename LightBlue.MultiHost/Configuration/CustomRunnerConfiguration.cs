namespace LightBlue.MultiHost.Configuration
{
    /// <summary>
    /// Allows users to BYO runner, string matches
    /// the name of the configured Runner against the 
    /// Runner specified in the Service
    /// </summary>
    public class CustomRunnerConfiguration
    {
        public string RunnerName { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; } = string.Empty;
    }
}
