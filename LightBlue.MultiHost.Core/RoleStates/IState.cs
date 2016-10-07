namespace LightBlue.MultiHost.Core.RoleStates
{
    public interface IState
    {
        RoleStatus Status { get; }
        void Start();
        void Started();
        void Crashed();
        void Stop();
        void OnStopped();
        void StartAutomatically();
    }
}