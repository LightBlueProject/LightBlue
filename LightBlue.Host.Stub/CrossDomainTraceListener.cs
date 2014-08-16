using System.Diagnostics;

namespace LightBlue.Host.Stub
{
    internal class CrossDomainTraceListener : TraceListener
    {
        private readonly TraceShipper _shipper;

        public CrossDomainTraceListener(TraceShipper shipper)
        {
            _shipper = shipper;
        }

        public override void Write(string message)
        {
            _shipper.Write(message);
        }

        public override void WriteLine(string message)
        {
            _shipper.WriteLine(message);
        }
    }
}