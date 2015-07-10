using System;
using System.IO;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using LightBlue.Host.Stub;
using LightBlue.Infrastructure;
using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost.Runners
{
    class AppDomainRunner : IRunner
    {
        private readonly Role _role;
        private readonly AppDomainSetup _appDomainSetup;
        private readonly string _assemblyFilePath;
        private readonly string _configurationFilePath;
        private readonly string _serviceDefinitionFilePath;
        private readonly string _roleName;
        private readonly TaskCompletionSource<object> _started = new TaskCompletionSource<object>();
        private readonly TaskCompletionSource<object> _completed = new TaskCompletionSource<object>();
        private AppDomain _appDomain;
        private HostStub _hostStub;

        public Task Started { get { return _started.Task; } }
        public Task Completed { get { return _completed.Task; } }

        public AppDomainRunner(Role role,
            AppDomainSetup appDomainSetup,
            string assemblyPath,
            string configurationFilePath,
            string serviceDefinitionFilePath,
            string roleName)
        {
            _role = role;
            _appDomainSetup = appDomainSetup;
            _assemblyFilePath = assemblyPath;
            _configurationFilePath = configurationFilePath;
            _serviceDefinitionFilePath = serviceDefinitionFilePath;
            _roleName = roleName;
        }

        public void Start()
        {
            var t = new Thread(StartInternal) { IsBackground = true, Name = _role.Title + "Thread" };
            t.Start();
        }

        private void StartInternal()
        {
            ConfigurationManipulation.RemoveAzureTraceListenerFromConfiguration(_configurationFilePath);
            CopyStubAssemblyToRoleDirectory(_appDomainSetup.ApplicationBase, _role);
            _appDomain = AppDomain.CreateDomain("LightBlue", null, _appDomainSetup);
            _hostStub = (HostStub)_appDomain.CreateInstanceAndUnwrap(typeof(HostStub).Assembly.FullName, typeof(HostStub).FullName);

            var shipper = new EventTraceShipper();
            Action<string> twh = m => _role.TraceWrite(m);
            Action<string> twlh = m => _role.TraceWriteLine(m);
            shipper.TraceWrite += twh;
            shipper.TraceWriteLine += twlh;
            _hostStub.ConfigureTracing(shipper);

            // TODO: decide how this is going to work.
            _appDomain.UnhandledException += StubExceptionHandler.Handler;

            try
            {
                _started.SetResult(new object());
                _role.TraceWriteLine("Role started in app domain: " + _appDomain.Id + " by " + Thread.CurrentThread.Name);
                _hostStub.Run(_assemblyFilePath, _configurationFilePath, _serviceDefinitionFilePath, _roleName, false);
            }
            catch (Exception ex)
            {
                _role.TraceWriteLine(ex.ToString());
            }
            finally
            {
                shipper.TraceWrite -= twh;
                shipper.TraceWriteLine -= twlh;
                _completed.SetResult(new object());
            }
        }

        public void Dispose()
        {
            _role.TraceWriteLine("Request thread shutdown...");
            try
            {
                _hostStub.RequestShutdown();

                if (Completed.Wait(TimeSpan.FromSeconds(30)))
                {
                    _role.TraceWriteLine("Thread shutdown sucessful.");
                }
                else
                {
                    _role.TraceWriteLine("Thread shutdown failed to complete within 30 seconds.");
                }
            }
            catch (RemotingException ex)
            {
                _role.TraceWriteLine("That shutdown failed: " + ex.Message);
            }

            _role.TraceWriteLine("Unloading AppDomain.");

            while (true)
            {
                try
                {
                    AppDomain.Unload(_appDomain);
                    _role.TraceWriteLine("AppDomain Unloaded.");
                    return;
                }
                catch (Exception ex)
                {
                    _role.TraceWriteLine("Error Unloading AppDomain: " + ex.Message);
                    Thread.Sleep(1000);
                }
            }
        }

        public static void CopyStubAssemblyToRoleDirectory(string applicationBase, Role role)
        {
            var destinationHostStubPath = Path.Combine(applicationBase, Path.GetFileName(typeof(HostStub).Assembly.Location));
            try
            {
                File.Copy(typeof(HostStub).Assembly.Location, destinationHostStubPath, true);
            }
            catch (IOException)
            {
                role.TraceWriteLine("Could not copy Host Stub. Assuming this is because it already exists and continuing.");
            }
        }
    }
}