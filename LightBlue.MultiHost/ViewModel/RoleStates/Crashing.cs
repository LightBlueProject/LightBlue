using System;

namespace LightBlue.MultiHost.ViewModel.RoleStates
{
    class Crashing : IState
    {
        private readonly Role _r;

        public Crashing(Role r)
        {
            _r = r;
        }

        public RoleStatus Status
        {
            get { return RoleStatus.Crashing; }
        }

        public void Start()
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
            _r.State = new Stopping(_r);
        }

        public void OnStopped()
        {
            _r.State = new Recycling(_r);
            _r.RecycleInternal();
        }

        public void StartAutomatically()
        {
            throw new NotSupportedException();
        }
    }
}