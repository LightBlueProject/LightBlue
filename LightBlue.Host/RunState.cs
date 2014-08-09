namespace LightBlue.Host
{
    public enum RunState
    {
        NotRun = 0,
        ExitedCleanly = 1,
        FailedToStart = 2,
        ThrewException = 3
    }
}