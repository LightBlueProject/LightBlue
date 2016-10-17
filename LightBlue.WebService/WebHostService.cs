using System;
using System.Threading;
using LightBlue.Host.Stub;
using LightBlue.WebHost;
using Topshelf;

namespace LightBlue.WebService
{
    public class WebHostService : ServiceControl
    {
        private readonly WebHostArgs _hostArgs;
        private HostStub _stub;

        public WebHostService(WebHostArgs hostArgs)
        {
            _hostArgs = hostArgs;
        }

        public bool Start(HostControl hostControl)
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                try
                {
                    _stub = WebHostFactory.Create(_hostArgs);

                    _stub.Run(_hostArgs.Assembly,
                        _hostArgs.ConfigurationPath,
                        _hostArgs.ServiceDefinitionPath,
                        _hostArgs.RoleName,
                        _hostArgs.UseHostedStorage);
                }
                catch (Exception)
                {
                    hostControl.Stop();

                    throw;
                }
            });

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _stub.RequestShutdown();

            return true;
        }
    }
}