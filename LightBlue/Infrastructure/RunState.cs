namespace LightBlue.Infrastructure
{
    public enum RunState
    {
        NotRun = 0,
        ExitedCleanly = 1,
        FailedToStart = 2,
        Failed = 3
    }
}