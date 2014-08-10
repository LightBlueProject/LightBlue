namespace LightBlue.Infrastructure
{
    public enum RetryMode
    {
        NoReload = 0,
        Infinite = 1,
        SingleThenFreeze = 2,
        FreezeOnError = 3
    }
}