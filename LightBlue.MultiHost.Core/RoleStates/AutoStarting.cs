using System;

namespace LightBlue.MultiHost.Core.RoleStates
{
    public class AutoStarting : IState
    {
        private readonly Role _r;

        public AutoStarting(Role r)
        {
            _r = r;
        }

        public RoleStatus Status
        {
            get { return RoleStatus.Sequenced; }
        }

        public void Start()
        {
            _r.State = new Starting(_r);
            _r.StartInternal();
        }

        public void StartAutomatically()
        {
            _r.State = new Starting(_r);
            _r.StartInternal();
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
            _r.State = new Stopped(_r);
        }

        public void OnStopped()
        {
            throw new NotSupportedException();
        }
    }
}