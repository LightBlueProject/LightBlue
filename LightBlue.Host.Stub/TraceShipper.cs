using System;

namespace LightBlue.Host.Stub
{
    public class TraceShipper : MarshalByRefObject
    {
        public void Write(string message)
        {
            Console.Write(message);
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}