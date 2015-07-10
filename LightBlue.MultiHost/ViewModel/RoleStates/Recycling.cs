using System;

namespace LightBlue.MultiHost.ViewModel.RoleStates
{
    class Recycling : IState
    {
        private readonly Role _r;

        public Recycling(Role r)
        {
            _r = r;
        }

        public RoleStatus Status { get { return RoleStatus.Recycling; } }

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