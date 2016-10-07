using System;

namespace LightBlue.MultiHost.Core.RoleStates
{
    public class Stopped : IState
    {
        private readonly Role _r;

        public Stopped(Role r)
        {
            _r = r;
        }

        public void Start()
        {
            _r.State = new Starting(_r);
            _r.StartInternal();
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

        public RoleStatus Status
        {
            get { return RoleStatus.Stopped; }
        }

        public void Stop()
        {

        }

        public void OnStopped()
        {
            throw new NotSupportedException();
        }
    }
}