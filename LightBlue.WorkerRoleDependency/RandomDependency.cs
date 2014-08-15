using System;

namespace LightBlue.WorkerRoleDependency
{
    public class RandomDependency
    {
        private readonly Random _random = new Random(Guid.NewGuid().GetHashCode());

        public int RandomNumber()
        {
            return _random.Next();
        }
    }
}
