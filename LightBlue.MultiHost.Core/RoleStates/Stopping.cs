using System;

namespace LightBlue.MultiHost.Core.RoleStates
{
    public class Stopping : IState
    {
        private readonly Role _r;

        public Stopping(Role r)
        {
            _r = r;
        }

        public RoleStatus Status { get { return RoleStatus.Stopping; } }

        public void Start()
        {

        }

        public void StartAutomatically()
        {

        }

        public void Started()
        {
            throw new NotSupportedException();
        }

        public void Crashed()
        {
            throw new NotSupportedException();
        }

        public void Stop()
        {

        }

        public void OnStopped()
        {
            _r.State = new Stopped(_r);
        }
    }
}