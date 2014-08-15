using System;

namespace LightBlue.Infrastructure
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