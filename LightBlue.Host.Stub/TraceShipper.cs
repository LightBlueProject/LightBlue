using System;

namespace LightBlue.Host.Stub
{
    public abstract class TraceShipper : MarshalByRefObject
    {
        public abstract void Write(string message);
        public abstract void WriteLine(string message);
    }

    public class ConsoleTraceShipper : TraceShipper
    {
        public override void Write(string message)
        {
            Console.Write(message);
        }

        public override void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class EventTraceShipper : TraceShipper
    {
        public event Action<string> TraceWrite;
        public event Action<string> TraceWriteLine;

        public override void Write(string message)
        {
            var h = TraceWrite;
            if (h != null) h(message);
        }

        public override void WriteLine(string message)
        {
            var h = TraceWriteLine;
            if (h != null) TraceWriteLine(message);
        }
    }
}