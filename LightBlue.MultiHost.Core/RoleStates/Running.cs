using System;

namespace LightBlue.MultiHost.Core.RoleStates
{
    public class Running : IState
    {
        private readonly Role _r;

        public Running(Role r)
        {
            _r = r;
        }

        public RoleStatus Status
        {
            get { return RoleStatus.Running; }
        }

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
            _r.State = new Crashing(_r);
            _r.StopInternal();
        }

        public void Stop()
        {
            _r.State = new Stopping(_r);
            _r.StopInternal();
        }

        public void OnStopped()
        {
            throw new NotSupportedException();
        }
    }
}