using System;

namespace LightBlue.MultiHost.Core.RoleStates
{
    public class Starting : IState
    {
        private readonly Role _r;

        public Starting(Role r)
        {
            _r = r;
        }

        public RoleStatus Status
        {
            get { return RoleStatus.Starting; }
        }

        public void Start()
        {

        }

        public void StartAutomatically()
        {

        }

        public void Started()
        {
            _r.State = new Running(_r);
            _r.StartedInternal();
        }

        public void Crashed()
        {
            throw new NotSupportedException();
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