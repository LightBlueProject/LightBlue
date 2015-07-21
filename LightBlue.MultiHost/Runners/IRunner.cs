using System;
using System.Threading.Tasks;

namespace LightBlue.MultiHost.Runners
{
    interface IRunner : IDisposable
    {
        Task Started { get; }
        Task Completed { get; }
        string Identifier { get; }
        void Start();
    }
}